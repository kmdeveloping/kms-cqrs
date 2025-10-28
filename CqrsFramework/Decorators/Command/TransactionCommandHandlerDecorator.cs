using System.Diagnostics;
using System.Transactions;
using CqrsFramework.Command;

namespace CqrsFramework.Decorators.Command
{
    [DebuggerStepThrough]
    public class TransactionCommandHandlerDecorator<TCommand>(ICommandHandler<TCommand> decoratedHandler, ICommandTransactionSettings commandTransactionSettings) : ICommandHandler<TCommand> where TCommand: ICommand
    {
        private readonly ICommandHandler<TCommand> _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        private readonly CommandTransactionSettings _ = commandTransactionSettings as CommandTransactionSettings ?? throw new ArgumentNullException(nameof(commandTransactionSettings));
        
        private readonly bool _transactionsEnabled = commandTransactionSettings.TransactionsEnabled;
        private readonly int _transactionTimeoutMinutes = commandTransactionSettings.TransactionTimeoutMinutes;
        

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (_transactionsEnabled)
            {
                var transactionOptions = new TransactionOptions{Timeout = new TimeSpan(0, _transactionTimeoutMinutes, 0)};
                if (Transaction.Current == null)
                {
                    using var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions);
                    await _decoratedHandler.HandleAsync(command, cancellationToken);

                    // NOTE: If command is no-op, don't complete the transaction...
                    if (!command.ExecuteAsNoOp)
                    {
                        transactionScope.Complete();
                        //transaction.Commit();
                    }
                    else
                    {
                        //transaction.Rollback();
                    }
                }
                else
                {
                    await _decoratedHandler.HandleAsync(command, cancellationToken);
                }
            }
            else
            {
                if (command.ExecuteAsNoOp)
                    throw new NotSupportedException("No-Op is not supported when transactions are disabled. Check configuration.");
                
                await _decoratedHandler.HandleAsync(command, cancellationToken);
            }
        }
    }
}