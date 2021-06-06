using System;

namespace RevitTools
{
    public class Log
    {
        public static Action<string, string> ShowTaskDialog;

        public static event Action<string> NewLog;
        public static event Action<string,int,int> NewProgress;
        public static event Action<string, int, int> NewProgress2;

        public static string Logs = "";
        public static void Info(string tag,string message)
        {
            string txt = string.Format("[{0}][{1}][{2}]{3}","Info", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tag, message);
            Logs = txt + "\n" + Logs;
            if (NewLog != null)
            {
                NewLog(txt);
            }
        }

        public static void Exception(string tag,string message,bool showDialog=true)
        {
            string txt = string.Format("[{0}][{1}][{2}]{3}", "Exception", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tag, message);
            Logs = txt + "\n" + Logs;
            if (NewLog != null)
            {
                NewLog(txt);
            }
            if (showDialog)
            {
                if (ShowTaskDialog != null)
                {
                    ShowTaskDialog(tag, message);
                }
                //TaskDialog.Show(tag, message);
            }
        }

        public static void Progress(string tag, string message,int index,int count)
        {
            string txt = string.Format("[{0}][{1}][{2}/{3}]{4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tag, index,count, message);
            Logs = txt + "\n" + Logs;
            if (NewProgress != null)
            {
                NewProgress(txt, index, count);
            }
        }

        public static void Progress2(string tag, string message, int index, int count)
        {
            string txt = string.Format("[{0}][{1}][{2}/{3}]{4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tag, index, count, message);
            Logs = txt + "\n" + Logs;
            if (NewProgress2 != null)
            {
                NewProgress2(txt, index, count);
            }
        }
    }
}
