using IGift.Application.CQRS.Files;

namespace IGift.Application.Interfaces.Files
{
    /// <summary>
    /// La clase UploadService permite subir archivos y garantiza que no haya conflictos de nombres utilizando métodos para generar el siguiente nombre de archivo disponible.
    /// </summary>
    public interface IUploadFileService
    {
        /// <summary>
        /// Guarda un archivo de cualquier tipo en una carpeta del servidor.
        /// </summary>
        /// <param name="request">Información para procesar el guardado</param>
        /// <param name="ReplaceFile">En caso de ser true se elimina la foto con el mismo nombre y se guarda una nueva</param>
        /// <returns></returns>
        Task<string> UploadAsync(UploadRequest request, bool ReplaceFile);
    }
}
