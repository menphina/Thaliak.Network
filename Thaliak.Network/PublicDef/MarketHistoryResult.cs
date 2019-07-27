
using System.Collections.Generic;

namespace Milvaneth.Common
{
    
    public class MarketHistoryResult : IResult
    {
        
        public int ItemId;
        
        public List<MarketHistoryItem> HistoryItems;
    }
}
