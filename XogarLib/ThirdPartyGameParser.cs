using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using XogarLib.Properties;

namespace XogarLib
{
    public class ThirdPartyGameParser:IGameListingParser
    {
        public IDictionary<String, Game> GetGameListing()
        {
            ThirdPartyGames thirdGames = ThirdPartyGames.Load();

            return thirdGames.GetGames();
        }
    }
}