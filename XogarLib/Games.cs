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
        private ThirdPartyGames thirdParty;

        public Games()
        {
            gameListingParsers = new List<IGameListingParser>();
            games = new Dictionary<String, Game>();

            IGameListingParser parser = new SimpleValveGameParser();
            gameListingParsers.Add(parser);

            ThirdPartyGameParser thirdPartyParser = new ThirdPartyGameParser();
            thirdParty = thirdPartyParser.thirdPartyGames;
            gameListingParsers.Add(thirdPartyParser);

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

            gameList.AppendFormat("Next Third Party Id: {0}\n", thirdParty.NextId);

            return gameList.ToString();
        }
    }
}
