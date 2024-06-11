namespace CqrsFramework.RateLimiting;

public class RateLimiterConfiguration
{
    public Dictionary<string, List<RateLimiterConfigurationItem>> RateLimiters { get; set; } = new Dictionary<string, List<RateLimiterConfigurationItem>>();
}

public class RateLimiterConfigurationItem
{
    public RateLimitingMode RateLimitingMode { get; set; }
    public int Count { get; set; }
    public int Time { get; set; }
    public RateLimitingUnits Units { get; set; }

    public TimeSpan GetTimeSpan()
    {
        TimeSpan ts = TimeSpan.Zero;
            
        switch (Units)
        {
            case RateLimitingUnits.Milliseconds:
                ts = TimeSpan.FromMilliseconds(Time);
                break;

            case RateLimitingUnits.Seconds:
                ts = TimeSpan.FromSeconds(Time);
                break;

            case RateLimitingUnits.Minutes:
                ts = TimeSpan.FromMinutes(Time);
                break;
        }

        return ts;
    }
}

public enum RateLimitingUnits
{
    Milliseconds,
    Seconds,
    Minutes
}
public enum RateLimitingMode
{
    CountByInterval,
    PersistentCountByInterval
}