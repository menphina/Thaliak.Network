using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Milvaneth.Common
{
    public class EnvironmentFile
    {
        private static bool _ensured; // once is enough
        private static readonly string pathEnv = Helper.GetMilFilePath("env.pack");

        private Dictionary<int, string> envItems = new Dictionary<int, string>();
        private readonly Serializer<Dictionary<int, string>> _serializer;

        public EnvironmentFile(DataStore localStore)
        {
            if(localStore == DataStore.Global)
                throw new ArgumentOutOfRangeException(nameof(localStore), "Occupied by global item store");

            _serializer = new Serializer<Dictionary<int, string>>(pathEnv, "");
        }

        public bool Load()
        {
            try
            {
                if (!File.Exists(pathEnv))
                {
                    File.Create(pathEnv);
                    _ensured = true;
                    return true;
                }

                envItems = _serializer.Load();
                _ensured = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Save()
        {
            _serializer.Save(envItems);
        }

        public string ReadEnvFile(DataStore store, int item)
        {
            if (!_ensured)
                Load();

            item |= (int)store;

            return envItems.TryGetValue(item, out var ret) ? ret : null;
        }

        public void WriteEnvFile(DataStore store, int item, string data, bool saveOnWrite = true)
        {
            if (!_ensured)
                Load();

            item |= (int)store;

            envItems[item] = data;

            if(saveOnWrite) Save();
        }
    }

    public enum DataStore
    {
        Global = 0x0000_0000,
        Thaliak = 0x0001_0000,
        ThaliakNetwork = 0x0002_0000,
    }

    public static class GlobalDataId
    {
        public const int LocalMilvanethVersion = 0x0000;
    }
}
