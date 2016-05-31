using System;
using System.Collections.Specialized;

namespace DoddleReport
{
    public static class StringExtensions
    {
        /// <summary>
        /// Parse a string into an enumeration
        /// </summary>
        /// <typeparam name="TEnum">The Enumeration type to cast to</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TEnum ParseEnum<TEnum>(this string source)
        {
            Type t = typeof(TEnum);

            if (!t.IsEnum)
                throw new ArgumentException("TEnum must be a valid Enumeration", "TEnum");

            return (TEnum)Enum.Parse(t, source);
        }

        public static string TrimBefore(this string source, string match)
        {
            return source.Substring(source.IndexOf(match) + match.Length);
        }

        public static string TrimAfter(this string source, string match)
        {
            return source.Substring(source.IndexOf(match) + match.Length);
        }

        public static int NumberOfLines(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return 0;

            int count = 1;
            int start = 0;
            while ((start = source.IndexOf(Environment.NewLine, start)) != -1)
            {
                count++;
                start++;
            }
            return count;
        }

        /// <summary>
        /// Parses a camel cased or pascal cased string and returns a new 
        /// string with spaces between the words in the string.
        /// </summary>
        /// <example>
        /// The string "PascalCasing" will return an array with two 
        /// elements, "Pascal" and "Casing".
        /// </example>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SplitUpperCaseToString(this string source)
        {
            return string.Join(" ", SplitUpperCase(source));
        }


        /// <summary>
        /// Parses a camel cased or pascal cased string and returns an array 
        /// of the words within the string.
        /// </summary>
        /// <example>
        /// The string "PascalCasing" will return an array with two 
        /// elements, "Pascal" and "Casing".
        /// </example>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string[] SplitUpperCase(this string source)
        {
            if (source == null)
                return new string[] { }; //Return empty array.

            if (source.Length == 0)
                return new string[] { "" };

            StringCollection words = new StringCollection();
            int wordStartIndex = 0;

            char[] letters = source.ToCharArray();

            // Skip the first letter. we don't care what case it is.
            for (int i = 1; i < letters.Length; i++)
            {
                if (char.IsUpper(letters[i]))
                {
                    if (i + 1 < letters.Length && !char.IsUpper(letters[i + 1]))
                    {
                        //Grab everything before the current index.
                        words.Add(new String(letters, wordStartIndex, i - wordStartIndex));
                        wordStartIndex = i;
                    }
                }
            }

            //We need to have the last word.
            words.Add(new String(letters, wordStartIndex, letters.Length - wordStartIndex));

            //Copy to a string array.
            string[] wordArray = new string[words.Count];
            words.CopyTo(wordArray, 0);
            return wordArray;
        }
    }
}
