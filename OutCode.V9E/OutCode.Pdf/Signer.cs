using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace OutCode.Pdf
{

    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Classe para assinar um ficheiro pdf
    /// </summary>
    public class Signer
    {
        #region Attributes
        private string inputPDF = "";
        private string outputPDF = "";
        private Cert myCert;
        private MetaData metadata = new MetaData();
        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa o Objecto que permite assinar um ficheiro PDF
        /// </summary>
        /// <param name="input">Path completa para o ficheiro de origem</param>
        /// <param name="output">Path completa para o ficheiro de destino</param>        
        public Signer(string input, string output)
        {
            this.inputPDF = input;
            this.outputPDF = output;
        }

        /// <summary>
        /// Inicializa o Objecto que permite assinar um ficheiro PDF
        /// </summary>
        /// <param name="input">Path completa para o ficheiro de origem</param>
        /// <param name="output">Path completa para o ficheiro de destino</param>
        /// <param name="cert">Objecto com o certificado a usar na assinatura</param
        public Signer(string input, string output, Cert cert)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.myCert = cert;
        }

        /// <summary>
        /// Inicializa o Objecto que permite assinar um ficheiro PDF
        /// </summary>
        /// <param name="input">Path completa para o ficheiro de origem</param>
        /// <param name="output">Path completa para o ficheiro de destino</param>
        /// <param name="md">Objecto com a metadata a colocar no pdf</param>
        public Signer(string input, string output, MetaData md)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.metadata = md;
        }

        /// <summary>
        /// Inicializa o Objecto que permite assinar um ficheiro PDF
        /// </summary>
        /// <param name="input">Path completa para o ficheiro de origem</param>
        /// <param name="output">Path completa para o ficheiro de destino</param>
        /// <param name="cert">Objecto com o certificado a usar na assinatura</param>
        /// <param name="md">Objecto com a metadata a colocar no pdf</param>
        public Signer(string input, string output, Cert cert, MetaData md)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.myCert = cert;
            this.metadata = md;
        }

        #endregion

        #region Methods

        public void Verify()
        {
        }

        /// <summary>
        /// Assina um PDF
        /// </summary>
        /// <param name="SigReason">Texto que define o porquê de estarmos a assinar o documento</param>
        /// <param name="SigContact">Contacto da entidade responsavel pela assinatura</param>
        /// <param name="SigLocation">Identifica a Localização onde se está a efectuar a assinatura</param>
        /// <param name="visible">Indica se a assinatura ficará ou não visivel</param>
        public void Sign(string SigReason, string SigContact, string SigLocation, bool visible)
        {
            PdfReader reader = null;
            PdfStamper stamper = null;

            try
            {
                reader = new PdfReader(this.inputPDF);
                //Activate MultiSignatures
                stamper = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0', null, true);
                //To disable Multi signatures uncomment this line : every new signature will invalidate older ones !
                //PdfStamper st = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0'); 

                stamper.MoreInfo = this.metadata.getMetaData();
                stamper.XmpMetadata = this.metadata.getStreamedMetaData();
                PdfSignatureAppearance signatureAppearence = stamper.SignatureAppearance;

                signatureAppearence.Reason = SigReason;
                signatureAppearence.Contact = SigContact;
                signatureAppearence.Location = SigLocation;
                if (visible)
                {
                    iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(0, 0, 100, 50);
                    signatureAppearence.SetVisibleSignature(rectangle, 1, null);
                }

                MakeSignature.SignDetached(signatureAppearence, myCert.Signature, myCert.Chain, null, null, null, 0, CryptoStandard.CMS);
                stamper.Close();
                reader.Close();
            }
            catch (Exception exObj)
            {
                if (stamper != null) stamper.Close();
                if (reader != null) reader.Close();
                throw new Exception("Error generating signature.", exObj);
            }
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

    }
}
