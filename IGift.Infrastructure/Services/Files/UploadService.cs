using IGift.Application.Extensions;
using IGift.Application.Interfaces.Files;
using IGift.Application.Requests.Files;

namespace IGift.Infrastructure.Services.Files
{
    //TODO estudiar esta clase
    public class UploadService : IUploadService
    {
        private static string numberPattern = " ({0})";

        public string Uploadsync(UploadRequest request)
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
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = NextAvailableFilename(fullPath);
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    streamData.CopyTo(stream);
                }
                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Este método genera el siguiente nombre de archivo disponible si el archivo con el nombre dado ya existe.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NextAvailableFilename(string path)
        {
            if (!File.Exists(path))// Si el archivo no existe, retorna la ruta original
                return path;

            if (Path.HasExtension(path))//Si el archivo tiene extensión, inserta un patrón de numeración justo antes de la extensión. Si no tiene extensión, agrega el patrón al final.
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            return GetNextFilename(path + numberPattern);
        }

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
