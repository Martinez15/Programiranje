using System;
using System.IO;
using System.Text;

namespace TaxiPrevoz.Services
{
    public class LogService
    {
        private readonly string filePath;

        public LogService(string filePath = "txt/log.txt")
        {
            this.filePath = filePath;
        }

        public void Log(string message)
        {
            try
            {
                string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(filePath, logLine + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }
    }
}
