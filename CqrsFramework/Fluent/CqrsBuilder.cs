using System.Reflection;
using CqrsFramework.Validation;
using SimpleInjector;

namespace CqrsFramework.Fluent;

public class CqrsConfigurationBuilder(Container container)
{
    private readonly Container _container = container ?? throw new ArgumentNullException(nameof(container));

    public CommandHandlerBatchConfigurationBuilder AddCommandHandlers(IList<Assembly> assemblies) => new(this, _container, assemblies);

    public QueryHandlerBatchConfigurationBuilder AddQueryHandlers(IList<Assembly> assemblies) => new(this, _container, assemblies);

    public EventHandlerBatchConfigurationBuilder AddEventHandlers(IList<Assembly> assemblies) => new(this, _container, assemblies);

    public ValidatorBatchConfigurationBuilder AddValidators(IList<Assembly> assemblies, IList<string> disabledValidators = null) => new(this, _container, assemblies, disabledValidators);

    public CqrsConfigurationBuilder WithCqrsValidation<TValidator>() where TValidator : class, IValidator
    {
        _container.RegisterSingleton<IValidator, TValidator>();
        return this;
    }

    public Container Build() => _container;
}