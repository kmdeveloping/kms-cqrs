﻿using System.Diagnostics;

namespace CqrsFramework.Logging;

[DebuggerStepThrough]
public static class LoggerExtensions
{
    public static void Verbose(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Verbose(null, messageTemplate, propertyValues);
    }
        
    public static void Verbose(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.Log(new LogEntry(LoggingEventLevel.Verbose, messageTemplate, exception, propertyValues));
    }

    public static void Debug(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Debug(null, messageTemplate, propertyValues);
    }
        
    public static void Debug(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.Log(new LogEntry(LoggingEventLevel.Debug, messageTemplate, exception, propertyValues));
    }
        
    public static void Information(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Information(null, messageTemplate, propertyValues);
    }

    public static void Information(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.Log(new LogEntry(LoggingEventLevel.Information, messageTemplate, exception, propertyValues));
    }

    public static void Warning(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Warning(null, messageTemplate, propertyValues);
    }

    public static void Warning(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.Log(new LogEntry(LoggingEventLevel.Warning, messageTemplate, exception, propertyValues));
    }

    public static void Error(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Error(null, messageTemplate, propertyValues);
    }

    public static void Error(this ILogger logger, Exception exception)
    {
        logger.Error(exception, "{exception}", exception.Message);
    }
        
    public static void Error(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.Log(new LogEntry(LoggingEventLevel.Error, messageTemplate, exception, propertyValues));
    }
        
    public static void Fatal(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Fatal(null, messageTemplate, propertyValues);
    }

    public static void Fatal(this ILogger logger, Exception exception)
    {
        logger.Fatal("{exception}", exception, exception.Message);
    }

    public static void Fatal(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.Log(new LogEntry(LoggingEventLevel.Fatal, messageTemplate, exception, propertyValues));
    }
}