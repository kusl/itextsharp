/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/sales
 */

using System;
using System.IO;
using System.Net;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;

namespace signatures.chapter4 {
    class C4_08_ServerClientSigning {
        public const String CERT = "../../../../resources/bruno.crt";
	    public const String KEYSTORE = "../../../../resources/pkcs12";
	    public static char[] PASSWORD = "password".ToCharArray();
	    public const String DEST = "../../../../results/chapter4/hello_server2.pdf";

	    public const String PRE = "http://demo.itextsupport.com/SigningApp/presign";
	    public const String POST = "http://demo.itextsupport.com/SigningApp/postsign";
    	
	    public static void Main(String[] args) {
		    // we make a connection to a PreSign servlet
		    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PRE);
	        request.Method = "POST";
	        // we upload our self-signed certificate
	        Stream os = request.GetRequestStream();
	        FileStream fis = new FileStream(CERT, FileMode.Open);
		    int read;
		    byte[] data = new byte[0x100];
		    while ((read = fis.Read(data, 0, data.Length)) != 0)
			    os.Write(data, 0, read);
	        os.Flush();
	        os.Close();
	        
	        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // we use cookies to maintain a session
	        String cookies = response.Headers["Set-Cookie"];
	        // we receive a hash that needs to be signed
	        Stream istream = response.GetResponseStream();
	        MemoryStream baos = new MemoryStream();
	        data = new byte[0x100];
            while ((read = istream.Read(data, 0, data.Length)) != 0)  
                baos.Write(data, 0, read);  
            istream.Close();
	        byte[] hash = baos.ToArray();
    	    
	        // we load our private key from the key store
            Pkcs12Store store = new Pkcs12Store(new FileStream(KEYSTORE, FileMode.Open), PASSWORD);
            String alias = "";
            // searching for private key
            foreach (string al in store.Aliases)
                if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate) {
                    alias = al;
                    break;
                }
            AsymmetricKeyEntry pk = store.GetKey(alias);

            // we sign the hash received from the server
            ISigner sig = SignerUtilities.GetSigner("SHA256withRSA");
		    sig.Init(true, pk.Key);
            sig.BlockUpdate(hash, 0, hash.Length);
            data = sig.GenerateSignature();
    		
		    // we make a connection to the PostSign Servlet
            request = (HttpWebRequest)WebRequest.Create(POST);
            request.Headers.Add(HttpRequestHeader.Cookie,cookies.Split(";".ToCharArray(), 2)[0]);
	        request.Method = "POST";
	        // we upload the signed bytes
	        os = request.GetRequestStream();
	        os.Write(data, 0, data.Length);
	        os.Flush();
	        os.Close();

	        // we receive the signed document
            response = (HttpWebResponse)request.GetResponse();
	        istream = response.GetResponseStream();
	        FileStream fos = new FileStream(DEST, FileMode.Create);
	        data = new byte[0x100];
            while ((read = istream.Read(data, 0, data.Length)) != 0)
                fos.Write(data, 0, read);
	        istream.Close();
	        fos.Flush();
	        fos.Close();
	    }
    }
}
