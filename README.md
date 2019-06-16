# Thaliak.Network

Thaliak.Network is the networking part of Thaliak FFXIV data extraction framework, a component of Milvaneth. 

Its main function is to identify, filter and analyze FFXIV game data packages. Support for customized processors set up (including connection filters and message analyzers) as well as event subscriptions for sub-message types. Currently Thaliak.Network does not support unpacking Lobby encrypted packets.

# Q & A

> How to use it

1. Identify network interface and game process.

2. Setup connection filters and data processors.

3. Initlize `MessageDispatcher`, `PacketAnalyzer` and `SocketSniffer`. Then subscribe to `MessageDispatcher` Events.

4. Start `PacketAnalyzer` and `SocketSniffer`.

5. Stop them when needed.

For actual workable code, see [Demo.cs](https://github.com/menphnia/Thaliak.Network/blob/master/Thaliak.Network/Demo.cs).

> How to add a processor

Inherit `NetworkMessage` Class, override (`new`) two functions and add it to the Type list. That's all.

You can view source code of existing processors in [/Thaliak.Network/PublicDefs](https://github.com/menphnia/Thaliak.Network/tree/master/Thaliak.Network/PublicDefs)

> This solution looks ... incomplete

True. This is part of a larger project. So there will be some code (such as Milvaneth.Common) and configuration (in the Solution file) left behind. But don't worry, everything needed in building the project is included in this repo.

> Almost half of packets captured have not been processed!

We have already recognized this. It's because the packets entering the processing flow do not contain TCP ACK packets (as they don't contain useful data), which are about half of the total number of packets game client/server send.

> Will there be Pcap support?

Maybe. Whether to implement Pcap packet capture depends on the overall development progress of Milvaneth Project, but you are free to implement a PcapSniffer by yourself.

> OpCodes will one day out-dated.

Sure. So I decided to use `GetMessageId` instead of employing enums or consts. Note that those currently in these methods are for test purpose ONLY. In fact, you can get them from config files, from some websites, from databases. Whatever, it's a method.

