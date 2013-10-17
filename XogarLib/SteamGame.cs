using System;
using System.Windows.Navigation;
using System.Windows.Controls;

namespace XogarLib
{
    public class SteamGame:Game
    {
        private const string SCHEME = "steam";
        private const string LAUNCH_COMMAND = "rungameid";
        private WebBrowser launch;

        public SteamGame(Int64 gameId):base(gameId)
        {
            launch = new WebBrowser();
        }

        public override void Launch()
        {
            Uri startGameUri = new Uri(String.Format("{0}://{1}/{2}", SCHEME, LAUNCH_COMMAND, base.gameId));
            Console.WriteLine(startGameUri.AbsoluteUri);
            launch.Navigate(startGameUri);
        }
    }
}
