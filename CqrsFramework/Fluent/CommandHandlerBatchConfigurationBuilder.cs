using System.Reflection;
using CqrsFramework.Auditing;
using CqrsFramework.Command;
using CqrsFramework.RateLimiting;
using SimpleInjector;

namespace CqrsFramework.Fluent;

public class CommandHandlerBatchConfigurationBuilder
{
    private readonly Container _container;
    private readonly IList<Assembly> _assemblies;
    private readonly CqrsConfigurationBuilder _parentBuilder;
    public CommandHandlerBatchConfigurationBuilder(CqrsConfigurationBuilder parentBuilder, Container container, IList<Assembly> assemblies)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        _parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));

        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        _container.Register(typeof(ICommandHandler<>), _assemblies);
    }

    public CommandHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType)
    {
        _container.RegisterDecorator(typeof(ICommandHandler<>), decoratorType);
        return this;
    }

    public CommandHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle)
    {
        _container.RegisterDecorator(typeof(ICommandHandler<>), decoratorType, lifestyle);
        return this;
    }

    public CommandHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Predicate<DecoratorPredicateContext> predicate)
    {
        this.DecorateWith(decoratorType, Lifestyle.Transient, predicate);
        return this;
    }

    public CommandHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate)
    {
        _container.RegisterDecorator(typeof(ICommandHandler<>), decoratorType, lifestyle, predicate);
        return this;
    }


    public CommandHandlerBatchConfigurationBuilder WithTransactionConfiguration<TSettings>(
        ICommandTransactionSettings settings)
        where TSettings : ICommandTransactionSettings
    {
        _container.RegisterInstance(settings);
        return this;
    }

    public CommandHandlerBatchConfigurationBuilder WithTransactionConfiguration<TSettings>(
        Func<ICommandTransactionSettings> getSettings)
        where TSettings: ICommandTransactionSettings
    {
        return this.WithTransactionConfiguration<TSettings>(getSettings());
    }

    public CommandHandlerBatchConfigurationBuilder WithAuditingConfiguration<TSettings,TAuditHistory>(
        IAuditSettings settings)
        where TSettings : IAuditSettings
        where TAuditHistory : IAuditHistory, new()
    {
        _container.RegisterInstance(settings);
        return this;
    }

    public CommandHandlerBatchConfigurationBuilder WithAuditingConfiguration<TSettings,TAuditHistory>(
        Func<IAuditSettings> getSettings)
            where TSettings : IAuditSettings
            where TAuditHistory : IAuditHistory, new()
    {
        return WithAuditingConfiguration<TSettings,TAuditHistory>(getSettings());
    }

    public CommandHandlerBatchConfigurationBuilder WithRateLimitingConfiguration(Func<RateLimiterConstraints> getSettings)
    {
        if (getSettings != null)
            return WithRateLimitingConfiguration(getSettings());
        else
            return this;
    }
        
    public CommandHandlerBatchConfigurationBuilder WithRateLimitingConfiguration(RateLimiterConstraints constraints)
    {
        if(constraints != null)
            _container.RegisterInstance(constraints);

        return this;
    }

    public CqrsConfigurationBuilder And()
    {
        return _parentBuilder;
    }
}