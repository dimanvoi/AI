using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace COMP472_A2
{
    public enum enumLanguage { English, French, Spanish }

    class Corpus
    {
        // language name used for printing
        public string sLanguage { get; private set; }

        // language-specific bigram dictionary, populated according to the text provided
        private SortedDictionary<string, double> bigramDictionary;

        // empty dictionary containing all bigrams. each language makes a copy and populates it
        private static SortedDictionary<string, double> emptyBigramDictionary;
        private static int nbBigramTypes = 28 * 28;
        
        public Corpus(enumLanguage language)
        {
            if (emptyBigramDictionary == null)
            {
                InitializeEmptyBigramDictionary();
            }

            sLanguage = language.ToString();
            PopulateBigramDictionary(File.ReadAllText(sLanguage.ToLower() + ".txt"));
        }

        public static void FreeEmptyDictionary()
        {
            emptyBigramDictionary = null;
        }

        public double TestBigram(string bigram)
        {
            return bigramDictionary.ContainsKey(bigram) ?
                bigramDictionary[bigram] / ComputeProbabilityFormulaDenominator(bigram) : 0.0;
        }

        private double ComputeProbabilityFormulaDenominator(string bigram)
        {
            return bigramDictionary[bigram] + nbBigramTypes * 0.5;
        }

        private static void InitializeEmptyBigramDictionary()
        {
            emptyBigramDictionary = new SortedDictionary<string, double>();
            
            AddNewBigram(' ', '*');

            for (char a = 'a'; a <= 'z'; a++)
            {
                AddNewBigram(a, ' ');
                AddNewBigram(a, '*');
                
                for (char b = 'a'; b <= 'z'; b++)
                {
                    AddNewBigram(a, b);
                }
            }
        }

        private static bool AddNewBigram(char a, char b)
        {
            bool newBigramAdded = false;
            string ab = a.ToString() + b.ToString();
            if (!emptyBigramDictionary.ContainsKey(ab))
            {
                emptyBigramDictionary.Add(ab, 0.0);
                newBigramAdded |= true ;
            }
            
            string ba = b.ToString() + a.ToString();
            if (!emptyBigramDictionary.ContainsKey(ba))
            {
                emptyBigramDictionary.Add(ba, 0.0);
                newBigramAdded |= true;
            }

            return newBigramAdded;
        }

        private void PopulateBigramDictionary(string sInput)
        {
            // replace all multiple whitespaces, newlines and carriage returns with ' '
            StringBuilder sbProcessor = new StringBuilder(Regex.Replace(sInput, @"\s+", " "));
            sbProcessor.Replace('\r', ' ');
            sbProcessor.Replace('\n', ' ');

            // convert punctuation into '*'
            sbProcessor = new StringBuilder(Regex.Replace(sbProcessor.ToString(), @"[\.|,|!|?|-|:|""|'|_]", "*"));
            sbProcessor = new StringBuilder(Regex.Replace(sbProcessor.ToString(), @"\*+", "*"));
            
            string sText = sbProcessor.ToString().ToLower();
            sbProcessor.Clear();

            bigramDictionary = new SortedDictionary<string, double>();
            foreach (string key in emptyBigramDictionary.Keys)
            {
                bigramDictionary.Add(key, 0.5);
            }

            // parse entire text and update stats
            for (int i = 0; i < sText.Length - 1; i++)
            {
                sbProcessor.Append(sText[i]);
                sbProcessor.Append(sText[i + 1]);

                if (bigramDictionary.ContainsKey(sbProcessor.ToString()))
                {
                    bigramDictionary[sbProcessor.ToString()]++;
                }
                sbProcessor.Clear();
            }
        }
    }
}