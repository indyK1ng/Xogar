using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XogarLib;

namespace XogarThirdPartyTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            ThirdPartyGame testGame = new ThirdPartyGame(1, "Need for Speed: Underground", "C:\\Program Files (x86)\\EA GAMES\\Need For Speed Underground\\Speed.exe");
            ThirdPartyGame testGame2 = new ThirdPartyGame(2, "Dungeon Keeper Gold", "C:\\GOG Games\\Dungeon Keeper Gold\\KEEPER.EXE");
            ThirdPartyGames games = new ThirdPartyGames();
            games.Add(testGame);
            games.Add(testGame2);
            games.Save();
        }
    }
}
