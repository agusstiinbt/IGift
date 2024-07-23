using IGift.Application.Requests.Files;

namespace IGift.Application.Interfaces.Files
{
    public interface IUploadService//TODO implementar
    {
        string Uploadsync(UploadRequest request);
    }
}
