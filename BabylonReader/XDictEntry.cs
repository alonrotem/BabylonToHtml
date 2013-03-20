using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BabylonToHtml.BabylonReader
{
    /// <summary>
    /// Single entry in the dictionary. Contains a head word and various information
    /// about it (e.g. POS, Translations, Synonyms etc.)
    /// </summary>
    public class XDictEntry : IComparable
    {
        [XmlElement]
        public XWordInfo Word;

        // Index of the source header of the entry
        [XmlAttribute("src")]
        public int SourceIndex = 0;

        // Important elements
        [XmlElement("Translation")]
        public List<XWordInfo> Translations = new List<XWordInfo>();

        [XmlElement("Definition")]
        public List<XWordInfo> Definitions = new List<XWordInfo>();

        // Skip serialization of SourceIndex on default value
        [XmlIgnore]
        public bool SourceIndexSpecified
        {
            get { return SourceIndex != 0; }
            set { /* empty */ }
        }

        // Less important elements
        [XmlElement("Synonym")]
        public List<XWordInfo> Synonyms = new List<XWordInfo>();

        [XmlElement("Antonym")]
        public List<XWordInfo> Antonyms = new List<XWordInfo>();

        [XmlElement("Sample")]
        public List<XWordInfo> Samples = new List<XWordInfo>();

        [XmlElement("Comment")]
        public List<XWordInfo> Comments = new List<XWordInfo>();

        [XmlElement("Classifier")] // Thai classifier (khon, tua, an etc.)
        public String Classifier;



        public XDictEntry() { }

        public XDictEntry(XWordInfo word)
        {
            Word = word;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Word.ToString());

            AppendWords(sb, "-->", Translations);
            AppendWords(sb, "def", Definitions);
            AppendWords(sb, "syn", Synonyms);
            AppendWords(sb, "ant", Antonyms);
            AppendWords(sb, "sam", Samples);
            AppendWords(sb, "com", Comments);
            if (!String.IsNullOrEmpty(Classifier)) { sb.AppendLine(" num: " + Classifier); }
            return sb.ToString();
        }

        public XWordInfo FirstTranslation
        {
            get
            {
                return null;
            }
        }

        public string ToShortString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Word.Text);
            if (Word.PartOfSpeechSpeficied) { sb.Append("/" + Word.PartOfSpeech); }

            XWordInfo wi = FirstTranslation;
            if (wi != null) { sb.Append(" -> " + wi.Text); }

            return sb.ToString();
        }


        void AppendWords(StringBuilder sb, String cat, List<XWordInfo> list)
        {
            foreach (XWordInfo s in list)
            {
                sb.AppendLine(" " + cat + ": " + s);
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            XDictEntry that = obj as XDictEntry;
            if (that == null) { return -1; }
            return this.Word.CompareTo(that.Word);
        }

        #endregion
    }
}
