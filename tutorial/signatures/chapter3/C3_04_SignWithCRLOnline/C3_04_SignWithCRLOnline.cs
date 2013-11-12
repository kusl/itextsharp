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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text.log;
using iTextSharp.text.pdf.security;

namespace signatures.chapter3 {

    public class C3_04_SignWithCRLOnline {
        public static String DEST = "../../../../results/chapter3/hello_cacert_crl.pdf";

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
            ICollection<X509Certificate> chain = new List<X509Certificate>();
            foreach (X509CertificateEntry entry in ks.GetCertificateChain(alias)) {
                chain.Add(entry.Certificate);
            }
            ICrlClient crlClient = new CrlClientOnline("https://crl.cacert.org/revoke.crl");
            IList<ICrlClient> crlList = new List<ICrlClient>();
            crlList.Add(crlClient);
            C3_01_SignWithCAcert.Sign(DEST, chain, pk, DigestAlgorithms.SHA256, CryptoStandard.CMS, "Test", "Ghent",
                     crlList, null, null, 0);
        }
    }
}
