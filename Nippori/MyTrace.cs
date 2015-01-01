using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Nippori
{
    /// <summary>
    /// Provides methods for tracing the execution of the code.
    /// </summary>
    public static class MyTrace
    {
        #region .: Variables :.
        
        private static TextWriter textWriter;
        
        #endregion

        #region .: Public Methods :.
        
        /// <summary>
        /// Initializes the tracer.
        /// </summary>
        public static void Init()
        {
            textWriter = new StreamWriter("trace.txt", true);
            
            textWriter.WriteLine();
            textWriter.WriteLine("================================================================================");
        }

        /// <summary>
        /// Closes the tracer.
        /// </summary>
        public static void Close()
        {
            textWriter.Flush();
            textWriter.Close();
        }

        /// <summary>
        /// Writes a message to the trace.
        /// </summary>
        /// <param name="source">Source of the message.</param>
        /// <param name="message">A composite format string.</param>
        /// <param name="args">An array of objects to write using <see cref="message"/>.</param>
        public static void WriteLine(string source, string message, params object[] args)
        {
            string formattedMessage, timestamp;

            formattedMessage = String.Format(message, args);
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            textWriter.WriteLine("{0} [{1}] {2}", timestamp, source, formattedMessage);
        }

        #endregion
    }
}
