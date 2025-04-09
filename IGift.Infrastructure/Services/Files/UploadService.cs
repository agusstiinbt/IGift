using IGift.Application.CQRS.Files;
using IGift.Application.Extensions;
using IGift.Application.Interfaces.Files;
using Microsoft.Extensions.Logging;

namespace IGift.Infrastructure.Services.Files
{
    public class UploadService : IUploadFileService
    {
        private readonly ILogger<UploadService> _logger;

        public UploadService(ILogger<UploadService> logger)
        {
            this._logger = logger;
        }

        private static string numberPattern = " ({0})";

        public async Task<string> UploadAsync(UploadRequest request, bool ReplaceFile)
        {
            if (request.Data == null) return string.Empty;
            var streamData = new MemoryStream(request.Data);
            if (streamData.Length > 0)
            {
                var folder = request.UploadType.ToDescriptionString();
                var folderName = Path.Combine("Files", folder);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                bool exists = Directory.Exists(pathToSave);
                if (!exists)
                    Directory.CreateDirectory(pathToSave);
                var fileName = request.FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);
                if (File.Exists(dbPath))
                {
                    if (!ReplaceFile)//Con esto evitamos eliminar duplicados
                    {
                        dbPath = NextAvailableFilename(dbPath);
                        fullPath = NextAvailableFilename(fullPath);
                    }
                    else
                    {
                        System.IO.File.Delete(dbPath);
                    }
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await streamData.CopyToAsync(stream);
                }
                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Este método devuelve el directorio actual del proceso en ejecución. En un entorno de ASP.NET Core, esto suele ser la raíz del proyecto donde se está ejecutando la aplicación, que generalmente es el proyecto servidor
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        /// <summary>
        /// Este método genera el siguiente nombre de archivo disponible si el archivo con el nombre dado ya existe.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <summary>
        /// Este método implementa una búsqueda binaria para encontrar el siguiente nombre de archivo disponible.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            //if (tmp == pattern)
            //throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }
    }
}
