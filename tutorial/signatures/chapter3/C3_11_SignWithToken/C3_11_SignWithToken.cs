/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/sales
 */

using System;
using iTextSharp.text.log;

namespace signatures.chapter3 {

    public class C3_11_SignWithToken : C3_01_SignWithCAcert {
        public static String DEST = "../../../../results/chapter3/hello_token.pdf";

        public static void Main(String[] args) {
            LoggerFactory.GetInstance().SetLogger(new SysoLogger());

            /*BouncyCastleProvider providerBC = new BouncyCastleProvider();
            Security.AddProvider(providerBC);
            SunMSCAPI providerMSCAPI = new SunMSCAPI();
            Security.AddProvider(providerMSCAPI);
            KeyStore ks = KeyStore.GetInstance("Windows-MY");
            ks.Load(null, null);
            String alias = (String) ks.Aliases().NextElement();
            PrivateKey pk = (PrivateKey) ks.GetKey(alias, null);
            Certificate[] chain = ks.GetCertificateChain(alias);
            OcspClient ocspClient = new OcspClientBouncyCastle();
            TSAClient tsaClient = null;
            for (int i = 0; i < chain.length; i++) {
                X509Certificate cert = (X509Certificate) chain[i];
                String tsaUrl = CertificateUtil.GetTSAURL(cert);
                if (tsaUrl != null) {
                    tsaClient = new TSAClientBouncyCastle(tsaUrl);
                    break;
                }
            }
            IList<CrlClient> crlList = new IList<CrlClient>();
            crlList.Add(new CrlClientOnline(chain));
            C3_11_SignWithToken app = new C3_11_SignWithToken();
            app.Sign(SRC, DEST, chain, pk, DigestAlgorithms.SHA384, providerMSCAPI.GetName(), CryptoStandard.CMS, "Test",
                     "Ghent",
                     crlList, ocspClient, tsaClient, 0);*/
        }
    }
}
