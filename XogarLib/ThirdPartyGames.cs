using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using XogarLib.Properties;

namespace XogarLib
{
    public class ThirdPartyGames
    {
        [XmlElement("Games")]
        public List<ThirdPartyGame> games;

        private Int64 nextId;
        private static string envFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Xogar\\" + Settings.Default.ThirdPartyListing;

        public ThirdPartyGames()
        {
            games = new List<ThirdPartyGame>();
        }

        [XmlElement("NextId")]
        public long NextId
        {
            set { nextId = value; }
            get { return nextId; }
        }

        public void Add(ThirdPartyGame game)
        {
            game.GameId = nextId;
            nextId++;
            games.Add(game);
        }

        public IDictionary<String, Game> GetGames()
        {
            Dictionary<String, Game> genericGames = new Dictionary<string, Game>();

            foreach (ThirdPartyGame game in games)
            {
                genericGames.Add(game.Hash(), game);
            }

            return genericGames;
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ThirdPartyGames));
            FileStream stream = new FileStream(envFolderPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            serializer.Serialize(stream, this);
        }

        public static ThirdPartyGames Load()
        {
            XmlSerializer deserialize = new XmlSerializer(typeof(ThirdPartyGames));

            if (File.Exists(envFolderPath))
            {
                FileStream stream = new FileStream(envFolderPath, FileMode.Open, FileAccess.Read);
                ThirdPartyGames thirdGames = (ThirdPartyGames) deserialize.Deserialize(stream);
                stream.Close();

                return thirdGames;
            }
            
            return new ThirdPartyGames();
        }
    }
}