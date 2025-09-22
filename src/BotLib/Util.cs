using PdkBot.BotLib.Collection;
using PdkBot.BotLib.Extensions;
using PdkBot.BotLib.Misc;
using PdkBot.BotLib.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PdkBot.BotLib
{
    public class Util
    {
        private static Encoding _encodingGb2312 = Encoding.GetEncoding("gb2312");

        public static Encoding EncodingGb2312
        {
            get
            {
                return _encodingGb2312;
            }
        }
        private static object _writeTraceSynObj = new object();
        private static int _lineCount = 1;
        private const bool IsWriteTraceToLog = false;
        private static DateTime _preWritaTraceTime = DateTime.MinValue;
        private static Cache<string, string> _normalizeStringCache = new Cache<string, string>(100, 0, null);

        public static bool Assert(bool condition, string msg = "", [System.Runtime.CompilerServices.CallerMemberName] string caller = "", [System.Runtime.CompilerServices.CallerFilePath] string path = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            if (!condition)
            {
                msg = string.Format("Assert false, msg={0},caller={1},path={2},line={3}", new object[]
				{
					msg,
					caller,
					PathEx.ConvertToRelativePath(path),
					line
				});
                Log.Assert(msg);
                throw new Exception(msg);
            }
            return condition;
        }

        public static void ThrowException(string msg = "", [System.Runtime.CompilerServices.CallerMemberName] string caller = "", [System.Runtime.CompilerServices.CallerFilePath] string path = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            msg = string.Format("ThrowException, msg={0},caller={1},path={2},line={3}",msg,caller,PathEx.ConvertToRelativePath(path),line);
            Log.Error(msg);
            throw new Exception(msg);
        }


        public static void Beep()
        {
            SystemSounds.Beep.Play();
        }

        public static void SleepWithDoEvent(int ms)
        {
            Assert(ms > 0);
            int millisecondsTimeout = Math.Min(30, ms);
            DateTime now = DateTime.Now;
            do
            {
                Thread.Sleep(millisecondsTimeout);
                DispatcherEx.DoEvents();
            }
            while ((DateTime.Now - now).TotalMilliseconds < ms);
        }

        public static void WriteTrace(object v)
        {
            WritaTrace(JsonSerializer.Serialize(v));
        }

        public static void WritaTrace(string v)
        {
                lock (_writeTraceSynObj)
                {
                    var ms = _preWritaTraceTime == DateTime.MinValue ? 0.0 : (DateTime.Now - _preWritaTraceTime).TotalMilliseconds;
                    _preWritaTraceTime = DateTime.Now;
                    var message = string.Format("{0}({1},{2}):{3}", _lineCount,DateTime.Now.ToString("MM:ss"),ms.ToString("0.000"),v);
                    Trace.WriteLine(message);
                    _lineCount++;
                }
        }

        public static void WriteTrace(string format, params object[] args)
        {
            WritaTrace(string.Format(format, args));
        }

        public static bool WaitFor(Func<bool> pred, int timeoutMs = 0, int testIntervalMs = 10, bool withDoEvent = false)
        {
            if (testIntervalMs < 1)
            {
                testIntervalMs = 10;
            }
            DateTime now = DateTime.Now;
            var isTimeout = false;
            while (true)
            {
                if (pred())
                {
                    break;
                }
                if (timeoutMs > 0 && now.xIsTimeElapseMoreThanMs(timeoutMs))
                {
                    isTimeout = true;
                    break;
                }
                if (withDoEvent)
                {
                    DispatcherEx.DoEvents();
                }
                Thread.Sleep(testIntervalMs);
            }
            return isTimeout;
        }

        public static T CallWithoutException<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
            return default;
        }

        public static void CallWithoutException(Action act)
        {
            try
            {
                act();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        public static void CallOnceAfterDelayInUiThread(Action act, int delayMs)
        {
            DelayCaller.CallAfterDelayInUIThread(act, delayMs);
        }

        public static bool IsProcessRuning(string name)
        {
            Process[] processesByName = Process.GetProcessesByName(name);
            return !processesByName.xIsNullOrEmpty();
        }

        public static void WriteTimeElapsed(DateTime t0, string msg = "")
        {
            WriteTrace("time elapsed={1} ms,{0}",msg,(DateTime.Now - t0).TotalMilliseconds);
        }
    }
}
