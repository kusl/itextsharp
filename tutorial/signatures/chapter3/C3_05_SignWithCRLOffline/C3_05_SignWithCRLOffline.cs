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
using System.util;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text.log;
using iTextSharp.text.pdf.security;

namespace signatures.chapter3 {

    public class C3_05_SignWithCRLOffline {
        public static String CRLURL = "../../../../resources/revoke.crl";
        public static String DEST = "../../../../results/chapter3/hello_cacert_crl_offline.pdf";

        public static void Main(String[] args) {
            LoggerFactory.GetInstance().SetLogger(new SysoLogger());
            Properties properties = new Properties();
            properties.Load(new FileStream("c:/home/blowagie/key.properties", FileMode.Open));
            String path = properties["PRIVATE"];
            char[] pass = properties["PASSWORD"].ToCharArray();

            Pkcs12Store ks = new Pkcs12Store();
            ks.Load(new FileStream(path, FileMode.Open), pass);
            String alias = "";
            foreach (string al in ks.Aliases) {
                if (ks.IsKeyEntry(al) && ks.GetKey(al).Key.IsPrivate) {
                    alias = al;
                    break;
                }
            }
            AsymmetricKeyParameter pk = ks.GetKey(alias).Key;
            IList<X509Certificate> chain = new List<X509Certificate>();
            foreach (X509CertificateEntry entry in ks.GetCertificateChain(alias)) {
                chain.Add(entry.Certificate);    
            }
            FileStream ins  = new FileStream(CRLURL, FileMode.Open);
            MemoryStream baos = new MemoryStream();
            byte[] buf = new byte[1024];
            int readedBytes;
            while ((readedBytes = ins.Read(buf, 0, 1024)) > 0) baos.Write(buf, 0, readedBytes);
            ins.Close();
            ICrlClient crlClient = new CrlClientOffline(baos.ToArray());
            
            X509CrlParser crlParser = new X509CrlParser();
            X509Crl crl = crlParser.ReadCrl(new FileStream(CRLURL, FileMode.Open));
            Console.WriteLine("CRL valid until: " + crl.NextUpdate);
            Console.WriteLine("Certificate revoked: " + crl.IsRevoked(chain[0]));

            IList<ICrlClient> crlList = new List<ICrlClient>();
            crlList.Add(crlClient);
            C3_01_SignWithCAcert.Sign(DEST, chain, pk, DigestAlgorithms.SHA256, CryptoStandard.CMS, "Test",
                     "Ghent",
                     crlList, null, null, 0);
        }
    }
}
