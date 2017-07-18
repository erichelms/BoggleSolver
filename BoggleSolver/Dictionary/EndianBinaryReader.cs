using System;
using System.IO;
using System.Text;

namespace Anagrams
{
    /// <summary>
    /// Reades files written in Big Endian format.
    /// Intercepts Read()s to the base class and performs ReadBytes instead.
    /// The ReadBytes buffer is reversed and then returned formatted as one of
    /// the .Net primitives.
    /// </summary>
    class EndianBinaryReader : BinaryReader
    {
        public EndianBinaryReader(Stream input) : base(input) { }

        public EndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }

        public EndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

        /// <summary>
        /// Determines if End of Stream has been reached. Returns true if it has.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return (BaseStream.Position >= BaseStream.Length) ? true : false;
            }
        }

        /// <summary>
        /// Reads 2 bytes from underlying stream.
        /// </summary>
        /// <returns>ushort</returns>
        new public ushort ReadUInt16()
        {
            return (ushort)reverseBuffer(base.ReadBytes(2));
        }

        /// <summary>
        /// Reads 4 bytes from underlying stream.
        /// </summary>
        /// <returns>uint</returns>
        new public uint ReadUInt32()
        {
            return (uint)reverseBuffer(base.ReadBytes(4));
        }

        /// <summary>
        /// Reads 8 bytes from underlying stream.
        /// </summary>
        /// <returns>ulong</returns>
        new public ulong ReadUInt64()
        {
            return reverseBuffer(base.ReadBytes(8));
        }

        private static ulong reverseBuffer(byte[] buffer)
        {
            Array.Reverse(buffer);
            ulong value = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                value += (ulong)(buffer[i] << i * 8);
            }
            return value;
        }
    }
}

