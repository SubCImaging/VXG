using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubC.Viperfish
{
    public class Shell
    {
        /// <summary>
        /// Runs a shell command on the Rayfin.
        /// </summary>
        /// <param name="command">The command you wish to execute.</param>
        /// <param name="timeout">The maximum time the command is allowed to run before timing out.</param>
        /// <returns>Anything that comes from stdout</returns>
        public static string ShellSync(string command, int timeout = 0)
        {
            try
            {
                // Run the command
                var log = new System.Text.StringBuilder();
                var process = Runtime.GetRuntime().Exec(new[] { "su", "-c", command });
                var bufferedReader = new BufferedReader(
                new InputStreamReader(process.InputStream));

                // Grab the results
                if (timeout > 0)
                {
                    process.Wait(timeout);
                    return string.Empty;
                }

                string line;

                while ((line = bufferedReader.ReadLine()) != null)
                {
                    log.AppendLine(line);
                }

                return log.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}