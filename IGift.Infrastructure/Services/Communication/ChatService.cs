using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Interfaces.Communication.Chat;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Models.Chat;
using IGift.Application.Responses.Identity.Users;
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


        public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryByIdAsync(string ToUserId)
        {
            var chatHistories = await _context.ChatHistories
                .Where(x => x.ToUserId == ToUserId)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync(); // Traemos todos los mensajes en una sola consulta

            if (!chatHistories.Any())
                return await Result<IEnumerable<ChatHistoryResponse>>.FailAsync("No existen chats con el usuario");

            // Marcar como leído solo el primer mensaje más antiguo
            var firstMessage = chatHistories.FirstOrDefault();
            if (firstMessage != null && !firstMessage.Seen)
            {
                firstMessage.Seen = true;
                await _context.SaveChangesAsync();
            }

            //Usamos un select de esta manera (en ejecucion en memoria) y no una expression (ejecucion en sql) porque en este metodo evitamos el uso de AsNoTracking al tener que modificar el seen del ultimo mensaje.
            //La proyección (Select) se hace en memoria para evitar problemas de traducción en EF Core
            var response = chatHistories.Select(e => new ChatHistoryResponse
            {
                FromUserId = e.FromUserId,
                ToUserId = e.ToUserId,
                Message = e.Message,
                Seen = e.Seen,
                DateSend = e.CreatedDate
            }).ToList();

            return await Result<IEnumerable<ChatHistoryResponse>>.SuccessAsync(response);
        }

        public async Task<IResult<IEnumerable<ChatUserResponse>>> LoadChatUsers(string CurrentUserId)
        {
            //Expression<Func<ChatRoom, ChatUserResponse>> expression = e => new ChatUserResponse
            //{
            //    LastMessage = e.LastMessage,
            //    Seen = e.Seen,
            //    LastMessageFrom = e.LastMessageFrom,
            //};// La expression no funciona si la consulta liqn tiene un GroupBy

            var salas = await _context.ChatRoom
                          .Where(x => x.IdUser1 == CurrentUserId || x.IdUser2 == CurrentUserId)
                          //.Select(expression)
                          .AsNoTracking()
                          .ToListAsync();

            if (!salas.Any())
                return await Result<IEnumerable<ChatUserResponse>>.FailAsync("No hay chats historicos");

            var result = new List<ChatUserResponse>();

            for (int i = 0; i < salas.Count; i++)
            {
                var chat = new ChatUserResponse()
                {
                    LastMessage = salas[i].LastMessage,
                    Seen = salas[i].Seen,
                    LastMessageFrom = salas[i].LastMessageFrom,
                };

                if (salas[i].LastMessageFrom != CurrentUserId)
                {
                    var user = await _userService.GetByIdAsync(salas[i].LastMessageFrom!);
                    if (user.Succeeded)
                        chat.ProfilePictureUrl = user.Data.Url;
                }

                result.Add(chat);
            }
            return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(result);
        }

        public async Task<IResult> SaveMessage(SaveChatMessage chat)
        {
            var userResponse = await _userService.GetByIdAsync(chat.ToUserId);

            if (userResponse.Succeeded)
            {

                var sala = await _context.ChatRoom.Where(x =>
                       (x.IdUser1 == chat.FromUserId && x.IdUser2 == chat.ToUserId) ||
                       (x.IdUser1 == chat.ToUserId && x.IdUser2 == chat.FromUserId)).FirstOrDefaultAsync();

                if (sala != null)
                {
                    sala.LastMessage = chat.Message;
                    sala.Seen = false;
                    sala.LastMessageFrom = chat.FromUserId;
                }
                else
                {
                    await _context.ChatRoom.AddAsync(new ChatRoom
                    {
                        IdUser1 = chat.FromUserId,
                        IdUser2 = chat.ToUserId,
                        LastMessage = chat.Message,
                        Seen = false,
                        LastMessageFrom = chat.FromUserId,
                    });
                }

                await _context.ChatHistories.AddAsync(new ChatHistory<IGiftUser>
                {
                    FromUserId = chat.FromUserId,
                    ToUserId = chat.ToUserId,
                    Message = chat.Message,
                    CreatedDate = DateTime.Now,
                    Seen = false
                });

                await _context.SaveChangesAsync();

                return await Result.SuccessAsync();
            }

            return await Result.FailAsync("El usuario no existe");
        }
    }
}
