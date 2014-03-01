using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP472_A2
{
    class Program
    {
        static void Main(string[] args)
        {
            Corpus english = new Corpus(enumLanguage.english);
            Corpus french  = new Corpus(enumLanguage.french);
            Corpus spanish = new Corpus(enumLanguage.spanish);

            Corpus.FreeEmptyDictionary(); // free-up memory

            Console.ReadKey();
        }
    }
}
