using Destructurama.Attributed;

namespace CqrsFramework.Serialization;

public static class LogMaskedAttributeExtensions
{
    const string? DefaultMask = "***";
    
     
    public static bool TryCreateLogMaskedProperty(this LogMaskedAttribute logMaskedAttribute, object? value, out object? property)
    {
        property = CreateValue(logMaskedAttribute, value) ?? string.Empty;
        return true;
    }
    
    private static bool IsDefaultMask(LogMaskedAttribute logMaskedAttribute)
    {
        return logMaskedAttribute.Text == DefaultMask;
    }
    
    private static object? FormatMaskedValue(LogMaskedAttribute logMaskedAttribute, string? val)
    {
        if (string.IsNullOrEmpty(val)) return val;

        if (logMaskedAttribute.ShowFirst == 0 && logMaskedAttribute.ShowLast == 0)
        {
            if (logMaskedAttribute.PreserveLength)
                return new string(logMaskedAttribute.Text[0], val.Length);

            return logMaskedAttribute.Text;
        }

        if (logMaskedAttribute.ShowFirst > 0 && logMaskedAttribute.ShowLast == 0)
        {
            var first = val.Substring(0, Math.Min(logMaskedAttribute.ShowFirst, val.Length));

            if (!logMaskedAttribute.PreserveLength || !IsDefaultMask(logMaskedAttribute))
                return first + logMaskedAttribute.Text;

            var mask = "";
            if (logMaskedAttribute.ShowFirst <= val.Length)
                mask = new(logMaskedAttribute.Text[0], val.Length - logMaskedAttribute.ShowFirst);

            return first + mask;

        }

        if (logMaskedAttribute.ShowFirst == 0 && logMaskedAttribute.ShowLast > 0)
        {
            var last = logMaskedAttribute.ShowLast > val.Length ? val : val.Substring(val.Length - logMaskedAttribute.ShowLast);

            if (!logMaskedAttribute.PreserveLength || !IsDefaultMask(logMaskedAttribute))
                return logMaskedAttribute.Text + last;

            var mask = string.Empty;
            if (logMaskedAttribute.ShowLast <= val.Length)
                mask = new(logMaskedAttribute.Text[0], val.Length - logMaskedAttribute.ShowLast);

            return mask + last;
        }

        if (logMaskedAttribute.ShowFirst > 0 && logMaskedAttribute.ShowLast > 0)
        {
            if (logMaskedAttribute.ShowFirst + logMaskedAttribute.ShowLast >= val.Length)
                return val;

            var first = val.Substring(0, logMaskedAttribute.ShowFirst);
            var last = val.Substring(val.Length - logMaskedAttribute.ShowLast);

            string mask = null!;
            if (logMaskedAttribute.PreserveLength && IsDefaultMask(logMaskedAttribute))
                mask = new string(logMaskedAttribute.Text[0], val.Length - logMaskedAttribute.ShowFirst - logMaskedAttribute.ShowLast);

            return first + (mask ?? logMaskedAttribute.Text) + last;
        }

        return val;
    }
    
    private static object? CreateValue(LogMaskedAttribute logMaskedAttribute, object? value)
    {
        if (value is IEnumerable<string> strings)
            return strings.Select(s => FormatMaskedValue(logMaskedAttribute, s));
        
        if (value is string s1)
            return FormatMaskedValue(logMaskedAttribute, s1);
        
        return null;
    }
}