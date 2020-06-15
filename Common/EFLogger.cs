using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace PointsMall.Common
{
    public class EFLogger : ILogger
    {
        private readonly string categoryName;
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public EFLogger(string categoryName) => this.categoryName = categoryName;

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var logContent = formatter(state, exception);            
            _logger.Info($"SQL:{logContent}");            
            //Debug.WriteLine($"时间:{DateTime.Now.ToString("o")} 日志级别: {logLevel} {eventId.Id} 产生的类{this.categoryName}");
            //Debug.WriteLine($"SQL语句:{logContent}");
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public class NullLogger : ILogger
    {
        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        { }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public class MyFilteredLoggerProvider : ILoggerProvider
    {
        private static NullLogger nullLogger { get; set; } = new NullLogger();
        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName == DbLoggerCategory.Database.Command.Name)
            {
                return new EFLogger(categoryName);
            }
            return nullLogger;
        }
        public void Dispose()
        { }
    }
}
