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
            Corpus english = new Corpus(enumLanguage.English);
            Corpus french = new Corpus(enumLanguage.French);
            Corpus spanish = new Corpus(enumLanguage.Spanish);
            Corpus.FreeEmptyDictionary();

            double probabilityFrench;
            double probabilityEnglish;
            double probabilitySpanish;
            
            bool done = false;

            do
            {
                probabilityEnglish = 0.0;
                probabilityFrench = 0.0;
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
                    if ((!Char.IsLetter(firstChar)  && !Char.IsPunctuation(firstChar)  && !Char.IsWhiteSpace(firstChar)) ||
                        (!Char.IsLetter(secondChar) && !Char.IsPunctuation(secondChar) && !Char.IsWhiteSpace(secondChar)))
                        continue;
                                            
                    firstChar = Char.IsPunctuation(firstChar) ? '*' : firstChar;
                    secondChar = Char.IsPunctuation(secondChar) ? '*' : secondChar;
                    
                    bigramToTest = firstChar.ToString() + secondChar.ToString();
                    Console.WriteLine("\nBIGRAM: " + bigramToTest);
                    
                    TestBigram(french, bigramToTest, ref probabilityFrench);
                    TestBigram(english, bigramToTest, ref probabilityEnglish);
                    TestBigram(spanish, bigramToTest, ref probabilitySpanish);
                    bigramToTest = string.Empty;
                }

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

                string answer = string.Empty;
                do 
                {
                    Console.Write("\nWould you like to enter another sentence? (y\\n): ");
                    answer = Console.ReadLine();
                } while (!answer.StartsWith("n") && !answer.StartsWith("y"));

                if (answer[0] == 'n')
                    done = true;
                else
                    Console.WriteLine("\n================================================================================\n");
            } while (!done);
        }

        static void TestBigram(Corpus corpus, string bigramToTest, ref double probabilitySoFar)
        {
            double probability = corpus.TestBigram(bigramToTest);
            probabilitySoFar += Math.Log10(probability);
            
            string languageName = corpus.sLanguage == "French" ? corpus.sLanguage.ToUpper() + ": " : corpus.sLanguage.ToUpper() + ':';
            string joinProbability = "P(" + bigramToTest[0].ToString() + "," + bigramToTest[1].ToString() + ")";
            
            Console.WriteLine("{0} {1} = {2:e11} ===> log prob of sequence so far: {3:f4}",
                    languageName, joinProbability.ToUpper(), probability, probabilitySoFar);
            bigramToTest = string.Empty;

        }
    }
}
