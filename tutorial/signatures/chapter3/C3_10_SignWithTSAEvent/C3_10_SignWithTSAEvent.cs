using System;
using System.Collections.Generic;
using System.IO;
using System.util;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iTextSharp.text.pdf.security;

namespace signatures.chapter3 {

    public class TSAInfoTimeStampLogger : ITSAInfoBouncyCastle {
        public void InspectTimeStampTokenInfo(TimeStampTokenInfo info) {
            Console.WriteLine(info.GenTime);
        }    
    }

    public class C3_10_SignWithTSAEvent : C3_01_SignWithCAcert {
        public static String DEST = "../../../../results/chapter3/hello_cacert_ocsp_ts.pdf";

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

            tsaClient.SetTSAInfo(new TSAInfoTimeStampLogger());
            C3_10_SignWithTSAEvent app = new C3_10_SignWithTSAEvent();
            app.Sign(SRC, DEST, chain, pk, DigestAlgorithms.SHA256, CryptoStandard.CMS, "Test",
                     "Ghent",
                     null, ocspClient, tsaClient, 0);
            Console.ReadKey();
        }
    }
}
