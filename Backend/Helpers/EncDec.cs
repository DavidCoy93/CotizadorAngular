using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MSSPAPI.Helpers
{
    public class EncDec
    {

        //Generador de números aleatorios
        private static Random rdms = new Random();

        // Llave de encripción
        private static byte[] key_arr = { 0x1C, 0x68, 0x06, 0x26, 0xD8, 0x4C, 0x0A, 0x09, 0xD4, 0x61, 0xE8, 0x56, 0x86, 0xA9, 0x4A, 0xD4,
                                          0xAF, 0x71, 0xA5, 0xE5, 0x91, 0x3F, 0xA4, 0xA9, 0xF8, 0xF0, 0x5C, 0xB1, 0xB4, 0x52, 0x78, 0xB4 };
        // Vector de inicialización
        private static byte[] iv_arr = { 0x6E, 0x23, 0x92, 0xA4, 0x68, 0x9D, 0x7B, 0x4C, 0xA3, 0xD7, 0xBC, 0xEB, 0xD1, 0xB0, 0x1B, 0x07 };

        //Encriptador del módulo de criptografía de .Net
        private static Aes cipher = null;

        /***************************************************************************************************
         Método:        InitializeCipher
         Descripción:   Inicializa las variables del encriptador
         Entradas:      <ninguna>
         Salidas:       <ninguna>
        ***************************************************************************************************/
        private static void InitializeCipher()
        {
            cipher = Aes.Create();
            cipher.KeySize = 256;
            cipher.BlockSize = 128;
            cipher.Mode = CipherMode.CBC;
            cipher.Padding = PaddingMode.PKCS7;
            cipher.Key = key_arr;
            cipher.IV = iv_arr;
        }

        /// <summary>
        ///  Método:        EncDec   
        ///  Descripción:   Constructor de la clase
        ///  Entradas:      ninguna
        ///  Salidas:       ninguna
        /// </summary>
        /// <param></param>
        /// <returns></returns>

        public EncDec()
        {
            InitializeCipher();
        }


        /// <summary>
        /// Método:        Encript
        /// Descripción:   Encripta la cadena especificada
        /// Entradas:      data - Cadena de caracteres a encriptar
        /// Salidas:       result - Cadena de caracteres encriptados
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encript(string data)
        {
            string result = null;

            if ((data != null) && (data.Length > 0))
            {
                if (cipher == null) InitializeCipher();
                byte[] clearBytes = Encoding.Unicode.GetBytes(data);

                ICryptoTransform encryptor = cipher.CreateEncryptor(cipher.Key, cipher.IV);
                MemoryStream msEncrypt = new MemoryStream();
                CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                csEncrypt.Write(clearBytes, 0, clearBytes.Length);
                csEncrypt.Close();
                result = Convert.ToBase64String(msEncrypt.ToArray());
            }
            return result;
        }

        /// <summary>
        /// Método:        Decript   
        /// Descripción:   Des-encripta la cadena especificada
        /// Entradas:      data - Cadena de caracteres a des-encriptar
        /// Salidas:       result - Cadena de caracteres des-encriptados
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decript(string data)
        {
            string result = null;

            if ((data != null) && (data.Length > 0))
            {
                if (cipher == null) InitializeCipher();
                byte[] cipherBytes = Convert.FromBase64String(data);

                ICryptoTransform decryptor = cipher.CreateDecryptor(cipher.Key, cipher.IV);
                MemoryStream msDecrypt = new MemoryStream(cipherBytes);
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                int countBytes = csDecrypt.Read(cipherBytes, 0, cipherBytes.Length);
                result = Encoding.Unicode.GetString(cipherBytes, 0, countBytes);
            }
            return result;
        }

        /// <summary>
        /// 
        ///  Descripción:   Obtiene una contraseña aleatoria de 8 caracteres con 2 números y 6 letras
        ///  Entradas:      ninguna
        ///  
        /// </summary>
        /// <returns></returns>
        public static string GetRandomPassword()
        {
            string res = "";
            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int posSpecial = rdms.Next(0, 8);
            int posNumber = posSpecial;
            while (posNumber == posSpecial)
                posNumber = rdms.Next(0, 8);
            for (int i = 0; i < 8; i++)
            {
                if (i == posNumber)
                    res += numbers[rdms.Next(0, 10)];
                else if (i == posSpecial)
                    res += numbers[rdms.Next(0, 10)];
                else
                    res += letters[rdms.Next(0, 52)];
            }
            return res;
        }
    }
}