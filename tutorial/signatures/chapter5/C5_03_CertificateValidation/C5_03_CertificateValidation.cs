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
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace signatures.chapter5 {
    class C5_03_CertificateValidation : C5_01_SignatureIntegrity {
        public const String ADOBE = "../../../../resources/adobeRootCA.cer";
        public const String CACERT = "../../../../resources/CACertSigningAuthority.crt";
        public const String BRUNO = "../../../../resources/bruno.crt";

        public const String EXAMPLE1 = "../../../../results/chapter3/hello_cacert_ocsp_ts.pdf";
        public const String EXAMPLE2 = "../../../../results/chapter3/hello_token.pdf";
        public const String EXAMPLE3 = "../../../../results/chapter2/hello_signed1.pdf";
        public const String EXAMPLE4 = "../../../../results/chapter4/hello_smartcard_Signature.pdf";

        readonly private List<X509Certificate> certificates = new List<X509Certificate>();

	    override public PdfPKCS7 VerifySignature(AcroFields fields, String name) {
		    PdfPKCS7 pkcs7 = base.VerifySignature(fields, name);
		    X509Certificate[] certs = pkcs7.SignCertificateChain;
		    DateTime cal = pkcs7.SignDate;
            
		    Object[] errors = CertificateVerification.VerifyCertificates(certs, certificates, null, cal);
		    if (errors == null)
			    Console.WriteLine("Certificates verified against the KeyStore");
		    else
		        foreach (object error in errors)
		            Console.WriteLine(error);
		    for (int i = 0; i < certs.Length; ++i) {
			    X509Certificate cert = certs[i];
			    Console.WriteLine("=== Certificate " + i + " ===");
			    ShowCertificateInfo(cert, cal.ToLocalTime());
		    }
		    X509Certificate signCert = certs[0];
		    X509Certificate issuerCert = (certs.Length > 1 ? certs[1] : null);
		    Console.WriteLine("=== Checking validity of the document at the time of signing ===");
		    CheckRevocation(pkcs7, signCert, issuerCert, cal);
		    Console.WriteLine("=== Checking validity of the document today ===");
		    CheckRevocation(pkcs7, signCert, issuerCert, DateTime.Now);
		    return pkcs7;
	    }
    	
	    public static void CheckRevocation(PdfPKCS7 pkcs7, X509Certificate signCert, X509Certificate issuerCert, DateTime date) {
		    List<BasicOcspResp> ocsps = new List<BasicOcspResp>();
		    if (pkcs7.Ocsp != null)
			    ocsps.Add(pkcs7.Ocsp);
		    OcspVerifier ocspVerifier = new OcspVerifier(null, ocsps);
		    List<VerificationOK> verification =
			    ocspVerifier.Verify(signCert, issuerCert, date);
		    if (verification.Count == 0) {
			    List<X509Crl> crls = new List<X509Crl>();
			    if (pkcs7.CRLs != null)
				    foreach (X509Crl crl in pkcs7.CRLs)
					    crls.Add(crl);
			    CrlVerifier crlVerifier = new CrlVerifier(null, crls);
			    verification.AddRange(crlVerifier.Verify(signCert, issuerCert, date));
		    }
		    if (verification.Count == 0)
			    Console.WriteLine("The signing certificate couldn't be verified");
		    else
			    foreach (VerificationOK v in verification)
				    Console.WriteLine(v);
	    }

	    public void ShowCertificateInfo(X509Certificate cert, DateTime signDate) {
		    Console.WriteLine("Issuer: " + cert.IssuerDN);
		    Console.WriteLine("Subject: " + cert.SubjectDN);
            Console.WriteLine("Valid from: " + cert.NotBefore.ToString("yyyy-MM-dd HH:mm:ss.ff"));
            Console.WriteLine("Valid to: " + cert.NotAfter.ToString("yyyy-MM-dd HH:mm:ss.ff"));
		    try {
			    cert.CheckValidity(signDate);
			    Console.WriteLine("The certificate was valid at the time of signing.");
		    } catch (CertificateExpiredException e) {
			    Console.WriteLine("The certificate was expired at the time of signing.");
		    } catch (CertificateNotYetValidException e) {
			    Console.WriteLine("The certificate wasn't valid yet at the time of signing.");
		    }
		    try {
			    cert.CheckValidity();
			    Console.WriteLine("The certificate is still valid.");
		    } catch (CertificateExpiredException e) {
			    Console.WriteLine("The certificate has expired.");
		    } catch (CertificateNotYetValidException e) {
			    Console.WriteLine("The certificate isn't valid yet.");
		    }
	    }

	    static void Main(String[] args) {
		    LoggerFactory.GetInstance().SetLogger(new SysoLogger());
		    C5_03_CertificateValidation app = new C5_03_CertificateValidation();
            
		    X509CertificateParser parser = new X509CertificateParser();
            app.certificates.Add(parser.ReadCertificate(new FileStream(ADOBE, FileMode.Open)));
            app.certificates.Add(parser.ReadCertificate(new FileStream(CACERT, FileMode.Open)));
            app.certificates.Add(parser.ReadCertificate(new FileStream(BRUNO, FileMode.Open)));
		    app.VerifySignatures(EXAMPLE1);
		    app.VerifySignatures(EXAMPLE2);
		    app.VerifySignatures(EXAMPLE3);
		    app.VerifySignatures(EXAMPLE4);
	    }
    }
}
