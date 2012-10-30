/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/sales
 */

using System;
using System.Collections.Generic;
using Org.BouncyCastle.X509;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter5 {
    class C5_05_CheckLTV {
        public const String EXAMPLE1 = "../../../../results/chapter5/ltv_1.pdf";
        public const String EXAMPLE2 = "../../../../results/chapter5/ltv_2.pdf";
        public const String EXAMPLE3 = "../../../../results/chapter5/ltv_3.pdf";
        public const String EXAMPLE4 = "../../../../results/chapter5/ltv_4.pdf";

	    public PdfPKCS7 VerifySignature(AcroFields fields, String name) {
		    Console.WriteLine("Signature covers whole document: " + fields.SignatureCoversWholeDocument(name));
		    Console.WriteLine("Document revision: " + fields.GetRevision(name) + " of " + fields.TotalRevisions);
            PdfPKCS7 pkcs7 = fields.VerifySignature(name);
            Console.WriteLine("Integrity check OK? " + pkcs7.Verify());
		    Console.WriteLine("Digest algorithm: " + pkcs7.GetHashAlgorithm());
		    Console.WriteLine("Encryption algorithm: " + pkcs7.GetEncryptionAlgorithm());
		    Console.WriteLine("Filter subtype: " + pkcs7.GetFilterSubtype());
		    X509Certificate cert = pkcs7.SigningCertificate;
		    Console.WriteLine("Name of the signer: " + CertificateInfo.GetSubjectFields(cert).GetField("CN"));
            return pkcs7;
	    }
    	
	    public void VerifySignatures(String path) {
		    Console.WriteLine(path);
            PdfReader reader = new PdfReader(path);
            AcroFields fields = reader.AcroFields;
            List<String> names = fields.GetSignatureNames();
		    foreach (String name in names) {
			    Console.WriteLine("===== " + name + " =====");
			    VerifySignature(fields, name);
		    }
		    Console.WriteLine();
	    }
    	
	    public static void Main(String[] args) {
		    LoggerFactory.GetInstance().SetLogger(new SysoLogger());
		    C5_05_CheckLTV app = new C5_05_CheckLTV();
		    app.VerifySignatures(EXAMPLE1);
		    app.VerifySignatures(EXAMPLE2);
		    app.VerifySignatures(EXAMPLE3);
		    app.VerifySignatures(EXAMPLE4);
	    }
    }
}
