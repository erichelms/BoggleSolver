using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Anagrams;

namespace BoggleSolver
{
    class Program
    {
        static char[,] puzzle = new char[4, 4]
        {
            {'T','E','S','O' },
            {'D','A','S','S' },
            {'M','E','E','T' },
            {'H','I','S','R' }
        };

        static void Main(string[] args)
        {
            List<string> answer = new Boggle(puzzle).Solve();
            ;
        }
    }

    class Boggle
    {
        /// <summary>
        /// The dictionary.
        /// </summary>
        private static WordList dictionary = WordList.Instance;

        /// <summary>
        /// The puzzle result.
        /// </summary>
        private HashSet<string> words = new HashSet<string>();

        /// <summary>
        /// The boggle puzzle.
        /// </summary>
        private char[,] puzzle;

        /// <summary>
        /// The dimensions of the boggle puzzle.
        /// </summary>
        private Point dimension;

        /// <summary>
        /// The stack contains all possibble moves as they are discovered.
        /// </summary>
        private Stack<Frame> stack = new Stack<Frame>();

        /// <summary>
        /// Possible directions that can be reached from each position in the puzzle.
        /// </summary>
        private static List<Point> directions = new List<Point>
        {
            new Point(1, 1),
            new Point(1, 0),
            new Point(1, -1),
            new Point(0, 1),

            new Point(0, -1),
            new Point(-1, 1),
            new Point(-1, 0),
            new Point(-1, -1),       
        };

        public Boggle(char[,] puzzle)
        {
            this.puzzle = puzzle;
            dimension = new Point(puzzle.GetLength(0), puzzle.GetLength(1));
            // create the dictionary structure
            dictionary.Set(Book.ALL, Difficulty.EXPERT, false);
        }

        /// <summary>
        /// Does a depth first search through the solution space to 
        /// find all possible words in the boggle puzzle.
        /// </summary>
        /// <returns>The list of words.</returns>
        public List<string> Solve()
        {
            bool[,] visited;

            // populate the initial stack with every starting point and direction in the puzzle grid.
            for (int x = 0; x < dimension.X; x++)
            {
                for (int y = 0; y < dimension.Y; y++)
                {
                    visited = new bool[dimension.X, dimension.Y];
                    visited[x, y] = true;
                    PushFrames(new Point(x, y), puzzle[x, y].ToString(), visited);
                }
            }

            // Solve the puzzle by exploring every potential path in the stack.
            while (stack.Count > 0)
            {
                Solve(stack.Pop());
            }

            return words.ToList();
        }

        private void Solve(Frame f)
        {
            // if the next location is off the puzzle grid or
            // has already been visited then reached a dead end so just return.
            if (f.NextLocation.X < 0 || f.NextLocation.X > dimension.X - 1 ||
                f.NextLocation.Y < 0 || f.NextLocation.Y > dimension.Y - 1 ||
                f.Visited[f.NextLocation.X, f.NextLocation.Y])
            {
                return;
            }

            // set the nextLocation as visited
            f.Visited[f.NextLocation.X, f.NextLocation.Y] = true;

            // add the next letter to create a word
            string word = f.Letters + puzzle[f.NextLocation.X, f.NextLocation.Y];

            // test if the word is valid
            if (dictionary.Contains(word))
            {
                words.Add(word);
            }
            // test if the word is a valid partial word (ie prefix)
            else if (!dictionary.Partial(word))
            {
                // no word can be formed so return
                return;
            }

            // we have a valid word or a partial word so add all possible directions to current path
            PushFrames(f.NextLocation, word, f.Visited);
        }

        /// <summary>
        /// Creates stack frames from the current position.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="letters"></param>
        /// <param name="visited"></param>
        private void PushFrames(Point location, string letters, bool[,] visited)
        {
            foreach (var direction in directions)
            {
                stack.Push(new Frame(new Point(location.X + direction.X, location.Y + direction.Y), letters, visited));
            }
        }

        private class Frame
        {
            /// <summary>
            /// The current location in the puzzle.
            /// </summary>
            public Point NextLocation;

            /// <summary>
            /// The set of all visited positions in the puzzle for the curent path.
            /// </summary>
            public bool[,] Visited;

            /// <summary>
            /// The set of letters that have been visited so far, ie the word.
            /// Precondition: It is a valid word or a valid prefix.
            /// </summary>
            public string Letters;

            /// <summary>
            /// Constructor creates a new stack frame.
            /// </summary>
            /// <param name="nextLocation">The current location.</param>
            /// <param name="letters">The current set of letters.</param>
            /// <param name="visited">The current visited array.</param>
            public Frame(Point nextLocation, string letters, bool[,] visited)
            {
                this.NextLocation = new Point(nextLocation.X, nextLocation.Y);
                this.Letters = letters.Clone() as string;
                this.Visited = visited.Clone() as bool[,];
            }
        }
    }
}
