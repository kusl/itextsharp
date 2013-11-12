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
    class C2_09_SignatureTypes {
        public const String KEYSTORE = "../../../../resources/pkcs12";
        public static char[] PASSWORD = "password".ToCharArray();
        public const String SRC = "../../../../resources/hello.pdf";
        public const String DEST = "../../../../results/chapter2/hello_level_{0}.pdf";

        public void Sign(String src, String dest, ICollection<X509Certificate> chain, ICipherParameters pk,
                String digestAlgorithm, CryptoStandard subfilter, int certificationLevel, String reason, String location) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(new Rectangle(36, 748, 144, 780), 1, "sig");
            appearance.CertificationLevel = certificationLevel;
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, subfilter);
        }

        public void AddText(String src, String dest) {
		    PdfReader reader = new PdfReader(src);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create), '\0', true);
		    ColumnText.ShowTextAligned(stamper.GetOverContent(1), Element.ALIGN_LEFT, new Phrase("TOP SECRET"), 36, 820, 0);
		    stamper.Close();
	    }
    	
	    public void AddAnnotation(String src, String dest) {
		    PdfReader reader = new PdfReader(src);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create), '\0', true);
		    PdfAnnotation comment = PdfAnnotation.CreateText(stamper.Writer,
				    new Rectangle(200, 800, 250, 820), "Finally Signed!",
				    "Bruno Specimen has finally signed the document", true, "Comment");
		    stamper.AddAnnotation(comment, 1);
		    stamper.Close();
	    }
    	
	    public void AddWrongAnnotation(String src, String dest) {
		    PdfReader reader = new PdfReader(src);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create));
		    PdfAnnotation comment = PdfAnnotation.CreateText(stamper.Writer,
				    new Rectangle(200, 800, 250, 820), "Finally Signed!",
				    "Bruno Specimen has finally signed the document", true, "Comment");
		    stamper.AddAnnotation(comment, 1);
		    stamper.Close();
	    }

        public void SignAgain(String src, String dest, ICollection<X509Certificate> chain, ICipherParameters pk,
               String digestAlgorithm, CryptoStandard subfilter, String reason, String location) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0', null, true);
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(new Rectangle(36, 700, 144, 732), 1, "Signature2");
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, subfilter);
        }

        public static void Main(String[] args) {
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
            C2_09_SignatureTypes app = new C2_09_SignatureTypes();
            app.Sign(SRC, String.Format(DEST, 1), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, PdfSignatureAppearance.NOT_CERTIFIED, "Test 1", "Ghent");
            app.Sign(SRC, String.Format(DEST, 2), chain, parameters, DigestAlgorithms.SHA512,
                     CryptoStandard.CMS, PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS, "Test 1", "Ghent");
            app.Sign(SRC, String.Format(DEST, 3), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CADES, PdfSignatureAppearance.CERTIFIED_FORM_FILLING, "Test 1", "Ghent");
            app.Sign(SRC, String.Format(DEST, 4), chain, parameters, DigestAlgorithms.RIPEMD160,
                     CryptoStandard.CADES, PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED, "Test 1", "Ghent");

            app.AddWrongAnnotation(String.Format(DEST, 1), String.Format(DEST, "1_annotated_wrong"));
            app.AddAnnotation(String.Format(DEST, 1), String.Format(DEST, "1_annotated"));
            app.AddAnnotation(String.Format(DEST, 2), String.Format(DEST, "2_annotated"));
            app.AddAnnotation(String.Format(DEST, 3), String.Format(DEST, "3_annotated"));
            app.AddAnnotation(String.Format(DEST, 4), String.Format(DEST, "4_annotated"));
            app.AddText(String.Format(DEST, 1), String.Format(DEST, "1_text"));

            app.SignAgain(String.Format(DEST, 1), String.Format(DEST, "1_double"), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Second signature test", "Ghent");
            app.SignAgain(String.Format(DEST, 2), String.Format(DEST, "2_double"), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Second signature test", "Ghent");
            app.SignAgain(String.Format(DEST, 3), String.Format(DEST, "3_double"), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Second signature test", "Ghent");
            app.SignAgain(String.Format(DEST, 4), String.Format(DEST, "4_double"), chain, parameters, DigestAlgorithms.SHA256,
                     CryptoStandard.CMS, "Second signature test", "Ghent");
        }
    }
}