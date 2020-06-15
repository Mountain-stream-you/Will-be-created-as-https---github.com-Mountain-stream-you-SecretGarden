using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PointsMall.Common
{
    /// <summary>
    /// 加/解密相关静态类
    /// </summary>
    public static  class EncryptionHelper
    {
        #region RSA加密相关

        /// <summary>
        /// RSA公钥加密
        /// </summary>
        /// <param name="strPlainText">加密明文</param>
        /// <param name="publicKey"></param>

        /// <returns></returns>
        public static string RSA_Encrypt(this string strPlainText, string publicKey)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] DataToEncrypt = ByteConverter.GetBytes(strPlainText);
            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.ImportCspBlob(Convert.FromBase64String(publicKey));
                //OAEP padding is only available on Microsoft Windows XP or later. 
                byte[] bytes_Cypher_Text = RSA.Encrypt(DataToEncrypt, false);
                string str_Cypher_Text = Convert.ToBase64String(bytes_Cypher_Text);
                return str_Cypher_Text;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// RSA私钥解密
        /// </summary>
        /// <param name="strCypherText">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string RSA_Decrypt(this string strCypherText, string privateKey)
        {
            try
            {
                RSA rsa = CreateRsaProviderFromPrivateKey(privateKey);
                byte[] dataToDecrypt = System.Convert.FromBase64String(strCypherText);
                byte[] plainTextBytes = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);
                string plainText = Encoding.UTF8.GetString(plainTextBytes);
                return plainText;
            }
            catch (CryptographicException e)
            {
                throw new Exception (e.ToString());
            }
        }

        /// <summary>
        /// RSA实例化
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        private static RSA CreateRsaProviderFromPrivateKey(string privateKey)
        {
            privateKey = privateKey
                .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                .Replace("-----END RSA PRIVATE KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "");
            var privateKeyBits = Convert.FromBase64String(privateKey);
            var rsa = RSA.Create();
            RSAParameters rsaParameters = new RSAParameters();
            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }
            rsa.ImportParameters(rsaParameters);
            return rsa;
        }

         /// <summary>
        /// 解析密钥私钥对应指数及模数
        /// </summary>
        /// <param name="binr"></param>
        /// <returns></returns>
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }  

        #endregion

        #region Md5加密

        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="secret">加密内容</param>
        /// <returns>string</returns>
        public static string MD5Hash(this string secret)
        {
            using (MD5 md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(secret));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", string.Empty).ToLower();
            }
        }

        /// <summary>
        /// Md5加密
        /// 转大写
        /// </summary>
        /// <param name="secret">加密内容</param>
        /// <returns>string</returns>
        public static string MD5HashToUpper(string secret)
        {
            using (MD5 md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(secret));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", string.Empty).ToUpper();
            }
        }

        #endregion

        #region  HMACSHA1加密

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="EncryptText">待加密明文</param>
        /// <param name="EncryptKey">密钥</param>
        /// <returns></returns>
        public static string GetHMACSHA1Text( this string EncryptText, string EncryptKey)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(EncryptKey);

            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(EncryptText);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            String result = BitConverter.ToString(hashBytes);//将运算结果转为string类型
            result = result.Replace("-", "").ToUpper();//替换并转为大写
            return result;  
        }

        #endregion

        #region  DES加解密

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="strEncrypt">需要加密的</param>
        /// <param name="strKey">密匙</param>
        /// <returns></returns>
        public static string DES_Encrypt(this string strEncrypt, string strKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                byte[] inputByteArray = Encoding.Default.GetBytes(strEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(strKey);// 密匙
                des.IV = ASCIIEncoding.ASCII.GetBytes(strKey);// 初始化向量
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                var retB = BytesTohexString(ms.ToArray());
                return retB;
            }
            catch (Exception ex)
            {
                throw new Exception($"DES明文加密失败，失败原因：{ex.Message.ToString()}");
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="strDecrypt">需要解密的</param>
        /// <param name="strKey">密匙</param>
        /// <returns></returns>
        public static string DES_Decrypt( this string strDecrypt, string strKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                byte[] inputByteArray = HexStringToBytes(strDecrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(strKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(strKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);  // 如果两次密匙不一样，这一步可能会引发异常
                cs.FlushFinalBlock();
                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch (Exception ex) 
            {
                throw new Exception($"DES密文解密失败，失败原因：{ex.Message.ToString()}");
            }
        }

        /// <summary>
        /// 将16进制字符串转化为 字节流
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        private static byte[] HexStringToBytes(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr)) return new byte[0];

            if (hexStr.StartsWith("0x")) hexStr = hexStr.Remove(0, 2);

            var count = hexStr.Length;

            if (count % 2 == 1) throw new ArgumentException("Invalid length of bytes:" + count);

            var byteCount = count / 2;
            var result = new byte[byteCount];
            for (int ii = 0; ii < byteCount; ++ii)
            {
                var tempBytes = Byte.Parse(hexStr.Substring(2 * ii, 2), System.Globalization.NumberStyles.HexNumber);
                result[ii] = tempBytes;
            }

            return result;
        }


        /// <summary>
        /// 将字节流转化为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string BytesTohexString(byte[] bytes)
        {
            if (bytes == null || bytes.Count() < 1) return string.Empty;

            var count = bytes.Count();
            var cache = new StringBuilder();
            //cache.Append("0x");
            for (int ii = 0; ii < count; ++ii)
            {
                var tempHex = Convert.ToString(bytes[ii], 16).ToUpper();
                cache.Append(tempHex.Length == 1 ? "0" + tempHex : tempHex);
            }

            return cache.ToString();
        }

        #endregion

        #region  Aes加解密



        #endregion
    }
}
