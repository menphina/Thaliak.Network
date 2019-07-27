# Thaliak.Network

Thaliak.Network is the networking part of Thaliak FFXIV data extraction framework, a component of Milvaneth. 

Its main function is to identify, filter and analyze FFXIV game data packages. Support for customized processors set up (including connection filters and message analyzers) as well as event subscriptions for sub-message types. Currently Thaliak.Network does not support unpacking Lobby encrypted packets.

This repository is a partial mirror of a internal repository, so it not reflecting the current status of development, but we will accept issues and pull requests here.

# Q & A

> How to use it

1. Identify network interface and game process.

2. Setup connection filters and data processors.

3. Initlize `MessageDispatcher`, `PacketAnalyzer` and `SocketSniffer`. Then subscribe to `MessageDispatcher` Events.

4. Start `PacketAnalyzer` and `SocketSniffer`.

5. Stop them when needed.

For actual workable code, see [Demo.cs](https://github.com/menphina/Thaliak.Network/blob/master/Thaliak.Network/Demo.cs).

> Do you support lobby connection

Yes. Thaliak.Network features with fully automated lobby decryption.

> I want raw messages data! Why you dropped them!

Hmmmm. You could just ignore the built in `MessageDispatcher` and write your own version. As data inputed into `MessageDispatcher` is raw byte array.

> How to add a processor

Inherit `NetworkMessage` Class, override (`new`) two functions and add it to the Type list. That's all.

You can view source code of existing processors in [/Thaliak.Network/Messages](https://github.com/menphina/Thaliak.Network/tree/master/Thaliak.Network/Messages)

> This solution looks ... incomplete

True. This is part of a larger project. So there will be some code (such as Milvaneth.Common) and configuration (in the Solution file) left behind. But don't worry, everything needed in building the project is included in this repo.

> Almost half of packets captured have not been processed!

We recognized this. It's because the packets entering the processing flow do not contain TCP ACK packets (as they don't contain useful data), which are about half of the total number of packets game client/server send.

> Will there be Pcap support?

Maybe. Whether to implement Pcap packet capture depends on the overall development progress of Milvaneth Project, but you are free to implement a PcapSniffer by yourself.

> OpCodes will one day out-dated.

Sure. So I decided to use `GetMessageId` instead of employing enums or consts. Note that those literals currently in these methods are for testing purpose ONLY. In fact, you can get them from config files, from some websites, from databases. Whatever, it's a method.
