/* Credit to https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp by CraigTP
 * Example of usage:

using System;

namespace EncryptStringSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter a password to use:");
            string password = Console.ReadLine();
            Console.WriteLine("Please enter a string to encrypt:");
            string plaintext = Console.ReadLine();
            Console.WriteLine("");

            Console.WriteLine("Your encrypted string is:");
            string encryptedstring = StringCipher.Encrypt(plaintext, password);
            Console.WriteLine(encryptedstring);
            Console.WriteLine("");

            Console.WriteLine("Your decrypted string is:");
            string decryptedstring = StringCipher.Decrypt(encryptedstring, password);
            Console.WriteLine(decryptedstring);
            Console.WriteLine("");

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
*/

using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace FIX.Service.Utils
{
    public static class StringCipher
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}

public sealed class SecurePasswordHasher
{
    /// <summary>
    /// Size of salt
    /// </summary>
    private const int SaltSize = 16;

    /// <summary>
    /// Size of hash
    /// </summary>
    private const int HashSize = 20;

    /// <summary>
    /// Creates a hash from a password
    /// </summary>
    /// <param name="password">the password</param>
    /// <param name="iterations">number of iterations</param>
    /// <returns>the hash</returns>
    public static string Hash(string password, int iterations)
    {
        //create salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

        //create hash
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        var hash = pbkdf2.GetBytes(HashSize);

        //combine salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        //convert to base64
        var base64Hash = Convert.ToBase64String(hashBytes);

        //format hash with extra information
        return string.Format("$FIXHASH$V1${0}${1}", iterations, base64Hash);
    }
    /// <summary>
    /// Creates a hash from a password with 10000 iterations
    /// </summary>
    /// <param name="password">the password</param>
    /// <returns>the hash</returns>
    public static string Hash(string password)
    {
        return Hash(password, 8888);
    }

    /// <summary>
    /// Check if hash is supported
    /// </summary>
    /// <param name="hashString">the hash</param>
    /// <returns>is supported?</returns>
    public static bool IsHashSupported(string hashString)
    {
        return hashString.Contains("$FIXHASH$V1$");
    }

    /// <summary>
    /// verify a password against a hash
    /// </summary>
    /// <param name="password">the password</param>
    /// <param name="hashedPassword">the hash</param>
    /// <returns>could be verified?</returns>
    public static bool Verify(string password, string hashedPassword)
    {
        //check hash
        if (!IsHashSupported(hashedPassword))
        {
            throw new NotSupportedException("The hashtype is not supported");
        }

        //extract iteration and Base64 string
        var splittedHashString = hashedPassword.Replace("$FIXHASH$V1$", "").Split('$');
        var iterations = int.Parse(splittedHashString[0]);
        var base64Hash = splittedHashString[1];

        //get hashbytes
        var hashBytes = Convert.FromBase64String(base64Hash);

        //get salt
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        //create hash with given salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        //get result
        for (var i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
            {
                return false;
            }
        }
        return true;
    }
}