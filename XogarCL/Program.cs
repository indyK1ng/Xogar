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
            Int64 gameId = -1;
            Game launcher = new NullGame();
            GamePicker picker = new GamePicker();

            string firstArg = args[0].ToLowerInvariant();

            if (firstArg == "Random".ToLowerInvariant())
            {
                launcher = picker.PickRandomGame();
            }
            else
            {
                try
                {
                    gameId = Int64.Parse(firstArg);
                    launcher = new SteamGame(gameId);
                }
                catch 
                { 
                    Console.WriteLine("ERROR: The first argument was not a gameID or the random command.");
                    Environment.Exit(1);
                }
            }


            if (launcher.IsReal())
            {
                launcher.Launch();
            }
        }
    }
}
