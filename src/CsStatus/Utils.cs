using System.IO;
using System.Text.RegularExpressions;

namespace CsStatus {
    public class Ref<T> {
        public T Value { get; set; }

        public Ref(T value) {
            Value = value;
        }
    }

    public static class Utils {
        public static int Scan(string path, Regex regex, ref int[] vars) {
            using (StreamReader sr = new(path)) {
                string? lines = sr.ReadToEnd();
                if (lines == null)
                    return 0;

                Match? match = null;

                int count = 0;
                for (int i = 0; i < vars.Length; i++) {
                    if (match == null)
                        match = regex.Match(lines);
                    else
                        match = match.NextMatch();
                    bool success;
                    int res;

                    success = int.TryParse(match.Value, out res);

                    if (!success)
                        continue;

                    vars[i] = res;
                    count += 1;
                }

                return count;
            }
        }
    }
}
