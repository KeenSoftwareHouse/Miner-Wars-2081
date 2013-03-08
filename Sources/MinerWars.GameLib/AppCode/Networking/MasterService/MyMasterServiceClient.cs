using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Networking.Services;
using System.Diagnostics;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Networking.MasterService
{
    public partial class MyMasterServiceClient
    {
        private static MyMasterServiceClient m_instance;

        private static string m_username;
        private static string m_passwordHash;

        public static void SetCredentials(string username, string passwordHash)
        {
            m_username = username;
            m_passwordHash = passwordHash;
            SafeClose();
        }

        static void SafeClose()
        {
            try
            {
                if(m_instance != null) m_instance.Close();
            }
            catch (Exception) { }
            m_instance = null;
        }

        public static bool HasCredentials
        {
            get
            {
                return m_username != null && m_passwordHash != null;
            }
        }

        public static MyMasterServiceClient GetCheckedInstance()
        {
            Debug.Assert(HasCredentials, "Set credential prior getting instance method");

            if (m_instance == null || m_instance.State == CommunicationState.Faulted || m_instance.State == CommunicationState.Closed || m_instance.State == CommunicationState.Closing)
            {
                SafeClose();
                m_instance = CreateInstance();
            }
            return m_instance;
        }

        private static MyMasterServiceClient CreateInstance()
        {
            var uri = String.Format("net.tcp://{0}:{1}", MyMwcFinalBuildConstants.MASTER_SERVER_ADDRESS, MyMwcNetworkingConstants.NETWORKING_PORT_MASTER_SERVER_NEW);

            MyMwcLog.WriteLine("Creating master service client, url: " + uri);

            // create the URI which is used as the service endpoint
            Uri tcpBaseAddress = new Uri(uri);

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
            ntcBinding.SendTimeout = MyMwcNetworkingConstants.WCF_TIMEOUT_DISCONNECT;
            ntcBinding.ReceiveTimeout = MyMwcNetworkingConstants.WCF_TIMEOUT_DISCONNECT;
            ntcBinding.Security.Mode = SecurityMode.Message;
            ntcBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            ntcBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            ntcBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

            var decoratedBinding = MyCustomBinding.DecorateBinding(ntcBinding);

            MyMasterServiceClient client = new MyMasterServiceClient(decoratedBinding, new EndpointAddress(tcpBaseAddress, EndpointIdentity.CreateDnsIdentity(MyMwcNetworkingConstants.WCF_MASTER_SERVER_CN)));
            client.ClientCredentials.UserName.UserName = m_username;
            client.ClientCredentials.UserName.Password = m_passwordHash;
            client.ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator = new MyCertificateValidator(MyMwcNetworkingConstants.WCF_MASTER_SERVER_HASH);
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            return client;
        }
    }
}
