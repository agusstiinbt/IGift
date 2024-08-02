using IGift.Application.Interfaces;
using IGift.Application.Interfaces.Chat.Service;
using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace IGift.Infrastructure.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly byte[] _key;
        private readonly string _path;

        public ChatService(string key, string path)
        {
            _key = Encoding.UTF8.GetBytes(key);
            _path = path;
        }
        public async Task<IResult> SaveMessage<TUser>(ChatHistory<TUser> chatHistory, string userId) where TUser : IChatUser
        {
            var iv = GenerateIV();
            var encryptedUserId = Encrypt(userId, _key, iv);
            var filePath = Path.Combine(_path, $"{Convert.ToBase64String(encryptedUserId)}.txt");

            var serializedMessage = JsonConvert.SerializeObject(chatHistory);
            var encryptedMessage = Encrypt(serializedMessage, _key, iv);

            using (var stream = new FileStream(filePath, FileMode.Append))
            {
                // Write IV to the file
                stream.Write(iv, 0, iv.Length);

                // Write encrypted message to the file
                stream.Write(encryptedMessage, 0, encryptedMessage.Length);
            }
            return await Result.SuccessAsync();
        }
        private byte[] GenerateIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }
        private byte[] Encrypt(string simpletext, byte[] key, byte[] iv)
        {
            byte[] cipheredtext;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(simpletext);
                        }

                        cipheredtext = memoryStream.ToArray();
                    }
                }
            }
            return cipheredtext;
        }
        private string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv)
        {
            string simpletext = String.Empty;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            simpletext = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return simpletext;
        }
    }
}
