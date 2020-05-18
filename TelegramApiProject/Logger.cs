using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TelegramApiProject
{
    public class Logger
    {
        public static void Error(Exception ex)
        {
            Trace.Fail(DateTime.Now.ToString() + ex.Message, ex.StackTrace + 
                "\nInner exception: " + ex?.InnerException?.Message);
        }

        public static void Warning(Exception ex)
        {
            Trace.TraceError(DateTime.Now.ToString() + ex.Message, ex.StackTrace);
        }
    }
}
