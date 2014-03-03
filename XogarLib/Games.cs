using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XogarLib
{
    public class Games
    {
        private IDictionary<String, Game> games;
        private readonly ThirdPartyGames thirdParty;
        private IList<IGameListingParser> gameListingParsers;

        public Games()
        {
            gameListingParsers = new List<IGameListingParser>();
            games = new Dictionary<String, Game>();

            IGameListingParser parser = new SimpleValveGameParser();
            gameListingParsers.Add(parser);

            var thirdPartyParser = new ThirdPartyGameParser();
            thirdParty = thirdPartyParser.thirdPartyGames;
            gameListingParsers.Add(thirdPartyParser);

            MergeGamesLists();
        }

        private void MergeGamesLists()
        {
            games = new Dictionary<string, Game>();

            foreach (IGameListingParser parse in gameListingParsers)
            {
                games = GamesToPick.Union(parse.GetGameListing()).ToDictionary(k => k.Key, v => v.Value);
            }

            games = games.OrderBy(k => k.Value.Name).ToDictionary(k => k.Key, v => v.Value);
        }

        public Playlist SelectedPlaylist { get; set; }

        public ThirdPartyGames ThirdParty
        {
            get { return thirdParty; }
        }

        public IDictionary<String, Game> GamesToPick
        {
            get { return games; }
        }

        public Game PickRandomGame()
        {
            return GamesToPick.ElementAt((new Random()).Next(GamesToPick.Count())).Value;
        }

        public Game PickRandomGameFromPlaylist(int tries)
        {
            if (SelectedPlaylist == null)
            {
                return PickRandomGame();
            }
            List<string> hashes = SelectedPlaylist.GameHashes;

            try
            {
                return GamesToPick[hashes.ElementAt((new Random()).Next(hashes.Count))];
            }
            catch (KeyNotFoundException)
            {
                if (tries < hashes.Count)
                {
                    return PickRandomGameFromPlaylist(tries + 1);
                }
                else
                {
                    throw new Exception("No games in the playlist seem to exist.");
                }
            }
        }

        public override string ToString()
        {
            var gameList = new StringBuilder();

            foreach (Game game in GamesToPick.Values)
            {
                gameList.AppendLine(game.ToString());
            }

            gameList.AppendFormat("Next Third Party Id: {0}\n", ThirdParty.NextId);

            return gameList.ToString();
        }

        public void UpdateGameList()
        {
            MergeGamesLists();
        }
    }
}