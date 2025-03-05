using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Interfaces.Communication.Chat;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Models.Chat;
using IGift.Infrastructure.Data;
using IGift.Infrastructure.Models;
using IGift.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cmp;

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

            var listaTerminada = new List<ChatUserResponse>();

            // Obtener los últimos mensajes enviados por el usuario actual
            var ultimosMensajesMios = await _context.ChatHistories
                .Include(x => x.FromUser)
                .Where(x => x.FromUserId == CurrentUserId)
                .GroupBy(x => x.ToUserId)
                .Select(x => x.OrderByDescending(c => c.CreatedDate).FirstOrDefault())
                .AsNoTracking()
                .ToListAsync();

            if (!ultimosMensajesMios.Any())
                return await Result<IEnumerable<ChatUserResponse>>.FailAsync();

            // Obtener IDs de los usuarios con los que conversó
            var usersId = ultimosMensajesMios
                .Select(x => x!.ToUserId)
                .Distinct()
                .ToList();
            var a = usersId.Contains("2");
            // 🔥 Cargar todos los mensajes en una sola consulta antes de la ejecución en paralelo
            var ultimosMensajesParaMi = await _context.ChatHistories
                .Include(x => x.FromUser)
                .Where(x => usersId.Contains(x.FromUserId) && x.ToUserId == CurrentUserId)//Aca le estamos indicando que se filtre con 2 datos: el primero es que contenga como FromUserId algun valor dentro de UsersId y el otro filtro es que el ToUserId sea el CurrentUserID
                .GroupBy(x => x.FromUserId)
                .Select(x => x.OrderByDescending(x => x.CreatedDate).FirstOrDefault())
                .AsNoTracking()
                .ToListAsync();

            // 🔥 Organizar los datos de los chats
            for (int i = 0; i < ultimosMensajesMios.Count; i++)
            {
                var mensajeRecibido = ultimosMensajesParaMi
                  .FirstOrDefault(x => x!.FromUserId == ultimosMensajesMios[i]!.ToUserId);

                var mensajeMasReciente = mensajeRecibido != null && mensajeRecibido.CreatedDate > ultimosMensajesMios[i]!.CreatedDate
                    ? mensajeRecibido
                    : ultimosMensajesMios[i]!;

                listaTerminada.Add(new ChatUserResponse
                {
                    LastMessage = mensajeMasReciente.Message,
                    Seen = mensajeMasReciente.Seen,
                    IsLastMessageFromMe = mensajeMasReciente.FromUserId == CurrentUserId,
                    ProfilePictureUrl = string.Empty, // Se actualizará después
                    UserName = mensajeMasReciente.FromUser.UserName // Se actualizará después
                });
            }
            return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(listaTerminada);

            #region Codigo viejo

            //      var salas = await _context.ChatRoom
            //.Where(x => x.IdUser1 == CurrentUserId || x.IdUser2 == CurrentUserId)
            //.AsNoTracking()
            //.ToListAsync();

            //      if (!salas.Any())
            //      {
            //          return await Result<IEnumerable<ChatUserResponse>>.FailAsync("No hay chats históricos");
            //      }

            //      // Crear lista de ChatUserResponse con el ID del otro usuario
            //      var chatResponses = salas.Select(chat => new
            //      {
            //          ChatResponse = new ChatUserResponse
            //          {
            //              LastMessage = chat.LastMessage,
            //              Seen = chat.Seen,
            //              IsLastMessageFromMe = chat.LastMessageFrom == CurrentUserId,
            //              ProfilePictureUrl = string.Empty, // Se actualizará después
            //              ToUserId = chat.IdUser1 != CurrentUserId ? chat.IdUser1 : chat.IdUser2,
            //              UserName = string.Empty // Se actualizará después
            //          },
            //          OtherUserId = chat.IdUser1 != CurrentUserId ? chat.IdUser1 : chat.IdUser2
            //      }).ToList();

            //      // Obtener IDs únicos de los otros usuarios
            //      var userIdsToFetch = chatResponses
            //          .Select(c => c.OtherUserId)
            //          .Distinct()
            //          .ToList();

            //      // Consultar todos los usuarios en paralelo
            //      var userTasks = userIdsToFetch
            //          .Select(async userId =>
            //          {
            //              var userResponse = await _userService.GetByIdAsync(userId);
            //              return userResponse.Succeeded
            //                  ? new Tuple<string, string?, string?>(userId, userResponse.Data.ProfilePictureDataUrl, userResponse.Data.UserName)
            //                  : null;
            //          });

            //      // Ejecutar todas las tareas en paralelo
            //      var users = (await Task.WhenAll(userTasks))
            //          .Where(u => u != null)
            //          .ToDictionary(u => u!.Item1, u => new { ProfilePictureDataUrl = u!.Item2, UserName = u.Item3 });

            //      // Asignar la foto y el username correctos a cada ChatUserResponse
            //      foreach (var chat in chatResponses)
            //      {
            //          if (users.TryGetValue(chat.OtherUserId, out var userData))
            //          {
            //              chat.ChatResponse.ProfilePictureUrl = userData.ProfilePictureDataUrl;
            //              chat.ChatResponse.UserName = userData.UserName;
            //          }
            //      }

            //      return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(chatResponses.Select(c => c.ChatResponse));

            #endregion

        }

        public async Task<IResult> SaveMessage(SaveChatMessage chat)
        {
            //var userResponse = await _userService.GetByIdAsync(chat.ToUserId);

            //if (userResponse.Succeeded)
            //{

            //    var sala = await _context.ChatRoom.Where(x =>
            //           (x.IdUser1 == chat.FromUserId && x.IdUser2 == chat.ToUserId) ||
            //           (x.IdUser1 == chat.ToUserId && x.IdUser2 == chat.FromUserId)).FirstOrDefaultAsync();

            //    if (sala != null)
            //    {
            //        sala.LastMessage = chat.Message;
            //        sala.Seen = false;
            //        sala.LastMessageFrom = chat.FromUserId;
            //    }
            //    else
            //    {
            //        await _context.ChatRoom.AddAsync(new ChatRoom
            //        {
            //            IdUser1 = chat.FromUserId,
            //            IdUser2 = chat.ToUserId,
            //            LastMessage = chat.Message,
            //            Seen = false,
            //            LastMessageFrom = chat.FromUserId,
            //        });
            //    }

            //    await _context.ChatHistories.AddAsync(new ChatHistory<IGiftUser>
            //    {
            //        FromUserId = chat.FromUserId,
            //        ToUserId = chat.ToUserId,
            //        Message = chat.Message,
            //        CreatedDate = DateTime.Now,
            //        Seen = false
            //    });

            //    await _context.SaveChangesAsync();

            //    return await Result.SuccessAsync();
            //}

            return await Result.FailAsync("El usuario no existe");
        }
    }
}
