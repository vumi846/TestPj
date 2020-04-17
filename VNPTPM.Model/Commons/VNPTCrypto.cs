using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Commons
{
    public sealed class VNPTCrypto
    {
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        // Verify a hash against a string. 
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input. 
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool VerifyMd5Hash(string hashOfInput, string hash)
        {
            MD5 md5Hash = MD5.Create();
            // Hash the input. 

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private const int TOKEN_SIZE_IN_BYTES = 16;
        private const int PBKDF2_ITER_COUNT = 1000;
        // default for Rfc2898DeriveBytes
        private const int PBKDF2_SUBKEY_LENGTH = 256 / 8;
        // 256 bits
        private const int SALT_SIZE = 128 / 8;
        // 128 bits
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "It really is a byte length")]
        internal static byte[] GenerateSaltInternal(int byteLength = SALT_SIZE)
        {
            byte[] buf = new byte[byteLength];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buf);
            }
            return buf;
        }

        public static string GenerateToken()
        {
            byte[] tokenBytes = new byte[TOKEN_SIZE_IN_BYTES];
            using (RNGCryptoServiceProvider prng = new RNGCryptoServiceProvider())
            {
                prng.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes);
            }
        }

        public static string GenerateSalt(int byteLength = SALT_SIZE)
        {
            return Convert.ToBase64String(GenerateSaltInternal(byteLength));
        }

        public static string Hash(string input, string algorithm = "sha256")
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            return Hash(Encoding.UTF8.GetBytes(input), algorithm);
        }

        public static string Hash(byte[] input, string algorithm = "sha256")
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            using (HashAlgorithm alg = HashAlgorithm.Create(algorithm))
            {
                if (alg != null)
                {
                    byte[] hashData = alg.ComputeHash(input);
                    return BinaryToHex(hashData);
                }
                else
                {
                    throw new InvalidOperationException(String.Format(string.Format("Not supported hash algorhitm {0}", algorithm)));
                }
            }
        }

        public static string SHA1(string input)
        {
            return Hash(input, "sha1");
        }

        public static string SHA256(string input)
        {
            return Hash(input, "sha256");
        }

        // =======================
        //         * HASHED PASSWORD FORMATS
        //         * =======================
        //         * 
        //         * Version 0:
        //         * PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
        //         * (See also: SDL crypto guidelines v5.1, Part III)
        //         * Format: { 0x00, salt, subkey }
        //         

        public static string HashPassword(string password)
        {
            //if (password == null)
            //{
            //    throw new ArgumentNullException("password");
            //}

            // Produce a version 0 (see comment above) password hash.
            byte[] salt;
            byte[] subkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SALT_SIZE, PBKDF2_ITER_COUNT))
            {
                salt = deriveBytes.Salt;
                subkey = deriveBytes.GetBytes(PBKDF2_SUBKEY_LENGTH);
            }

            byte[] outputBytes = new byte[1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH];
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SALT_SIZE);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SALT_SIZE, PBKDF2_SUBKEY_LENGTH);
            return Convert.ToBase64String(outputBytes);
        }

        // hashedPassword must be of the format of HashWithPassword (salt + Hash(salt+input)
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException("hashedPassword");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            // Verify a version 0 (see comment above) password hash.

            if (hashedPasswordBytes.Length != (1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH) || hashedPasswordBytes[0] != (byte)0x00)
            {
                // Wrong length or version header.
                return false;
            }

            byte[] salt = new byte[SALT_SIZE];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SALT_SIZE);
            byte[] storedSubkey = new byte[PBKDF2_SUBKEY_LENGTH];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SALT_SIZE, storedSubkey, 0, PBKDF2_SUBKEY_LENGTH);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, PBKDF2_ITER_COUNT))
            {
                generatedSubkey = deriveBytes.GetBytes(PBKDF2_SUBKEY_LENGTH);
            }
            return ByteArraysEqual(storedSubkey, generatedSubkey);
        }

        internal static string BinaryToHex(byte[] data)
        {
            char[] hex = new char[data.Length * 2];

            for (int iter = 0; iter < data.Length; iter++)
            {
                byte hexChar = ((byte)(data[iter] >> 4));
                hex[iter * 2] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
                hexChar = ((byte)(data[iter] & 0xF));
                hex[iter * 2 + 1] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
            }
            return new string(hex);
        }

        // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            bool areSame = true;
            for (int i = 0; i <= a.Length - 1; i++)
            {
                areSame = areSame & (a[i] == b[i]);
            }
            return areSame;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Basic Encryption/Decryption Functionality
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region enums, constants & fields

        public enum CryptoTypes
        {
            encTypeDES = 0,
            encTypeRC2,
            encTypeRijndael,
            encTypeTripleDES
        }

        private const string CRYPT_DEFAULT_PASSWORD = "CB06cfE507a1";
        private const CryptoTypes CRYPT_DEFAULT_METHOD = CryptoTypes.encTypeRijndael;

        private byte[] mKey = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
        private byte[] mIV = { 65, 110, 68, 26, 69, 178, 200, 219 };
        private byte[] SaltByteArray = { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };
        private CryptoTypes mCryptoType = CRYPT_DEFAULT_METHOD;
        private string mPassword = CRYPT_DEFAULT_PASSWORD;

        #endregion

        #region Constructors

        public VNPTCrypto()
        {
            calculateNewKeyAndIV();
        }

        public VNPTCrypto(CryptoTypes CryptoType)
        {
            this.CryptoType = CryptoType;
        }

        #endregion

        #region Props

        public CryptoTypes CryptoType
        {
            get
            {
                return mCryptoType;
            }
            set
            {
                if (mCryptoType != value)
                {
                    mCryptoType = value;
                    calculateNewKeyAndIV();
                }
            }
        }

        public string Password
        {
            get
            {
                return mPassword;
            }
            set
            {
                if (mPassword != value)
                {
                    mPassword = value;
                    calculateNewKeyAndIV();
                }
            }
        }

        #endregion

        #region Encryption

        public string Encrypt(string inputText)
        {
            //declare a new encoder
            UTF8Encoding UTF8Encoder = new UTF8Encoding();
            //get byte representation of string
            byte[] inputBytes = UTF8Encoder.GetBytes(inputText);

            //convert back to a string
            return Convert.ToBase64String(EncryptDecrypt(inputBytes, true));
        }

        public string Encrypt(string inputText, string password)
        {
            this.Password = password;
            return this.Encrypt(inputText);
        }

        public string Encrypt(string inputText, string password, CryptoTypes cryptoType)
        {
            mCryptoType = cryptoType;
            return this.Encrypt(inputText, password);
        }

        public string Encrypt(string inputText, CryptoTypes cryptoType)
        {
            this.CryptoType = cryptoType;
            return this.Encrypt(inputText);
        }

        #endregion

        #region Decryption

        public string Decrypt(string inputText)
        {
            try
            {
                //declare a new encoder
                UTF8Encoding UTF8Encoder = new UTF8Encoding();
                //get byte representation of string
                byte[] inputBytes = Convert.FromBase64String(inputText);

                //convert back to a string
                return UTF8Encoder.GetString(EncryptDecrypt(inputBytes, false));
            }
            catch (Exception)
            {

                return inputText;
            }

        }

        public string Decrypt(string inputText, string password)
        {
            this.Password = password;
            return Decrypt(inputText);
        }

        public string Decrypt(string inputText, string password, CryptoTypes cryptoType)
        {
            mCryptoType = cryptoType;
            return Decrypt(inputText, password);
        }

        public string Decrypt(string inputText, CryptoTypes cryptoType)
        {
            this.CryptoType = cryptoType;
            return Decrypt(inputText);
        }
        #endregion

        #region Symmetric Engine

        private byte[] EncryptDecrypt(byte[] inputBytes, bool Encrpyt)
        {
            //get the correct transform
            ICryptoTransform transform = getCryptoTransform(Encrpyt);

            //memory stream for output
            MemoryStream memStream = new MemoryStream();

            try
            {
                //setup the cryption - output written to memstream
                CryptoStream cryptStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);

                //write data to cryption engine
                cryptStream.Write(inputBytes, 0, inputBytes.Length);

                //we are finished
                cryptStream.FlushFinalBlock();

                //get result
                byte[] output = memStream.ToArray();

                //finished with engine, so close the stream
                cryptStream.Close();

                return output;
            }
            catch (Exception e)
            {
                throw new Exception("Error in symmetric engine. Error : " + e.Message, e);
            }
        }

        private ICryptoTransform getCryptoTransform(bool encrypt)
        {
            SymmetricAlgorithm SA = selectAlgorithm();
            SA.Key = mKey;
            SA.IV = mIV;
            if (encrypt)
            {
                return SA.CreateEncryptor();
            }
            else
            {
                return SA.CreateDecryptor();
            }
        }

        private SymmetricAlgorithm selectAlgorithm()
        {
            SymmetricAlgorithm SA;
            switch (mCryptoType)
            {
                case CryptoTypes.encTypeDES:
                    SA = DES.Create();
                    break;
                case CryptoTypes.encTypeRC2:
                    SA = RC2.Create();
                    break;
                case CryptoTypes.encTypeRijndael:
                    SA = Rijndael.Create();
                    break;
                case CryptoTypes.encTypeTripleDES:
                    SA = TripleDES.Create();
                    break;
                default:
                    SA = TripleDES.Create();
                    break;
            }
            return SA;
        }

        private void calculateNewKeyAndIV()
        {
            //use salt so that key cannot be found with dictionary attack
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(mPassword, SaltByteArray);
            SymmetricAlgorithm algo = selectAlgorithm();
            mKey = pdb.GetBytes(algo.KeySize / 8);
            mIV = pdb.GetBytes(algo.BlockSize / 8);
        }

        #endregion

        public class Hashing
        {
            #region Enum, constants and fields

            public enum HashingTypes
            {
                SHA, SHA256, SHA384, SHA512, MD5
            }

            #endregion

            #region Static members

            public static string Hash(String inputText)
            {
                return ComputeHash(inputText, HashingTypes.MD5);
            }

            public static string Hash(String inputText, HashingTypes hashingType)
            {
                return ComputeHash(inputText, hashingType);
            }

            public static bool isHashEqual(string inputText, string hashText)
            {
                return (Hash(inputText) == hashText);
            }

            public static bool isHashEqual(string inputText, string hashText, HashingTypes hashingType)
            {
                return (Hash(inputText, hashingType) == hashText);
            }
            #endregion

            #region Hashing Engine

            private static string ComputeHash(string inputText, HashingTypes hashingType)
            {
                HashAlgorithm HA = getHashAlgorithm(hashingType);

                //declare a new encoder
                UTF8Encoding UTF8Encoder = new UTF8Encoding();
                //get byte representation of input text
                byte[] inputBytes = UTF8Encoder.GetBytes(inputText);


                //hash the input byte array
                byte[] output = HA.ComputeHash(inputBytes);

                //convert output byte array to a string
                return Convert.ToBase64String(output);
            }

            private static HashAlgorithm getHashAlgorithm(HashingTypes hashingType)
            {
                switch (hashingType)
                {
                    case HashingTypes.MD5:
                        return new MD5CryptoServiceProvider();
                    case HashingTypes.SHA:
                        return new SHA1CryptoServiceProvider();
                    case HashingTypes.SHA256:
                        return new SHA256Managed();
                    case HashingTypes.SHA384:
                        return new SHA384Managed();
                    case HashingTypes.SHA512:
                        return new SHA512Managed();
                    default:
                        return new MD5CryptoServiceProvider();
                }
            }

            #endregion
        }
    }
}
