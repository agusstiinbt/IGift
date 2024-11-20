﻿using IGift.Application.Interfaces.Serialization.Options;
using IGift.Application.Serialization.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IGift.Application.Serialization.Serializers
{  /// <summary>
   /// Implementacion que se puede especificar si usar Newtonsoft.Json
   /// </summary>
    public class NewtonSoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonSoftJsonSerializer(IOptions<NewtonsoftJsonSettings> settings)
        {
            _settings = settings.Value.JsonSerializerSettings;
        }

        public T Deserialize<T>(string text) => JsonConvert.DeserializeObject<T>(text, _settings);

        public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj, _settings);
    }
}