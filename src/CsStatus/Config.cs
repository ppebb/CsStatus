using System;
using static CsStatus.Components;

namespace CsStatus {
    public struct Component {
        public Func<string, string> cmd;
        public string fmt;
        public string args;

        public Component(Func<string, string> cmd, string fmt, string args) {
            this.cmd = cmd;
            this.fmt = fmt;
            this.args = args;
        }
    }

    /*
        function           description                  argument (example)
        RunCommand         Custom shell command         command (echo), args (foo)
        CpuPerc            CPU usage in percent         null!
        RamPerc            Memory usage in percent      null!
        DiscPerc           Disk usage in percent        mountpoint path (/)
        Uptime             System uptime                null!
        DateTime           Date and time                format specifier (en-US ddd MMM dd hh:mm:ss tt). The culture must be specified first, then separated by a space, and then the normal format string can be used. If not speicifying a culture, put anything, as long as there is a space. Check https://learn.microsoft.com/en-us/dotnet/api/system.datetime.tostring?view=net-7.0#system-datetime-tostring.
    */

    public static class Config {
        public static Component[] components = {
            //            function          format string         args
            new Component(RunCommand,       "[VOL: {0}%]",       "pamixer --get-volume"),
            new Component(CpuPerc,          " [CPU: {0}%]",       null!),
            new Component(RamPerc,          " [RAM: {0}%]",       null!),
            new Component(DiskPerc,         " [Disk: {0}%]",      "/"),
            new Component(RunCommand,       " [PKG: {0}]",        "pacman -Q | wc -l"),
            new Component(RunCommand,       " [TEMP: {0}]",       "sensors | awk '/^Tctl/ {print $2}'"),
            new Component(Uptime,           " [UP: {0}]",         null!),
            new Component(DateTime,         " [{0}]",             "en-US ddd MMM dd hh:mm:ss tt"),
        };

        // Interval between updates in ms
        public static uint interval = 500;

        // Text to show if no value can be retrieved
        public static string unknownStr = "n/a";
    }
}
