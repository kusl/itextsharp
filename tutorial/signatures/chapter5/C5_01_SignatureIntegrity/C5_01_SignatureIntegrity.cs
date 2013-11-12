/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/learn
 */

using System;
using System.Collections.Generic;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter5 {
    public class C5_01_SignatureIntegrity {
        public const String EXAMPLE1 = "../../../../results/chapter2/hello_level_1_annotated_wrong.pdf";
	    public const String EXAMPLE2 = "../../../../results/chapter2/step_4_signed_by_alice_bob_carol_and_dave.pdf";
	    public const String EXAMPLE3 = "../../../../results/chapter2/step_6_signed_by_dave_broken_by_chuck.pdf";

	    virtual public PdfPKCS7 VerifySignature(AcroFields fields, String name) {
		    Console.WriteLine("Signature covers whole document: " + fields.SignatureCoversWholeDocument(name));
		    Console.WriteLine("Document revision: " + fields.GetRevision(name) + " of " + fields.TotalRevisions);
            PdfPKCS7 pkcs7 = fields.VerifySignature(name);
            Console.WriteLine("Integrity check OK? " + pkcs7.Verify());
            return pkcs7;
	    }
    	
	    public void VerifySignatures(String path) {
		    Console.WriteLine(path);
            PdfReader reader = new PdfReader(path);
            AcroFields fields = reader.AcroFields;
            List<String> names = fields.GetSignatureNames();
	        foreach (string name in names) {
	            Console.WriteLine("===== " + name + " =====");
			    VerifySignature(fields, name);
	        }
	        Console.WriteLine();
	    }
    	
	    public static void Main(String[] args) {
		    LoggerFactory.GetInstance().SetLogger(new SysoLogger());
		    C5_01_SignatureIntegrity app = new C5_01_SignatureIntegrity();
		    app.VerifySignatures(EXAMPLE1);
		    app.VerifySignatures(EXAMPLE2);
		    app.VerifySignatures(EXAMPLE3);
	    }
    }
}
