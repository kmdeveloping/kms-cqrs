namespace CqrsFramework.Auditing;

[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
public class AuditingDisabledAttribute : Attribute
{
}