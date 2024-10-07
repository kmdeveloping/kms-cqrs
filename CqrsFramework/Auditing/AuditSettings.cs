namespace CqrsFramework.Auditing;

public interface IAuditSettings
{
    bool CommandAuditingEnabled { get; set; }
    bool EventAuditingEnabled { get; set; }
    Action<IAuditHistory> SaveAuditHistoryRecordAction { get; set; }
}

public class AuditSettings : IAuditSettings
{
    public bool CommandAuditingEnabled { get; set; }
    public bool EventAuditingEnabled { get; set; }
    public Action<IAuditHistory> SaveAuditHistoryRecordAction { get; set; }
}