using IGift.Application.Requests.Files;

namespace IGift.Application.Interfaces.Files
{
    public interface IUploadService
    {
        string Uploadsync(UploadRequest request);
    }
}
