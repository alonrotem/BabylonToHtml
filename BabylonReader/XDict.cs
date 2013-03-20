using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BabylonToHtml.BabylonReader
{
    /// <summary>
    /// Dictionary
    /// </summary>
    public class XDict
    {
        [XmlArray("Sources")]
        public List<XDictHeader> Headers = new List<XDictHeader>();

        /// <summary>
        /// Entries for each word.
        /// </summary>
        /// <remarks>Do not access directly, use AddEntry instead.</remarks>
        [XmlArray("Words")]
        public List<XDictEntry> Entries = new List<XDictEntry>();

        [XmlIgnore]
        MultiDictionary<String, XDictEntry> _index = new MultiDictionary<string, XDictEntry>();

        public XDict() { } // XML

        /// <summary>
        /// Load from an XML file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static XDict Load(String file)
        {
            XDict d = Utils.LoadXml<XDict>(file);
            d.Reindex();
            return d;
        }

        /// <summary>
        /// Add an entry to the dictionary and index
        /// </summary>
        /// <param name="entry"></param>
        public void AddEntry(XDictEntry entry)
        {
            Entries.Add(entry);
            _index.Add(entry.Word.Text, entry);
        }

        /// <summary>
        /// Get the entry for the given word
        /// </summary>
        /// <param name="wordText"></param>
        /// <returns></returns>
        public List<XDictEntry> this[String wordText]
        {
            get { return _index[wordText]; }
        }

        void Reindex()
        {
            _index.Clear();
            foreach (XDictEntry e in Entries)
            {
                _index.Add(e.Word.Text, e);
            }
        }

        /// <summary>
        /// Save to an XML file
        /// </summary>
        /// <param name="file"></param>
        public void Save(String file)
        {
            Entries.Sort();
            Utils.SaveXml<XDict>(file, this);
        }

        /// <summary>
        /// Get unique words in the given language. Pass null to get words in any language.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public List<String> GetUniqueWords(String lang)
        {
            Set<String> uniqueWords = new Set<string>();
            foreach (XDictEntry entry in this.Entries)
            {
                uniqueWords.Add(entry.Word.Text);
            }
            List<String> wl = new List<string>(uniqueWords);
            wl.Sort();
            return wl;
        }
    }
}
