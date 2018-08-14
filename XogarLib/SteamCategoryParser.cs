using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XogarLib
{
    public class SteamCategoryParser
    {
        private readonly Games games;

        public IEnumerable<Playlist> Categories { get; private set; }

        public SteamCategoryParser(Games games) : this(games, Properties.Settings.Default.SteamInstallDirectory) { }

        public SteamCategoryParser(Games games, string steamPath)
        {
            this.games = games;
            Load(steamPath);
        }

        private void Load(string steamPath)
        {
            var allConfigContents = ReadAllConfigContents(steamPath);
            Categories = allConfigContents.SelectMany(GetAllGameCategoriesFromConfig).ToList();
        }

        private IEnumerable<string> ReadAllConfigContents(string steamInstallDir)
        {
            return Directory.EnumerateDirectories(Path.Combine(steamInstallDir, "userdata"))
                .AsParallel()
                .Select(userdir => Path.Combine(userdir, @"7\remote\sharedconfig.vdf"))
                .Select(File.ReadAllText);
        }

        public IEnumerable<Playlist> GetAllGameCategoriesFromConfig(string configContents)
        {
            /*
             * Example:
             * 	"326460"
			 * 	{
			 * 		"tags"
			 * 		{
			 * 			"0"		"Category 1"
			 * 			"1"		"Category 2"
			 * 		}
			 * 	}
             */
            var regex = new Regex(@"""(?<gameId>\d+)""\s*{\s*""tags""\s*{\s*(?<tagsString>.+?)\s*}");
            var allMatches = regex.Matches(configContents);
            var gamesToCategories = allMatches.Cast<Match>().ToDictionary(match => Int64.Parse(match.Groups["gameId"].Value),
                match => GetCategoriesFromTagsString(match.Groups["tagsString"].Value));
            return gamesToCategories.Values
                .SelectMany(o => o)
                .Distinct()
                .Select(category => new Playlist
                {
                    Name = category,
                    GameHashes = gamesToCategories.Keys
                        .Where(gameId => gamesToCategories[gameId].Contains(category))
                        .Select(gameId => new SteamGame(gameId).Hash())
                        .Where(games.IsInstalled)
                        .ToList()
                })
                .Where(c => c.GameHashes.Any())
                .ToList();
        }

        private IEnumerable<string> GetCategoriesFromTagsString(string tagsString)
        {
            /*
             * tagsString will be something like
             *   "0"		"Category 1"
			 * 	 "1"		"Category 2"
             * we want it to output ["Category 1", "Category 2"]
             */
            var regex = new Regex(@"""\d+""\s*""(?<categoryName>.+?)""");
            return tagsString.Split('\n')
                .Where(s => !String.IsNullOrWhiteSpace(s) && regex.IsMatch(s))
                .Select(s => regex.Match(s).Groups["categoryName"].Value);
        }
    }
}
