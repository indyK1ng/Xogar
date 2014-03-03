using System;
using System.Xml.Serialization;

namespace XogarLib
{
    public abstract class Game
    {
        protected Int64 gameId;

        public Game(Int64 gameId)
        {
            this.gameId = gameId;
        }

        protected Game()
        {
            
        }

        [XmlElement("Name")]
        public string Name { get; set; }

        public virtual bool IsReal()
        {
            return true;
        }

        public virtual bool ShouldSerialize()
        {
            return false;
        }

        public abstract void Launch();

        public abstract String Hash();
    }
}
