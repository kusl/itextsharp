/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/learn
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Cryptware.NCryptoki;
using Org.BouncyCastle.Security;
using iTextSharp.text;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace signatures.chapter4 {
    class CryptokiPrivateKeySignature : IExternalSignature
    {
        private readonly Session session;
        RSAPrivateKey privateKey;

        public CryptokiPrivateKeySignature(Session session, String alias) {
            this.session = session;
            CryptokiCollection template = new CryptokiCollection();
            template.Add(new ObjectAttribute(ObjectAttribute.CKA_CLASS, CryptokiObject.CKO_PRIVATE_KEY));
            template.Add(new ObjectAttribute(ObjectAttribute.CKA_KEY_TYPE, Key.CKK_RSA));
            template.Add(new ObjectAttribute(ObjectAttribute.CKA_LABEL, alias));
            privateKey = (RSAPrivateKey)session.Objects.Find(template);
        }

        public String GetHashAlgorithm() {
            return "SHA1";
        }
        
        public String GetEncryptionAlgorithm() {
            return "RSA";
        }

        public byte[] Sign(byte[] message) {
            session.SignInit(Mechanism.SHA1_RSA_PKCS, privateKey);
            return session.Sign(message);
        }
    }

    class C4_03_SignWithPKCS11SC {
        public const String SRC = "../../../../resources/hello.pdf";
	    public const String DEST = "../../../../results/chapter4/hello_smartcard_{0}.pdf";
	    public const String DLL = "c:/windows/system32/beidpkcs11.dll";

        public void Sign(String src, String dest, ICollection<X509Certificate> chain, Session session, String alias,
                         String digestAlgorithm, CryptoStandard subfilter, String reason, String location,
                         ICollection<ICrlClient> crlList, IOcspClient ocspClient, ITSAClient tsaClient, int estimatedSize) {
            // Creating the reader and the stamper
            PdfReader reader = null;
            PdfStamper stamper = null;
            FileStream os = null;
            try {
                reader = new PdfReader(src);
                os = new FileStream(dest, FileMode.Create);
                stamper = PdfStamper.CreateSignature(reader, os, '\0');
                // Creating the appearance
                PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                appearance.Reason = reason;
                appearance.Location = location;
                appearance.SetVisibleSignature(new Rectangle(36, 748, 144, 780), 1, "sig");
                // Creating the signature
                IExternalSignature pks = new CryptokiPrivateKeySignature(session, alias);
                MakeSignature.SignDetached(appearance, pks, chain, crlList, ocspClient, tsaClient, estimatedSize, subfilter);
            } finally {
                if (reader != null)
                    reader.Close();
                if (stamper != null)
                    stamper.Close();
                if (os != null)
                    os.Close();
            }
        }

	    static void Main(String[] args) {
		    LoggerFactory.GetInstance().SetLogger(new SysoLogger());

            // Creates a Cryptoki object related to the specific PKCS#11 native library
            Cryptoki cryptoki = new Cryptoki("beidpkcs11.dll");
            cryptoki.Initialize();
            

            // Reads the set of slots containing a token
            SlotList slots = cryptoki.Slots;
            if(slots.Count == 0) {
               Console.WriteLine("No slot available");
               return;
            }
            // Gets the first slot available
            Slot slot = slots[0];
            if(!slot.IsTokenPresent) {
                Console.WriteLine("No token inserted in the slot: " + slots[0].Info.Description);
                return;
            }

            // Gets the first token available
            Token token = slot.Token;

            // Opens a read/write serial session
            Session session = 
                token.OpenSession(Session.CKF_SERIAL_SESSION | Session.CKF_RW_SESSION, null, null);

            // Executes the login passing the user PIN
            //int nRes = session.Login(Session.CKU_USER, "0000");
            /*if (nRes != 0) {
                Console.WriteLine("Wrong PIN");
                return;
            }*/

		    Smartcardsign(session, "Authentication");
            Smartcardsign(session, "Signature");
            
            // Logouts and closes the session
            session.Logout();
            session.Close();
            cryptoki.Finalize(IntPtr.Zero);
	    }

        public static void Smartcardsign(Session session, String alias) {
            // Searchs for an RSA certificate object
            // Sets the template with its attributes
            CryptokiCollection template = new CryptokiCollection();
            template.Add(new ObjectAttribute(ObjectAttribute.CKA_CLASS, CryptokiObject.CKO_CERTIFICATE));
            template.Add(new ObjectAttribute(ObjectAttribute.CKA_CERTIFICATE_TYPE, Certificate.CKC_X_509));
            template.Add(new ObjectAttribute(ObjectAttribute.CKA_LABEL, alias));

            Cryptware.NCryptoki.X509Certificate nCert = (Cryptware.NCryptoki.X509Certificate)session.Objects.Find(template);
            
            X509Certificate2 cert = Utils.ConvertCertificate(nCert);
            ICollection<X509Certificate> chain = new List<X509Certificate>();
            X509Chain x509chain = new X509Chain();
            x509chain.Build(cert);

            foreach (X509ChainElement x509ChainElement in x509chain.ChainElements) {
                chain.Add(DotNetUtilities.FromX509Certificate(x509ChainElement.Certificate));
            }

            IOcspClient ocspClient = new OcspClientBouncyCastle();
            List<ICrlClient> crlList = new List<ICrlClient>();
            crlList.Add(new CrlClientOnline(chain));
            C4_03_SignWithPKCS11SC app = new C4_03_SignWithPKCS11SC();
		    app.Sign(SRC, String.Format(DEST, alias), chain, session, alias, DigestAlgorithms.SHA256, CryptoStandard.CMS,
				    "Test", "Ghent", crlList, ocspClient, null, 0);
	    }
    }
}
