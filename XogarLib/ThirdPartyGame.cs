using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Xml.Serialization;

namespace XogarLib
{
    public class ThirdPartyGame : Game
    {
        [XmlElement("Id")]
        public Int64 GameId
        {
            get { return gameId; }
            set { gameId = value; }
        }

        [XmlElement("Executable")]
        public string Executable { get; set; }

        [XmlElement("Arguments")]
        public string Arguments { get; set; }

        public ThirdPartyGame(Int64 gameId, string name, string executable, string arguments) 
            : this(gameId, name, executable) 
        {
            Arguments = arguments;
        }

        public ThirdPartyGame(Int64 gameId, string name, string executable)
            : this(gameId)
        {
            Name = name;
            Executable = executable;
        }

        public ThirdPartyGame(Int64 gameId):base(gameId)
        {

        }

        public ThirdPartyGame():base()
        {
            
        }

        public override void Launch()
        {
            try
            {
                Process.Start(Executable, Arguments);
            }
            catch
            {
                MessageBox.Show("It appears that this game is not compatible with your system.  Sorry.",
                    "Error starting game.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override String Hash()
        {
            return String.Format("{0}{1}", "ThirdParty", gameId.ToString());
        }

        public override bool ShouldSerialize()
        {
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
