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

        public ThirdPartyGames()
        {
            games = new List<ThirdPartyGame>();
        }

        public void Add(ThirdPartyGame game)
        {
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
            FileStream stream = new FileStream(Settings.Default.ThirdPartyListing, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            serializer.Serialize(stream, this);
        }

        public static ThirdPartyGames Load()
        {
            XmlSerializer deserialize = new XmlSerializer(typeof(ThirdPartyGames));
            FileStream stream = new FileStream(Settings.Default.ThirdPartyListing, FileMode.Open, FileAccess.Read);

            ThirdPartyGames thirdGames = (ThirdPartyGames)deserialize.Deserialize(stream);

            return thirdGames;
        }
    }
}