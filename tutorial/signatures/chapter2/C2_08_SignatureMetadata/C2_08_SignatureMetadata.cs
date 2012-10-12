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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter2 {
    class C2_08_SignatureMetadata {
        public const String KEYSTORE = "../../../../resources/pkcs12";
        public static char[] PASSWORD = "password".ToCharArray();
        public const String SRC = "../../../../resources/hello_to_sign.pdf";
        public const String DEST = "../../../../results/chapter2/field_metadata.pdf";

        class MySignatureEvent : PdfSignatureAppearance.ISignatureEvent {
            private String fullName;

            public String FullName {
                set { fullName = value; }
            }

            public void GetSignatureDictionary(PdfDictionary sig) {
                sig.Put(PdfName.NAME, new PdfString(fullName));
            }
        }

        public void Sign(String src, String name, String dest, ICollection<X509Certificate> chain, ICipherParameters pk,
                         String digestAlgorithm, CryptoStandard subfilter, String reason, String location,
                         String contact, DateTime signDate, String fullName) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(name);
            appearance.Contact = contact;
            appearance.SignDate = signDate;
            MySignatureEvent eEvent = new MySignatureEvent();
            eEvent.FullName = fullName;
            appearance.SignatureEvent = eEvent;
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, subfilter);
        }

        static void Main(string[] args) {
            Pkcs12Store store = new Pkcs12Store(new FileStream(KEYSTORE, FileMode.Open), PASSWORD);
            String alias = "";
            ICollection<X509Certificate> chain = new List<X509Certificate>();
            // searching for private key

            foreach (string al in store.Aliases)
                if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate) {
                    alias = al;
                    break;
                }

            AsymmetricKeyEntry pk = store.GetKey(alias);
            foreach (X509CertificateEntry c in store.GetCertificateChain(alias))
                chain.Add(c.Certificate);
            RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;
            C2_08_SignatureMetadata app = new C2_08_SignatureMetadata();
            app.Sign(SRC, "Signature1", String.Format(DEST, 1), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Appearance 1", "Ghent", "555 123 456", new DateTime(2012, 8, 5), "Bruno L. Specimen");
        }
    }
}
