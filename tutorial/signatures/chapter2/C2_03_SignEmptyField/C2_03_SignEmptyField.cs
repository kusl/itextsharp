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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter2 {

    public class C2_03_SignEmptyField {

        public static String KEYSTORE = "../../../../resources/pkcs12";
        public static char[] PASSWORD = "password".ToCharArray();
        public static String SRC = "../../../../resources/hello_to_sign.pdf";
        public static String DEST = "../../../../results/chapter2/field_signed{0}.pdf";

        public void Sign(String src, String name, String dest,
                         ICollection<X509Certificate> chain, ICipherParameters pk,
                         String digestAlgorithm, CryptoStandard subfilter,
                         String reason, String location) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(name);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            //ExternalDigest digest = new BouncyCastleDigest();
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, subfilter);
        }

        public static void Main(String[] args) {
            Pkcs12Store store = new Pkcs12Store(new FileStream(KEYSTORE, FileMode.Open), PASSWORD);
            String alias = "";
            ICollection<X509Certificate> chain = new List<X509Certificate>();
            // searching for private key

            foreach (string al in store.Aliases) {
                if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate) {
                    alias = al;
                    break;
                }
            }

            AsymmetricKeyEntry pk = store.GetKey(alias);
            foreach (X509CertificateEntry c in store.GetCertificateChain(alias)) {
                chain.Add(c.Certificate);
            }

            RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;
            C2_03_SignEmptyField app = new C2_03_SignEmptyField();
            app.Sign(SRC, "Signature1", String.Format(DEST, 1), chain, parameters, DigestAlgorithms.SHA256, 
                     CryptoStandard.CMS, "Test 1", "Ghent");
            app.Sign(SRC, "Signature1", String.Format(DEST, 2), chain, parameters, DigestAlgorithms.SHA512, 
                     CryptoStandard.CMS, "Test 2", "Ghent");
            app.Sign(SRC, "Signature1", String.Format(DEST, 3), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CADES, "Test 3", "Ghent");
            app.Sign(SRC, "Signature1", String.Format(DEST, 4), chain, parameters, DigestAlgorithms.RIPEMD160,
                     CryptoStandard.CADES, "Test 4", "Ghent");
        }
    }
}
