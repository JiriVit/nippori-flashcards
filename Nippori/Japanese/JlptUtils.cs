using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nippori.Japanese
{
    /// <summary>
    /// Provides methods related to JLPT material.
    /// </summary>
    public static class JlptUtils
    {
        #region .: Private Static Variables :.

        /// <summary>
        /// Dictionary with lists of kanjis, indexed by JLPT level the kanjis belong to.
        /// </summary>
        private static Dictionary<JlptLevels, List<char>> jlptKanji;

        #endregion

        #region .: Public Static Methods :.

        /// <summary>
        /// Initializes the class.
        /// </summary>
        public static void Init()
        {
            jlptKanji = new Dictionary<JlptLevels, List<char>>
            {
                { JlptLevels.N5, CreateKanjiList(Properties.Resources.kanji_n5) },
                { JlptLevels.N4, CreateKanjiList(Properties.Resources.kanji_n4) }
            };
        }

        /// <summary>
        /// Finds out if given character is a kanji and belongs to given or lower JLPT level.
        /// </summary>
        /// <param name="c">Character to be evaluated.</param>
        /// <param name="maxLevel">Maximum JLPT level to be checked.</param>
        /// <returns></returns>
        public static bool IsKanjiUpToJlptLevel(char c, JlptLevels maxLevel)
        {
            bool isOfTheLevel = false;
            JlptLevels level = JlptLevels.N5;

            if (IsKanji(c))
            {
                while ((level <= maxLevel) && !isOfTheLevel)
                {
                    isOfTheLevel = jlptKanji[level].Contains(c);
                    level++;
                }
            }

            return isOfTheLevel;
        }

        #endregion

        #region .: Private Static Methods :.

        /// <summary>
        /// Creates kanji list from a string where the kanjis are listed one per line.
        /// </summary>
        /// <param name="textSource">Source string to create the list from.</param>
        /// <returns></returns>
        private static List<char> CreateKanjiList(string textSource)
        {
            string[] separator = new string[] { "\r\n", "\n" };
            string[] separated = textSource.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            List<char> kanjiList = separated.Select(s => s[0]).ToList<char>();

            return kanjiList;
        }

        /// <summary>
        /// Determines if given character is a kanji.
        /// </summary>
        /// <param name="c">Character to be evaluated.</param>
        /// <returns>Boolean value indicating if the character is a kanji.</returns>
        private static bool IsKanji(char c) => Regex.IsMatch(c.ToString(), @"\p{IsCJKUnifiedIdeographs}");

        #endregion
    }
}
