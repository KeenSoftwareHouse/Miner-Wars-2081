using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ServiceModel;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.Networking.Services;
using MinerWars.AppCode.Game.World;
using System.Timers;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Networking.SectorService
{
    public partial class MySectorServiceClient
    {
        public struct MyUserLoginArgs
        {
            public MyUserLoginArgs(Exception error, MyUserInfo userInfo, bool alreadyLoggedIn)
                : this()
            {
                Error = error;
                UserInfo = userInfo;
                AlreadyLoggedIn = alreadyLoggedIn;
            }

            public Exception Error { get; private set; }
            public MyUserInfo UserInfo { get; private set; }
            public bool AlreadyLoggedIn { get; private set; }
        }

        private static MySectorServiceClient m_instance;
        private static object m_syncRoot = new object();

        private static string m_username;
        private static string m_passwordHash;
        private static string m_url;

        public TimeSpan OperationTimeout
        {
            set
            {
                var channelContext = this.GetChannelContext();
                if (channelContext != null)
                {
                    channelContext.OperationTimeout = value;
                }
            }
        }

        public static bool IsInstanceValid
        {
            get
            {
                return m_instance != null && m_instance.State != CommunicationState.Faulted && m_instance.State != CommunicationState.Closed && m_instance.State != CommunicationState.Closing;
            }
        }

        public static void SetCredentials(string username, string passwordHash)
        {
            m_username = username;
            m_passwordHash = passwordHash;
            SafeClose();
        }

        public static bool HasCredentials
        {
            get
            {
                return m_username != null && m_passwordHash != null;
            }
        }

        public static bool HasUrl
        {
            get
            {
                return m_url != null;
            }
        }

        public static void ClearAndClose()
        {
            m_username = null;
            m_passwordHash = null;
            m_url = null;
            SafeClose();
        }

        static void CloseOnBackground(object state)
        {
            try
            {
                m_instance.Close();
            }
            catch (Exception) { }
        }

        public static void SafeClose()
        {
            // We want to close instance, but when it fails we dont care
            lock (m_syncRoot)
            {
                if (m_instance != null)
                {
                    try
                    {
                        m_instance.Close();
                    }
                    catch (Exception)
                    {
                    }
                    m_instance = null;
                }
            }
        }

        public static void HardClose()
        {
            lock (m_syncRoot)
            {
                if (m_instance != null)
                {
                    try
                    {
                        m_instance.Abort();
                    }
                    catch (Exception) { }
                    m_instance = null;
                }
            }
        }

        public static void SetUrl(string url)
        {
            m_url = MyMwcFinalBuildConstants.SECTOR_SERVER_ADDRESS ?? url;
            SafeClose();
        }

        public static MySectorServiceClient PrepareInstance()
        {
            MyMwcLog.WriteLine("Creating sector service client, url: " + m_url);

            // create the URI which is used as the service endpoint
            Uri tcpBaseAddress = new Uri(String.Format("net.tcp://{0}", m_url));

            if (MyFakes.TEST_DNS_UNAVAILABLE)
            {
                tcpBaseAddress = new Uri("d23>?<'12`/.,;'l,;l"); // Non sense URL
            }

            // create the net.tcp binding for the service endpoint
            NetTcpBinding ntcBinding = new NetTcpBinding();
            ntcBinding.Security.Mode = SecurityMode.None;
            ntcBinding.MaxBufferPoolSize = MyMwcNetworkingConstants.MAX_WCF_MESSAGE_POOL_SIZE;
            ntcBinding.MaxBufferSize = MyMwcNetworkingConstants.MAX_WCF_MESSAGE_SIZE;
            ntcBinding.MaxConnections = MyMwcNetworkingConstants.MAX_WCF_CONNECTIONS;
            ntcBinding.MaxReceivedMessageSize = MyMwcNetworkingConstants.MAX_WCF_MESSAGE_SIZE;
            ntcBinding.ReaderQuotas.MaxArrayLength = MyMwcNetworkingConstants.MAX_WCF_MESSAGE_SIZE;
            ntcBinding.ReaderQuotas.MaxBytesPerRead = MyMwcNetworkingConstants.MAX_WCF_MESSAGE_SIZE;
            ntcBinding.SendTimeout = MyMwcNetworkingConstants.WCF_TIMEOUT_SEND;
            ntcBinding.ReceiveTimeout = MyMwcNetworkingConstants.WCF_TIMEOUT_INACTIVITY;
            ntcBinding.CloseTimeout = MyMwcNetworkingConstants.WCF_TIMEOUT_CLOSE;
            ntcBinding.OpenTimeout = MyMwcNetworkingConstants.WCF_TIMEOUT_OPEN;
            ntcBinding.Security.Mode = SecurityMode.Message;
            ntcBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            ntcBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            ntcBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
            ntcBinding.ReliableSession = new OptionalReliableSession() { Enabled = true, InactivityTimeout = MyMwcNetworkingConstants.WCF_TIMEOUT_DISCONNECT, Ordered = false };

            if (MyFakes.MWBUILDER)
            {
                var msgSize = 16 * 1024 * 1024;
                ntcBinding.MaxConnections = 1;
                ntcBinding.MaxBufferSize = msgSize;
                ntcBinding.MaxBufferPoolSize = (int)(msgSize * 1.2f);
                ntcBinding.MaxReceivedMessageSize = msgSize;
                ntcBinding.ReaderQuotas.MaxArrayLength = msgSize;
            }

            var decoratedBinding = MyCustomBinding.DecorateBinding(ntcBinding);

            InstanceContext instanceContext = new InstanceContext(new MySectorServerCallback());

            MySectorServiceClient client = new MySectorServiceClient(instanceContext, decoratedBinding, new EndpointAddress(tcpBaseAddress, EndpointIdentity.CreateDnsIdentity(MyMwcNetworkingConstants.WCF_SECTOR_SERVER_CN)));
            if (MyMwcFinalBuildConstants.IS_DEVELOP) // On develop log WCF calls
            {
                client.Endpoint.Behaviors.Add(new MyLoggingBehavior());
            }
            client.ClientCredentials.UserName.UserName = m_username;
            client.ClientCredentials.UserName.Password = m_passwordHash;
            client.ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator = new MyCertificateValidator(MyMwcNetworkingConstants.WCF_SECTOR_SERVER_HASH);
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            client.InnerChannel.Faulted += new EventHandler(InnerChannel_Faulted);
            client.InnerChannel.Closed += new EventHandler(InnerChannel_Closed);
            return client;
        }

        public IContextChannel GetChannelContext()
        {
            return this.Channel as IContextChannel;
        }

        public static MySectorServiceClient GetCheckedInstance()
        {
            Debug.Assert(HasCredentials, "Set credential prior getting instance method");
            Debug.Assert(m_url != null, "Set URL prior getting instance method");

            try
            {
                // Make sure to create only one instance
                lock (m_syncRoot)
                {
                    if (!IsInstanceValid)
                    {
                        SafeClose();

                        m_instance = PrepareInstance();
                        m_instance.LoginCompleted += client_LoginCompleted;
                        m_instance.LoginAsync(MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION);
                    }
                    return m_instance;
                }
            }
            catch (Exception e)
            {
                throw new MyServiceException("Error occured during initialization of server connection", e);
            }
        }

        /// <summary>
        /// Login always creates new instance
        /// </summary>
        public static IAsyncResult BeginLoginStatic()
        {
            // Make sure to create only one instance
            lock (m_syncRoot)
            {
                SafeClose();
                m_instance = PrepareInstance();
                return m_instance.BeginLogin(MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION, null, m_instance);
            }
        }

        public static MyUserInfo EndLoginStatic(IAsyncResult result)
        {
            lock (m_syncRoot)
            {
                var client = (MySectorServiceClient)result.AsyncState;

                try
                {
                    var info = client.EndLogin(result);
                    return info;
                }
                catch (Exception)
                {
                    SafeClose();
                    MyMwcLog.WriteLine(String.Format("Login failed, parameters: address '{0}', username '{1}'", client.Endpoint.Address, client.ClientCredentials.UserName.UserName));
                    throw;
                }
            }
        }

        static void client_LoginCompleted(object sender, LoginCompletedEventArgs e)
        {
            lock (m_syncRoot)
            {
                MySectorServiceClient client = (MySectorServiceClient)sender;
                client.LoginCompleted -= client_LoginCompleted;

                if (e.Error != null)
                {
                    // On error, close client
                    SafeClose();
                }
            }
        }

        static void InnerChannel_Closed(object sender, EventArgs e)
        {
            // Called only on proper close, NOT called on fault
            SafeClose();
        }

        static void InnerChannel_Faulted(object sender, EventArgs e)
        {
            // Called only on fault
            SafeClose();
        }
    }
}
