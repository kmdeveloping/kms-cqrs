using RateLimiter;

namespace CqrsFramework.RateLimiting;

public class RateLimiterConstraints
{
    public RateLimiterConstraints()
    {
        RateLimitingConstraints = new Dictionary<Type, List<IAwaitableConstraint>>();
    }

    public RateLimiterConstraints(IDictionary<Type, List<IAwaitableConstraint>> constraints)
    {
        RateLimitingConstraints = constraints ?? new Dictionary<Type, List<IAwaitableConstraint>>();
    }

    public IDictionary<Type, List<IAwaitableConstraint>> RateLimitingConstraints { get; set; }

    public bool HasKey(Type typeKey)
    {
        return RateLimitingConstraints.ContainsKey(typeKey);
    }

    public List<IAwaitableConstraint> this[Type type]
    {
        get { return RateLimitingConstraints[type]; }
        set
        {
            if(value == null || !value.Any())
                throw new ArgumentNullException(nameof(value), "At least one constraint is required.");

            RateLimitingConstraints[type] = value;
        }
    }
}