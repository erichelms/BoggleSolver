using System.Collections.Generic;
using System.IO;

namespace Anagrams
{
    /// <summary>
    /// This class reads from an encoded Directed Acyclic Word Graph and creates an array
    /// of nodes that represent the DAWG.
    /// Each element in the array contains the following info:
    ///
    /// End of Word : 1 bit                 If End of Word bit set then
    /// |     Size of set : 5 bits          Difficulty Code
    /// |     |- --|                        |------|
    /// 00000000 00000000 00000000 00000000 00000000 00000000
    ///  |---|      |---- -------- -------|          |------|
    ///  |   |      Index of next node : 21 bits     WordList bit mask
    ///  Letter : 5 bits
    /// </summary>
    class DawgDecoder
    {
        private static uint endOfWordMask = 0x80000000;
        private static uint letterMask = 0x7C000000;
        private static uint setSizeMask = 0x03E00000;
        private static uint nextIdxMask = 0x001FFFFF;

        private static int difficultyMask = 0xFF00;
        private static int dictionaryMask = 0x00FF;

        private static int endOfWordShift = 31;
        private static int letterShift = 26;
        private static int setSizeShift = 21;
        private static int difficultyShift = 8;

        /// <summary>
        /// Takes the given file and attempts to build a dictionary.
        /// </summary>
        /// <param name="fileName">The full path to the file.</param>
        /// <returns>A dictionary represented by a list of dictionary nodes.</returns>
        public static List<DawgNode> Decode(string fileName)
        {
            List<DawgNode> list = new List<DawgNode>();

            if (!File.Exists(fileName))
                throw new FileNotFoundException("The file " + fileName + " does not exist.");

            using (EndianBinaryReader reader = new EndianBinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                while (!reader.EndOfStream)
                {
                    DawgNode node;
                    uint value = reader.ReadUInt32();

                    if (IsEndOfWord(value))
                    {
                        node = DecodeAcceptNode(value, reader.ReadUInt16());
                    }
                    else
                    {
                        node = DecodeNode(value);
                    }
                    list.Add(node);
                }
            }

            return list;
        }

        /// <summary>
        /// Determines if this potential node has End OF Word marker.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsEndOfWord(uint value)
        {
            return ((value & endOfWordMask) >> endOfWordShift) == 0 ? false : true;
        }

        /// <summary>
        /// Decodes the inputs into a dictionary node.
        /// </summary>
        /// <param name="value">The encoded character, set size, and next index.</param>
        /// <param name="value2">The encoded word difficulty and lexicon.</param>
        /// <returns></returns>
        private static DawgNode DecodeAcceptNode(uint value, uint value2)
        {
            return Dawg.NodeFactory(
                (char)(((value & letterMask) >> letterShift) + 65),
                (byte)((value & setSizeMask) >> setSizeShift),
                (int)(value & nextIdxMask),
                ((byte)((value2 & difficultyMask) >> difficultyShift)),
                ((byte)(value2 & dictionaryMask))
            );
        }

        /// <summary>
        /// Decodes the inputs into a dictionary node.
        /// </summary>
        /// <param name="value">The encoded character, set size, and next index.</param>
        /// <returns></returns>
        private static DawgNode DecodeNode(uint value)
        {
            return Dawg.NodeFactory(
                (char)(((value & letterMask) >> letterShift) + 65),
                (byte)((value & setSizeMask) >> setSizeShift),
                (int)(value & nextIdxMask)
            );
        }

    }
}
