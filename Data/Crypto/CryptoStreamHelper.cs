using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Willowsoft.WillowLib.Data.Crypto
{
    public class CryptoStreamHelper
    {
        private SymmetricAlgorithm mAlgorithm;
        private byte[] mKey;
        private byte[] mInitializationVector;

        public CryptoStreamHelper(string password)
            : this(new AesManaged(), password)
        {
        }

        public CryptoStreamHelper(SymmetricAlgorithm algorithm, string password)
        {
            mAlgorithm = algorithm;
            Password = password;
            SetInitializationVector();
        }

        public string Password
        {
            set
            {
                char[] passwordChars = value.ToCharArray();
                Encoder encoder = Encoding.Unicode.GetEncoder();
                int byteCount = encoder.GetByteCount(passwordChars, 0, passwordChars.Length, true);
                byte[] passwordBytes = new byte[byteCount];
                encoder.GetBytes(passwordChars, 0, passwordChars.Length, passwordBytes, 0, true);
                mKey = GetPasswordHasher().ComputeHash(passwordBytes);
            }
        }

        private HashAlgorithm GetPasswordHasher()
        {
            if (SupportsKeySize(512))
                return new SHA512Managed();
            else if (SupportsKeySize(384))
                return new SHA384Managed();
            else if (SupportsKeySize(256))
                return new SHA256Managed();
            else
                throw new InvalidOperationException("Unable to find password hash algorithm for crypto algorithm");
        }

        private void SetInitializationVector()
        {
            if (SupportsBlockSize(2048))
                mInitializationVector = new byte[2048 / 8];
            else if (SupportsBlockSize(1024))
                mInitializationVector = new byte[1024 / 8];
            else if (SupportsBlockSize(512))
                mInitializationVector = new byte[512 / 8];
            else if (SupportsBlockSize(256))
                mInitializationVector = new byte[256 / 8];
            else if (SupportsBlockSize(128))
                mInitializationVector = new byte[128 / 8];
            else if (SupportsBlockSize(64))
                mInitializationVector = new byte[64 / 8];
            else if (SupportsBlockSize(32))
                mInitializationVector = new byte[32 / 8];
            else
                throw new InvalidOperationException("Could not find a supported block size");
            for (int index = 0; index < mInitializationVector.Length; index++)
                mInitializationVector[index] = 0;
        }

        private bool SupportsBlockSize(int blockSize)
        {
            return SupportsSize(mAlgorithm.LegalBlockSizes, blockSize);
        }

        private bool SupportsKeySize(int keySize)
        {
            return SupportsSize(mAlgorithm.LegalKeySizes, keySize);
        }

        private bool SupportsSize(KeySizes[] legalSizes, int size)
        {
            foreach (KeySizes sizes in legalSizes)
            {
                for (int legalSize = sizes.MinSize;
                    legalSize <= sizes.MaxSize;
                    legalSize += sizes.SkipSize)
                {
                    if (size == legalSize)
                        return true;
                    if (sizes.SkipSize == 0)
                        break;
                }
            }
            return false;
        }

        public CryptoStream GetEncryptingStream(Stream outputStream)
        {
            return new CryptoStream(outputStream,
                mAlgorithm.CreateEncryptor(mKey, mInitializationVector),
                CryptoStreamMode.Write);
        }

        public TextWriter GetEncryptingWriter(Stream outputStream)
        {
            return new StreamWriter(GetEncryptingStream(outputStream));
        }

        public TextWriter GetEncryptingWriter(string fileName)
        {
            return GetEncryptingWriter(
                new FileStream(fileName, FileMode.Create, FileAccess.Write));
        }

        public CryptoStream GetDecryptingStream(Stream inputStream)
        {
            return new CryptoStream(inputStream, 
                mAlgorithm.CreateDecryptor(mKey, mInitializationVector),
                CryptoStreamMode.Read);
        }

        public TextReader GetDecryptingReader(Stream inputStream)
        {
            return new StreamReader(GetDecryptingStream(inputStream));
        }

        public TextReader GetDecryptingReader(string fileName)
        {
            return GetDecryptingReader(
                new FileStream(fileName, FileMode.Open, FileAccess.Read));
        }
    }
}
