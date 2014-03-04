using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using XogarLib.Properties;

namespace XogarLib
{
    public class SimpleValveGameParser : IGameListingParser
    {
        private string installDir;
        private string configLocation;
        private readonly string APP_FILENAME_TEMPLATE = "appmanifest_{0}.acf";

        public SimpleValveGameParser(string steamInstallDir)
        {
            if (steamInstallDir == null)
            {
                throw new Exception("Steam not found.  This application currently requires Steam in order to function.");
            }

            installDir = steamInstallDir;

            configLocation = steamInstallDir + "\\config\\config.vdf";
        }

        public IDictionary<String, Game> GetGameListing()
        {
            return FindAllSteamGames();
        }

        private IDictionary<String, Game> FindAllSteamGames()
        {
            StreamReader configReader = new StreamReader(configLocation);
            string configContents = configReader.ReadToEnd();
            configReader.Close();

            Dictionary<String, Game> steamGames = new Dictionary<String, Game>();
            MatchCollection gameListing = GetAllAppListingsFromConfig(configContents);

            foreach (Match game in gameListing)
            {
                Game matchedGame = GetGameInformation(game);

                if (matchedGame.IsReal())
                {
                    steamGames.Add(matchedGame.Hash(), matchedGame);
                }
            }
            return steamGames;
        }

        private MatchCollection GetAllAppListingsFromConfig(string configContents)
        {
            // Less than ideal Regex.  Making one that automatically excludes "DecryptionKey" would be ideal
            string appRegexString = @"\u0022(\d)+\u0022\s*{\s*([^}])*}";
            Regex gamesRegex = new Regex(appRegexString);
            MatchCollection gamesListing = gamesRegex.Matches(configContents);

            return gamesListing;
        }

        private String GetGameName(Int64 gameId)
        {
            StringBuilder gameFileStringBuilder = new StringBuilder();
            gameFileStringBuilder.Append(installDir);
            gameFileStringBuilder.Append("\\SteamApps\\");
            gameFileStringBuilder.AppendFormat(APP_FILENAME_TEMPLATE, gameId);

            try
            {
                var manifestReader = new StreamReader(gameFileStringBuilder.ToString());

                string manifestFile = manifestReader.ReadToEnd();
                string appRegexString = @"\u0022(name)\u0022\s*\u0022.*";

                string name = Regex.Match(manifestFile, appRegexString).ToString();

                name = name.Replace("\"name\"", "");
                name = name.Replace("\"", "");
                name = name.Trim();

                return name;
            }
            catch
            {
                return gameId.ToString();
            }
        }

        private Game GetGameInformation(Match gameMatch)
        {
            string matchContent = gameMatch.ToString();

            if (matchContent.Contains("DecryptionKey"))
            {
                return new NullGame();
            }

            // Gets the game ID
            string gameIdRegex = @"\u0022(\d)+\u0022";
            Int64 gameId = Int64.Parse(Regex.Match(matchContent, gameIdRegex).ToString().Replace("\"", ""));

            // Makes sure the game is installed
            string installedAndUpToDate = @"\u0022HasAllLocalContent\u0022\s*\u0022\d\u0022";
            string hasLocalContentLine = Regex.Match(matchContent, installedAndUpToDate).ToString();
            
            // After retargeting to .NET 4.0, experienced a weird issue here where no line was being returned.
            if (hasLocalContentLine == "")
            {
                return new NullGame();
            }

            string textRemoved = hasLocalContentLine.Replace("HasAllLocalContent", "");
            string quotesRemoved = textRemoved.Replace("\"", "");
            string sterilizedValue = quotesRemoved.Replace("\t\t", "");
            Int16 isInstalled = Int16.Parse(sterilizedValue);

            if (isInstalled == 0)
            {
                return new NullGame();
            }
            var game = new SteamGame(gameId);
            game.Name = GetGameName(gameId);

            return game;
        }
    }
}
