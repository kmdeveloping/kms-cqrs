using System.Diagnostics;
using System.Security;
using System.Security.Principal;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using CqrsFramework.Query;

namespace CqrsFramework.Decorators.Query;

[DebuggerStepThrough]
public class AuthorizationQueryHandlerDecorator<TQuery,TResult> : IQueryHandler<TQuery,TResult>
    where TQuery: IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _decoratedHandler;
    private readonly IPrincipal _currentPrincipal;
    private readonly ILogger _logger;

    public AuthorizationQueryHandlerDecorator(IQueryHandler<TQuery, TResult> decoratedHandler, IPrincipal currentPrincipal, ILogger logger)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        _currentPrincipal = currentPrincipal ?? throw new ArgumentNullException(nameof(currentPrincipal));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(AuthorizationQueryHandlerDecorator<TQuery,TResult>));
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        Authorize();

        return await _decoratedHandler.HandleAsync(query, cancellationToken);
    }

    private void Authorize()
    {
        // TODO: Read roles from query or queryHandler and check here...
        // Another option would be to store matrix of roles externally in config or database.

        if (typeof(TQuery).Namespace.Contains("Admin") && !_currentPrincipal.IsInRole("Admin"))
            throw new SecurityException();

        _logger.Information("User {Principal} has been authorized to execute {Query}",
            _currentPrincipal?.Identity?.Name ?? "Unknown", typeof(TQuery).GetFriendlyName());
    }
}