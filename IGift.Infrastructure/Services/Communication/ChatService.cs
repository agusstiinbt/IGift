using System.Linq.Expressions;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Interfaces.Communication.Chat;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Models.Chat;
using IGift.Infrastructure.Data;
using IGift.Infrastructure.Models;
using IGift.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Services.Communication
{
    public class ChatService : IChatService
    {
        #region Para cuando querramos encriptar el codigo
        //private readonly byte[] _key;
        //private readonly string _path;

        //public ChatService(string key, string path)
        //{
        //    _key = Encoding.UTF8.GetBytes(key);
        //    _path = path;
        //}
        //public async Task<IResult> SaveMessage(ChatHistory chat)
        //{

        //    //var iv = GenerateIV();
        //    //var encryptedUserId = Encrypt(chat.FromUserId, _key, iv);
        //    //var filePath = Path.Combine(_path, $"{Convert.ToBase64String(encryptedUserId)}.txt");

        //    //var serializedMessage = JsonConvert.SerializeObject(chat);
        //    //var encryptedMessage = Encrypt(serializedMessage, _key, iv);

        //    //using (var stream = new FileStream(filePath, FileMode.Append))
        //    //{
        //    //    // Write IV to the file
        //    //    stream.Write(iv, 0, iv.Length);

        //    //    // Write encrypted message to the file
        //    //    stream.Write(encryptedMessage, 0, encryptedMessage.Length);
        //    //}
        //    return await Result.SuccessAsync();
        //}

        //private byte[] GenerateIV()
        //{
        //    using (Aes aes = Aes.Create())
        //    {
        //        aes.GenerateIV();
        //        return aes.IV;
        //    }
        //}
        //private byte[] Encrypt(string simpletext, byte[] key, byte[] iv)
        //{
        //    byte[] cipheredtext;
        //    using (Aes aes = Aes.Create())
        //    {
        //        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
        //                {
        //                    streamWriter.Write(simpletext);
        //                }

        //                cipheredtext = memoryStream.ToArray();
        //            }
        //        }
        //    }
        //    return cipheredtext;
        //}
        //private string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv)
        //{
        //    string simpletext = String.Empty;
        //    using (Aes aes = Aes.Create())
        //    {
        //        ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
        //        using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader streamReader = new StreamReader(cryptoStream))
        //                {
        //                    simpletext = streamReader.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //    return simpletext;
        //}

        //private async Task<IResult> SaveChatToFile(ChatHistory chat)
        //{
        //    return null;
        //}

        #endregion

        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ChatService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryByIdAsync(string chatId)
        {
            Expression<Func<ChatHistory<IGiftUser>, ChatHistoryResponse>> expression = e => new ChatHistoryResponse
            {
                FromUserId = e.FromUserId,
                ToUserId = e.ToUserId,
                Message = e.Message,
                Seen = e.Seen,
                DateSend = e.CreatedDate
            };

            var result = await _context.ChatHistories
                            .Where(x => x.ToUserId == chatId)
                            .Select(expression)
                            .AsNoTracking()
                            .ToListAsync();

            if (result.Any())
                return await Result<IEnumerable<ChatHistoryResponse>>.SuccessAsync(result);

            return await Result<IEnumerable<ChatHistoryResponse>>.FailAsync("No existen chats con el usuario");
        }

        public async Task<IResult<IEnumerable<ChatUser>>> LoadChatUsers(string CurrentUserId)
        {
            //Expression<Func<ChatHistory<IGiftUser>, ChatUser>> expression = e => new ChatUser
            //{
            //    ChatId = e.Id,
            //    UserName = e.ToUser.UserName,
            //    UserProfileImageUrl = e.ToUser.ProfilePictureDataUrl,
            //    LastMessage = e.Message,
            //    Seen = e.Seen
            //}; La expression no funciona si la consulta liqn tiene un GroupBy

            var response = await _context.ChatHistories
                        .Include(m => m.ToUser) // Incluir la información del usuario destinatario
                        .Where(x => x.FromUser.Id == CurrentUserId)
                        .GroupBy(x => x.ToUserId)
                        .Select(g => g.OrderByDescending(m => m.CreatedDate).First())
                        .AsNoTracking()//aumentamos la velocidad de la consulta
                        .ToListAsync();
            //TODO debemos poner un asnotracking en todo el codigo... donde haga falta revisar todos los services del infraestructure

            if (response != null)
            {
                var chatUsers = response.Select(e => new ChatUser
                {
                    ChatId = e.Id,
                    UserName = e.ToUser.UserName,
                    UserProfileImageUrl = e.ToUser.ProfilePictureDataUrl,
                    LastMessage = e.Message,
                    Seen = e.Seen
                }).ToList();

                return await Result<IEnumerable<ChatUser>>.SuccessAsync(chatUsers);
            }

            return await Result<IEnumerable<ChatUser>>.FailAsync("No hay chats historicos");
        }

        public async Task<IResult> SaveMessage(SaveChatMessage saveChatMessage)
        {
            await _context.ChatHistories.AddAsync(new ChatHistory<IGiftUser>
            {
                FromUserId = saveChatMessage.FromUserId,
                ToUserId = saveChatMessage.ToUserId,
                Message = saveChatMessage.Message,
                CreatedDate = DateTime.Now,
                Seen = false
            });

            await _context.SaveChangesAsync();

            return await Result.SuccessAsync();
        }
    }
}
