using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Anagrams
{
    /// <summary>
    /// This singleton class provides a WordList front end for the Dawg class
    /// which is the class that holds the searchable compact structure for this WordList.
    /// This class does contain a list of the valid words for the given lexicon and difficulty level if
    /// the call to Set() method has flag value set true.
    /// </summary>
    class WordList : IEnumerable<string>
    {
        /// <summary>
		/// Holds the only instance of this singleton class.
		/// </summary>
		private static WordList instance = null;

        /// <summary>
        /// Is this dictionary initialized?
        /// </summary>
        private static bool initialized = false;

        /// <summary>
        /// Selects which dictionary load into this dictionary instance.
        /// This parameter only effects words available in <code>wordList</code>.
        /// </summary>
        private Book lexicon;

        /// <summary>
        /// Sets the difficulty level of the words available in this dictionary.
        /// This parameter only effects words available in <code>wordList</code>.
        /// </summary>
        private Difficulty difficulty;

        /// <summary>
        /// The list of all the words contained in the dictionary given the lexicon and difficulty settings.
        /// This is used only by computer player.
        /// </summary>
        private static List<string> wordList = null;

        /// <summary>
        /// Private constructor for Singleton class.
        /// </summary>
        private WordList()
        {
            BuildWordList();
        }

        /// <summary>
        /// Get the instance of this singleton class - WordList.
        /// </summary>
        /// <returns></returns>
        public static WordList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WordList();
                }

                return instance;
            }
        }

        /// <summary>
        /// Returns the count of words in this dictionary.
        /// Returns the count of items in wordList or if wordList
        /// not defined, returns -1. 
        /// </summary>
        public int Count
        {
            get
            {
                return wordList == null ? -1 : wordList.Count;
            }
            
        }

        /// <summary>
		/// Specifies the selected Scrabble dictionary that this dictionary contains. 
        /// This must be called to generate the wordlist that the computer player uses.
		/// </summary>
        /// <param name="book">Enumeration specifing which dictionary words to select
		/// from the WordList file.</param>
        /// <param name="diff">Filters the list for difficulty of the words.</param>
        /// <param name="generateWordList">Generate the word list used by the computer player if true.</param>
		public void Set(Book book, Difficulty diff, bool generateWordList)
        {
            if (generateWordList && (lexicon != book || difficulty != diff))
            {
                wordList = Dawg.GetAllWords(diff, book);
            }

            lexicon = book;
            difficulty = diff;
            initialized = true;
        }

        public bool Partial(string word)
        {
            if (!initialized) throw new Exception("Word List has not been initialized.");
            return Dawg.Partial(word);
        }

        /// <summary>
        /// Determines if the string exists in this dictionary with the specified lexicon.
        /// </summary>
        /// <param name="word">The word to search for.</param>
        /// <param name="difficulty"></param>
        /// <returns>Returns true if the word is in the dictionary and it is less than or equal to the Difficulty</returns>
        public bool Contains(string word, Difficulty diff)
        {
            if (!initialized) throw new Exception("Word List has not been initialized.");
            return Dawg.Contains(word, diff, lexicon);
        }

        /// <summary>
        /// Determines if the string exists in this dictionary with the specified lexicon.
        /// By default this method does not take difficulty into account.
        /// </summary>
        /// <param name="word">The word to search for.</param>
        /// <returns>Returns true if the word is in the dictionary</returns>
        public bool Contains(string word)
        {
            if (!initialized) throw new Exception("Word List has not been initialized.");
            return Contains(word, Difficulty.EXPERT);
        }

        /// <summary>
		/// Builds the dictionary based upon the choosen word list
		/// defined by the lexicon class member.
		/// </summary>
		private static void BuildWordList()
        {
            try
            {
                Dawg.BuildDictionary(@"C:\Users\Eric\Documents\visual studio 2015\Projects\BoggleSolver\BoggleSolver\Dictionary\Dictionary2.dawg");
            }
            catch (FileNotFoundException ex)
            {
                throw new IOException("Dictionary file not found.", ex);
            }
        }

        /// <summary>
        /// Implements IEnumerable on the word list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            if (wordList == null)
                throw new Exception("Word List has not been initialized.");
            return wordList.GetEnumerator();
        }

        /// <summary>
        /// Implements IEnumerable on the word list.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (wordList == null)
                throw new Exception("Word List has not been initialized.");
            return wordList.GetEnumerator();
        }
    }
}
