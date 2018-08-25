using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XogarLib
{
    public class Games
    {
        private readonly IList<IGameListingParser> gameListingParsers;
        private readonly Random random = new Random();

        public ThirdPartyGames ThirdParty { get; }
        public IDictionary<String, Game> GamesToPick { get; private set; }
        public Playlist SelectedPlaylist { get; set; }
        public Playlist AllInstalledGamesPlaylist { get; private set; }

        public Games() : this(Properties.Settings.Default.SteamInstallDirectory) {}

        public Games(String steamInstallDir)
        {
            gameListingParsers = new List<IGameListingParser>();
            GamesToPick = new Dictionary<String, Game>();

            IGameListingParser parser = new SimpleValveGameParser(steamInstallDir);
            gameListingParsers.Add(parser);

            var thirdPartyParser = new ThirdPartyGameParser();
            ThirdParty = thirdPartyParser.thirdPartyGames;

            gameListingParsers.Add(thirdPartyParser);

            UpdateGameList();
        }

        private void MergeGamesLists()
        {
            GamesToPick = new Dictionary<string, Game>();

            foreach (IGameListingParser parse in gameListingParsers)
            {
                GamesToPick = GamesToPick.Union(parse.GetGameListing()).ToDictionary(k => k.Key, v => v.Value);
            }

            GamesToPick = GamesToPick.OrderBy(k => k.Value.Name).ToDictionary(k => k.Key, v => v.Value);
        }

        private void UpdateAllInstalledGamesPlaylist()
        {
            AllInstalledGamesPlaylist = new Playlist("All Steam Games");
            foreach (var game in GamesToPick.Keys)
            {
                AllInstalledGamesPlaylist.AddGame(game);
            }
        }

        public Game PickRandomGameFromPlaylist(int tries)
        {
            List<string> hashes = SelectedPlaylist.GameHashes;

            try
            {
                return GamesToPick[hashes.ElementAt(random.Next(hashes.Count))];
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

        public bool IsInstalled(string gameHash) => GamesToPick.ContainsKey(gameHash);

        public void UpdateGameList()
        {
            MergeGamesLists();
            UpdateAllInstalledGamesPlaylist();
        }
    }
}