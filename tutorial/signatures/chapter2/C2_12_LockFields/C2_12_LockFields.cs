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
    class C2_12_LockFields {
        public const String FORM = "../../../../results/chapter2/form_lock.pdf";
        public const String ALICE = "../../../../resources/alice";
        public const String BOB = "../../../../resources/bob";
        public const String CAROL = "../../../../resources/carol";
        public const String DAVE = "../../../../resources/dave";
        public static char[] PASSWORD = "password".ToCharArray();
        public const String DEST = "../../../../results/chapter2/step_{0}_signed_by_{1}.pdf";

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
    		
		    public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases) {
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
		    table.AddCell(CreateSignatureFieldCell(writer, "sig1", null));
		    table.AddCell("For approval by Bob");
		    table.AddCell(CreateTextFieldCell("approved_bob"));
		    PdfSigLockDictionary Lock = new PdfSigLockDictionary(PdfSigLockDictionary.LockAction.INCLUDE, "sig1", "approved_bob", "sig2");
		    table.AddCell(CreateSignatureFieldCell(writer, "sig2", Lock));
		    table.AddCell("For approval by Carol");
		    table.AddCell(CreateTextFieldCell("approved_carol"));
		    Lock = new PdfSigLockDictionary(PdfSigLockDictionary.LockAction.EXCLUDE, "approved_dave", "sig4");
		    table.AddCell(CreateSignatureFieldCell(writer, "sig3", Lock));
		    table.AddCell("For approval by Dave");
		    table.AddCell(CreateTextFieldCell("approved_dave"));
		    Lock = new PdfSigLockDictionary(PdfSigLockDictionary.LockPermissions.NO_CHANGES_ALLOWED);
		    table.AddCell(CreateSignatureFieldCell(writer, "sig4", Lock));
		    document.Add(table);
		    document.Close();
	    }
    	
	    protected PdfPCell CreateTextFieldCell(String name) {
		    PdfPCell cell = new PdfPCell();
		    cell.MinimumHeight = 20;
		    cell.CellEvent = new MyTextFieldEvent(name);
		    return cell;
	    }
    	
	    protected PdfPCell CreateSignatureFieldCell(PdfWriter writer, String name, PdfDictionary Lock) {
		    PdfPCell cell = new PdfPCell();
		    cell.MinimumHeight = 50;
		    PdfFormField field = PdfFormField.CreateSignature(writer);
            field.FieldName = name;
            if (Lock != null)
        	    field.Put(PdfName.LOCK, writer.AddToBody(Lock).IndirectReference);
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
		    AcroFields form = stamper.AcroFields;
		    form.SetFieldProperty(name, "setfflags", PdfFormField.FF_READ_ONLY, null);
            // Creating the signature
            PrivateKeySignature pks = new PrivateKeySignature(parameters, DigestAlgorithms.SHA256);
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
		    form.SetFieldProperty(name, "setfflags", PdfFormField.FF_READ_ONLY, null);
		    form.SetFieldProperty(fname, "setfflags", PdfFormField.FF_READ_ONLY, null);
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.SetVisibleSignature(name);
            // Creating the signature
            PrivateKeySignature pks = new PrivateKeySignature(parameters, DigestAlgorithms.SHA256);
            MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);
	    }
    	
	    public void FillOut(String src, String dest, String name, String value) {
		    PdfReader reader = new PdfReader(src);
		    PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create), '\0', true);
		    AcroFields form = stamper.AcroFields;
		    form.SetField(name, value);
		    stamper.Close();
	    }
    	
	    public static void Main(String[] args) {
		    C2_12_LockFields app = new C2_12_LockFields();
		    app.CreateForm();
		    app.Certify(ALICE, FORM, "sig1", String.Format(DEST, 1, "alice"));
		    app.FillOutAndSign(BOB, String.Format(DEST, 1, "alice"), "sig2", "approved_bob", "Read and Approved by Bob", String.Format(DEST, 2, "alice_and_bob"));
		    app.FillOutAndSign(CAROL, String.Format(DEST, 2, "alice_and_bob"), "sig3", "approved_carol", "Read and Approved by Carol", String.Format(DEST, 3, "alice_bob_and_carol"));
		    app.FillOutAndSign(DAVE, String.Format(DEST, 3, "alice_bob_and_carol"), "sig4", "approved_dave", "Read and Approved by Dave", String.Format(DEST, 4, "alice_bob_carol_and_dave"));
		    app.FillOut(String.Format(DEST, 2, "alice_and_bob"), String.Format(DEST, 5, "alice_and_bob_broken_by_chuck"), "approved_bob", "Changed by Chuck");
		    app.FillOut(String.Format(DEST, 4, "alice_bob_carol_and_dave"), String.Format(DEST, 6, "dave_broken_by_chuck"), "approved_carol", "Changed by Chuck");
	    }
    }
}