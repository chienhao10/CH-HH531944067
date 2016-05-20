using System;
using System.Text;

namespace AutoBuddy.Utilities.AutoShop
{
    internal static class StringDistance
    {
        private const double defaultMismatchScore = 0.0;
        private const double defaultMatchScore = 1.0;

        public static double RateSimilarity(string _firstWord, string _secondWord)
        {
            _firstWord = _firstWord.Replace("\'", string.Empty).Replace(" ", string.Empty).ToLower();
            _secondWord = _secondWord.Replace("\'", string.Empty).Replace(" ", string.Empty).ToLower();
            if (_firstWord == _secondWord)
                return defaultMatchScore;
            int halfLength = Math.Min(_firstWord.Length, _secondWord.Length)/2 + 1;

            StringBuilder common1 = GetCommonCharacters(_firstWord, _secondWord, halfLength);
            int commonMatches = common1.Length;

            if (commonMatches == 0)
                return defaultMismatchScore;

            StringBuilder common2 = GetCommonCharacters(_secondWord, _firstWord, halfLength);

            if (commonMatches != common2.Length)
                return defaultMismatchScore;
            int transpositions = 0;
            for (int i = 0; i < commonMatches; i++)
            {
                if (common1[i] != common2[i])
                    transpositions++;
            }

            transpositions /= 2;
            double jaroMetric = commonMatches/(3.0*_firstWord.Length) + commonMatches/(3.0*_secondWord.Length) +
                                (commonMatches - transpositions)/(3.0*commonMatches);
            return jaroMetric;
        }

        private static StringBuilder GetCommonCharacters(string firstWord, string secondWord, int separationDistance)
        {
            if ((firstWord == null) || (secondWord == null)) return null;
            StringBuilder returnCommons = new StringBuilder(20);
            StringBuilder copy = new StringBuilder(secondWord);
            int firstWordLength = firstWord.Length;
            int secondWordLength = secondWord.Length;

            for (int i = 0; i < firstWordLength; i++)
            {
                char character = firstWord[i];
                bool found = false;

                for (int j = Math.Max(0, i - separationDistance);
                    !found && j < Math.Min(i + separationDistance, secondWordLength);
                    j++)
                {
                    if (copy[j] == character)
                    {
                        found = true;
                        returnCommons.Append(character);
                        copy[j] = '#';
                    }
                }
            }
            return returnCommons;
        }

        public static double Match(this string s, string t)
        {
            return RateSimilarity(t, s);
        }
    }
}