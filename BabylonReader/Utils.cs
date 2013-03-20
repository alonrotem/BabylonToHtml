using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace BabylonToHtml.BabylonReader
{
    public static class Utils
    {
        public static Encoding ThaiEncoding
        {
            get { return Encoding.GetEncoding("TIS-620"); }
        }

        /// <summary>
        /// Split the string in two parts at the first occurence of the separator.
        /// </summary>
        /// <param name="source">String to split</param>
        /// <param name="separator">Separator to use</param>
        /// <param name="first">Part before the separator, or full string if no separator</param>
        /// <param name="second">Part after the separator, String.Empty if no separator occurs</param>
        /// <returns>True if string was split, false otherwise (no separator occurs)</returns>
        public static bool SplitInTwo(String source, String separator, out String first, out String second)
        {
            first = source;
            second = String.Empty;

            int sepPos = source.IndexOf(separator);
            if (sepPos < 0) { return false; }
            first = source.Substring(0, sepPos);
            second = source.Substring(sepPos + separator.Length);
            return true;
        }

        /// <summary>
        /// Return the string rep of the first item in a collection, empty string if collection is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static String FirstToString<T>(List<T> items)
        {
            T obj;
            if (TryGetFirst(items, out obj)) { return obj.ToString(); }
            return String.Empty;
        }

        /// <summary>
        /// Try to get the first item from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryGetFirst<T>(IList<T> items, out T val)
        {
            val = default(T);
            if (items == null || items.Count == 0) { return false; }
            val = items[0];
            return true;
        }

        public static void SaveXml<T>(String file, T obj)
        {
            Console.WriteLine("Saving " + typeof(T).Name + " to " + file);

            XmlSerializer x = new XmlSerializer(typeof(T));
            XmlWriterSettings s = new XmlWriterSettings();
            s.Encoding = Encoding.UTF8;
            s.Indent = true;
            using (XmlWriter w = XmlWriter.Create(file, s))
            {
                x.Serialize(w, obj);
            }
        }
        public static T LoadXml<T>(String file)
            where T : class
        {
            Console.WriteLine("Loading " + typeof(T).Name + " from " + file);

            XmlSerializer x = new XmlSerializer(typeof(T));
            using (XmlReader r = XmlReader.Create(file))
            {
                return x.Deserialize(r) as T;
            }
        }

        public static void SaveAsTxt(List<String> words, String outFile, bool useThaiEncoding)
        {
            Encoding encoding = Encoding.UTF8;
            if (useThaiEncoding) { encoding = ThaiEncoding; }

            using (TextWriter tw = new StreamWriter(outFile, false, encoding))
            {
                foreach (String word in words)
                {
                    tw.WriteLine(word);
                }
            }
        }
    }
}
