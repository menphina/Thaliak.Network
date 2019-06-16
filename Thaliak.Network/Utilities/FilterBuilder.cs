using System.Diagnostics;
using Thaliak.Network.Analyzer;
using Thaliak.Network.Filter;
using Thaliak.Network.Sniffer;

namespace Thaliak.Network.Utilities
{
    public class FilterBuilder
    {
        public static Filters<IPPacket> BuildDefaultFilter(Process p)
        {
            var conn = ConnectionPicker.GetGameConnections(p);
            var filters = new Filters<IPPacket>(FilterOperator.OR);

            for (var i = 0; i < conn.Count; i++)
            {
                filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.Connection, conn[i], MessageAttribute.DirectionSend | i.ToPort())); // C2S
                filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.Connection, conn[i].Reverse, MessageAttribute.DirectionReceive | i.ToPort())); // S2C
            }

            return filters;
        }
    }
}
