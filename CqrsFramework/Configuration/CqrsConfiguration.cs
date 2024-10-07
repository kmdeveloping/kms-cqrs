using CqrsFramework.Auditing;

namespace CqrsFramework.Configuration;

public class CqrsConfiguration
{
    public IAuditSettings AuditingConfiguration { get; set; } = new AuditSettings();
    public List<string> DisabledValidators { get; set; } = new List<string>();
}