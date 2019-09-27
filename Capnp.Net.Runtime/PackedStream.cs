using System.IO;

namespace Capnp
{
    public static class PackedStream
    {
        private const byte cEmptyByte = 0x00;
        private const byte cBit = 0x01;

        public static MemoryStream Pack(MemoryStream data)
        {
            MemoryStream packedStream = new MemoryStream();

            using (BinaryReader reader = new BinaryReader(data))
            {
                PackStream(reader, packedStream);
                packedStream.Seek(0, SeekOrigin.Begin);
                return packedStream;
            }
        }

        public static MemoryStream Pack(byte[] data)
        {
            return Pack(new MemoryStream(data));
        }

        private static void PackStream(BinaryReader reader, MemoryStream packedStream)
        {
            if (reader.BaseStream.Position >= reader.BaseStream.Length)
            {
                return;
            }

            byte tag = cEmptyByte;
            byte valueCount = cEmptyByte;
            byte[] valueBytes = new byte[8];

            for (byte i = 0; i < 8; i++)
            {
                if (reader.BaseStream.Position <= reader.BaseStream.Length)
                {
                    byte currentByte = reader.ReadByte();
                    if (currentByte != cEmptyByte)
                    {
                        tag += (byte)(cBit << i);
                        valueBytes[valueCount] = currentByte;
                        valueCount++;
                    }
                }
            }

            packedStream.WriteByte(tag);

            if (tag == cEmptyByte)
            {
                packedStream.WriteByte(CountZeroWords(reader, 0));
            }
            else
            {
                packedStream.Write(valueBytes, 0, valueCount);
            }

            PackStream(reader, packedStream);
        }

        private static byte CountZeroWords(BinaryReader reader, byte emptyWordCount)
        {
            bool wordIsEmpty = true;
            long startPos = reader.BaseStream.Position;

            for (byte i = 0; i < 8; i++)
            {
                if (startPos + i < reader.BaseStream.Length)
                {
                    if (reader.ReadByte() != cEmptyByte)
                    {
                        wordIsEmpty = false;
                        reader.BaseStream.Seek(startPos, SeekOrigin.Current);
                        break;
                    }
                }
            }

            if (wordIsEmpty)
            {
                return CountZeroWords(reader, (byte)(emptyWordCount + 1));
            }

            return emptyWordCount;
        }
    }
}
