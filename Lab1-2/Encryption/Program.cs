using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using CNnet_Lab1.Models;

namespace Encryption
{
    class Program
    {
        static void Main( string[] Fileargs)
        {

            var employee = new Employee(1, "adr", "John", "446677");
            string json = JsonConvert.SerializeObject(employee);
            var deserialized = JsonConvert.DeserializeObject<Employee>(json);
           
         
            using (Aes myAes = Aes.Create())
            {
                byte[] encrypted = EncryptStringToBytes_Aes(json, myAes.Key, myAes.IV);
                //Decrypt te bytes to a string

                string roundtrip = DecryptStringFromBytes_Aes(encrypted,myAes.Key,myAes.IV);
                string encryptingData = null;
                //Displa the original data and the decrypted data
               Console.WriteLine("Original {0}", json);
                for (var i = 0; i < encrypted.Length; i++)
                {  
                   encryptingData += encrypted[i];
                }
               Console.WriteLine(encryptingData);
                Console.WriteLine("Round Trip {0}\n", roundtrip);

                //Cryptography with RSA
                RSAParameters privateKey;
                RSAParameters publicKey;

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048);

                privateKey = RSA.ExportParameters(true);
                publicKey = RSA.ExportParameters(false);

                UnicodeEncoding byteConverter = new UnicodeEncoding();
                byte[] Encryptinput = byteConverter.GetBytes(json);
                byte[] Encryptoutput;
                Console.WriteLine("RSA Encrypt Input = {0}",byteConverter.GetString(Encryptinput));

                Encryptoutput = RSAEncrypt(Encryptinput, publicKey, false);
                Console.WriteLine("RSA Encrypt Output = {0}", Convert.ToBase64String(Encryptoutput)); 

                byte[] Descryptoutput;
                Descryptoutput = RSADecrypt(Encryptoutput, privateKey, false);
                Console.WriteLine("RSA Decrypt Output = {0}\n", byteConverter.GetString(Descryptoutput));

                //DSACryptoServiceProvider
                try
                {
                    DSAParameters privateKeyInfo;
                    DSAParameters publicKeyInfo;

                    // Create a new instance of DSACryptoServiceProvider to generate
                    // a new key pair.
                    using (DSACryptoServiceProvider DSA = new DSACryptoServiceProvider())
                    {  
                        // Set the hash algorithm to the passed value.
                       // DSADeformatter.SetHashAlgorithm(HashAlg);
                        privateKeyInfo = DSA.ExportParameters(true);
                        publicKeyInfo = DSA.ExportParameters(false);
                        byte[] TestValue = byteConverter.GetBytes(json);
                        //Вычисляет хэш-значение фрагмента заданного массива байтов с помощью указанного алгоритма хэширования 
                        //и подписывает результирующее хэш-значение.
                        byte[] sign = DSA.SignData(TestValue, 0, TestValue.Length, HashAlgorithmName.SHA1);
                        bool verified = DSA.VerifyData(TestValue, 0, TestValue.Length, sign,HashAlgorithmName.SHA1);
                        if (verified)
                        {
                            Console.WriteLine("The hash value was verified.");
                        }
                        else
                        {
                            Console.WriteLine("The hash value was not verified.");
                        }
                    }
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine(e.Message);
                }

            }
           
