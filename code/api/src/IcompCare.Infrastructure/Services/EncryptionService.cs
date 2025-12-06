using System.Security.Cryptography;
using System.Text;
using IcompCare.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace IcompCare.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    private readonly string _key;

    public EncryptionService(IConfiguration configuration)
    {
        _key = configuration["Encryption:Key"] ?? "b14ca5898a4e4133bbce2ea2315a1916";
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        var key = Encoding.UTF8.GetBytes(_key);
        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(key, aes.IV);
        using var ms = new MemoryStream();

        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            var key = Encoding.UTF8.GetBytes(_key);

            using var aes = Aes.Create();

            var iv = new byte[aes.BlockSize / 8];

            if (fullCipher.Length < iv.Length)
                return cipherText;

            Array.Copy(fullCipher, 0, iv, 0, iv.Length);

            var cipher = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var decryptor = aes.CreateDecryptor(key, iv);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
        catch
        {
            return cipherText;
        }
    }
}
