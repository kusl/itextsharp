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
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter2 {
    class C2_10_SequentialSignatures {
        public const String FORM = "../../../../results/chapter2/multiple_signatures.pdf";
        public const String ALICE = "../../../../resources/alice";
        public const String BOB = "../../../../resources/bob";
        public const String CAROL = "../../../../resources/carol";
        public static char[] PASSWORD = "password".ToCharArray();
        public const String DEST = "../../../../results/chapter2/signed_by_{0}.pdf";

        public void CreateForm() {
		    Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FORM, FileMode.Create));
		    document.Open();
		    PdfPTable table = new PdfPTable(1);
		    table.WidthPercentage = 100;
		    table.AddCell("Signer 1: Alice");
		    table.AddCell(CreateSignatureFieldCell(writer, "sig1"));
		    table.AddCell("Signer 2: Bob");
		    table.AddCell(CreateSignatureFieldCell(writer, "sig2"));
		    table.AddCell("Signer 3: Carol");
		    table.AddCell(CreateSignatureFieldCell(writer, "sig3"));
		    document.Add(table);
		    document.Close();
	    }
    	
	    protected PdfPCell CreateSignatureFieldCell(PdfWriter writer, String name) {
		    PdfPCell cell = new PdfPCell();
		    cell.MinimumHeight = 50;
		    PdfFormField field = PdfFormField.CreateSignature(writer);
            field.FieldName = name;
            field.Flags = PdfAnnotation.FLAGS_PRINT;
            cell.CellEvent = new MySignatureFieldEvent(field);
		    return cell;
	    }
    	
	    public class MySignatureFieldEvent : IPdfPCellEvent {
		    public PdfFormField field;
    		
		    public MySignatureFieldEvent(PdfFormField field) {
			    this.field = field;
		    }
    		
		    public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases) {
			    PdfWriter writer = canvases[0].PdfWriter;
			    field.SetPage();
			    field.SetWidget(position, PdfAnnotation.HIGHLIGHT_INVERT);
			    writer.AddAnnotation(field);
		    }
	    }

        public void Sign(String keystore, int level, String src, String name, String dest) {
            Pkcs12Store store = new Pkcs12Store(new FileStream(keystore, FileMode.Open), PASSWORD);
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

            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0', null, true);
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.SetVisibleSignature(name);
            appearance.CertificationLevel = level;
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(parameters, "SHA-256");
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);
        }

        static void Main(string[] args) {
            C2_10_SequentialSignatures app = new C2_10_SequentialSignatures();
            app.CreateForm();

            app.Sign(ALICE, PdfSignatureAppearance.CERTIFIED_FORM_FILLING, FORM, "sig1", String.Format(DEST, "alice"));
            app.Sign(BOB, PdfSignatureAppearance.NOT_CERTIFIED, String.Format(DEST, "alice"), "sig2", String.Format(DEST, "bob"));
            app.Sign(CAROL, PdfSignatureAppearance.NOT_CERTIFIED, String.Format(DEST, "bob"), "sig3", String.Format(DEST, "carol"));

            app.Sign(ALICE, PdfSignatureAppearance.NOT_CERTIFIED, FORM, "sig1", String.Format(DEST, "alice2"));
            app.Sign(BOB, PdfSignatureAppearance.NOT_CERTIFIED, String.Format(DEST, "alice2"), "sig2", String.Format(DEST, "bob2"));
            app.Sign(CAROL, PdfSignatureAppearance.CERTIFIED_FORM_FILLING, String.Format(DEST, "bob2"), "sig3", String.Format(DEST, "carol2"));

            app.Sign(ALICE, PdfSignatureAppearance.NOT_CERTIFIED, FORM, "sig1", String.Format(DEST, "alice3"));
            app.Sign(BOB, PdfSignatureAppearance.NOT_CERTIFIED, String.Format(DEST, "alice3"), "sig2", String.Format(DEST, "bob3"));
            app.Sign(CAROL, PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED, String.Format(DEST, "bob3"), "sig3", String.Format(DEST, "carol3"));

            app.Sign(ALICE, PdfSignatureAppearance.CERTIFIED_FORM_FILLING, FORM, "sig1", String.Format(DEST, "alice4"));
            app.Sign(BOB, PdfSignatureAppearance.NOT_CERTIFIED, String.Format(DEST, "alice4"), "sig2", String.Format(DEST, "bob4"));
            app.Sign(CAROL, PdfSignatureAppearance.CERTIFIED_FORM_FILLING, String.Format(DEST, "bob4"), "sig3", String.Format(DEST, "carol4"));
        }
    }
}
