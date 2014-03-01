using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace COMP472_A2
{
    public enum enumLanguage { english, french, spanish }

    class Corpus
    {
        // language-specific bigram dictionary, populated according to the text provided
        private SortedDictionary<string, double> bigramDictionary;

        // empty dictionary containing all bigrams. each language makes a copy and populates it
        private static SortedDictionary<string, double> emptyBigramDictionary;
        
        public Corpus(enumLanguage language)
        {
            if (emptyBigramDictionary == null)
            {
                InitializeEmptyBigramDictionary();
            }
            
            PopulateBigramDictionary(File.ReadAllText(language.ToString() + ".txt"));
        }

        public static void FreeEmptyDictionary()
        {
            emptyBigramDictionary = null;
        }

        private void InitializeEmptyBigramDictionary()
        {
            emptyBigramDictionary = new SortedDictionary<string, double>();

            for (char a = 'a'; a <= 'z'; a++)
            {
                AddNewBigram(a, ' ');

                for (char b = 'a'; b <= 'z'; b++)
                {
                    AddNewBigram(a, b);
                }
            }
            //TODO: Do we ignore punctuation? '.' and ',' might be relevant
        }

        private void AddNewBigram(char a, char b)
        {
            string ab = a.ToString() + b.ToString();
            if (!emptyBigramDictionary.ContainsKey(ab))
            {
                emptyBigramDictionary.Add(ab, 0.0);
            }
            
            string ba = b.ToString() + a.ToString();
            if (!emptyBigramDictionary.ContainsKey(ba))
            {
                emptyBigramDictionary.Add(ba, 0.0);
            }
        }

        private void PopulateBigramDictionary(string sInput)
        {
            // replace all multiple whitespaces, newlines and carriage returns with ' '
            StringBuilder sbProcessor = new StringBuilder(Regex.Replace(sInput, @"\s+", " "));
            sbProcessor.Replace('\r', ' ');
            sbProcessor.Replace('\n', ' ');

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