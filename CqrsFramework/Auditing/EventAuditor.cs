using System.Diagnostics;
using System.Security.Principal;
using CqrsFramework.Event;
using CqrsFramework.Serialization;
using Newtonsoft.Json;

namespace CqrsFramework.Auditing;

public interface IEventAuditor
{
    Task AuditAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
}

[DebuggerStepThrough]
public class EventAuditor : IEventAuditor
{
    private readonly Func<IAuditHistory, Task> _saveAuditRecordFunc;
    private readonly bool _eventAuditingEnabled;
    private readonly IPrincipal _principal;
    private readonly JsonMaskedSerializerSettings _serializerSettings;
    
    public EventAuditor(IAuditSettings auditingConfiguration, IPrincipal principal, JsonMaskedSerializerSettings serializerSettings)
    {
        ArgumentNullException.ThrowIfNull(auditingConfiguration);
        if (auditingConfiguration is { CommandAuditingEnabled: true, SaveAuditHistoryRecordAction: null }) 
            throw new ArgumentNullException(nameof(auditingConfiguration.SaveAuditHistoryRecordAction), $"Command auditing is enabled, but {nameof(auditingConfiguration.SaveAuditHistoryRecordAction)} is null");
    
        _principal = principal ?? throw new ArgumentNullException(nameof(principal));
        _serializerSettings = serializerSettings ?? throw new ArgumentNullException(nameof(serializerSettings));
        _saveAuditRecordFunc = auditingConfiguration.SaveAuditHistoryRecordAction!;
        _eventAuditingEnabled = auditingConfiguration.CommandAuditingEnabled;
    }
    
    public async Task AuditAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
    {
        if (_eventAuditingEnabled)
        {
            var executedBy = Environment.UserName;
            if (_principal?.Identity?.Name != null)
                executedBy = _principal.Identity.Name;
    
            var executedOn = Environment.MachineName;
            var commandData = JsonConvert.SerializeObject(@event, _serializerSettings);
                
            var commandHistory = new AuditHistory()
            {
                Name = @event.GetType().Name,
                ExecutionData = commandData,
                ExecutedBy = executedBy,
                ExecutedOn = executedOn,
                CreatedDt = DateTime.UtcNow
            };
            
            await _saveAuditRecordFunc.Invoke(commandHistory);
        }
    }
}