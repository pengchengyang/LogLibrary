using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Rolling.Logging
{
    public class CallerEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            if (frames == null) return;

            // 跳过 Serilog 和 RollingLogger 内部方法
            var callerFrame = frames.FirstOrDefault(f =>
            {
                var method = f.GetMethod();
                var type = method?.DeclaringType;
                if (type == null) return false;
                if (type.Namespace?.StartsWith("Serilog") == true) return false;
                if (type == typeof(RollingLogger) || type == typeof(CallerEnricher)) return false;
                return true;
            });

            if (callerFrame == null) return;

            string filePath = callerFrame.GetFileName();
            int lineNumber = callerFrame.GetFileLineNumber();
            if (string.IsNullOrEmpty(filePath) || lineNumber == 0) return;

            // 可选：只保留文件名（不含路径）
            string fileName = Path.GetFileName(filePath);

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CallerFile", fileName));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CallerLineNumber", lineNumber));
        }
    }
}