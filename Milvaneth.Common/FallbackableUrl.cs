using Flurl;
using System;

namespace Milvaneth.Common
{
    public class FallbackableUrl
    {
        public const string FallbackDomain = @"READCTED";
        public const string BackupDomain = @"READCTED";

        private string _defaultDomain;
        private string _appendingUrl;
        private string _fallbackIdentifier;

        public string DefaultUrl => Url.Combine(_defaultDomain, _appendingUrl);
        public string FallbackUrl => Url.Combine(FallbackDomain, _fallbackIdentifier, _appendingUrl);
        public string BackupUrl => Url.Combine(BackupDomain, _fallbackIdentifier, _appendingUrl);

        public FallbackableUrl(string defaultDomain, string appendingUrl, string fallbackIdentifier)
        {
            if(!defaultDomain.StartsWith("https://"))
                throw new InvalidOperationException("https");

            _defaultDomain = defaultDomain;
            _appendingUrl = appendingUrl;
            _fallbackIdentifier = fallbackIdentifier;
        }
    }
}
