using static CsStatus.Config;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CsStatus {
    public static class Components {
        public static string RunCommand(string args) {
            try {
                ProcessStartInfo processStartInfo = new("sh", $"-c \"{args}\"");

                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;

                Process proc = new();
                proc.StartInfo = processStartInfo;
                proc.Start();

                string res = proc.StandardOutput.ReadToEnd();

                return res;
            }
            catch (Exception e) {
                throw e;
            }
        }

        static int[] o = new int[7]; // Old stats
        static readonly Regex DigitRegex = new Regex(@"(\d)+", RegexOptions.Compiled);
        public static string CpuPerc(string args) {
            int[] p = new int[7]; // Parsed stats
            if (Utils.Scan("/proc/stat", DigitRegex, ref p) != 7)
                return unknownStr;

            int sum = (o[0] + o[1] + o[2] + o[3] + o[4] + o[5] +o[6]) -
                (p[0] + p[1] + p[2] + p[3] + p[4] + p[5] + p[6]);

            if (sum == 0) {
                o = p;
                return unknownStr;
            }

            int avg = (int)(100 *
                    ((o[0] + o[1] + o[2] + o[5] + o[6]) -
                     (p[0] + p[1] + p[2] + p[5] + p[6])) / sum);

            o = p;
            return avg.ToString();
        }

        public static string RamPerc(string args) {
            int[] vars = new int[5];
            if (Utils.Scan("/proc/meminfo", DigitRegex, ref vars) != 5) {
                return unknownStr;
            }

            int total = vars[0], free = vars[1], buffers = vars[3], cached = vars[4];

            if (total == 0)
                return unknownStr;

            return (100 * ((total - free) - (buffers + cached)) / total).ToString();
        }

        public static string DiskPerc(string args) {
            string? root = Path.GetPathRoot(args);
            if (root == null)
                throw new Exception($"RamPerc: Path {root} does not exist");

            DriveInfo di = new(root);
            if (di.IsReady) {
                return ((int) (100 * (1.0f - ((double)di.TotalFreeSpace / (double)di.TotalSize)))).ToString();
            }

            return unknownStr;
        }

        public static string Uptime(string args) {
            TimeSpan span = TimeSpan.FromMilliseconds(Environment.TickCount64);
            return $"{span.Hours}h {span.Minutes}m";
        }

        public static string DateTime(string args) {
            string[] splitArgs = args.Split(" ");
            CultureInfo culture = new(splitArgs[0]);
            return System.DateTime.Now.ToString(String.Join(" ", splitArgs[1..]), culture);
        }
    }
}
