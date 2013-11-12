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
    class C2_11_SignatureWorkflow {
        public const String FORM = "../../../../results/chapter2/form.pdf";
        public const String ALICE = "../../../../resources/alice";
        public const String BOB = "../../../../resources/bob";
        public const String CAROL = "../../../../resources/carol";
        public const String DAVE = "../../../../resources/dave";
        public static char[] PASSWORD = "password".ToCharArray();
        public const String DEST = "../../../../results/chapter2/step{0}_signed_by_{1}.pdf";

        public class MyTextFieldEvent : IPdfPCellEvent {
		    public String name;
    		
		    public MyTextFieldEvent(String name) {
			    this.name = name;
		    }

		    public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases) {
			    PdfWriter writer = canvases[0].PdfWriter;
			    TextField text = new TextField(writer, position, name);
			    writer.AddAnnotation(text.GetTextField());
		    }
	    }
    	
	    public class MySignatureFieldEvent : IPdfPCellEvent {
		    public PdfFormField field;
    		
		    public MySignatureFieldEvent(PdfFormField field) {
			    this.field = field;
		    }
    		
		    public void CellLayout(PdfPCell cell, Rectangle position,
				    PdfContentByte[] canvases) {
			    PdfWriter writer = canvases[0].PdfWriter;
			    field.SetPage();
                field.SetWidget(position, PdfAnnotation.HIGHLIGHT_INVERT);
			    writer.AddAnnotation(field);
		    }
	    }
    	
	    public void CreateForm() {
		    Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FORM, FileMode.Create));
		    document.Open();
		    PdfPTable table = new PdfPTable(1);
		    table.WidthPercentage = 100;
		    table.AddCell("Written by Alice");
		    table.AddCell(CreateSignatureFieldCell(writer, "sig1"));
		    table.AddCell("For approval by Bob");
		    table.AddCell(CreateTextFieldCell("approved_bob"));
		    table.AddCell(CreateSignatureFieldCell(writer, "sig2"));
		    table.AddCell("For approval by Carol");
		    table.AddCell(CreateTextFieldCell("approved_carol"));
		    table.AddCell(CreateSignatureFieldCell(writer, "sig3"));
		    table.AddCell("For approval by Dave");
		    table.AddCell(CreateTextFieldCell("approved_dave"));
		    table.AddCell(CreateSignatureFieldCell(writer, "sig4"));
		    document.Add(table);
		    document.Close();
	    }
    	
	    protected PdfPCell CreateTextFieldCell(String name) {
		    PdfPCell cell = new PdfPCell();
		    cell.MinimumHeight = 20;
		    cell.CellEvent = new MyTextFieldEvent(name);
		    return cell;
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
    	
	    public void Certify(String keystore, String src, String name, String dest) {
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
            appearance.CertificationLevel = PdfSignatureAppearance.CERTIFIED_FORM_FILLING;
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(parameters, "SHA-256");
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);
	    }
    	
	    public void FillOut(String src, String dest, String name, String value) {
		    PdfReader reader = new PdfReader(src);
		    PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create), '\0', true);
		    AcroFields form = stamper.AcroFields;
            form.SetField(name, value);
            form.SetFieldProperty(name, "setfflags", PdfFormField.FF_READ_ONLY, null);
		    stamper.Close();
	    }
    	
	    public void Sign(String keystore, String src, String name, String dest) {
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
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(parameters, "SHA-256");
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);
	    }
    	
	    public void FillOutAndSign(String keystore, String src, String name, String fname, String value, String dest) {
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
		    AcroFields form = stamper.AcroFields;
            form.SetField(fname, value);
            form.SetFieldProperty(fname, "setfflags", PdfFormField.FF_READ_ONLY, null);
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.SetVisibleSignature(name);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(parameters, "SHA-256");
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);
	    }
    	
	    public static void Main(String[] args) {
		    C2_11_SignatureWorkflow app = new C2_11_SignatureWorkflow();
		    app.CreateForm();
		    app.Certify(ALICE, FORM, "sig1", String.Format(DEST, 1, "alice"));
		    app.FillOut(String.Format(DEST, 1, "alice"), String.Format(DEST, 2, "alice_and_filled_out_by_bob"), "approved_bob", "Read and Approved by Bob");
		    app.Sign(BOB, String.Format(DEST, 2, "alice_and_filled_out_by_bob"), "sig2", String.Format(DEST, 3, "alice_and_bob"));
		    app.FillOut(String.Format(DEST, 3, "alice_and_bob"), String.Format(DEST, 4, "alice_and_bob_filled_out_by_carol"), "approved_carol", "Read and Approved by Carol");
		    app.Sign(CAROL, String.Format(DEST, 4, "alice_and_bob_filled_out_by_carol"), "sig3", String.Format(DEST, 5, "alice_bob_and_carol"));
		    app.FillOutAndSign(DAVE, String.Format(DEST, 5, "alice_bob_and_carol"), "sig4", "approved_dave", "Read and Approved by Dave", String.Format(DEST, 6, "alice_bob_carol_and_dave"));
	    }
    }
}