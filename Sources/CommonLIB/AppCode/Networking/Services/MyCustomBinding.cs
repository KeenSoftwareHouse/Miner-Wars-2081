using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using MinerWars.CommonLIB.AppCode.Utils;
using System.ServiceModel.Security.Tokens;

namespace MinerWars.CommonLIB.AppCode.Networking.Services
{
    public static class MyCustomBinding
    {
        public static Binding DecorateBinding(Binding binding, int? maxNegotiationCount)
        {
            CustomBinding customBinding = new CustomBinding(binding);
            SymmetricSecurityBindingElement security = customBinding.Elements.Find<SymmetricSecurityBindingElement>();
            if (security != null)
            {
                security.IncludeTimestamp = false;
                security.LocalClientSettings.DetectReplays = false;
                security.LocalServiceSettings.DetectReplays = false;
                security.LocalClientSettings.MaxClockSkew = TimeSpan.MaxValue;
                security.LocalServiceSettings.MaxClockSkew = TimeSpan.MaxValue;
                security.LocalClientSettings.SessionKeyRenewalInterval = TimeSpan.MaxValue;
                security.LocalServiceSettings.SessionKeyRenewalInterval = TimeSpan.FromMilliseconds(Int32.MaxValue);

                if (maxNegotiationCount.HasValue)
                {
                    security.LocalServiceSettings.MaxPendingSessions = maxNegotiationCount.Value;
                    security.LocalServiceSettings.MaxStatefulNegotiations = maxNegotiationCount.Value;
                }

                // Get the System.ServiceModel.Security.Tokens.SecureConversationSecurityTokenParameters
                SecureConversationSecurityTokenParameters secureTokenParams = (SecureConversationSecurityTokenParameters)security.ProtectionTokenParameters;

                // From the collection, get the bootstrap element.
                SecurityBindingElement bootstrap = secureTokenParams.BootstrapSecurityBindingElement;

                // Set the MaxClockSkew on the bootstrap element.
                bootstrap.IncludeTimestamp = false;
                bootstrap.LocalClientSettings.DetectReplays = false;
                bootstrap.LocalServiceSettings.DetectReplays = false;
                bootstrap.LocalClientSettings.MaxClockSkew = TimeSpan.MaxValue;
                bootstrap.LocalServiceSettings.MaxClockSkew = TimeSpan.MaxValue;
                bootstrap.LocalClientSettings.SessionKeyRenewalInterval = TimeSpan.MaxValue;
                bootstrap.LocalServiceSettings.SessionKeyRenewalInterval = TimeSpan.FromMilliseconds(Int32.MaxValue);

                if (maxNegotiationCount.HasValue)
                {
                    bootstrap.LocalServiceSettings.MaxPendingSessions = maxNegotiationCount.Value;
                    bootstrap.LocalServiceSettings.MaxStatefulNegotiations = maxNegotiationCount.Value;
                }

                return customBinding;
            }
            else
            {
                return binding;
            }
        }

        public static Binding DecorateBinding(Binding binding)
        {
            return DecorateBinding(binding, null);
        }
    }
}
