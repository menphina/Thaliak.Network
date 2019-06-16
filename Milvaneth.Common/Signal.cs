namespace Milvaneth.Common
{
    // @formatter:off
    public enum Signal
    {
        InternalException =                         SignalCatalog.Internal | 0x0000, // Message
        InternalUnmanagedException =                SignalCatalog.Internal | 0x0001, // Message
        InternalDebug =                             SignalCatalog.Internal | 0x0002, // Message
        ClientDisconnected =                        SignalCatalog.Client | 0x0000, // null - Dispose and wait next alive
        ClientNetworkFail =                         SignalCatalog.Client | 0x0001, // Pid - Msgbox and wait next alive (Client PC has no available interface or network)
        ClientProcessDown =                         SignalCatalog.Client | 0x0002, // Pid - Dispose and wait next alive
        ClientPacketParseFail =                     SignalCatalog.Client | 0x0003, // null - Do nothing (QA counter, may suggest switch to Pcap [if implemented])
        ClientFirewallNotAllowed =                  SignalCatalog.Client | 0x0004, // null - Just log
        MilvanethInsuffcientPrivilege =             SignalCatalog.Milvaneth | 0x0000, // null - Show msgbox and exit (this should not occur but who knows)
        MilvanethSubprocessDown =                   SignalCatalog.Milvaneth | 0x0001, // ProcessName - Try to revive, if failed, show msgbox and exit (except debugger, that will crash EVERYTHING)
        MinvanethComponentExit =                    SignalCatalog.Milvaneth | 0x0002, // ComponentName - Try to revive, if failed, show msgbox and exit
        MinvanethFileUnreachable =                  SignalCatalog.Milvaneth | 0x0003, // null - Show msgbox and exit (may use alternative location?)
        CollaborationDisplayMessageBoxYesAndNo =    SignalCatalog.Collaboration | 0x0000, // Message, Title, TicketID
        CollaborationDisplayedMessageBoxResultYes = SignalCatalog.Collaboration | 0x0001, // TicketID
        CollaborationDisplayedMessageBoxResultNo =  SignalCatalog.Collaboration | 0x0002, // TicketID
        CollaborationDisplayMessageBoxOk =          SignalCatalog.Collaboration | 0x0003, // Message, Title, TicketID
        CollaborationDisplayedMessageBoxResultOk =  SignalCatalog.Collaboration | 0x0004, // TicketID
    }

    public enum SignalCatalog
    {
        Internal =      0x0000_0000, // some random internal things
        Client =        0x0001_0000, // well, this is not our fault, but we have to deal with the mess
        Milvaneth =     0x0002_0000, // threats the live of Nald
        Collaboration = 0x0003_0000, // request other thread/process to do something
    }
    // @formatter:on
}
