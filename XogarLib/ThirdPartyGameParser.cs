using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using XogarLib.Properties;

namespace XogarLib
{
    public class ThirdPartyGameParser:IGameListingParser
    {
        internal ThirdPartyGames thirdPartyGames;

        public ThirdPartyGameParser()
        {
            thirdPartyGames = ThirdPartyGames.Load();
        }

        public IDictionary<String, Game> GetGameListing()
        {
            return thirdPartyGames.GetGames();
        }
    }
}