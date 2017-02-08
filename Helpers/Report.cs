using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class Report
    {
        static string logName = DateTime.Now.Year +
            "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + 
            "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute  + ".txt";

        public static void log(DeviceToReport device, LogLevel level, string message)
        {
            try
            {
                string extendedMessage = DateTime.Now.ToString() + "\t" + "\t" +
                                             device.ToString() + "\t" + "\t" + "\t" +
                                             level.ToString() + "\t" + "\t" +
                                             message + Environment.NewLine;
                string file = device.ToString() + logName;
                File.AppendAllText(file, extendedMessage);
            }
            catch (Exception)
            {                
            }
        }
    }

    public enum DeviceToReport
    { 
        Client_MainWindow,
        Client_WelcomeWindow,
        Client_ContactWindow,
        Client_Proxy,
        Client_ChatResult, 
        DAL,
        Host,
        Service
        
    }

    public enum LogLevel
    {
        Information,
        Warning,
        Exception
    }
}
