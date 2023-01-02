namespace cqrsCore.Validation;

public sealed class ValidationMessage
{
  public string Message { get; internal set; }
  public ValidationMessageType MessageType { get; internal set; }
  public int? Code { get; internal set; }
  internal ValidationMessage(string message, ValidationMessageType messageType, int? code = 0)
  {
    Message = message;
    MessageType = messageType;
    Code = code;
  }

  public enum ValidationMessageType
  {
    Error,
    Warning,
    Success
  }
}