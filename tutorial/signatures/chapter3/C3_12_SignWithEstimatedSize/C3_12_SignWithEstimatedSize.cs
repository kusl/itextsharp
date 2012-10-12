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
using iTextSharp.text.pdf.security;

namespace signatures.chapter3 {

    public class C3_12_SignWithEstimatedSize {
        public static String DEST = "../../../../results/chapter3/hello_estimated.pdf";

        public static void Main(String[] args) {
            Properties properties = new Properties();
            properties.Load(new FileStream("c:/home/blowagie/key.properties", FileMode.Open));
            String path = properties["PRIVATE"];
            char[] pass = properties["PASSWORD"].ToCharArray();
            String tsaUrl = properties["TSAURL"];
            String tsaUser = properties["TSAUSERNAME"];
            String tsaPass = properties["TSAPASSWORD"];

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
            IOcspClient ocspClient = new OcspClientBouncyCastle();
            TSAClientBouncyCastle tsaClient = new TSAClientBouncyCastle(tsaUrl, tsaUser, tsaPass);

            C3_12_SignWithEstimatedSize app = new C3_12_SignWithEstimatedSize();
            bool succeeded = false;
            int estimatedSize = 10300;
            while (!succeeded) {
                try {
                    Console.WriteLine("Attempt: " + estimatedSize + " bytes");
                    C3_01_SignWithCAcert.Sign(DEST, chain, pk, DigestAlgorithms.SHA256, CryptoStandard.CMS, "Test", "Ghent",
                             null, ocspClient, tsaClient, estimatedSize);
                    succeeded = true;
                    Console.WriteLine("Succeeded!");
                }
                catch (IOException ioe) {
                    Console.WriteLine("Not succeeded: " + ioe.Message);
                    estimatedSize += 50;
                }
            }
            Console.ReadKey();
        }
    }
}
