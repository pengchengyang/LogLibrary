using System;

namespace Rolling.Logging
{
    /// <summary>
    /// Rolling日志配置项
    /// </summary>
    public class RollingLoggerOptions
    {
        /// <summary>
        /// 日志输出路径（默认：./Logs/Rolling-.log）
        /// </summary>
        public string LogFilePath { get; set; } = "./Logs/Rolling-.log";

        /// <summary>
        /// 日志级别（默认：Information）
        /// </summary>
        public Serilog.Events.LogEventLevel MinimumLevel { get; set; } = Serilog.Events.LogEventLevel.Information;

        /// <summary>
        /// 单个日志文件大小限制（默认：100MB）
        /// </summary>
        public long FileSizeLimitBytes { get; set; } = 1024 * 1024 * 100;

        /// <summary>
        /// 保留日志文件数量（默认：30个）
        /// </summary>
        public int RetainedFileCountLimit { get; set; } = 30;

        /// <summary>
        /// 是否输出到控制台（默认：true）
        /// </summary>
        public bool EnableConsole { get; set; } = true;

        /// <summary>
        /// 是否输出到文件（默认：true）
        /// </summary>
        public bool EnableFile { get; set; } = true;

        /// <summary>
        /// 日志格式模板（默认包含Rolling标识、时间、级别、消息等）
        /// </summary>
        public string OutputTemplate { get; set; } = "[Rolling-{Timestamp:yyyy-MM-dd HH:mm:ss.fff}][{Level:u3}][{SourceContext}] {Message:lj}{NewLine}{Exception}";
    }
}