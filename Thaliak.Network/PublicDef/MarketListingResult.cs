
using System.Collections.Generic;

namespace Milvaneth.Common
{
    
    public class MarketListingResult : IResult
    {
        
        public List<MarketListingItem> ListingItems;
        
        public byte ListingIndexEnd;
        
        public byte ListingIndexStart;
        
        public short RequestId;
        
        public short Padding;
    }
}
