using System;
using System.Collections.Generic;
using Thaliak.Network.Dispatcher;

namespace Thaliak.Network.Utilities
{
    public class MessageIdRetriver
    {
        private Dictionary<int, int> msgDic;
        private static readonly Lazy<MessageIdRetriver> lazy =
            new Lazy<MessageIdRetriver>(() => new MessageIdRetriver());

        public static MessageIdRetriver Instance => lazy.Value;

        private MessageIdRetriver()
        {
            msgDic = new Dictionary<int, int>
            {
                [(int)MessageIdRetriveKey.VersionData] = 4550,
                [(int)MessageIdRetriveKey.NetworkMarketHistory] = 0x012A,
                [(int)MessageIdRetriveKey.NetworkRetainerHistory] = 0x012B,
                [(int)MessageIdRetriveKey.NetworkMarketListing] = 0x0126,
                [(int)MessageIdRetriveKey.NetworkMarketListingCount] = 0x0125,
                [(int)MessageIdRetriveKey.NetworkMarketResult] = 0x0139,
                [(int)MessageIdRetriveKey.NetworkPlayerSpawn] = 0x0175,
                [(int)MessageIdRetriveKey.NetworkItemInfo] = 0x0196,
                [(int)MessageIdRetriveKey.NetworkItemPriceInfo] = 0x0194,
                [(int)MessageIdRetriveKey.NetworkRetainerSummary] = 0x0192,
                [(int)MessageIdRetriveKey.NetworkRetainerSumEnd] = 0x0190,
                [(int)MessageIdRetriveKey.NetworkItemInfoEnd] = 0x0193,
                [(int)MessageIdRetriveKey.NetworkUpdateHpMpTp] = 0x0145,
                [(int)MessageIdRetriveKey.NetworkCharacterName] = 0x018E,

                // Below zero is send
                [(int)MessageIdRetriveKey.NetworkLogout] = -0x0074,
                [(int)MessageIdRetriveKey.NetworkRequestRetainer] = -0x0161,
                [(int)MessageIdRetriveKey.NetworkInventoryModify] = -0x0146,

                // OPMASK_LOBBY means lobby package
                [(int)MessageIdRetriveKey.NetworkLobbyCharacter] = MessageDispatcher.OPMASK_LOBBY | 0x000D,
            };
        }

        public int GetMessageId(MessageIdRetriveKey rawId)
        {
            if (msgDic.TryGetValue((int) rawId, out var id)) return id;

            throw new Exception("Message Id not found");
        }

        public int GetVersion()
        {
            return GetMessageId(MessageIdRetriveKey.VersionData);
        }
    }

    public enum MessageIdRetriveKey
    {
        VersionData = 0,
        NetworkMarketHistory = 1,
        NetworkRetainerHistory = 2,
        NetworkMarketListing = 3,
        NetworkMarketListingCount = 4,
        NetworkMarketResult = 5,
        NetworkPlayerSpawn = 6,
        NetworkItemInfo = 7,
        NetworkItemPriceInfo = 8,
        NetworkLogout = 9,
        NetworkRetainerSummary = 10,
        NetworkRetainerSumEnd = 11,
        NetworkItemInfoEnd = 12,
        NetworkUpdateHpMpTp = 13,
        NetworkLobbyCharacter = 14,
        NetworkCharacterName = 15,
        NetworkRequestRetainer = 16,
        NetworkInventoryModify = 17,
    }
}
