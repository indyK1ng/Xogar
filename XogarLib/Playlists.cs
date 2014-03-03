using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Xml.Serialization;
using XogarLib.Properties;

namespace XogarLib
{
    public class Playlists
    {
        [XmlElement("Playlists")]
        public List<Playlist> Lists { get; set; }

        public Playlists()
        {
            Lists = new List<Playlist>();
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Playlists));
            FileStream stream = new FileStream(Settings.Default.PlaylistFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            serializer.Serialize(stream, this);
        }

        public static Playlists Load()
        {
            XmlSerializer deserialize = new XmlSerializer(typeof(Playlists));

            if (File.Exists(Settings.Default.PlaylistFile))
            {
                FileStream stream = new FileStream(Settings.Default.PlaylistFile, FileMode.Open, FileAccess.Read);
                Playlists gameSelectLists = (Playlists)deserialize.Deserialize(stream);

                return gameSelectLists;
            }

            return new Playlists();
        }
    }
}