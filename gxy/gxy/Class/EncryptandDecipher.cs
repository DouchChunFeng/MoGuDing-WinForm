using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace gxy
{
    class EncryptandDecipher
    {   
        private static string key = "12345678901234567890123456789012";  //设置本地加密密钥 (加密本地文件)
        #region 处理本地文件密码加密
        //AES加密  
        public static string Encrypt(string content)
        {
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(key);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateEncryptor();
            byte[] contentBytes = UTF8Encoding.UTF8.GetBytes(content);
            byte[] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
            return Convert.ToBase64String(resultBytes, 0, resultBytes.Length);
        }
        //AES解密  
        public static string Decipher(string content)
        {
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(key);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateDecryptor();
            byte[] contentBytes = Convert.FromBase64String(content);
            byte[] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
            return UTF8Encoding.UTF8.GetString(resultBytes);
        }
        #endregion

        private static string gxykey = "23DbtQHR2UMbH6mJ"; //工学云0902日后的密钥
        #region 工学云一些算法
        //登录密钥加密
        public static string GXY_Login_V3_Encrypt(string content)
        {
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(gxykey);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateEncryptor();
            byte[] contentBytes = UTF8Encoding.UTF8.GetBytes(content);
            byte[] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
            string resultString = "";
            foreach (byte b in resultBytes)
            {
                resultString += b.ToString("x2");
            }
            return resultString;
        }
        //MD5加密
        public static string md5jm(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] byteArray = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            string resultString = "";
            foreach (byte b in byteArray)
            {
                resultString += b.ToString("x2"); //将字节数组转成16进制的字符串。X表示16进制，2表示每个16字符占2位
            }
            return resultString;
        }
        //登录密钥解密
        public static string GXY_Login_V3_Decipher(string content)
        {
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(gxykey);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateDecryptor();
            byte[] contentBytes = new byte[content.Length / 2];
            for (int i = 0; i < content.Length / 2; i++)
            {
                contentBytes[i] = Convert.ToByte(content.Substring(i + i, 2), 16);
            }
            byte[] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
            return UTF8Encoding.UTF8.GetString(resultBytes);
        }
        #endregion
    }
}
