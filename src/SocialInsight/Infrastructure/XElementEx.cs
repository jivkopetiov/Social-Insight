using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Abilitics.SearchPoint.Engine.Infrastructure
{
    internal static class XElementEx
    {
        public static string ToString(this XElement element, XmlWriterSettings settings)
        {
            var builder = new StringBuilder();

            using (var writer = XmlWriter.Create(builder, settings))
                element.WriteTo(writer);

            return builder.ToString();
        }

        public static string GetSubElementValue(this XElement top, XName subName)
        {
            if (top == null)
                throw new ArgumentException(
                    "top",
                    "Top element searching the element {0} is empty".Fmt(subName));

            var element = top.Element(subName);
            if (element == null)
                throw new ArgumentException(
                            "subName", "Element with the name {0} is missing.".Fmt(subName));

            return element.Value;
        }

        public static string GetSubElementValueNoThrow(this XElement top, XName subName)
        {
            if (top == null)
                return null;

            var element = top.Element(subName);
            if (element == null)
                return null;

            return element.Value;
        }

        public static int GetSubElementNumber(this XElement top, XName subName)
        {
            if (top == null)
                throw new ArgumentException(
                            "top", "Top element searching the element {0} is empty".Fmt(subName));

            var element = top.Element(subName);
            if (element == null)
                throw new ArgumentException(
                            "subName", "Element with the name {0} is missing.".Fmt(subName));

            int val = 0;
            bool parseSuccess = int.TryParse(element.Value, out val);
            if (!parseSuccess)
                throw new ArgumentException(
                    "subName",
                    "Element with the name {0} does not contain an integer value."
                    .Fmt(subName));

            return val;
        }

        public static int GetSubElementNumberNoThrow(this XElement top, XName subName)
        {
            if (top == null)
                return 0;

            var element = top.Element(subName);
            if (element == null)
                return 0;

            int val = 0;
            bool parseSuccess = int.TryParse(element.Value, out val);
            if (!parseSuccess)
                throw new ArgumentException(
                    "subName",
                    "Element with the name {0} does not contain an integer value."
                    .Fmt(subName));

            return val;
        }

        public static string GetSubSubElementValue(this XElement top, XName first, XName second)
        {
            var firstElement = top.Element(first);

            if (firstElement == null)
                throw new ArgumentException(
                                "Element with the name {0} is missing.".Fmt(first));

            return firstElement.GetSubElementValue(second);
        }

        public static string GetSubSubElementValueNoThrow(this XElement top, XName first, XName second)
        {
            var firstElement = top.Element(first);

            if (firstElement == null)
                return null;

            return firstElement.GetSubElementValueNoThrow(second);
        }

        public static Uri GetSubElementUrl(this XElement top, XName subName)
        {
            if (top == null)
                throw new ArgumentException(
                                "Top element searching the element {0} is empty".Fmt(subName));

            var element = top.Element(subName);
            if (element == null || string.IsNullOrEmpty(element.Value))
                throw new ArgumentException(
                                "Element with the name {0} is missing.".Fmt(subName));

            return new Uri(element.Value);
        }

        public static Uri GetSubElementUrlNoThrow(this XElement top, XName subName)
        {
            if (top == null)
                return null;

            var element = top.Element(subName);
            if (element == null || string.IsNullOrEmpty(element.Value))
                return null;

            return new Uri(element.Value);
        }

        public static string GetSubElementAttribute(this XElement top, XName subName, XName attributeName)
        {
            if (top == null)
                throw new ArgumentException(
                                "Top element searching the element {0} is empty".Fmt(subName));

            var element = top.Element(subName);
            if (element == null)
                throw new ArgumentException(
                                "Element with the name {0} is missing.".Fmt(subName));

            var attribute = element.Attribute(attributeName);
            if (attribute == null)
                throw new ArgumentException(
                                "Attribute with the name {0} is missing.".Fmt(attributeName));

            return attribute.Value;
        }

        public static string GetSubElementAttributeNoThrow(this XElement top, XName subName, XName attributeName)
        {
            if (top == null)
                return null;

            var element = top.Element(subName);
            if (element == null)
                return null;

            var attribute = element.Attribute(attributeName);
            if (attribute == null)
                return null;

            return attribute.Value;
        }

        public static string GetAttributeValue(this XElement top, XName attributeName)
        {
            if (top == null)
                throw new ArgumentException(
                                "top",
                                "Top element searching the attribute {0} is empty".Fmt(attributeName));

            var attribute = top.Attribute(attributeName);
            if (attribute == null)
                throw new ArgumentException(
                                  "attributeName",
                                  "Attribute with the name {0} is missing in {1}."
                                  .Fmt(attributeName, top.Name));

            return attribute.Value;
        }

        public static string GetAttributeValueNoThrow(this XElement top, XName attributeName)
        {
            if (top == null)
                return null;

            var attribute = top.Attribute(attributeName);
            if (attribute == null)
                return null;

            return attribute.Value;
        }

        public static Uri GetAttributeAsUri(this XElement top, XName attributeName)
        {
            if (top == null)
            {
                throw new ArgumentException(
                            "attributeName",
                            "Top element searching the attribute {0} is empty"
                            .Fmt(attributeName));
            }

            var attribute = top.Attribute(attributeName);
            if (attribute == null)
            {
                throw new ArgumentException(
                                  "attributeName",
                                  "Attribute with the name {0} is missing in {1}."
                                  .Fmt(attributeName, top.Name));
            }

            Uri url = null;
            bool parseSuccess = Uri.TryCreate(attribute.Value, UriKind.Absolute, out url);

            if (!parseSuccess)
            {
                throw new ArgumentException(
                                  "attributeName",
                                  "Attribute with the name {0} is not an url in {1}."
                                  .Fmt(attributeName, top.Name));
            }

            return url;
        }

        public static Uri GetAttributeAsUriNoThrow(this XElement top, XName attributeName)
        {
            if (top == null)
                return null;

            var attribute = top.Attribute(attributeName);
            if (attribute == null)
                return null;

            Uri url = null;
            bool parseSuccess = Uri.TryCreate(attribute.Value, UriKind.Absolute, out url);

            if (!parseSuccess)
                return null;

            return url;
        }

        public static int GetAttributeAsInt(this XElement top, XName attributeName)
        {
            if (top == null)
                throw new ArgumentException("attributeName", "Top element searching the attribute {0} is empty".Fmt(attributeName));

            var attribute = top.Attribute(attributeName);
            if (attribute == null)
                throw new ArgumentException("attributeName", "Attribute with the name {0} is missing in {1}.".Fmt(attributeName, top.Name));

            int result = 0;

            bool parseSuccess = int.TryParse(attribute.Value.Trim(), out result);

            if (!parseSuccess)
                throw new ArgumentException("attributeName", "Attribute with the name {0} is not an integer in {1}.".Fmt(attributeName, top.Name));

            return result;
        }

        public static int? GetAttributeAsIntNoThrow(this XElement top, XName attributeName)
        {
            if (top == null)
                return null;

            var attribute = top.Attribute(attributeName);
            if (attribute == null)
                return null;

            int result = 0;

            bool parseSuccess = int.TryParse(attribute.Value.Trim(), out result);

            if (!parseSuccess)
                return null;

            return result;
        }

        public static double GetAttributeAsDouble(this XElement top, XName attributeName)
        {
            if (top == null)
                throw new ArgumentException("attributeName", "Top element searching the attribute {0} is empty".Fmt(attributeName));

            var attribute = top.Attribute(attributeName);
            if (attribute == null)
                throw new ArgumentException("attributeName", "Attribute with the name {0} is missing in {1}.".Fmt(attributeName, top.Name));

            double result = 0;

            bool parseSuccess = double.TryParse(attribute.Value.Trim(), NumberStyles.Any, CultureEx.Invariant, out result);

            if (!parseSuccess)
                throw new ArgumentException("attributeName", "Attribute with the name {0} is not a double in {1}.".Fmt(attributeName, top.Name));

            return result;
        }

        public static double? GetAttributeAsDoubleNoThrow(this XElement top, XName attributeName)
        {
            if (top == null)
                return null;

            var attribute = top.Attribute(attributeName);
            if (attribute == null)
                return null;

            double result = 0;

            bool parseSuccess = double.TryParse(attribute.Value.Trim(), NumberStyles.Any, CultureEx.Invariant, out result);

            if (!parseSuccess)
                return null;

            return result;
        }
    }
}
