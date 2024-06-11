namespace CqrsFramework.Validation;

public interface IValidatable
{
  IEnumerable<string> DisabledValidators { get; set; }
}