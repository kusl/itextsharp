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
using System.Net;
using Org.BouncyCastle.X509;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace signatures.chapter4 {
    class C4_07_ClientServerSigning {
        public const String SRC = "../../../../resources/hello.pdf";
        public const String DEST = "../../../../results/chapter4/hello_server.pdf";

	    public const String CERT = "http://demo.itextsupport.com/SigningApp/itextpdf.cer";
    	
	    public class ServerSignature : IExternalSignature {
		    public const String SIGN = "http://demo.itextsupport.com/SigningApp/signbytes";
    		
		    public String GetHashAlgorithm() {
			    return DigestAlgorithms.SHA256;
		    }

		    public String GetEncryptionAlgorithm() {
			    return "RSA";
		    }

		    public byte[] Sign(byte[] message) {
                MemoryStream baos = new MemoryStream();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SIGN);
		        request.Method = "POST";
		        Stream ostream = request.GetRequestStream();
		        ostream.Write(message, 0, message.Length);
		        ostream.Close();
		        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		        Stream istream = response.GetResponseStream();
		        byte[] b = new byte[0x1000];
	            int read;  
	            while ((read = istream.Read(b, 0, b.Length)) != 0)  
	                baos.Write(b, 0, read);
		        istream.Close();
                return baos.ToArray();
		    }
    		
	    }
    	
	    public void Sign(String src, String dest, ICollection<X509Certificate> chain,
			    CryptoStandard subfilter, String reason, String location) {
            // Creating the reader and the stamper
            PdfReader reader = new PdfReader(src);
            FileStream os = new FileStream(dest, FileMode.Create);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
            // Creating the appearance
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(new Rectangle(36, 748, 144, 780), 1, "sig");
            // Creating the signature
            IExternalSignature signature = new ServerSignature();
            MakeSignature.SignDetached(appearance, signature, chain, null, null, null, 0, subfilter);
	    }
    	
	    public static void Main(String[] args) {
            X509CertificateParser parser = new X509CertificateParser();
            Stream url = WebRequest.Create(CERT).GetResponse().GetResponseStream();
            ICollection<X509Certificate> chain = new List<X509Certificate>();
		    chain.Add(parser.ReadCertificate(url));
		    C4_07_ClientServerSigning app = new C4_07_ClientServerSigning();
		    app.Sign(SRC, DEST, chain, CryptoStandard.CMS, "Test", "Ghent");
	    }
    }
}
