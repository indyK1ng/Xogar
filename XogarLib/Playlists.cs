using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Serialization;
using XogarLib.Properties;

namespace XogarLib
{
    public class Playlists
    {
        [XmlElement("Playlists")]
        public List<Playlist> CustomPlaylists { get; set; }

        private static readonly string envFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Xogar\\" + Settings.Default.PlaylistFile;

        public Playlists()
        {
            CustomPlaylists = new List<Playlist>();
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Playlists));
            FileStream stream = new FileStream(envFolderPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            serializer.Serialize(stream, this);
        }

        public static Playlists Load()
        {
            XmlSerializer deserialize = new XmlSerializer(typeof(Playlists));

            if (File.Exists(envFolderPath))
            {
                FileStream stream = new FileStream(envFolderPath, FileMode.Open, FileAccess.Read);
                Playlists gameSelectLists = (Playlists)deserialize.Deserialize(stream);
                stream.Close();

                return gameSelectLists;
            }

            return new Playlists();
        }
    }
}