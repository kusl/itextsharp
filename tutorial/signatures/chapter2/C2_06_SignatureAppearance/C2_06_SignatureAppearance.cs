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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter2 {
    class C2_06_SignatureAppearance {
        public const String KEYSTORE = "../../../../resources/pkcs12";
        public static char[] PASSWORD = "password".ToCharArray();
        public const String SRC = "../../../../resources/hello_to_sign.pdf";
        public const String DEST = "../../../../results/chapter2/signature_appearance{0}.pdf";
        public const String IMG = "../../../../resources/1t3xt.gif";

        public void Sign1(String src, String name, String dest, ICollection<X509Certificate> chain, ICipherParameters pk,
                         String digestAlgorithm, CryptoStandard subfilter, String reason, String location) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(name);
            // Custom text and custom font
            appearance.Layer2Text = "This document was signed by Bruno Specimen";
            appearance.Layer2Font = new Font(Font.FontFamily.TIMES_ROMAN);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, subfilter);
        }

        public void Sign2(String src, String name, String dest, ICollection<X509Certificate> chain, ICipherParameters pk,
                         String digestAlgorithm, CryptoStandard subfilter, String reason, String location) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(name);
            // Custom text, custom font, and right-to-left writing
            appearance.Layer2Text = "\u0644\u0648\u0631\u0627\u0646\u0633 \u0627\u0644\u0639\u0631\u0628";
            appearance.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            appearance.Layer2Font = new Font(BaseFont.CreateFont("C:/windows/fonts/arialuni.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 12);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, subfilter);
        }

        public void Sign3(String src, String name, String dest, ICollection<X509Certificate> chain, ICipherParameters pk,
                         String digestAlgorithm, CryptoStandard subfilter, String reason, String location) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(name);
            // Custom text and background image
            appearance.Layer2Text = "This document was signed by Bruno Specimen";
            appearance.Image = Image.GetInstance(IMG);
            appearance.ImageScale = 1;
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, subfilter);
        }

        public void Sign4(String src, String name, String dest, ICollection<X509Certificate> chain, ICipherParameters pk,
                         String digestAlgorithm, CryptoStandard subfilter, String reason, String location) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(name);
            // Default text and scaled background image
            appearance.Image = Image.GetInstance(IMG);
            appearance.ImageScale = -1;
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
                if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate)
                {
                    alias = al;
                    break;
                }

            AsymmetricKeyEntry pk = store.GetKey(alias);
            foreach (X509CertificateEntry c in store.GetCertificateChain(alias))
                chain.Add(c.Certificate);

            RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;
            C2_06_SignatureAppearance app = new C2_06_SignatureAppearance();
            app.Sign1(SRC, "Signature1", String.Format(DEST, 1), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Custom appearance example", "Ghent");
            app.Sign2(SRC, "Signature1", String.Format(DEST, 2), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Custom appearance example", "Ghent");
            app.Sign3(SRC, "Signature1", String.Format(DEST, 3), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Custom appearance example", "Ghent");
            app.Sign4(SRC, "Signature1", String.Format(DEST, 4), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Custom appearance example", "Ghent");
        }
    }
}
