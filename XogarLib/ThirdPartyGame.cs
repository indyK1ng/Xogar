using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace XogarLib
{
    class ThirdPartyGame : Game
    {
        public Int64 GameId
        {
            get { return gameId; }
        }

        public string Name { get; set; }

        public string Executable { get; set; }

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

        public override void Launch()
        {
            Process.Start(Executable, Arguments);
        }

        public override String Hash()
        {
            return String.Format("{0}{1}", "ThirdParty", gameId.ToString());
        }
    }
}
