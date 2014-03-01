using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XogarLib
{
    public class Games
    {
        private IDictionary<String, Game> games;
        private IList<IGameListingParser> gameListingParsers;

        public Games()
        {
            gameListingParsers = new List<IGameListingParser>();
            games = new Dictionary<String, Game>();

            IGameListingParser parser = new SimpleValveGameParser();
            gameListingParsers.Add(parser);

            IGameListingParser thirdPartypParser = new ThirdPartyGameParser();
            gameListingParsers.Add(thirdPartypParser);

            foreach (var parse in gameListingParsers)
            {
                games = games.Union(parse.GetGameListing()).ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public Game PickRandomGame()
        {
            return games.ElementAt((new Random()).Next(games.Count())).Value;
        }

        public override string ToString()
        {
            StringBuilder gameList = new StringBuilder();

            foreach (Game game in games.Values)
            {
                gameList.AppendLine(game.ToString());
            }

            return gameList.ToString();
        }
    }
}
