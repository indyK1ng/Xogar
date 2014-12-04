using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<String> gameInstallDirs;
        private IDictionary<String, Game> _gamesListing;

        public SimpleValveGameParser(string steamInstallDir)
        {
            if (steamInstallDir == null)
            {
                throw new Exception("Steam not found.  This application currently requires Steam in order to function.");
            }

            installDir = steamInstallDir;
            
            gameInstallDirs = new List<String>();

            _gamesListing = new Dictionary<string, Game>();

            configLocation = steamInstallDir + "\\config\\config.vdf";
        }

        public IDictionary<String, Game> GetGameListing()
        {
            if (_gamesListing.Count == 0)
            {
                _gamesListing = FindAllSteamGames();
            }

            return _gamesListing;
        }

        private void FindAllGameInstallDirs()
        {
            string configContents = ReadConfigFile();
            gameInstallDirs.Add(installDir);

            string gamesInstallLocations = @"\u0022BaseInstallFolder_\d+\u0022\s*\u0022.*\u0022";
            Regex installDirsRegex = new Regex(gamesInstallLocations);
            MatchCollection installListing = installDirsRegex.Matches(configContents);

            foreach (Match gameInstallDir in installListing)
            {
                string dir = gameInstallDir.ToString();
                dir = dir.Replace("\"", "");
                var splits = dir.Split('\t');

                dir = splits[splits.Length - 1];

                gameInstallDirs.Add(dir);
            }
        }

        private IDictionary<String, Game> FindAllSteamGames()
        {
            FindAllGameInstallDirs();
            var configFileDict = FindAllSteamGamesFromConfigFile();
            var manifestFileDict = FindAllSteamGamesFromManifestFiles();

            var mergedDict = configFileDict.Concat(manifestFileDict)
                .GroupBy(d => d.Key)
                .ToDictionary(d => d.Key, d => d.First().Value);
            return mergedDict;
        }

        private IDictionary<string, Game> FindAllSteamGamesFromManifestFiles()
        {
            var manifestGames = new Dictionary<string, Game>();

            foreach (string dir in gameInstallDirs)
            {
                string steamAppsDir = ManifestPathStringBuilder(dir).ToString();

                if (Directory.Exists(steamAppsDir))
                {
                    List<string> manifestFiles = new List<string>(Directory.EnumerateFiles(steamAppsDir));

                    foreach (var manifestFile in manifestFiles)
                    {
                        if (manifestFile.Contains("appmanifest_"))
                        {
                            string[] splitManifest = manifestFile.Split(new string[] {"appmanifest_"},
                                StringSplitOptions.None);
                            Int64 gameId = Int64.Parse(splitManifest[1].Replace(".acf", ""));
                            String gameName = GetGameName(gameId);
                            Game manifestGame = new SteamGame(gameId);

                            if (manifestGames.ContainsKey(manifestGame.Hash()))
                            {
                                continue;
                            }

                            manifestGame.Name = gameName;
                            manifestGames.Add(manifestGame.Hash(), manifestGame);
                        }
                    }
                }
            }

            return manifestGames;
        }

        private IDictionary<string, Game> FindAllSteamGamesFromConfigFile()
        {
            var configContents = ReadConfigFile();

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

        private string ReadConfigFile()
        {
            StreamReader configReader = new StreamReader(configLocation);
            string configContents = configReader.ReadToEnd();
            configReader.Close();
            return configContents;
        }

        private MatchCollection GetAllAppListingsFromConfig(string configContents)
        {
            string appRegexString = @"\u0022(\d)+\u0022\s*{\s*([^}])*}";
            Regex gamesRegex = new Regex(appRegexString);
            MatchCollection gamesListing = gamesRegex.Matches(configContents);

            return gamesListing;
        }

        private String GetGameName(Int64 gameId)
        {
            var gameFileStringBuilder = GetFilePath(gameId);

            if (gameFileStringBuilder == "DNE")
            {
                return gameId.ToString();
            }

            try
            {
                var manifestReader = new StreamReader(gameFileStringBuilder);

                var gameName = ParseOutGameName(manifestReader);
                manifestReader.Close();
                return gameName;
            }
            catch
            {
                return gameId.ToString();
            }
        }

        private String GetFilePath(long gameId)
        {
            foreach (string gamesInstallDir in gameInstallDirs)
            {
                var gameFileStringBuilder = ManifestPathStringBuilder(gamesInstallDir);
                gameFileStringBuilder.AppendFormat(APP_FILENAME_TEMPLATE, gameId);

                if (File.Exists(gameFileStringBuilder.ToString()))
                {
                    return gameFileStringBuilder.ToString();
                }
            }

            return "DNE";
        }

        private static StringBuilder ManifestPathStringBuilder(string gamesInstallDir)
        {
            StringBuilder gameFileStringBuilder = new StringBuilder();
            gameFileStringBuilder.Append(gamesInstallDir);
            gameFileStringBuilder.Append("\\SteamApps\\");
            return gameFileStringBuilder;
        }

        private static string ParseOutGameName(StreamReader manifestReader)
        {
            string manifestFile = manifestReader.ReadToEnd();
            string appRegexString = @"\u0022(name)\u0022\s*\u0022.*";

            string name = Regex.Match(manifestFile, appRegexString).ToString();

            name = name.Replace("\"name\"", "");
            name = name.Replace("\"", "");
            name = name.Trim();

            return name;
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

            var isInstalled = IsInstalled(hasLocalContentLine);

            if (isInstalled == 0)
            {
                return new NullGame();
            }

            var game = new SteamGame(gameId);
            game.Name = GetGameName(gameId);

            if (game.Name == gameId.ToString())
            {
                game.Name = GetGameNameFromConfigSection(matchContent);
            }

            return game;
        }

        private String GetGameNameFromConfigSection(string matchContent)
        {
            var contentLines = matchContent.Split(Environment.NewLine.ToCharArray());
            string filePath = "";

            foreach (string line in contentLines)
            {
                if (line.Contains("installdir"))
                {
                    filePath = line.Split('\t').Last();
                    filePath = filePath.Replace("\"", "");
                }
            }

            string makeshiftName = Path.GetFullPath(filePath).TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar).Last();
            return makeshiftName;
        }

        private static short IsInstalled(string hasLocalContentLine)
        {
            string textRemoved = hasLocalContentLine.Replace("HasAllLocalContent", "");
            string quotesRemoved = textRemoved.Replace("\"", "");
            string sterilizedValue = quotesRemoved.Replace("\t\t", "");
            Int16 isInstalled = Int16.Parse(sterilizedValue);
            return isInstalled;
        }
    }
}
