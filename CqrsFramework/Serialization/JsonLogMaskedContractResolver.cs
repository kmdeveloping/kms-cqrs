using System.Reflection;
using CqrsFramework.Command;
using Destructurama.Attributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CqrsFramework.Serialization;

public class JsonLogMaskedContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);
        foreach (var jsonProperty in properties)
        {
            var property = type.GetProperty(jsonProperty.UnderlyingName);
            if (property != null)
            {
                LogMaskedAttribute? logMaskedAttribute = property.GetCustomAttribute<LogMaskedAttribute>();
                if (logMaskedAttribute != null)
                {
                    jsonProperty.PropertyType = typeof(string);
                    jsonProperty.ValueProvider = new JsonLogMaskedValueProvider(property, logMaskedAttribute);   
                }
            }
        }

        return properties;
    }
}