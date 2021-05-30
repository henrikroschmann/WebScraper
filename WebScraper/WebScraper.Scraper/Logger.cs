using System;

namespace WebScraper.Scraper
{
    public static class Logger
    {
        public static Action<string> WriteEvent { get; set; }

        /// <summary>
        ///     Write formatted debug output.
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            WriteEvent(message);
        }

        /// <summary>
        ///     Write formatted debug output, and start new line.
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message)
        {
            Write($"{DateTime.Now} {message}");
            Write(Environment.NewLine);
        }
    }
}