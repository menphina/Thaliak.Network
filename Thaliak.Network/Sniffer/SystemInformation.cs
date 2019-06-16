// Modifications copyright (C) 2019 Menphnia

using System.Security.Principal;

namespace Thaliak.Network.Sniffer
{
    public static class SystemInformation
    {
        public static bool IsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}