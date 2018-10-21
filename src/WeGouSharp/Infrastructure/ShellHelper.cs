
using System.Diagnostics;
using System.Threading.Tasks;

namespace WeGouSharp.Infrastructure
{
 public static class ShellHelper
    {
        //默认*nix shell,可以兼容powershell
        public static string RunAsShell(this string cmd,bool isPowerShell =false)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = isPowerShell? "Powershell":"/bin/bash",
                    Arguments = isPowerShell? $"-Command \"{escapedArgs}\"" : $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }

        
        
        public static async Task ExecuteShellAsync(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
          await Task.Run( () =>
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                process.Start();
            });

        }
    }
}