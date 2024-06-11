using System.Reflection;
using CqrsFramework.Validation;
using SimpleInjector;

namespace CqrsFramework.Fluent;

public class CqrsConfigurationBuilder
{
    private readonly Container _container;
    public CqrsConfigurationBuilder(Container container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public CommandHandlerBatchConfigurationBuilder AddCommandHandlers(IList<Assembly> assemblies)
    {
        return new CommandHandlerBatchConfigurationBuilder(this, _container, assemblies);
    }

    public QueryHandlerBatchConfigurationBuilder AddQueryHandlers(IList<Assembly> assemblies)
    {
        return new QueryHandlerBatchConfigurationBuilder(this, _container, assemblies);
    }

    public EventHandlerBatchConfigurationBuilder AddEventHandlers(IList<Assembly> assemblies)
    {
        return new EventHandlerBatchConfigurationBuilder(this, _container, assemblies);
    }

    public ValidatorBatchConfigurationBuilder AddValidators(IList<Assembly> assemblies, IList<string> disabledValidators = null)
    {
        return new ValidatorBatchConfigurationBuilder(this, _container, assemblies, disabledValidators);
    }

    public CqrsConfigurationBuilder WithCqrsValidation<TValidator>()
        where TValidator : class, IValidator
    {
        _container.RegisterSingleton<IValidator, TValidator>();
        return this;
    }

    public Container Build()
    {
        return _container;
    }
}