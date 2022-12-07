using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static CsStatus.Config;
#if X11
using static X11.Xlib;
#endif

namespace CsStatus {
    public class Program {
        static Stopwatch watch = new();

        public static void Main(string[] args) {
            string status = "";
            bool stdout = false;
            bool once = false;
            foreach (string arg in args) {
                switch (arg) {
                    case "-s":
                        stdout = true;
                        break;
                    case "-1":
                        once = true;
                        break;
                }
            }

#if X11
            // Initialize X
            nint dpy;
            dpy = XOpenDisplay(null);
            if (!stdout && dpy == 0)
                throw new Exception("XOpenDisplay: Failed to open display");
#endif
            do {
                watch.Start();

                string[] splitStatus = new string[components.Length];
                Parallel.For(0, splitStatus.Length, (int i) => {
                    Component component = components[i];
                    splitStatus[i] = String.Format(component.fmt, component.cmd(component.args));
                });
                status = string.Join("", splitStatus);

                if (stdout) {
                    Console.WriteLine(status.Replace("\n", string.Empty));
                }
#if X11
                else {
                    if (XStoreName(dpy, XDefaultRootWindow(dpy), status) < 0) {
                        throw new Exception("XStoreName: Allocation failed");
                    }
                    XFlush(dpy);
                }
#endif

                status = "";
                watch.Stop();
                if (!Debugger.IsAttached)
                    Thread.Sleep((int)(interval - (int)watch.Elapsed.TotalMilliseconds));
                watch.Reset();
            } while (!once);
        }
    }
}
