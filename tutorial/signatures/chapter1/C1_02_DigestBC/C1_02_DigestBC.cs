/*
 * This class is part of the white paper entitled
 * "Digital Signatures for PDF documents"
 * written by Bruno Lowagie
 * 
 * For more info, go to: http://itextpdf.com/learn
 */

using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace signatures.chapter1
{
    class C1_02_DigestBC
    {
        protected byte[] digest;
	    protected IDigest hash;
    	
	    protected C1_02_DigestBC(String password, String algorithm) {
	        hash = DigestUtilities.GetDigest(algorithm);
	        byte[] bytes = new UTF8Encoding().GetBytes(password);
            hash.BlockUpdate(bytes, 0, bytes.Length);
	        digest = new byte[hash.GetDigestSize()];
	        hash.DoFinal(digest, 0);
	    }
    	
	    public static C1_02_DigestBC GetInstance(String password, String algorithm) {
		    return new C1_02_DigestBC(password, algorithm);
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
            byte[] bytes = new UTF8Encoding().GetBytes(password);
            hash.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[hash.GetDigestSize()];
            hash.DoFinal(result, 0);
            return Arrays.AreEqual(result, digest);
	    }
    	
	    public static void ShowTest(String algorithm) {
		    try {
			    C1_02_DigestBC app = GetInstance("password", algorithm);
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
		    ShowTest("SHA-224");
		    ShowTest("SHA-256");
		    ShowTest("SHA-384");
		    ShowTest("SHA-512");
		    ShowTest("RIPEMD128");
		    ShowTest("RIPEMD160");
		    ShowTest("RIPEMD256");
	    }
    	
	    static void Main(String[] args) {
		    TestAll();
            Console.ReadKey();
	    }
    }
}
