using System.Diagnostics;
using System.Security.Principal;
using CqrsFramework.Auditing;
using CqrsFramework.Command;
using CqrsFramework.Serialization;
using Newtonsoft.Json;

namespace CqrsFramework.Decorators.Command;

[DebuggerStepThrough]
public class AuditingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand: ICommand
{
    private readonly ICommandHandler<TCommand> _decoratedHandler;
    private readonly Func<IAuditHistory, Task> _saveAuditRecordFunc;
    private readonly IPrincipal _principal;
    private readonly JsonMaskedSerializerSettings _serializerSettings;
    private readonly bool _commandAuditingEnabled;

    public AuditingCommandHandlerDecorator(ICommandHandler<TCommand> decoratedHandler, IAuditSettings auditingConfiguration, IPrincipal principal, JsonMaskedSerializerSettings serializerSettings)
    {
        ArgumentNullException.ThrowIfNull(auditingConfiguration);
        if (auditingConfiguration.CommandAuditingEnabled && auditingConfiguration.SaveAuditHistoryRecordAction == null) 
            throw new ArgumentNullException(nameof(auditingConfiguration.SaveAuditHistoryRecordAction), $"Command auditing is enabled, but {nameof(auditingConfiguration.SaveAuditHistoryRecordAction)} is null");
        
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        _principal = principal;
        _serializerSettings = serializerSettings ?? throw new ArgumentNullException(nameof(serializerSettings));
        _saveAuditRecordFunc = auditingConfiguration.SaveAuditHistoryRecordAction;
        _commandAuditingEnabled = auditingConfiguration.CommandAuditingEnabled;
    }
    
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        await _decoratedHandler.HandleAsync(command, cancellationToken);
            
        if (_commandAuditingEnabled)
        {
            var executedBy = Environment.UserName;
            if (_principal?.Identity?.Name != null) executedBy = _principal.Identity.Name;

            var executedOn = Environment.MachineName;
            var executionData = JsonConvert.SerializeObject(command, _serializerSettings);
            
            var auditHistory = new AuditHistory
            {
                Name = command.GetType().Name,
                ExecutionData = executionData,
                ExecutedBy = executedBy,
                ExecutedOn = executedOn,
                CreatedDt = DateTime.UtcNow
            };
            
            await _saveAuditRecordFunc.Invoke(auditHistory);
        }
    }
}