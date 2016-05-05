using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;
using System.Text.RegularExpressions;
namespace ItemTransferBranchDemo
{
   public  class Utilities
    {
        public static void LogErrors(string message)
        {
            WriteToEventLog(message, "Application", EventLogEntryType.Error, "ITCO");
        }
        public static void WriteToEventLog(string entry, string appName, EventLogEntryType eventType, string logName)
        {
            var objEventLog = new EventLog();
            try
            {
                objEventLog.Source = appName;
                objEventLog.WriteEntry(entry, eventType);


            }
            catch (Exception ex)
            {

            }
        }
        public static void LogException(string ex)
        {
            Application.SBO_Application.SetStatusBarMessage(ex, SAPbouiCOM.BoMessageTime.bmt_Short, true);
        }

        public static string GetErrorMessage()
        {
            string ErrMsg = string.Empty;
            int errCode = int.MinValue;
            B1Helper.DiCompany.GetLastError(out errCode, out ErrMsg);
            return ErrMsg;
        }

        public static bool IsNumber(string key)
        {
            return Regex.IsMatch(key, @"^[0-9]*\.?[0-9]+$");
        }
        public static void StatusbarMessage(string message, SAPbouiCOM.BoStatusBarMessageType type)
        {
            Application.SBO_Application.StatusBar.SetText(message, SAPbouiCOM.BoMessageTime.bmt_Medium, type);
        }
    }
    }

