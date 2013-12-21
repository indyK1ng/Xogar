using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XogarLib
{
    interface IGameListingParser
    {
        IDictionary<String, Game> GetGameListing();
    }
}
