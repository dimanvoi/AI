using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace COMP472_A2
{
    class Program
    {
        static void Main(string[] args)
        {
            Corpus french  = new Corpus(enumLanguage.french);
            Corpus english = new Corpus(enumLanguage.english);
            Corpus spanish = new Corpus(enumLanguage.spanish);
            Corpus.FreeEmptyDictionary();

            double probabilityFrench;
            double probabilityEnglish;
            double probabilitySpanish;
            
            bool done = false;
            string answer = string.Empty;

            do
            {
                probabilityEnglish = 0.0;
                probabilityFrench  = 0.0;
                probabilitySpanish = 0.0;

                Console.Write("Enter a sentence: ");
                
                string inputSentence = Console.ReadLine().ToLower();
                string bigramToTest = string.Empty;
                char firstChar;
                char secondChar;
                
                for (int i = 0; i < inputSentence.Length - 1; i++)
                {
                    firstChar = inputSentence[i];
                    secondChar = inputSentence[i + 1];
                    bigramToTest = firstChar.ToString() + secondChar.ToString();
                    bigramToTest = Regex.Replace(bigramToTest, @"[\.|,|!|\?|\-|:|""|'|_]", "*");
                    Console.WriteLine("\nBIGRAM: " +
                                      (Char.IsWhiteSpace(bigramToTest[0]) ? "_" : bigramToTest[0].ToString()) +
                                      (Char.IsWhiteSpace(bigramToTest[1]) ? "_" : bigramToTest[1].ToString()));

                    if (!english.DictionaryContainsBigram(bigramToTest))
                    {
                        Console.WriteLine("=== Not found in training corpus. Skipping ===");
                    }
                    else
                    {
                        french.TestBigram( bigramToTest, ref probabilityFrench);
                        english.TestBigram(bigramToTest, ref probabilityEnglish);
                        spanish.TestBigram(bigramToTest, ref probabilitySpanish);
                    }
                }

                if (probabilityEnglish != 0) // if the sentence contained valid bigrams in any of the languages
                {
                    Console.Write("The sentence is ");
                    
                    if (probabilityEnglish > probabilityFrench)
                    {
                        if (probabilityEnglish > probabilitySpanish)
                            Console.WriteLine("English");
                        else
                            Console.WriteLine("Spanish");
                    }
                    else
                    {
                        if (probabilityFrench > probabilitySpanish)
                            Console.WriteLine("French");
                        else
                            Console.WriteLine("Spanish");
                    }
                }
                
                do 
                {
                    Console.Write("\nWould you like to enter another sentence? (y\\n): ");
                    answer = Console.ReadLine();
                } while (!answer.ToLower().StartsWith("n") && !answer.ToLower().StartsWith("y"));

                if (answer[0] == 'n')
                    done = true;
                else
                    Console.Clear();
            } while (!done);
        }
    }
}
