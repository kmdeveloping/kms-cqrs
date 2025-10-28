namespace CqrsFramework.Auditing;

public interface IAuditSettings
{
    bool CommandAuditingEnabled { get; set; }
    bool EventAuditingEnabled { get; set; }
    Func<IAuditHistory, Task> SaveAuditHistoryRecordAction { get; set; }
}

public class AuditSettings : IAuditSettings
{
    public bool CommandAuditingEnabled { get; set; }
    public bool EventAuditingEnabled { get; set; }
    public Func<IAuditHistory, Task> SaveAuditHistoryRecordAction { get; set; }
}