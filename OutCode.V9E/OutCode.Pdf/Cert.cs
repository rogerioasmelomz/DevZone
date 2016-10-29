using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Pdf
{
    /// <summary>
    /// Classe para reter a informação de um certificado
    /// </summary>
    public class Cert
    {
        #region Attributes

        private string path = "";
        private string password = "";
        private AsymmetricKeyParameter asymKeyParameter;
        private Org.BouncyCastle.X509.X509Certificate[] chain;

        #endregion

        #region Accessors

        public IExternalSignature Signature { get; set; }

        public Org.BouncyCastle.X509.X509Certificate[] Chain
        {
            get { return chain; }
        }

        public AsymmetricKeyParameter AsymKeyParameter
        {
            get;
            set;
        }

        public string Path
        {
            get { return path; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }


        /// <summary>
        /// Devolve a versão da DLL
        /// </summary>
        /// <returns>string com a versão</returns>
        public static string GetVersion()
        {
            return (Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        #endregion

        #region Helpers

        private void ProcessCert()
        {
            Pkcs12Store pk12;

            //First we'll read the certificate file

            pk12 = new Pkcs12Store(new FileStream(this.Path, FileMode.Open, FileAccess.Read), this.password.ToCharArray());

            //then Iterate throught certificate entries to find the private key entry
            ProcessCert(pk12);
        }

        private void ProcessCert(Pkcs12Store pk12)
        {
            string alias = null;

            try
            {
                //then Iterate throught certificate entries to find the private key entry
                foreach (string a in pk12.Aliases)
                {
                    alias = a;
                    if (pk12.IsKeyEntry(a))
                    {
                        alias = a;
                        break;
                    }
                }

                if (alias == null) throw new Exception("O certificado não é válido!");

                this.asymKeyParameter = pk12.GetKey(alias).Key;
                this.Signature = new PrivateKeySignature((ICipherParameters)this.AsymKeyParameter, "SHA-1");

                X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
                this.chain = new Org.BouncyCastle.X509.X509Certificate[ce.Length];
                for (int k = 0; k < ce.Length; ++k)
                    chain[k] = ce[k].Certificate;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao processar o certificado!", ex);
            }

        }

        /// <summary>
        /// Devolve a lista de certificados existente na store do windows para o utilizador em questão.
        /// </summary>
        /// <returns>Lista com os nomes dos certificados</returns>
        static public List<string> GetCertListFromStore()
        {
            X509Store CertStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            CertStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = CertStore.Certificates;

            List<string> sCerts = new List<string>();

            foreach (X509Certificate2 c in certificates)
            {
                if (c.HasPrivateKey) sCerts.Add(c.Subject.Split('=')[1]);
            }

            return (sCerts);
        }

        public bool ReadFromStore(string subject)
        {

            try
            {
                // Certificados do utilizador actual do windows
                X509Store CertStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                CertStore.Open(OpenFlags.MaxAllowed | OpenFlags.OpenExistingOnly);

                bool parsed = false;

                //Pkcs12Store pk12 = new Pkcs12Store();

                foreach (X509Certificate2 c in CertStore.Certificates)
                {
                    if (subject.Equals(c.Subject.Split('=')[1], StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (c.HasPrivateKey)
                        {
                            Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
                            try
                            {
                                this.Signature = new X509Certificate2Signature(c, "SHA-1");
                                this.chain = new Org.BouncyCastle.X509.X509Certificate[1];
                                chain[0] = cp.ReadCertificate(c.RawData);

                                parsed = true;
                            }
                            catch (Exception pkex)
                            {
                                throw new Exception("Erro ao ler a chave privada!", pkex);
                            }

                        }
                        break;
                    }
                }

                CertStore.Close();
                return (parsed);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar o certificado do Windows!", ex);
            }
        }

        public static AsymmetricKeyParameter TransformRSAPrivateKey(AsymmetricAlgorithm privateKey)
        {
            RSACryptoServiceProvider prov = privateKey as RSACryptoServiceProvider;
            RSAParameters parameters = prov.ExportParameters(true);

            return new RsaPrivateCrtKeyParameters(
                new BigInteger(1, parameters.Modulus),
                new BigInteger(1, parameters.Exponent),
                new BigInteger(1, parameters.D),
                new BigInteger(1, parameters.P),
                new BigInteger(1, parameters.Q),
                new BigInteger(1, parameters.DP),
                new BigInteger(1, parameters.DQ),
                new BigInteger(1, parameters.InverseQ));
        }

        #endregion

        #region Constructors

        public Cert()
        { }

        public Cert(string cpath)
        {
            this.path = cpath;
            this.ProcessCert();
        }

        public Cert(string cpath, string cpassword)
        {
            this.path = cpath;
            this.Password = cpassword;
            this.ProcessCert();
        }

        #endregion

    }
}
