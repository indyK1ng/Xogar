using System;
using System.Threading;
using System.Linq.Expressions;
using XogarLib;

namespace XogarCL
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var game = parseGame(args);
                execute(game, args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        public static Game parseGame(string[] args)
        {
            Int64 gameId = -1;
            if (args.at(0).Equals("Random", StringComparison.OrdinalIgnoreCase))
            {
                return new GamePicker().PickRandomGame();
            }
            else if (Int64.TryParse(args.at(0), out gameId))
            {
                return new SteamGame(gameId);
            }
            else
            {
                throw new ArgumentException("ERROR: The first argument was not \"Random\" or a game ID.");
            }
        }

        public static void execute(Game game, string[] args)
        {
            if (args.contains("store"))
            {
                game.Store();
            }
            else
            {
                game.Launch();
            }
        }

        /// <summary>
        /// Extension method; retrieves the element at index or an empty string if it does not exist.
        /// </summary>
        /// <param name="index">The index to retrieve if possible</param>
        /// <returns>string</returns>
        internal static string at(this string[] args, int index)
        {
            return args.Length >= index ? args[index] : "";
        }

        /// <summary>
        /// Extension method; returns the results of a case-insensitive Equals on every element in this array.
        /// </summary>
        /// <param name="token">Token to search for</param>
        /// <returns>bool: presense of token</returns>
        internal static bool contains(this string[] args, string token)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (token.Equals(args[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
