namespace CqrsFramework.Auditing;

public interface IAuditHistory
{
    string Name { get; set; }
    string ExecutionData { get; set; }
    string ExecutedBy { get; set; }
    string ExecutedOn { get; set; }
    DateTime CreatedDt { get; set; }
    DateTime? ModifiedDt { get; set; }
}

public class AuditHistory : IAuditHistory
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string ExecutionData { get; set; }
    public string ExecutedBy { get; set; }
    public string ExecutedOn { get; set; }
    public DateTime CreatedDt { get; set; }
    public DateTime? ModifiedDt { get; set; }
}