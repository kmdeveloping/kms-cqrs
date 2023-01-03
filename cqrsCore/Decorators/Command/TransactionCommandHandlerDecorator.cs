using System.Transactions;
using cqrsCore.Command;

namespace cqrsCore.Decorators.Command;

public class TransactionCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand: ICommand
    {
        private readonly ICommandHandler<TCommand> _decoratedHandler;
        private readonly bool _transactionsEnabled;
        private readonly int _transactionTimeoutMinutes;
        private readonly ICommandTransactionSettings _commandTransactionSettings;

        public TransactionCommandHandlerDecorator(ICommandHandler<TCommand> decoratedHandler, ICommandTransactionSettings commandTransactionSettings)
        {
            _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
            _commandTransactionSettings = commandTransactionSettings ?? throw new ArgumentNullException(nameof(commandTransactionSettings));
            
            _transactionsEnabled = _commandTransactionSettings.TransactionsEnabled;
            _transactionTimeoutMinutes = _commandTransactionSettings.TransactionTimeoutMinutes;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (_transactionsEnabled)
            {
                var transactionOptions = new TransactionOptions{Timeout = new TimeSpan(0, _transactionTimeoutMinutes, 0)};
                if (Transaction.Current == null)
                {
                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                    // NOTE: For some reason, the transaction scope above (ambient transaction) was not working, so had to change to enlisting the transaction declaratively.
                    // I think this is due to the lifetime of the connection, which should technically be inside the scope of a transaction, but our repositories share the connection lifetime of the decorators.
                    //using(var transaction = _dbConnection.BeginTransaction())
                    {
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