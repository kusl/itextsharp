/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/sales
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.util;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter3 {
    public class C3_01_SignWithCAcert {
        public static String SRC = "../../../../resources/hello.pdf";
        public static String DEST = "../../../../results/chapter3/hello_cacert.pdf";

        public void Sign(String src, String dest,
                         ICollection<X509Certificate> chain, ICipherParameters pk,
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
                IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
                MakeSignature.SignDetached(appearance, pks, chain, crlList, ocspClient, tsaClient, estimatedSize,
                                           subfilter);
            } finally {
                if (reader != null)
                    reader.Close();
                if (stamper != null)
                    stamper.Close();
                if (os != null)
                    os.Close();
            }
        }

        public static void Main(String[] args) {
            Properties properties = new Properties();
            properties.Load(new FileStream("c:/home/blowagie/key.properties", FileMode.Open));
            String path = properties["PRIVATE"];
            char[] pass = properties["PASSWORD"].ToCharArray();

            Pkcs12Store ks = new Pkcs12Store();
            ks.Load(new FileStream(path, FileMode.Open), pass);
            String alias = "";
            foreach (string al in ks.Aliases) {
                if (ks.IsKeyEntry(al) && ks.GetKey(al).Key.IsPrivate) {
                    alias = al;
                    break;
                }
            }
            AsymmetricKeyParameter pk = ks.GetKey(alias).Key;
            ICollection<X509Certificate> chain = new List<X509Certificate>();
            foreach (X509CertificateEntry entry in ks.GetCertificateChain(alias)) {
                chain.Add(entry.Certificate);    
            }
            C3_01_SignWithCAcert app = new C3_01_SignWithCAcert();
            app.Sign(SRC, DEST, chain, pk, DigestAlgorithms.SHA256, CryptoStandard.CMS, "Test",
                     "Ghent", null, null, null, 0);
        }
    }
}
