using System;
using Milvaneth.Common;
using System.Collections.Generic;
using System.IO;

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
            var path = Helper.GetMilFilePath("network.pack");
            if (!File.Exists(path))
            {
                Notifier.Raise(Signal.MilvanethNeedUpdate, new[] {"Network", "MessageIdRetriver" });
                throw new FileNotFoundException("Could not found update file");
            }

            var ser = new Serializer<Dictionary<int, int>>(path, "");
            try
            {
                msgDic = ser.Load();
            }
            catch (Exception e)
            {
                Notifier.Raise(Signal.MilvanethNeedUpdate, new[] { "Network", "MessageIdRetriver" });
                Notifier.Raise(Signal.InternalException, new []{ e.Message, "Network", "MessageIdRetriver", ".ctor" });
                throw new FileNotFoundException("Could not parse update file");
            }
        }

        public int GetMessageId(MessageIdRetriveKey rawId)
        {
            if (msgDic.TryGetValue((int) rawId, out var id)) return id;

            Notifier.Raise(Signal.MilvanethNeedUpdate, new[] { "Network", "MessageIdRetriver" });
            throw new FileNotFoundException("Could not read update file");
        }
        
        [Obsolete("For debug and internal use only")]
        internal static void SetMessageId()
        {
            var msgId = new Dictionary<int, int>
            {
                [(int) MessageIdRetriveKey.NetworkCharacterName] = 0x018E,
                [(int) MessageIdRetriveKey.NetworkMarketHistory] = 0x012A,
                [(int) MessageIdRetriveKey.NetworkRetainerHistory] = 0x012B,
                [(int) MessageIdRetriveKey.NetworkMarketListing] = 0x0126,
                [(int) MessageIdRetriveKey.NetworkMarketListingCount] = 0x0125,
                [(int) MessageIdRetriveKey.NetworkMarketResult] = 0x0139,
                [(int) MessageIdRetriveKey.NetworkPlayerSpawn] = 0x0175,
                [(int) MessageIdRetriveKey.NetworkItemInfo] = 0x0196,
                [(int) MessageIdRetriveKey.NetworkItemPriceInfo] = 0x0194,
                // below zero means client packet
                [(int) MessageIdRetriveKey.NetworkLogout] = -0x0074,
                [(int) MessageIdRetriveKey.NetworkLogoutCancel] = -0x0075,

                [(int) MessageIdRetriveKey.NetworkRetainerSummary] = 0x0192,
            };
            var path = Helper.GetMilFilePath("network.pack");
            var ser = new Serializer<Dictionary<int, int>>(path, "");
            ser.Save(msgId);
        }
    }

    public enum MessageIdRetriveKey
    {
        NetworkCharacterName = 0,
        NetworkMarketHistory = 1,
        NetworkRetainerHistory = 2,
        NetworkMarketListing = 3,
        NetworkMarketListingCount = 4,
        NetworkMarketResult = 5,
        NetworkPlayerSpawn = 6,
        NetworkItemInfo = 7,
        NetworkItemPriceInfo = 8,
        NetworkLogout = 9,
        NetworkLogoutCancel = 10,
        NetworkRetainerSummary = 11,

    }
}
