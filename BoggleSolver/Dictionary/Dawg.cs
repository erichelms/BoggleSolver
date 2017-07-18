using System;
using System.Collections.Generic;

namespace Anagrams
{
    /// <summary>
    /// This class contains a list of words expressed as a dawg. Each node in the dawg
    /// contains a letter, a reference to the set of letters that can follow that
    /// letter in valid words (transitions), and an index that that letter points to in the list.
    /// AcceptDawgNode represent the end of a word, but may also contain outgoing transitions.
    /// Any valid word string will end at an AcceptDawgNode.
    /// 
    /// This is a static class since this contains all words from all defined word lists.
    /// 
    /// The interface to this class is the WordList class.
    /// </summary>
    internal class Dawg
    {
        /// <summary>
        /// A reference to the dictionary.
        /// </summary>
        public static List<DawgNode> dictionary;


        /// <summary>
        /// Builds the dictionary from a file.
        /// </summary>
        /// <param name="path"></param>
        public static void BuildDictionary(string path)
        {
            dictionary = DawgDecoder.Decode(path);
        }

        /// <summary>
        /// Determines if this character is in the transition set for this node.
        /// Performs a sequential search. Binary search overhead makes it slower 
        /// than sequential.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static DawgNode Transition(DawgNode node, char ch)
        {
            for (int i = node.TransitionSetBeginIndex; i < node.TransitionSetBeginIndex + node.TransitionSetSize; i++)
            {
                if (ch == dictionary[i].Letter)
                {
                    return dictionary[i];
                }
            }
            return null;
        }

        public static bool Partial(string str)
        {
            // For the first letter in the string point to the correct node.
            DawgNode node = dictionary[str[0] - 65];

            // For each subsequent letter transition to the next node.
            foreach (char ch in str.Substring(1))
            {
                node = Transition(node, ch);
                if (node == null) return false;
            }

            return true;
        }

        /// <summary>
        /// Tests to see it the string can be found in the dictionary.
        /// </summary>
        /// <param name="str">The word to search for.</param>
        /// <param name="diff"></param>
        /// <param name="lexicon"></param>
        /// <returns>True if word is found, otherwise false.</returns>
        public static bool Contains(string str, Difficulty diff, Book lexicon)
        {
            // For the first letter in the string point to the correct node.
            DawgNode node = dictionary[str[0] - 65];

            // For each subsequent letter transition to the next node.
            foreach (char ch in str.Substring(1))
            {
                node = Transition(node, ch);
                if (node == null) return false;
            }

            // Check to see if the tranisiton path end a valid AcceptNode
            return node.TestAcceptNode(diff, lexicon);
        }

        /// <summary>
        /// Returns all the words contained in this graph that meet the criteria.
        /// Uses Queue to perform Breadth First Search.
        /// This is an expensive operation that should only be performed once during the
        /// creation of the WordList and only if a computer player exists.
        /// </summary>
        /// <param name="diff">The difficulty level.</param>
        /// <param name="lexicon">The wordList to use.</param>
        /// <returns>A list of strings.</returns>
        public static List<string> GetAllWords(Difficulty diff, Book lexicon)
        {
            List<string> wordList = new List<string>();
            var Q = new Queue<Tuple<DawgNode, string, int>>();
            Q.Enqueue(new Tuple<DawgNode, string, int>(dictionary[0], string.Empty, 0));

            while (Q.Count > 0)
            {
                var T = Q.Dequeue();

                for (int i = T.Item3; i < T.Item3 + T.Item1.TransitionSetSize; i++)
                {
                    DawgNode node = dictionary[i];
                    string word = T.Item2 + node.Letter;

                    if (node.TestAcceptNode(diff, lexicon))
                    {
                        wordList.Add(word.ToString());
                    }

                    if (node.TransitionSetSize >= 1)
                    {
                        Q.Enqueue(new Tuple<DawgNode, string, int>(node, word, node.TransitionSetBeginIndex));
                    }
                }
            }

            wordList.Sort();
            return wordList;
        }

        /// <summary>
        /// Method that creates nodes for the graph. This way the Node classes can be private.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="transSetSize"></param>
        /// <param name="transIndex"></param>
        /// <param name="list"></param>
        /// <returns>DictionaryNode or an AcceptDictionaryNode</returns>
        public static DawgNode NodeFactory(char letter, byte transSetSize, int transIndex, params byte[] list)
        {
            if (list.Length == 0)
            {
                return new DawgNode(letter, transSetSize, transIndex);
            }
            else
            {
                return new AcceptDawgNode(letter, transSetSize, transIndex, list[0], list[1]);
            }
        }
    } // end class

    /// <summary>
    /// A node that indicates the end of valid set of transitions.
    /// Stores additional information relative to a DawgNode.
    /// </summary>
    internal class AcceptDawgNode : DawgNode
    {
        //The level that this word
        private byte difficulty;

        //Which dictionary this word occurs in
        private byte lexicon;

        public byte Difficulty { get { return difficulty; } }

        public byte Lexicon { get { return lexicon; } }

        public AcceptDawgNode(char letter, byte transitionSetSize, int transitionSetBeginIndex, byte difficulty, byte dictionary)
            : base(letter, transitionSetSize, transitionSetBeginIndex)
        {
            this.difficulty = difficulty;
            this.lexicon = dictionary;
        }

        public override string ToString()
        {
            return (letter + " " + transitionSetSize + " " + transitionSetBeginIndex + " " + difficulty + " " + lexicon);
        }
    }

    internal class DawgNode
    {
        /// <summary>
        /// The letter for this node.
        /// </summary>
        protected char letter;

        /// <summary>
        /// The int denoting the size of this node's outgoing transition set.
        /// </summary>
        protected byte transitionSetSize;

        /// <summary>
        /// The int denoting the index (in the array which contains this node) at which this node's transition set begins
        /// </summary>
        protected int transitionSetBeginIndex;

        public char Letter
        {
            get
            {
                return letter;
            }
        }

        public byte TransitionSetSize
        {
            get
            {
                return transitionSetSize;
            }
        }

        public int TransitionSetBeginIndex
        {
            get
            {
                return transitionSetBeginIndex;
            }
        }

        /// <summary>
        /// Creates a dictionary node
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="transitionSetSize"></param>
        /// <param name="transitionSetBeginIndex"></param>
        public DawgNode(char letter, byte transitionSetSize, int transitionSetBeginIndex)
        {
            this.letter = letter;
            this.transitionSetSize = transitionSetSize;
            this.transitionSetBeginIndex = transitionSetBeginIndex;
        }

        /// <summary>
        /// See if the node is valid end of word node given the criteria parameters.
        /// </summary>
        /// <param name="diff"></param>
        /// <param name="lexicon"></param>
        /// <returns>Return true if node is AcceptNode and meets criteria.</returns>
        public bool TestAcceptNode(Difficulty diff, Book lexicon)
        {
            if (!(this is AcceptDawgNode)) return false;

            AcceptDawgNode node = this as AcceptDawgNode;
            return (Difficulty)node.Difficulty <= diff && (lexicon & (Book)node.Lexicon) == lexicon;
        }

        public override string ToString()
        {
            return (letter + " " + transitionSetSize + " " + transitionSetBeginIndex);
        }
    }

}
