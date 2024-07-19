using log4net.Config;
using log4net;
using System.Reflection;

namespace ApiTestingSolution.Logging
{
    public static class Logger
    {
        private static ILog Log { get; } = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static Logger()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            var directory = currentDirectory.Parent.Parent.Parent.FullName;
            var path = Path.Combine(directory, "Logging", "log4net.config");
            XmlConfigurator.Configure(logRepository, new FileInfo(path));
        }

        public static void Debug(string message) => Log.Debug(message);

        public static void Info(string message) => Log.Info(message);

        public static void Warn(string message) => Log.Warn(message);

        public static void Error(string message, Exception ex) => Log.Error(message, ex);

        public static void Fatal(string message, Exception ex) => Log.Fatal(message, ex);
    }
}
