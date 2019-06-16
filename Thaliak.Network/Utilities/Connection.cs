using System.Net;

namespace Thaliak.Network.Utilities
{
    public class Connection
    {
        public IPEndPoint LocalEndPoint { get; }
        public IPEndPoint RemoteEndPoint { get; }
        public Connection Reverse => new Connection(RemoteEndPoint, LocalEndPoint);

        public Connection(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remoteEndPoint;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return obj is Connection connection && LocalEndPoint.Equals(connection.LocalEndPoint) && RemoteEndPoint.Equals(connection.RemoteEndPoint);
        }

        public override int GetHashCode()
        {
            return (LocalEndPoint.GetHashCode() + 0x0609) ^ RemoteEndPoint.GetHashCode();
        }

        public override string ToString()
        {
            return $"{LocalEndPoint} -> {RemoteEndPoint}";
        }
    }
}
