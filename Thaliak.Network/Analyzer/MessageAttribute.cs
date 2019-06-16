using System;
using System.Collections.Generic;

namespace Thaliak.Network.Analyzer
{
    [Flags]
    public enum MessageAttribute
    {
        DirectionSend = 0x10,
        DirectionReceive = 0x20,
        DirectionMask = 0xF0,

        PortA = 0x00,
        PortB = 0x01,
        PortC = 0x02,
        PortD = 0x03,
        PortE = 0x04,
        PortF = 0x05,
        PortMask = 0x0F,
    }

    public static class MessageAttributeHelper
    {
        public static MessageAttribute ToPort(this int value)
        {
            return (MessageAttribute) value;
        }

        public static MessageAttribute GetPort(this MessageAttribute value)
        {
            return value & MessageAttribute.PortMask;
        }

        public static MessageAttribute GetDirection(this MessageAttribute value)
        {
            return value & MessageAttribute.DirectionMask;
        }
    }
}
