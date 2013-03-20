using System;
using System.Xml.Serialization;

namespace BabylonToHtml.BabylonReader
{
    public class XWordInfo : IComparable
    {
        [XmlAttribute("w")]
        public String Text;

        [XmlAttribute("pos")]
        public String PartOfSpeech;

        [XmlIgnore]
        public bool PartOfSpeechSpeficied
        {
            get { return String.IsNullOrEmpty(PartOfSpeech); }
            set { } // framework bug
        }

        public XWordInfo() { } // XML

        public XWordInfo(String text) : this(text, null) { }

        public XWordInfo(String text,  String partOfSpeech)
        {
            Text = text;
            PartOfSpeech = partOfSpeech;
        }


        public override string ToString()
        {
            return Text + (String.IsNullOrEmpty(PartOfSpeech) ? "" : "/" + PartOfSpeech);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            XWordInfo that = obj as XWordInfo;
            if (that == null) { return -1; }

            return this.Text.CompareTo(that.Text);
        }

        #endregion
    }
}
