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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter2 {
    class C2_05_CustomAppearance {
        public const String KEYSTORE = "../../../../resources/pkcs12";
        public static char[] PASSWORD = "password".ToCharArray();
        public const String SRC = "../../../../resources/hello_to_sign.pdf";
        public const String DEST = "../../../../results/chapter2/signature_custom.pdf";

        public void Sign(String src, String name, String dest, ICollection<X509Certificate> chain, ICipherParameters pk,
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
            // Creating the appearance for layer 0
            PdfTemplate n0 = appearance.GetLayer(0);
            float x = n0.BoundingBox.Left;
            float y = n0.BoundingBox.Bottom;
            float width = n0.BoundingBox.Width;
            float height = n0.BoundingBox.Height;
            n0.SetColorFill(BaseColor.LIGHT_GRAY);
            n0.Rectangle(x, y, width, height);
            n0.Fill();
            // Creating the appearance for layer 2
            PdfTemplate n2 = appearance.GetLayer(2);
            ColumnText ct = new ColumnText(n2);
            ct.SetSimpleColumn(n2.BoundingBox);
            Paragraph p = new Paragraph("This document was signed by Bruno Specimen.");
            ct.AddElement(p);
            ct.Go();
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
            C2_05_CustomAppearance app = new C2_05_CustomAppearance();
            app.Sign(SRC, "Signature1", DEST, chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Custom appearance example", "Ghent");
        }
    }
}
