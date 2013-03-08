using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.Security;

namespace MinerWars.CommonLIB.AppCode.Networking.Services
{
    public class MyCertificateValidator: X509CertificateValidator
    {
        string hash;

        /// <summary>
        /// Empty hash means automatic always success
        /// </summary>
        /// <param name="certificateHash"></param>
        public MyCertificateValidator(string certificateHash)
        {
            hash = certificateHash;
        }

        public override void Validate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            if (!String.IsNullOrEmpty(hash) && certificate.GetCertHashString() != hash)
            {
                throw new SecurityException("Server cannot be authenticated");
            }
        }
    }
}
