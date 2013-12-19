using System;
using System.Collections.Generic;
using System.Linq;

namespace XogarLib
{
    public class GamePicker
    {
        private IDictionary<String, Game> games;
        public GamePicker()
        {
            SimpleValveGameParser parser = new SimpleValveGameParser();

            games = parser.FindAllSteamGames();
        }

        public Game PickRandomGame()
        {
            return games.ElementAt((new Random()).Next(games.Count())).Value;
        }
    }
}
