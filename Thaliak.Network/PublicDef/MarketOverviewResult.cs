
using System.Collections.Generic;

namespace Milvaneth.Common
{
    
    public class MarketOverviewResult : IResult
    {
        
        public List<MarketOverviewItem> ResultItems;
        
        public int ItemIndexEnd;
        
        public int Padding;
        
        public int ItemIndexStart;
        
        public int RequestId;
    }
}
