using System.Text.Json;

namespace IGift.Application.Interfaces.Serialization.Options
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }

    
}
