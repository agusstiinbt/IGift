namespace IGift.Application.Interfaces.Serialization.Options
{
    /// <summary>
    /// Interfaz que define los métodos Serialize y Deserialize para convertir objetos a y desde JSON.
    /// </summary>
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}//TODO documentar bien para que casos seria util este tipo de serializador y el otro tambien. Asi sabemos bien que microservicio deberia ser utilizado
