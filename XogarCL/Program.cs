using System;
using System.Threading;
using XogarLib;

namespace XogarCL
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            int exitCode = 0;
            Arguments arguments = null;
            try
            {
                arguments = new Arguments(args);
                arguments.Parse();
                Game launcher = new NullGame();

                if (!arguments.ShowUsage)
                {
                    if (!string.IsNullOrEmpty(arguments.SteamPath))
                    {
                        Console.WriteLine("Setting steam install path.");
                        Configuration.SetSteamPath(arguments.SteamPath);
                    }
                    else if (arguments.UseRandom)
                    {
                        var picker = new Games();
                        launcher = picker.PickRandomGame();
                    }
                    else
                    {
                        launcher = new SteamGame(arguments.GameId);
                    }

                    if (launcher.IsReal())
                    {
                        launcher.Launch();
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR: " + e.Message);
                exitCode = 1;
            }
            finally
            {
                if (arguments != null && arguments.ShowUsage)
                {
                    Console.WriteLine(arguments.GetUsageStatement());
                }
            }

            if (exitCode != 0)
            {
                Environment.Exit(exitCode);
            }
        }
    }
}
