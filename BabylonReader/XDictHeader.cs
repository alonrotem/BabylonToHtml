using System;

namespace BabylonToHtml.BabylonReader
{
    public class XDictHeader
    {
        public String Source;
        public String Url;
        public String Comment;

        public XDictHeader() { }

        public XDictHeader(String source, String url, String comment)
        {
            Source = source;
            Url = url;
            Comment = comment;
        }
    }
}
