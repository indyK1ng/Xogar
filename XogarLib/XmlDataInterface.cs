using System.IO;
using System.Xml.Serialization;

namespace XogarLib
{
    public class XmlDataInterface <T> where T : new()
    {
        private string _folderPath;

        public XmlDataInterface (string folderPath)
        {
            _folderPath = folderPath;
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            FileStream stream = new FileStream(_folderPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            serializer.Serialize(stream, this);
        }

        public T Load()
        {
            XmlSerializer deserialize = new XmlSerializer(typeof(T));

            if (File.Exists(_folderPath))
            {
                FileStream stream = new FileStream(_folderPath, FileMode.Open, FileAccess.Read);
                T gameSelectLists = (T)deserialize.Deserialize(stream);
                stream.Close();

                return gameSelectLists;
            }

            return new T();
        }
    }
}