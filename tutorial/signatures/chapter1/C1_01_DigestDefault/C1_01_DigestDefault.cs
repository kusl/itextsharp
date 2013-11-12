/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/learn
 */

using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace signatures.chapter1
{
    public class C1_01_DigestDefault
    {
        protected byte[] digest;
        protected HashAlgorithm hash = null;
    	
	    protected C1_01_DigestDefault(String password, String algorithm) {
	        switch (algorithm) {
                case "MD5":
                    hash = new MD5CryptoServiceProvider();
	                break;
	            case "SHA-1":
                    hash = new SHA1Managed();
                    break;
                case "SHA-256":
                    hash = new SHA256Managed();
                    break;
                case "SHA-384":
                    hash = new SHA384Managed();
                    break;
                case "SHA-512":
                    hash = new SHA512Managed();
                    break;
                case "RIPEMD160":
                    hash = new RIPEMD160Managed();
                    break;
            }
	        digest = hash.ComputeHash(new UTF8Encoding().GetBytes(password));
	    }
    	
	    public static C1_01_DigestDefault GetInstance(String password, String algorithm) {
		    return new C1_01_DigestDefault(password, algorithm);
	    }
    	
	    public int DigestSize {
            get {
                return digest.Length;
            }
	    }
    	
	    public String GetDigestAsHexString() {
	        return new BigInteger(1, digest).ToString(16);
	    }

    	
	    public bool CheckPassword(String password) {
            byte[] result = hash.ComputeHash(new UTF8Encoding().GetBytes(password));
            return Arrays.AreEqual(result, digest);
	    }
    	
	    public static void ShowTest(String algorithm) {
		    try {
			    C1_01_DigestDefault app = GetInstance("password", algorithm);
			    Console.WriteLine("Digest using " + algorithm + ": " + app.DigestSize);
			    Console.WriteLine("Digest: " + app.GetDigestAsHexString());
			    Console.WriteLine("Is the password 'password'? " + app.CheckPassword("password"));
			    Console.WriteLine("Is the password 'secret'? " + app.CheckPassword("secret"));
		    } catch (GeneralSecurityException e) {
                Console.WriteLine(e.Message);
		    }
	    }
    	
	    public static void TestAll() {
		    ShowTest("MD5");
		    ShowTest("SHA-1");
		    ShowTest("SHA-256");
		    ShowTest("SHA-384");
		    ShowTest("SHA-512");
		    ShowTest("RIPEMD160");
	    }
    	
	    static void Main(String[] args) {
		    TestAll();
            Console.ReadKey();
	    }
    }
}
