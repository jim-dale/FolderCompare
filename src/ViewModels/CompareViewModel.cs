
using System;

namespace FolderCompare
{
    public class CompareViewModel
    {
        public FileMetadata LeftItem { get; set; }
        public FileMetadata RightItem { get; set; }
        public int Comparison { get; set; }
        public bool? AreEqual { get; set; }

        public ConsoleColor LeftPathColour { get; set; }
        public ConsoleColor RightPathColour { get; set; }
        public ConsoleColor LeftSizeColour { get; set; }
        public ConsoleColor RightSizeColour { get; set; }
        public ConsoleColor LeftDateColour { get; set; }
        public ConsoleColor RightDateColour { get; set; }
    }
}
