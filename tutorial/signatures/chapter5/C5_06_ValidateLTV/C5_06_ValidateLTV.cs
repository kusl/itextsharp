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
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter5 {
    class C5_06_ValidateLTV {
        public const String ADOBE = "../../../../resources/adobeRootCA.cer";
        public const String EXAMPLE1 = "../../../../results/chapter5/ltv_1.pdf";
        public const String EXAMPLE2 = "../../../../results/chapter5/ltv_2.pdf";
        public const String EXAMPLE3 = "../../../../results/chapter5/ltv_3.pdf";
        public const String EXAMPLE4 = "../../../../results/chapter5/ltv_4.pdf";
    	
	    static void Main(String[] args) {
		    C5_06_ValidateLTV app = new C5_06_ValidateLTV();
		    Console.WriteLine(EXAMPLE1);
		    app.Validate(new PdfReader(EXAMPLE1));
		    Console.WriteLine();
		    Console.WriteLine(EXAMPLE2);
		    app.Validate(new PdfReader(EXAMPLE2));
		    Console.WriteLine();
		    Console.WriteLine(EXAMPLE3);
		    app.Validate(new PdfReader(EXAMPLE3));
		    Console.WriteLine();
		    Console.WriteLine(EXAMPLE4);
		    app.Validate(new PdfReader(EXAMPLE4));
	    }

        class MyVerifier : CertificateVerifier {
            public MyVerifier(CertificateVerifier verifier) : base(verifier) {}

            override public List<VerificationOK> Verify(X509Certificate signCert, X509Certificate issuerCert, DateTime signDate) {
				Console.WriteLine(signCert.SubjectDN + ": ALL VERIFICATIONS DONE");
				return new List<VerificationOK>();
			}
        }
    	
	    public void Validate(PdfReader reader) {
		    List<X509Certificate> certificates = new List<X509Certificate>();
            X509CertificateParser parser = new X509CertificateParser();
	        FileStream file = new FileStream(ADOBE, FileMode.Open);
            certificates.Add(parser.ReadCertificate(file));
    		
		    MyVerifier custom = new MyVerifier(null);
    		
 		    LtvVerifier data = new LtvVerifier(reader);
	        data.Certificates = certificates;
 		    data.CertificateOption = LtvVerification.CertificateOption.WHOLE_CHAIN;
 		    data.Verifier = custom;
 		    data.OnlineCheckingAllowed = false;
 		    data.VerifyRootCertificate = false;
 		    List<VerificationOK> list = new List<VerificationOK>();
 		    try {
 			    data.Verify(list);
 		    }
 		    catch (GeneralSecurityException e) {
 			    Console.WriteLine(e.ToString());
 		    }
		    Console.WriteLine();
            if (list.Count == 0)
                Console.WriteLine("The document can't be verified");
 		    foreach (VerificationOK v in list)
 			    Console.WriteLine(v.ToString());
            file.Close();
	    }
    }
}
