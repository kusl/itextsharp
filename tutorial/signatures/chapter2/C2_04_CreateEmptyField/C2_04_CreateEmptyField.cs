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
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter2 {
    class C2_04_CreateEmptyField {

        public const String KEYSTORE = "../../../../resources/pkcs12";
        public static char[] PASSWORD = "password".ToCharArray();
        public const String SRC = "../../../../resources/hello.pdf";
        public const String SIGNAME = "Signature1";
        public const String DEST = "../../../../results/chapter2/field_signed.pdf";
        public const String UNSIGNED = "../../../../results/chapter2/hello_empty.pdf";
        public const String UNSIGNED2 = "../../../../results/chapter2/hello_empty.pdf2";

        public void CreatePdf(String filename) {
    	    // step 1: Create a Document
            Document document = new Document();
            // step 2: Create a PdfWriter
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filename, FileMode.Create));
            // step 3: Open the Document
            document.Open();
            // step 4: Add content
            document.Add(new Paragraph("Hello World!"));
            // create a signature form field
            PdfFormField field = PdfFormField.CreateSignature(writer);
            field.FieldName = SIGNAME;
            // set the widget properties
            field.SetPage();
            field.SetWidget(new Rectangle(72, 732, 144, 780), PdfAnnotation.HIGHLIGHT_INVERT);
            field.Flags = PdfAnnotation.FLAGS_PRINT;
            // add it as an annotation
            writer.AddAnnotation(field);
            // maybe you want to define an appearance
            PdfAppearance tp = PdfAppearance.CreateAppearance(writer, 72, 48);
            tp.SetColorStroke(BaseColor.BLUE);
            tp.SetColorFill(BaseColor.LIGHT_GRAY);
            tp.Rectangle(0.5f, 0.5f, 71.5f, 47.5f);
            tp.FillStroke();
            tp.SetColorFill(BaseColor.BLUE);
            ColumnText.ShowTextAligned(tp, Element.ALIGN_CENTER, new Phrase("SIGN HERE"), 36, 24, 25);
            field.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, tp);
            // step 5: Close the Document
            document.Close();
        }

        public void AddField(String src, String dest) {
    	    PdfReader reader = new PdfReader(src);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create));
            // create a signature form field
            PdfFormField field = PdfFormField.CreateSignature(stamper.Writer);
            field.FieldName = SIGNAME;
            // set the widget properties
            field.SetWidget(new Rectangle(72, 732, 144, 780), PdfAnnotation.HIGHLIGHT_OUTLINE);
            field.Flags = PdfAnnotation.FLAGS_PRINT;
            // add the annotation
            stamper.AddAnnotation(field, 1);
            // close the stamper
    	    stamper.Close();
        }

        static void Main(string[] args) {
            C2_04_CreateEmptyField appCreate = new C2_04_CreateEmptyField();
            appCreate.CreatePdf(UNSIGNED);
            appCreate.AddField(SRC, UNSIGNED2);

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
            C2_03_SignEmptyField appSign = new C2_03_SignEmptyField();
            appSign.Sign(UNSIGNED, SIGNAME, DEST, chain, parameters, DigestAlgorithms.SHA256, CryptoStandard.CMS, "Test", "Ghent");
        }

    }
}