            string dataFile;
            string signedFile;
            //If no file names are specified, create them.
            if (Fileargs.Length < 2)
            {
                dataFile = @"text.txt";
                signedFile = "signedFile.enc";

                if (!File.Exists(dataFile))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(dataFile))
                    {
                        sw.WriteLine(json);
                    }
                }
            }
            else
            {
                dataFile = Fileargs[0];
                signedFile = Fileargs[1];
            }
            try
            {
                // Create a random key using a random number generator. This would be the
                //  secret key shared by sender and receiver.
                byte[] secretkey = new Byte[64];
                //RNGCryptoServiceProvider is an implementation of a random number generator.
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    // The array is now filled with cryptographically strong random bytes.
                    rng.GetBytes(secretkey);

                    // Use the secret key to sign the message file.
                    SignFile(secretkey, dataFile, signedFile);

                    // Verify the signed file
                    VerifyFile(secretkey, signedFile);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error: File not found", e);
            }
        }
        static  byte[] EncryptStringToBytes_Aes(string plainText, byte [] Key, byte [] IV)
        {
            //Check arguments
            if (plainText==null||plainText.Length<=0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }
            byte[] encrypted;
            //Create an Aes object with the specified key and IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                //Create an encryptor to perform the stream transfor
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key,aesAlg.IV);

                //create the streams used for encrption
                using (MemoryStream msEncrypt = new MemoryStream()) 
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt,encryptor,CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            //Return the encrypted bytes from he memor stream
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes (byte[] cipherText, byte[] Key, byte[] IV)
        {
            //Check arguments
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            //Declare the string used to hold the decrypted text
            string plaintext = null;

            //Create an Aes object with the specified key and IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                //Create a decryptor to perform the strea transform
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key,aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream( msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            //Read the decrypted bytes from the decrypting stream and place them in a string
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        static public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            //Create a new instance of RSACryptoServiceProvider.
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            //Import the RSA Key information. This only needs
            //toinclude the public key information.
            RSA.ImportParameters(RSAKeyInfo);

            //Encrypt the passed byte array and specify OAEP padding.  
            //OAEP padding is only available on Microsoft Windows XP or
            //later.  
            return RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
        }

        static public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            //Create a new instance of RSACryptoServiceProvider.
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            //Import the RSA Key information. This needs
            //to include the private key information.
            RSA.ImportParameters(RSAKeyInfo);

            //Decrypt the passed byte array and specify OAEP padding.  
            //OAEP padding is only available on Microsoft Windows XP or
            //later.  
            return RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
        }

        public static void SignFile(byte[] key, String sourceFile, String destFile)
        {
            // Initialize the keyed hash object.
            using (HMACSHA384 hmac = new HMACSHA384(key))
            {
                using (FileStream inStream = new FileStream(sourceFile, FileMode.Open))
                {
                    using (FileStream outStream = new FileStream(destFile, FileMode.Create))
                    {
                        // Compute the hash of the input file.
                        byte[] hashValue = hmac.ComputeHash(inStream);
                        // Reset inStream to the beginning of the file.
                        inStream.Position = 0;
                        // Write the computed hash value to the output file.
                        outStream.Write(hashValue, 0, hashValue.Length);
                        // Copy the contents of the sourceFile to the destFile.
                        int bytesRead;
                        // read 1K at a time
                        byte[] buffer = new byte[1024];
                        do
                        {
                            // Read from the wrapping CryptoStream.
                            bytesRead = inStream.Read(buffer, 0, 1024);
                            outStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead > 0);
                    }
                }
            }
            return;
        } // end SignFile

        // Compares the key in the source file with a new key created for the data portion of the file. If the keys
        // compare the data has not been tampered with.
        public static bool VerifyFile(byte[] key, String sourceFile)
        {
            bool err = false;
            // Initialize the keyed hash object.
            using (HMACSHA384 hmac = new HMACSHA384(key))
            {
                // Create an array to hold the keyed hash value read from the file.
                byte[] storedHash = new byte[hmac.HashSize / 8];
                // Create a FileStream for the source file.
                using (FileStream inStream = new FileStream(sourceFile, FileMode.Open))
                {
                    // Read in the storedHash.
                    inStream.Read(storedHash, 0, storedHash.Length);
                    // Compute the hash of the remaining contents of the file.
                    // The stream is properly positioned at the beginning of the content,
                    // immediately after the stored hash value.
                    byte[] computedHash = hmac.ComputeHash(inStream);
                    // compare the computed hash with the stored value

                    for (int i = 0; i < storedHash.Length; i++)
                    {
                        if (computedHash[i] != storedHash[i])
                        {
                            err = true;
                        }
                    }
                }
            }
            if (err)
            {
                Console.WriteLine("Hash values differ! Signed file has been tampered with!");
                return false;
            }
            else
            {
                Console.WriteLine("Hash values agree -- no tampering occurred.");
                return true;
            }
        } //end VerifyFile

    }
}
