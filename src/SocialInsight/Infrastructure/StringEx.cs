using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Abilitics.SearchPoint.Engine.Infrastructure
{
    internal static class StringEx
    {
        private static readonly Regex smartFormatRegexPattern = new Regex(
            @"(\{+)([^\}]+)(\}+)", RegexOptions.Compiled);

        public static StringBuilder AppendFormatInvariant(
            this StringBuilder builder, string format, params object[] parameters)
        {
            return builder.AppendFormat(CultureEx.Invariant, format, parameters);
        }

        public static string Fmt(this string target, params object[] parameters)
        {
            return string.Format(CultureEx.Invariant, target, parameters);
        }

        public static string SmartFormat(this string pattern, object template)
        {
            Type type = template.GetType();
            var cache = new Dictionary<string, string>();
            string replacedText = smartFormatRegexPattern.Replace(pattern, match =>
            {
                int leftCount = match.Groups[1].Value.Length;
                int rightCount = match.Groups[3].Value.Length;

                if ((leftCount % 2) != (rightCount % 2))
                    throw new InvalidOperationException("Unbalanced braces");

                string leftBrace = leftCount == 1 ? "" : new string('{', leftCount / 2);
                string rightBrace = rightCount == 1 ? "" : new string('}', rightCount / 2);
                string key = match.Groups[2].Value;
                string value;

                if (leftCount % 2 == 0)
                {
                    value = key;
                }
                else
                {
                    if (!cache.TryGetValue(key, out value))
                    {
                        var prop = type.GetProperty(key);
                        if (prop == null)
                            throw new ArgumentException("pattern", "Not found: " + key);

                        value = Convert.ToString(prop.GetValue(template, null), CultureEx.Invariant);
                        cache.Add(key, value);
                    }
                }
                return leftBrace + value + rightBrace;
            });

            return replacedText;
        }

        public static bool IsNumeric(this string target)
        {
            return Regex.IsMatch(target, "^[0-9]+$");
        }

        public static bool IsAlphanumeric(this string target)
        {
            return Regex.IsMatch(target, "^[a-zA-Z0-9]+$");
        }

        public static bool Contains(this string original,
                                    string value,
                                    StringComparison comparisonType)
        {
            return original.IndexOf(value, comparisonType) >= 0;
        }

        public static bool ContainsOrdinal(this string original, string value)
        {
            return original.IndexOf(value, StringComparison.Ordinal) >= 0;
        }

        public static bool ContainsOrdinalNoCase(this string original, string value)
        {
            return original.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool EqualsOrdinalNoCase(this string original, string value)
        {
            return original.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        public static int LastIndexOfOrdinal(this string original, string value)
        {
            return original.LastIndexOf(value, StringComparison.Ordinal);
        }

        public static string Replace(this string original,
                                     string oldValue,
                                     string newValue,
                                     StringComparison comparisonType)
        {
            string result = original;

            if (!string.IsNullOrEmpty(newValue))
            {
                int index = -1;
                int lastIndex = 0;

                var buffer = new StringBuilder();

                while ((index = original.IndexOf(oldValue, index + 1, comparisonType)) >= 0)
                {
                    buffer.Append(original, lastIndex, index - lastIndex);
                    buffer.Append(newValue);

                    lastIndex = index + oldValue.Length;
                }
                buffer.Append(original, lastIndex, original.Length - lastIndex);

                result = buffer.ToString();
            }
            return result;
        }

        public static string JoinStrings<T>(this IEnumerable<T> source,
                                                Func<T, string> projection, string separator)
        {
            var builder = new StringBuilder();
            bool first = true;
            foreach (T element in source)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(separator);
                }
                builder.Append(projection(element));
            }
            return builder.ToString();
        }

        public static string JoinStrings<T>(this IEnumerable<T> source, string separator)
        {
            return JoinStrings(source, t => t.ToString(), separator);
        }

        public static bool IsNullOrEmpty(this string argument)
        {
            return string.IsNullOrEmpty(argument);
        }

        public static bool IsNotNullOrEmpty(this string argument)
        {
            return !string.IsNullOrEmpty(argument);
        }

        public static bool IsWhitespace(this string argument)
        {
            return Regex.IsMatch(argument, @"^(\s)*$");
        }

        public static string[] GetWords(this string camelCaseWord)
        {
            return Regex.Split(camelCaseWord, @"\W+");
        }

        public static string GetFileName(this string path)
        {
            int lastIndexOfSlash = path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
            if (lastIndexOfSlash == -1)
                return null;

            string filename = path.Substring(lastIndexOfSlash + 1);

            if (filename.Contains("."))
                return filename;
            else
                return null;
        }

        public static string Slice(this string self, int start, int end)
        {
            if (start < 0 || start >= self.Length)
                throw new ArgumentOutOfRangeException("start");

            if (end < 0)
                end += self.Length + 1;

            if (end < start || end > self.Length)
                throw new ArgumentOutOfRangeException("end");

            return self.Substring(start, end - start);
        }

        public static bool IsNullOrWhitespace(this string self)
        {
            return (self == null || self.Trim() == "");
        }

        public static string CollapseWhitespace(this string self)
        {
            return Regex.Replace(self, @"\s+", " ");
        }

        public static string EmptyIfNull(this string self)
        {
            if (self == null)
                return "";
            else
                return self;
        }

        public static bool StartsWithOrdinal(this string self, string value)
        {
            return self.StartsWith(value, StringComparison.Ordinal);
        }

        public static bool StartsWithOrdinalNoCase(this string self, string value)
        {
            return self.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithOrdinal(this string self, string value)
        {
            return self.EndsWith(value, StringComparison.Ordinal);
        }

        public static bool EndsWithOrdinalNoCase(this string self, string value)
        {
            return self.EndsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        public static string Reverse(this string self)
        {
            if (self.IsNullOrEmpty())
                return self;

            var reversed = self.ToCharArray();
            Array.Reverse(reversed);
            return new string(reversed);
        }
    }
}