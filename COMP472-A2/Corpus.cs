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
        private string m_language;

        // language-specific bigram dictionary, populated according to the text provided
        private SortedDictionary<string, int> m_bigramDictionary;

        // empty dictionary of all bigrams. each language makes a copy and populates it
        private static SortedDictionary<string, int> s_emptyBigramDictionary;
        
        // 26 letters, whitespace, and punctuations (all are treated as one character)
        private static int s_nbBigramTypes = 28 * 28;

        public Corpus(enumLanguage language)
        {
            if (s_emptyBigramDictionary == null)
            {
                // create an empty dictionary of all bigrams (28 * 28)
                InitializeEmptyBigramDictionary();
            }

            m_language = language.ToString();
            // make a language-specific copy of bigram dictionary, parse text, and compute frequencies
            PopulateBigramDictionary(File.ReadAllText(m_language + ".txt"));
        }

        // compute probability of bigram in a training corpus, smooth, and print result
        public void TestBigram(string bigramToTest, ref double probabilitySoFar)
        {
            double delta = 0.5;
            double probability = (m_bigramDictionary[bigramToTest] + delta) / 
                                 (m_bigramDictionary.Count + delta * s_nbBigramTypes);
            probabilitySoFar += Math.Log10(probability);

            string joinProbability = "P(" + (Char.IsWhiteSpace(bigramToTest[0]) ? "_" : bigramToTest[0].ToString()) +
                                     "," + (Char.IsWhiteSpace(bigramToTest[1]) ? "_" : bigramToTest[1].ToString()) + ")";

            string languageName = m_language.ToUpper() + ":";
            if (m_language == "french")
                m_language += " "; // print exta space for aesthetic alignment

            Console.WriteLine("{0} {1} = {2:e11} ===> log prob of sequence so far: {3:f4}",
                        languageName, joinProbability.ToUpper(), probability, probabilitySoFar);
        }

        public bool DictionaryContainsBigram(string bigram)
        {
            return m_bigramDictionary.ContainsKey(bigram);
        }

        private static void InitializeEmptyBigramDictionary()
        {
            s_emptyBigramDictionary = new SortedDictionary<string, int>();
            
            AddNewBigram(' ', '*');
            AddNewBigram('*', '*');
            AddNewBigram(' ', ' ');

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

        public static void FreeEmptyDictionary()
        {
            s_emptyBigramDictionary = null;
        }

        // helper method to populate bigram dictionary
        private static void AddNewBigram(char a, char b)
        {
            string ab = a.ToString() + b.ToString();
            if (!s_emptyBigramDictionary.ContainsKey(ab))
            {
                s_emptyBigramDictionary.Add(ab, 0);
            }
            
            string ba = b.ToString() + a.ToString();
            if (!s_emptyBigramDictionary.ContainsKey(ba))
            {
                s_emptyBigramDictionary.Add(ba, 0);
            }
        }

        private void PopulateBigramDictionary(string sInput)
        {
            // replace all multiple whitespaces, newlines and carriage returns with ' '
            StringBuilder sbProcessor = new StringBuilder(Regex.Replace(sInput, @"\s+", " "));
            sbProcessor.Replace('\r', ' ');
            sbProcessor.Replace('\n', ' ');

            // convert punctuation into '*', get rid of spaces between ponctuations
            sbProcessor = new StringBuilder(Regex.Replace(sbProcessor.ToString(), @"[\.|,|!|\?|\-|:|""|'|_]", "*"));
            sbProcessor = new StringBuilder(Regex.Replace(sbProcessor.ToString(), @"\*\s\*", "**"));
            sbProcessor = new StringBuilder(Regex.Replace(sbProcessor.ToString(), @"\*+", "*"));
                        
            string sText = sbProcessor.ToString().ToLower();
            sbProcessor.Clear();

            m_bigramDictionary = new SortedDictionary<string, int>();
            foreach (string key in s_emptyBigramDictionary.Keys)
            {
                m_bigramDictionary.Add(key, 0);
            }

            // parse text and fill frequencies of bigrams
            for (int i = 0; i < sText.Length - 1; i++)
            {
                sbProcessor.Append(sText[i]);
                sbProcessor.Append(sText[i + 1]);

                if (m_bigramDictionary.ContainsKey(sbProcessor.ToString()))
                {
                    m_bigramDictionary[sbProcessor.ToString()]++;
                }
                sbProcessor.Clear();
            }
        }
    }
}