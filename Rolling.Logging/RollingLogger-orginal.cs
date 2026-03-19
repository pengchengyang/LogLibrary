using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Rolling.Logging
{
    /// <summary>
    /// Rolling公司日志封装类
    /// </summary>
    public class RollingLogger
    {
        private static ILogger _logger;
        private readonly ILogger _instanceLogger;

        /// <summary>
        /// 静态构造函数（默认初始化）
        /// </summary>
        static RollingLogger()
        {
            Initialize(new RollingLoggerOptions());
        }

        /// <summary>
        /// 实例构造函数（指定配置）
        /// </summary>
        /// <param name="options">日志配置</param>
        public RollingLogger(RollingLoggerOptions options)
        {
            _instanceLogger = CreateLogger(options);
        }

        /// <summary>
        /// 初始化日志（全局配置）
        /// </summary>
        /// <param name="options">日志配置项</param>
        public static void Initialize(RollingLoggerOptions options)
        {
            _logger = CreateLogger(options);
            Log.Logger = _logger; // 关联Serilog全局日志
        }

        /// <summary>
        /// 创建日志实例
        /// </summary>
        /// <param name="options">配置项</param>
        /// <returns>ILogger实例</returns>
        public static ILogger CreateLogger(RollingLoggerOptions options)
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Is(options.MinimumLevel)
                // 丰富日志上下文（添加机器名/应用名）
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName();

            // 控制台输出
            if (options.EnableConsole)
            {
                loggerConfig.WriteTo.Console(
                    outputTemplate: options.OutputTemplate,
                    restrictedToMinimumLevel: options.MinimumLevel);
            }

            // 文件输出（按大小滚动）
            if (options.EnableFile)
            {
                loggerConfig.WriteTo.File(
                    path: options.LogFilePath,
                    outputTemplate: options.OutputTemplate,
                    restrictedToMinimumLevel: options.MinimumLevel,
                    fileSizeLimitBytes: options.FileSizeLimitBytes,
                    retainedFileCountLimit: options.RetainedFileCountLimit,
                    rollingInterval: RollingInterval.Day, // 按天滚动（可选：Minute/Hour/Day/Month）
                    rollOnFileSizeLimit: true, // 达到大小限制时新建文件
                    shared: true, // 多进程共享日志文件
                    flushToDiskInterval: TimeSpan.FromSeconds(1)); // 1秒刷新到磁盘
            }

            return loggerConfig.CreateLogger();
        }

        #region 静态日志方法（全局使用）
        public static void Debug(string message, params object[] args) => _logger.Debug(message, args);
        public static void Info(string message, params object[] args) => _logger.Information(message, args);
        public static void Warn(string message, params object[] args) => _logger.Warning(message, args);
        public static void Error(string message, params object[] args) => _logger.Error(message, args);
        public static void Error(Exception ex, string message, params object[] args) => _logger.Error(ex, message, args);
        public static void Fatal(string message, params object[] args) => _logger.Fatal(message, args);
        public static void Fatal(Exception ex, string message, params object[] args) => _logger.Fatal(ex, message, args);
        #endregion

        #region 实例日志方法（指定上下文）
        public void Debug(string sourceContext, string message, params object[] args) => _instanceLogger.ForContext("SourceContext", sourceContext).Debug(message, args);
        public void Info(string sourceContext, string message, params object[] args) => _instanceLogger.ForContext("SourceContext", sourceContext).Information(message, args);
        public void Warn(string sourceContext, string message, params object[] args) => _instanceLogger.ForContext("SourceContext", sourceContext).Warning(message, args);
        public void Error(string sourceContext, Exception ex, string message, params object[] args) => _instanceLogger.ForContext("SourceContext", sourceContext).Error(ex, message, args);
        public void Fatal(string sourceContext, Exception ex, string message, params object[] args) => _instanceLogger.ForContext("SourceContext", sourceContext).Fatal(ex, message, args);
        #endregion

        /// <summary>
        /// 关闭日志（释放资源）
        /// </summary>
        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}