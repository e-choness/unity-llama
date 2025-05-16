using AOT;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LlamaWrapper
{
    public class LlamaEventHandler
    {
        public static Mutex mutex = new Mutex();
        public static List<string> txtList = new List<string>();

        [MonoPInvokeCallback(typeof(LlamaEventHandler))]
        public static void OnNewText(string text)
        {
            mutex.WaitOne();
            txtList.Add(text);
            mutex.ReleaseMutex();
        }

        public static bool TextFinished()
        {
            bool ret = false;
            mutex.WaitOne();
            if (txtList.Count > 0)
            {
                ret = true;
            }
            mutex.ReleaseMutex();
            return ret;
        }

        public static string PopText()
        {
            string ret = null;
            mutex.WaitOne();
            if (txtList.Count > 0)
            {
                ret = txtList[0];
                txtList.RemoveAt(0);
            }
            mutex.ReleaseMutex();
            return ret;
        }
    }
}
