namespace CqrsFramework.Auditing;

public interface IAuditSettings
{
    bool CommandAuditingEnabled { get; set; }
    bool EventAuditingEnabled { get; set; }
    Action<IAuditHistory> SaveAuditHistoryRecordAction { get; set; }
}