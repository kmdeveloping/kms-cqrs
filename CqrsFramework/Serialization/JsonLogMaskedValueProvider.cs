using System.Reflection;
using Destructurama.Attributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CqrsFramework.Serialization;

public class JsonLogMaskedValueProvider : IValueProvider
{
    private readonly PropertyInfo _propertyInfo;
    private readonly LogMaskedAttribute _jsonMaskedPropertyAttribute;
    
    public JsonLogMaskedValueProvider(PropertyInfo propertyInfo, LogMaskedAttribute jsonMaskedPropertyAttribute)
    {
        _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        _jsonMaskedPropertyAttribute = jsonMaskedPropertyAttribute ?? throw new ArgumentNullException(nameof(jsonMaskedPropertyAttribute));
    }

    /// <inheritdoc />
    public void SetValue(object target, object? value)
    {
        // used for unmasking masked values, which is not supported!
        throw new NotSupportedException("Can't unmask a masked value");
    }
    
    /// <inheritdoc />
    public object? GetValue(object target)
    {
        object unmaskedValue = _propertyInfo.GetValue(target);
        return unmaskedValue != null ? GetMaskedJson(unmaskedValue) : null;
    }
    
    private object? GetMaskedJson(object unmaskedValue)
    {
        if (unmaskedValue is string)
            return MaskJsonValues((string)unmaskedValue);
        
        string? unmaskedJson = JsonConvert.SerializeObject(unmaskedValue);
        return MaskJsonValues(unmaskedJson);
    }

    private object? MaskJsonValues(string? unmaskedJson)
    {
        // TODO: implement masking logic
        if (_jsonMaskedPropertyAttribute.TryCreateLogMaskedProperty(unmaskedJson, out object? maskedProperty))
            return maskedProperty;
        
        return unmaskedJson;
    }
}