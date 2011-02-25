using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SocialInsight
{
    public static class Arg
    {
        public static void ThrowIfNull(object argument, string name)
        {
            if (argument == null)
                throw new ArgumentNullException(name);
        }

        public static void ThrowInvalidIfNull(object argument, string message)
        {
            if (argument == null)
                throw new InvalidOperationException(message);
        }

        public static void ThrowInvalidIfEmpty(string argument, string message)
        {
            if (string.IsNullOrEmpty(argument))
                throw new InvalidOperationException(message);
        }

        public static void ThrowInvalidIfEmpty<T>(IEnumerable<T> collection, string errorMessage)
        {
            if (collection == null || !collection.Any())
                throw new InvalidOperationException(errorMessage);
        }

        public static void ThrowInvalidIfEmptyCollection(ICollection collection, string errorMessage)
        {
            if (collection == null || collection.Count == 0)
                throw new InvalidOperationException(errorMessage);
        }

        public static void ThrowIfNegative(int argument, string name)
        {
            if (argument < 0)
                throw new ArgumentOutOfRangeException(name);
        }

        public static void ThrowIfNonPositive(int argument, string name)
        {
            if (argument <= 0)
                throw new ArgumentOutOfRangeException(name);
        }

        public static void ThrowIfOutOfRange(int argument, string name, int minValue, int maxValue)
        {
            if (argument > maxValue || argument < minValue)
                throw new ArgumentException(
                    "The value must be between {0} and {1}".Fmt(minValue, maxValue), name);
        }

        public static void ThrowIfNoValue<T>(Nullable<T> argument, string name)
            where T : struct
        {
            if (!argument.HasValue)
                throw new ArgumentNullException(name);
        }

        public static void ThrowIfEmpty(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument))
                throw new ArgumentNullException(name);
        }

        public static void ThrowIfEmpty<T>(IEnumerable<T> argument, string name)
        {
            if (argument == null || !argument.Any())
                throw new ArgumentException("Collection is null or empty", name);
        }
    }
}
