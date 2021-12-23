using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyEBookReader
{
    class Program
    {
        string _theEBook = "";
        static void Main(string[] args)
        {
            Program _ = new();
            _.GetBook();
            Console.WriteLine("Downloading book...");
            Console.ReadLine();
        }

        void GetBook()
        {
            WebClient webClient = new();
            webClient.DownloadStringCompleted += (s, eArgs) =>
            {
                _theEBook = eArgs.Result;
                Console.WriteLine("Download complete.");
                GetStats();
            };
            webClient.DownloadStringAsync(new Uri("https://www.gutenberg.org/files/66986/66986-0.txt"));
        }
        void GetStats()
        {
            // Get the words from the ebook.
            string[] words = _theEBook.Split(new char[] { ' ', '\u000A', ',', '.', ';', ':', '-', '?', '/' },
                StringSplitOptions.RemoveEmptyEntries);

            string[] tenMostCommon = null;
            string longestWord = string.Empty;
            // Invoke the FindTenMostCommon() and FindLongestWord() methods in parallel to
            // use all available CPUs on the host machine
            Parallel.Invoke(
                () =>
                {
                    // Now, find the ten most common words.
                    tenMostCommon = FindTenMostCommon(words);

                },
                () =>
                {
                    // Get the longest word.
                    longestWord = FindLongestWord(words);
                }
            );



            // Now that all tasks are complete, build a string to show all stats.
            StringBuilder bookStats = new StringBuilder("Ten Most Common Words are:\n");
            foreach (string s in tenMostCommon)
            {
                bookStats.AppendLine(s);
            }


            bookStats.AppendFormat("Longest word is: {0}", longestWord);
            bookStats.AppendLine();
            Console.WriteLine(bookStats.ToString(), "Book Info");
        }
        string[] FindTenMostCommon(string[] words)
        {
            var frequencyOrder1 = from word in words
                                  where word.Length > 6
                                  group word by word into g
                                  orderby g.Count() descending
                                  select g;

            var frequencyOrder = from word in words
                                 where word.Length > 6
                                 group word by word into g
                                 orderby g.Count() descending
                                 select g.Key;
            string[] commanWords = (frequencyOrder.Take(10)).ToArray();
            return commanWords.Reverse().ToArray();
        }
        string FindLongestWord(string[] words)
        {
            var result = from w in words orderby w.Length descending select w;
            return result.FirstOrDefault();
        }
    }
}
