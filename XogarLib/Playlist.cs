using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace XogarLib
{
    public class Playlist
    {
        [XmlElement("Name")]
        public String Name { get; set; } 

        [XmlElement("Games")]
        public List<String> GameHashes { get; set; }

        public Playlist()
        {
            GameHashes = new List<string>();
        }

        public Playlist(string name):this()
        {
            Name = name;

        }

        public void AddGame(string hash)
        {
            GameHashes.Add(hash);
        }

        public void Random()
        {

            GameHashes.ElementAt((new Random()).Next(GameHashes.Count()));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}