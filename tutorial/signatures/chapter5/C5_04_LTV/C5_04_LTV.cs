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
using System.util;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter5 {
    class C5_04_LTV {
        public const String EXAMPLE1 = "../../../../results/chapter3/hello_token.pdf";
        public const String EXAMPLE2 = "../../../../results/chapter4/hello_smartcard_Signature.pdf";
        public const String EXAMPLE3 = "../../../../results/chapter3/hello_cacert_ocsp_ts.pdf";
        public const String DEST = "../../../../results/chapter5/ltv_{0}.pdf";
    	
	    public static void Main(String[] args) {
            LoggerFactory.GetInstance().SetLogger(new SysoLogger());
		    Properties properties = new Properties();
		    properties.Load(new FileStream("c:/home/blowagie/key.properties", FileMode.Open));
            String tsaUrl = properties["TSAURL"];
            String tsaUser = properties["TSAUSERNAME"];
            String tsaPass = properties["TSAPASSWORD"];
            C5_04_LTV app = new C5_04_LTV();
            ITSAClient tsa = new TSAClientBouncyCastle(tsaUrl, tsaUser, tsaPass, 6500, "SHA512");
            IOcspClient ocsp = new OcspClientBouncyCastle();
            app.AddLtv(EXAMPLE1, String.Format(DEST, 1), ocsp, new CrlClientOnline(), tsa);
            Console.WriteLine();
            app.AddLtv(EXAMPLE2, String.Format(DEST, 2), ocsp, new CrlClientOnline(), tsa);
            Console.WriteLine();
            app.AddLtv(EXAMPLE3, String.Format(DEST, 3), ocsp, new CrlClientOnline(), tsa);
            Console.WriteLine();
            app.AddLtv(String.Format(DEST, 1), String.Format(DEST, 4), null, new CrlClientOnline(), tsa);
	    }
    	
	    public void AddLtv(String src, String dest, IOcspClient ocsp, ICrlClient crl, ITSAClient tsa) {
            PdfReader r = new PdfReader(src);
            FileStream fos = new FileStream(dest, FileMode.Create);
            PdfStamper stp = PdfStamper.CreateSignature(r, fos, '\0', null, true);
            LtvVerification v = stp.LtvVerification;
            AcroFields fields = stp.AcroFields;
		    List<String> names = fields.GetSignatureNames();
            String sigName = names[names.Count - 1];
		    PdfPKCS7 pkcs7 = fields.VerifySignature(sigName);
            if (pkcs7.IsTsp) 
        	    v.AddVerification(sigName, ocsp, crl, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.NO);
            else foreach (String name in names)
        		v.AddVerification(name, ocsp, crl, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.NO);
            PdfSignatureAppearance sap = stp.SignatureAppearance;
            LtvTimestamp.Timestamp(sap, tsa, null); 
	    }
    }
}
