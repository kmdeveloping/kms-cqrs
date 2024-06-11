using Newtonsoft.Json;

namespace CqrsFramework.Serialization;

public class JsonMaskedSerializerSettings : JsonSerializerSettings
{
    public JsonMaskedSerializerSettings()
    {
        ContractResolver = new JsonLogMaskedContractResolver();
    }
}