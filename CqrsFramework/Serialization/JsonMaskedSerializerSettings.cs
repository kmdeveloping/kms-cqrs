using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CqrsFramework.Serialization;

public class JsonMaskedSerializerSettings : JsonSerializerSettings
{
    public JsonMaskedSerializerSettings()
    {
        ContractResolver = new JsonLogMaskedContractResolver();
    }
}