using NetFwTypeLib;
using System;
using System.Diagnostics;
using Milvaneth.Common;

namespace Thaliak.Network.Utilities
{
    public class FirewallRegister
    {
        //control.exe /name Microsoft.WindowsFirewall /page pageConfigureApps
        public static void RegisterToFirewall()
        {
            try
            {
                var exePath = Process.GetCurrentProcess().MainModule?.FileName ?? throw new InvalidOperationException("Current process has no main module");

                var netFwMgr = GetInstance<INetFwMgr>("HNetCfg.FwMgr");

                if(!netFwMgr.LocalPolicy.CurrentProfile.FirewallEnabled) return;

                var netAuthApps = netFwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications;

                var isExists = false;
                foreach (var netAuthAppObject in netAuthApps)
                {
                    if (netAuthAppObject is INetFwAuthorizedApplication netAuthApp && netAuthApp.ProcessImageFileName == exePath && netAuthApp.Enabled)
                    {
                        isExists = true;
                    }
                }

                if (!isExists)
                {
                    var netAuthApp = GetInstance<INetFwAuthorizedApplication>("HNetCfg.FwAuthorizedApplication");

                    netAuthApp.Enabled = true;
                    netAuthApp.Name = "Milvaneth Network Monitor";
                    netAuthApp.ProcessImageFileName = exePath;
                    netAuthApp.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;

                    netAuthApps.Add(netAuthApp);
                }
            }
            catch
            {
                Notifier.Raise(Signal.CollaborationDisplayMessageBoxYesAndNo, new[]
                {
                    "无法自动注册防火墙规则，是否手工注册？", "提示", "FirewallRegister"
                }, true);
                Notifier.Raise(Signal.ClientFirewallNotAllowed, null);
                Notifier.OnNotification += FirewallDisplayHandler;
            }

            T GetInstance<T>(string typeName)
            {
                return (T)Activator.CreateInstance(Type.GetTypeFromProgID(typeName));
            }
        }

        private static void FirewallDisplayHandler(Signal sig, DateTime time, string stack, string[] args)
        {
            if (sig != Signal.CollaborationDisplayedMessageBoxResultYes) return;
            if (args == null || args.Length < 1 || args[0] != "FirewallRegister") return;

            Notifier.Raise(Signal.CollaborationDisplayMessageBoxOk, new[]
            {
                "请在打开的页面中添加并允许 Milvaneth.exe 使用私人和公用网络\n并请在完成上述操作后重新启动本程序", "提示", "FirewallRegister"
            }, true);

            Process.Start("control.exe", "/name Microsoft.WindowsFirewall /page pageConfigureApps");

            Notifier.OnNotification -= FirewallDisplayHandler;
        }
    }
}
