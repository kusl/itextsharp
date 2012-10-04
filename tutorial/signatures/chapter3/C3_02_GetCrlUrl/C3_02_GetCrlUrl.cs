/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/sales
 */

using System;
using System.IO;
using System.util;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text.pdf.security;

namespace signatures.chapter3 {
    public class C3_02_GetCrlUrl {
        public static void Main(String[] args) {
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

            X509CertificateEntry[] chain = ks.GetCertificateChain(alias);
            for (int i = 0; i < chain.Length; i++) {
                X509Certificate cert = chain[i].Certificate;
                Console.WriteLine("[{0}] {1}", i, cert.SubjectDN);
                Console.WriteLine(CertificateUtil.GetCRLURL(cert));
            }
            Console.ReadKey();
        }
    }
}
