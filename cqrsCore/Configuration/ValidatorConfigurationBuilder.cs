using System.Reflection;
using cqrsCore.Validation;
using SimpleInjector;

namespace cqrsCore.Configuration;

public class ValidatorConfigurationBuilder
{
  private readonly Container _container;
  private readonly IList<Assembly> _assemblies;
  private readonly CqrsConfigurationBuilder _parentBuilder;
  private readonly IList<string> _disabledValidators;

  public ValidatorConfigurationBuilder(CqrsConfigurationBuilder parentBuilder, Container container,
    IList<Assembly> assemblies, IList<string> disabledValidators = null)
  {
    _container = container ?? throw new ArgumentNullException(nameof(container));
    _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    _parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));
    _disabledValidators = disabledValidators;
    
    RegisterValidators();
  }
  
  private void RegisterValidators()
  {
    var allValidators = _container.GetTypesToRegister(
      typeof(IValidator<>),
      _assemblies,
      options: new TypesToRegisterOptions()
      {
        IncludeGenericTypeDefinitions = true,
        IncludeComposites = true,
        IncludeDecorators = false
      });
    var validatorsToRegister = allValidators.Where(type => !_disabledValidators.Contains(type.Name));
    _container.Collection.Register(typeof(IValidator<>), validatorsToRegister);
  }

  public ValidatorConfigurationBuilder UseCompositeHandler()
  {
    _container.Register(typeof(IValidator<>), typeof(CompositeValidationHandler<>));
    return this;
  }

  public ValidatorConfigurationBuilder DecorateWith(Type decoratorType)
  {
    _container.RegisterDecorator(typeof(IValidator<>), decoratorType);
    return this;
  }

  public ValidatorConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle)
  {
    _container.RegisterDecorator(typeof(IValidator<>), decoratorType, lifestyle);
    return this;
  }

  public ValidatorConfigurationBuilder DecorateWith(Type decoratorType, Predicate<DecoratorPredicateContext> predicate)
  {
    this.DecorateWith(decoratorType, Lifestyle.Transient, predicate);
    return this;
  }

  public ValidatorConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate)
  {
    _container.RegisterDecorator(typeof(IValidator<>), decoratorType, lifestyle, predicate);
    return this;
  }
  
  public CqrsConfigurationBuilder And()
  {
    return _parentBuilder;
  }
}