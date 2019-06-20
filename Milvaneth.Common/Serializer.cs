using MsgPack.Serialization;
using System.IO;
using System.Text;

namespace Milvaneth.Common
{
    public class Serializer<T> where T : class
    {
        private readonly string filePath;
        private MessagePackSerializer<T> serializer;
        private byte[] entropy;

        public Serializer(string path, string pass)
        {
            filePath = path;
            entropy = Encoding.UTF8.GetBytes(pass);
            serializer = MessagePackSerializer.Get<T>();
        }

        public void Save(T obj)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var packStream = new MemoryStream();
            serializer.Pack(packStream, obj);

            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                var data = Encrypter.Encrypt(packStream.ToArray(), entropy);
                writer.Write(data, 0, data.Length);
                writer.Flush();
            }
        }

        public T Load()
        {
            using (var reader = new FileStream(filePath, FileMode.Open))
            {
                var data = Encrypter.Decrypt(ReadStream(reader), entropy);
                var packStream = new MemoryStream(data);
                return serializer.Unpack(packStream);
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
