using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anagrams
{
    public enum Difficulty : byte
    {
        NEWBIE = 2,
        AMATEUR = 3,
        INTERMEDIATE = 4,
        ADVANCED = 6,
        EXPERT = 9
    }

    public enum Book : byte
    {
        ENABLE1 = 1, // 0000 0001
        ENABLE2K = 2,// 0000 0010
        TWL98 = 4,   // 0000 0100
        TWL06 = 8,   // 0000 1000
        OSPD4 = 16,  // 0001 0000
        YAWL = 32,   // 0010 0000
        SOWPODS = 64,// 0100 0000
        ALL = 127    // 0111 1111
    }
}
