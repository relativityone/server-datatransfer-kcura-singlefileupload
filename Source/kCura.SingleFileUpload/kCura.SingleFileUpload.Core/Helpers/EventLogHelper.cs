using System;
using System.Diagnostics;
using System.Text;

namespace kCura.SingleFileUpload.Core.Helpers
{
    /// <summary>
    /// Logging helper
    /// </summary>
    public static class EventLogHelper
    {
        /// <summary>
        /// Get Resursive error message from exception and inner exceptions
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns>Error message of the exception and his Inner exceptions</returns>
        public static void WriteToAnEventLog(string message)
        {
            string sSource;
            string sLog;

            sSource = "Single FIle Upload";
            sLog = "Application";

            if (!EventLog.SourceExists(sSource))
            {
                EventLog.CreateEventSource(sSource, sLog);
            }
            EventLog.WriteEntry(sSource, message, EventLogEntryType.Error);
        }

        /// <summary>
        /// Get Resursive error message from exception and inner exceptions
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns>Error message of the exception and his Inner exceptions</returns>
        public static string GetRecursiveExceptionMsg(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                sb.AppendLine(GetRecursiveExceptionMsg(ex.InnerException));
            }
            return sb.ToString();
        }
    }
}
