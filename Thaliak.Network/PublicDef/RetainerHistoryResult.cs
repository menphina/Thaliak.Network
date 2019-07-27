
using System.Collections.Generic;

namespace Milvaneth.Common
{
    
    public class RetainerHistoryResult : IResult
    {
        
        public long RetainerId;
        
        public List<RetainerHistoryItem> HistoryItems;
    }
}
