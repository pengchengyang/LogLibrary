using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;

namespace Rolling.Logging
{
    /// <summary>
    /// 日志依赖注入扩展
    /// </summary>
    public static class RollingLoggerExtensions
    {
        /// <summary>
        /// 添加Rolling日志到DI容器
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="options">日志配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddRollingLogger(this IServiceCollection services, Action<RollingLoggerOptions> options = null)
        {
            var loggerOptions = new RollingLoggerOptions();
            options?.Invoke(loggerOptions);

            // 初始化Serilog
            var logger = RollingLogger.CreateLogger(loggerOptions);
            Log.Logger = logger;

            services.AddSingleton<RollingLogger>(_ => new RollingLogger(loggerOptions));

            // 手动替换Microsoft日志提供程序（不使用AddSerilog扩展）
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                // 直接添加Serilog日志提供程序
                logging.Services.AddSingleton<ILoggerProvider>(new SerilogLoggerProvider(logger, dispose: true));
            });

            return services;
        }
    }
}