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
using Org.BouncyCastle.Security;
using iTextSharp.text;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace signatures.chapter3 {

    public class C3_11_SignWithToken {
        public static String SRC = "../../../../resources/hello.pdf";
        public static String DEST = "../../../../results/chapter3/hello_token.pdf";

        public void Sign(String src, String dest,
                         ICollection<X509Certificate> chain, X509Certificate2 pk,
                         String digestAlgorithm, CryptoStandard subfilter,
                         String reason, String location,
                         ICollection<ICrlClient> crlList,
                         IOcspClient ocspClient,
                         ITSAClient tsaClient,
                         int estimatedSize) {
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
                IExternalSignature pks = new X509Certificate2Signature(pk, digestAlgorithm);
                MakeSignature.SignDetached(appearance, pks, chain, crlList, ocspClient, tsaClient, estimatedSize,
                                           subfilter);
            }
            finally {
                if (reader != null)
                    reader.Close();
                if (stamper != null)
                    stamper.Close();
                if (os != null)
                    os.Close();
            }
        }

        public static void Main(String[] args) {
            LoggerFactory.GetInstance().SetLogger(new SysoLogger());


            X509Store x509Store = new X509Store("My");
            x509Store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = x509Store.Certificates;
            IList<X509Certificate> chain = new List<X509Certificate>();
            X509Certificate2 pk = null;
            if (certificates.Count > 0) {
                X509Certificate2Enumerator certificatesEn = certificates.GetEnumerator();
                certificatesEn.MoveNext();
                pk = certificatesEn.Current;

                X509Chain x509chain = new X509Chain();
                x509chain.Build(pk);

                foreach (X509ChainElement x509ChainElement in x509chain.ChainElements) {
                    chain.Add(DotNetUtilities.FromX509Certificate(x509ChainElement.Certificate));
                }
            }
            x509Store.Close();


            IOcspClient ocspClient = new OcspClientBouncyCastle();
            ITSAClient tsaClient = null;
            for (int i = 0; i < chain.Count; i++) {
                X509Certificate cert = chain[i];
                String tsaUrl = CertificateUtil.GetTSAURL(cert);
                if (tsaUrl != null) {
                    tsaClient = new TSAClientBouncyCastle(tsaUrl);
                    break;
                }
            }
            IList<ICrlClient> crlList = new List<ICrlClient>();
            crlList.Add(new CrlClientOnline(chain));
            C3_11_SignWithToken app = new C3_11_SignWithToken();
            app.Sign(SRC, DEST, chain, pk, DigestAlgorithms.SHA256, CryptoStandard.CMS, "Test",
                     "Ghent",
                     crlList, ocspClient, tsaClient, 0);
        }
    }
}

