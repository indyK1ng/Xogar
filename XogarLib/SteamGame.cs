using System;
using System.Windows.Navigation;
using System.Windows.Controls;

namespace XogarLib
{
    public class SteamGame : Game
    {
        private const string SCHEME = "steam";
        private const string RUN_GAME_COMMAND = "rungameid";
        private const string OPEN_STORE_COMMAND = "store";

        public SteamGame(Int64 gameId)
            : base(gameId)
        {
        }

        public override void Launch()
        {
            LaunchWithBrowser(RUN_GAME_COMMAND, base.gameId.ToString());
        }

        public override void Store()
        {
            LaunchWithBrowser(OPEN_STORE_COMMAND, base.gameId.ToString());
        }

        /// <summary>
        /// Execute a command using Steam's browser protocol
        /// </summary>
        /// <param name="command">Command to execute; examples available 
        /// <a href="https://developer.valvesoftware.com/wiki/Steam_browser_protocol">here</a></param>
        /// <param name="argument">Optional argument which usually represents an application's 
        /// Steam application ID or the name of a component</param>
        /// <param name="scheme">The protocol used; should always be "steam"</param>
        internal void LaunchWithBrowser(string command, string argument = "", string scheme = SCHEME)
        {
            Uri uri = new Uri(String.Format("{0}://{1}/{2}", scheme, command, argument));
            Console.WriteLine(uri.AbsoluteUri);
            new WebBrowser().Navigate(uri);
        }
    }
}