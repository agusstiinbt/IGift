using IGift.Application.Requests.Files;

namespace IGift.Application.Interfaces.Files
{
    /// <summary>
    /// La clase UploadService permite subir archivos y garantiza que no haya conflictos de nombres utilizando métodos para generar el siguiente nombre de archivo disponible.
    /// </summary>
    public interface IUploadService//TODO implementar
    {
        string Uploadsync(UploadRequest request);
    }
}
