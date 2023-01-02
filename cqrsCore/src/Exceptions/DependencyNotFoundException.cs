namespace cqrsCore.Exceptions;

public class DependencyNotFoundException : Exception
{
  private const string MessageFormat = "An instance of the specified type {0} was not found in the container. Check if type is registered.";

  public DependencyNotFoundException(Type requestedType) 
    : base(string.Format(MessageFormat, requestedType.Name))
  {
  }

  public DependencyNotFoundException(Type requestedType, string message)
    : base(message)
  {
    RequestedType = requestedType;
  }
  
  public DependencyNotFoundException(Type requestedType, string message, Exception innerException) 
    : base(message, innerException)
  {
    RequestedType = requestedType;
  }
  
  public Type RequestedType { get; private set; }
}