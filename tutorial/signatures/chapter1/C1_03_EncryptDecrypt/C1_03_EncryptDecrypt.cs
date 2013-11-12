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
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto.Encodings;

namespace signatures.chapter1
{
    class C1_03_EncryptDecrypt
    {
        protected Pkcs12Store store;
        public const String KEYSTORE = "../../../../resources/pkcs12";

        public C1_03_EncryptDecrypt(String keystore, String ks_pass) {
            store = new Pkcs12Store(new FileStream(keystore, FileMode.Open), ks_pass.ToCharArray());
	    }

        public X509CertificateEntry GetCertificate(String alias){
            return store.GetCertificate(alias);
        }

        public AsymmetricKeyParameter GetPublicKey(String alias) {
		    return GetCertificate(alias).Certificate.GetPublicKey();
	    }

        public AsymmetricKeyEntry GetPrivateKey(String alias) {
		    return store.GetKey(alias);
	    }

        public byte[] Encrypt(ICipherParameters parameters, String message) {
            List<byte> encryptedBytes = new List<byte>();
            IAsymmetricBlockCipher cipher = new RsaEngine();
            cipher = new Pkcs1Encoding(cipher);
            cipher.Init(true, parameters);
            byte[] messageBytes = new UTF8Encoding().GetBytes(message);
            int i = 0;
            int len = cipher.GetInputBlockSize();
            while (i < messageBytes.Length)
            {
                if (i + len > messageBytes.Length)
                    len = messageBytes.Length - i;
                byte[] hexEncodedCipher = cipher.ProcessBlock(messageBytes, i, len);
                encryptedBytes.AddRange(hexEncodedCipher);
                i += cipher.GetInputBlockSize();
            }
            byte[] cipherData = new byte[encryptedBytes.Count];
            encryptedBytes.CopyTo(cipherData);
            return cipherData;
        }
    	
        public String Decrypt(ICipherParameters parameters, byte[] message) {
            List<byte> encryptedBytes = new List<byte>();
            IAsymmetricBlockCipher cipher = new RsaEngine();
            cipher = new Pkcs1Encoding(cipher);
            cipher.Init(false, parameters);
            int i = 0;
            int len = cipher.GetInputBlockSize();
            while (i < message.Length)
            {
                if (i + len > message.Length)
                    len = message.Length - i;
                byte[] hexEncodedCipher = cipher.ProcessBlock(message, i, len);
                encryptedBytes.AddRange(hexEncodedCipher);
                i += cipher.GetInputBlockSize();
            }
            byte[] cipherData = new byte[encryptedBytes.Count];
            encryptedBytes.CopyTo(cipherData);
            return new UTF8Encoding().GetString(cipherData);
        }
        
        static void Main(string[] args)
        {
            C1_03_EncryptDecrypt app = new C1_03_EncryptDecrypt(KEYSTORE, "password");
            AsymmetricKeyParameter publicKey = app.GetPublicKey("demo");
            AsymmetricKeyEntry privateKey = app.GetPrivateKey("demo");
            Console.Write("Let's encrypt 'secret message' with a public key\n");
            byte[] encrypted = app.Encrypt(publicKey, "secret message");
            Console.WriteLine("Encrypted message: " + new BigInteger(1, encrypted).ToString(16));
            Console.Write("Let's decrypt it with the corresponding private key\n");
            String decrypted = app.Decrypt(privateKey.Key, encrypted);
            Console.WriteLine(decrypted);

            Console.Write("\nYou can also encrypt the message with a private key\n");
            encrypted = app.Encrypt(privateKey.Key, "secret message");
            Console.WriteLine("Encrypted message: " + new BigInteger(1, encrypted).ToString(16));
            Console.Write("Now you need the public key to decrypt it\n");
            decrypted = app.Decrypt(publicKey, encrypted);
            Console.WriteLine(decrypted);
            Console.ReadKey();
        }
    }
}
