namespace cqrsCore.Validation;

public sealed class ValidationResult
{
    private bool _isValid = false;
    private bool _isDirty = true;

    public bool IsValid
    {
        get
        {
            if (_isDirty)
            {
                _isValid = !(Messages.Any(m => m.MessageType == ValidationMessage.ValidationMessageType.Error));
                _isDirty = false;
            }

            return _isValid;
        }
    }

    public IList<ValidationMessage> Messages { get; internal set; } = new List<ValidationMessage>();

    public void AddValidationMessage(ValidationMessage message)
    {
        if (message.MessageType == ValidationMessage.ValidationMessageType.Error)
            _isDirty = true;

        Messages.Add(message);
    }

    public void AddValidationError(string errorMessage, int code = 0)
    {
        AddValidationMessage(new ValidationMessage(errorMessage, ValidationMessage.ValidationMessageType.Error, code));
    }

    public void AddValidationWarning(string warningMessage, int? code = null)
    {
        AddValidationMessage(new ValidationMessage(warningMessage, ValidationMessage.ValidationMessageType.Warning, code));
    }

    public void AddValidationSuccess(string successMessage)
    {
        AddValidationMessage(new ValidationMessage(successMessage, ValidationMessage.ValidationMessageType.Success));
    }

    public override string ToString()
    {
        List<string> messages = Messages
            .Select(m => $"{m.MessageType} - {m.Code} - {m.Message}")
            .Distinct()
            .ToList();
        string joinedMessages = String.Join(separator: "\r\n", values: messages);

        return $"Is valid: {IsValid}, {joinedMessages}";
    }

    public string GetValidationErrors()
    {
        string errorMessage = string.Empty;
        var errors = Messages
            .Where(m => m.MessageType == ValidationMessage.ValidationMessageType.Error)
            .ToList();

        if (errors.Any())
        {
            errorMessage = errors
                .Select(e => $"{e.Code} - {e.Message}")
                .Distinct()
                .Aggregate((c, n) => $"{c}, {n}");
        }

        return errorMessage;
    }
}