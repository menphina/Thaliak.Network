using System;
using System.Collections.Generic;
using System.Linq;
using Thaliak.Network.Messages;

namespace Thaliak.Network.Utilities
{
    public class ConsumerSearcher
    {
        public static IEnumerable<Type> FindConsumers(string @namespace)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && t.Namespace == @namespace && t.IsSubclassOf(typeof(NetworkMessage)));
        }

        public static IEnumerable<Type> GetDefaultConsumers()
        {
            return new[]
            {
                typeof(NetworkCharacterName),
                typeof(NetworkMarketHistory),
                typeof(NetworkRetainerHistory),
                typeof(NetworkMarketListing),
                typeof(NetworkMarketListingCount),
                typeof(NetworkMarketResult),
                typeof(NetworkPlayerSpawn),
                typeof(NetworkItemInfo),
                typeof(NetworkItemPriceInfo),
                typeof(NetworkLogout),
                typeof(NetworkLogoutCancel),
                typeof(NetworkRetainerSummary),
            };
        }
    }
}
