using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MetaCase.GraphBrowser
{
    /// <summary>
    /// Subclass that enables to connect the MetaEdit+ API server with
    /// Url parameter and setting timeout.
    /// </summary>
    public class MEAPI : MetaEditAPI.MetaEditAPI
    {
        public MEAPI() 
            : base()
        { }

        public MEAPI(string Url)
            : base()
        {
            this.Url = Url;
        }

        public MEAPI(string Url, int _timeout) 
            : base()
        {
            this.Url = Url;
            this.Timeout = _timeout;
        }

        public void _SetTimeOut(int _timeout)
        {
            this.Timeout = _timeout;
        }

        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(int dwProcessId);

        public static void AllowSetForegroundWindow() 
        {
            AllowSetForegroundWindow(-1);
        }
    }
}
