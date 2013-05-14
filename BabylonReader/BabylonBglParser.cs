using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BabylonToHtml.SharpZipLib;

namespace BabylonToHtml.BabylonReader
{
    class BabylonBglParser //: DictParser
    {
        internal Encoding SrcEncoding { get; private set; }
        internal Encoding DstEncoding { get; private set; }

        static string[] PartOfSpeech = new string[]{
          "n",
          "adj",
          "v",
          "adv",
          "interj",
          "pron",
          "prep",
          "conj",
          "suff",
          "pref",
          "art" };

        public BabylonBglParser()
        {
        }

        public BabylonBglParser(Encoding srcEncoding, Encoding dstEncoding)
        {
            SrcEncoding = srcEncoding;
            DstEncoding = dstEncoding;
        }

        protected void FillHeader(XDict dict)
        {
            //dict.Source = "BCL: " + _bclName;
            //dict.Url = @"http://www.babylon.com/define/122/English-Thai-Dictionary.html";
            //dict.Comment = "License unknown. Content is user created, and the website screams 'free dictionary', but BCL format is closed, no public spec.";
        }

        public XDict Parse(string file)
        {
            XDict dict = new XDict();
            FillHeader(dict);

            // Read
            using (Stream s = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] buf = new byte[6];
                int pos = s.Read(buf, 0, buf.Length);

                // First four bytes: BGL signature 0x12340001 or 0x12340002 (big-endian) 
                if (pos < 6 ||
                    (buf[0] == 0x12 && buf[1] == 0x34 && buf[2] == 0x00 &&
                        (buf[4] == 0x01 || buf[4] == 0x02)))
                {
                    throw new FileLoadException("Invalid file: no BGL signature: " + file);
                }

                int gzipHeaderPos = buf[4] << 8 | buf[5];
                if (gzipHeaderPos < 6) { throw new FileLoadException("No gzip ptr"); }

                s.Seek(gzipHeaderPos, SeekOrigin.Begin);
                parseMetaData(new GZipInputStream(s));

                if (null == this.SrcEncoding && string.IsNullOrWhiteSpace(this.SrcEnc))
                {
                    throw new InvalidDataException("Failed to detect source encoding in BGL file. Please provide encoding via proper constructor.");
                }
                else if (null == this.SrcEncoding)
                {
                    // if SrcEncoding not set at command line
                    this.SrcEncoding = Encoding.GetEncoding(this.SrcEnc);
                }
                if (null == this.DstEncoding && string.IsNullOrWhiteSpace(this.DstEnc))
                {
                    throw new InvalidDataException("Failed to detect destination encoding in BGL file. Please provide encoding via proper constructor.");
                }
                else if (null == this.DstEncoding)
                {
                    // if DstEncoding not set at command line
                    this.DstEncoding = Encoding.GetEncoding(this.DstEnc);
                }

                s.Seek(gzipHeaderPos, SeekOrigin.Begin);
                ParseUnzipped(new GZipInputStream(s), dict);
            }

            return dict;
        }

        //----------------------------------------------
        public string DefCharset { get; set; }
        public string SrcEnc { get; set; }
        public string SrcEncName { get; set; }
        public string DstEnc { get; set; }
        public string DstEncName { get; set; }
        public string SrcLng { get; set; }
        public string DstLng { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        //----------------------------------------------

        void parseMetaData(Stream s)
        {
            string headword = string.Empty;
            int type = -1;
            while (true)
            {
                headword = string.Empty;
                BglBlock block = BglBlock.Read(s);
                if (block == null) { break; }

                if (block.Type == 0 && block.Data[0] == 8)
                {
                    type = block.Data[1];
                    if (type > 64) type -= 65;
                    this.DefCharset = BabylonConsts.Bgl_charset[type];
                }
                else if (block.Type == 3)
                {
                    int pos = 2;
                    switch (block.Data[1])
                    {
                        case 1:
                            for (int a = 0; a < block.Length - 2; a++) headword += (char)block.Data[pos++];
                            this.Title = headword;
                            break;
                        case 2:
                            for (int a = 0; a < block.Length - 2; a++) headword += (char)block.Data[pos++];
                            this.Author = headword;
                            break;
                        case 7:
                            this.SrcLng = BabylonConsts.Bgl_language[block.Data[5]];
                            break;
                        case 8:
                            this.DstLng = BabylonConsts.Bgl_language[block.Data[5]];
                            break;
                        case 26:
                            type = block.Data[2];
                            if (type > 64) type -= 65;
                            this.SrcEnc = BabylonConsts.Bgl_charset[type];
                            this.SrcEncName = BabylonConsts.Bgl_charsetname[type];
                            break;
                        case 27:
                            type = block.Data[2];
                            if (type > 64) type -= 65;
                            this.DstEnc = BabylonConsts.Bgl_charset[type];
                            this.DstEncName = BabylonConsts.Bgl_charsetname[type];
                            break;
                    }
                }
                else
                    continue;
            }
        }

        void ParseUnzipped(Stream s, XDict dict)
        {
            while (true)
            {
                BglBlock block = BglBlock.Read(s);
                if (block == null) { break; }

                if (block.Type == 1 || block.Type == 10)
                {
                    XDictEntry e = ParseEntry(block);
                    dict.AddEntry(e);
                }
            }
        }

        XDictEntry ParseEntry(BglBlock block)
        {
            int len = 0;
            int pos = 0;
            // Head
            len = block.Data[pos++];
            String headWord = block.GetString(pos, len, SrcEncoding);
            pos += len;

            // Definition
            len = block.Data[pos++] << 8 | block.Data[pos++];
            String def = block.GetDefString(pos, len, DstEncoding);
            pos += len;

            // Alternates
            List<string> alternates = new List<string>();
            while (pos < block.Length)
            {
                len = block.Data[pos++];
                String alternate = block.GetString(pos, len, SrcEncoding);
                pos += len;
                alternates.Add(alternate);
            }

            return CreateEntry(headWord, def, alternates);
        }

        /// <summary>
        /// Override to provide custom parsing of BGL fields, e.g.
        /// break up into multiple definitions, PartOfSpeech extraction etc.
        /// 
        /// Different BGL sources may use different conventions.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="def"></param>
        /// <param name="alternates"></param>
        /// <returns></returns>
        protected virtual XDictEntry CreateEntry(String head, String def, List<String> alternates)
        {
            XDictEntry entry = new XDictEntry(new XWordInfo(head));

            // TODO: Break up into multiple defs, PartOfSpeech parsing etc.
            if (!String.IsNullOrEmpty(def))
            {
                XWordInfo defWi = new XWordInfo(def);
                entry.Definitions.Add(defWi);
            }

            foreach (String alt in alternates)
            {
                // These are odd. In one of the sources, 
                // for "Austen", alt is "Jane" for "hang", it's "draw", "and quarter"
                // Might depend on the source.
                XWordInfo wi = new XWordInfo(alt);
                entry.Comments.Add(wi);
            }

            return entry;
        }

        protected XDictEntry ParseEntry(StreamReader sr)
        {
            throw new NotSupportedException();
        }

        static int ReadNum(Stream s, int bytes)
        {
            byte[] buf = new byte[4];
            if (bytes < 1 || bytes > 4) { throw new ArgumentException("Must be between 1 and 4", "bytes"); }
            s.Read(buf, 0, bytes);

            int val = 0;
            for (int i = 0; i < bytes; i++) { val = (val << 8) | buf[i]; }
            return val;
        }

        class BglBlock
        {
            public int Type;
            public int Length;
            public byte[] Data = new byte[0];

            public static BglBlock Read(Stream s)
            {
                BglBlock block = new BglBlock();
                block.Length = BabylonBglParser.ReadNum(s, 1);
                block.Type = block.Length & 0xF;
                if (block.Type == 4) { return null; } // end-of-file marker
                block.Length >>= 4;
                block.Length = block.Length < 4 ?
                    BabylonBglParser.ReadNum(s, block.Length + 1) : block.Length - 4;

                if (block.Length > 0)
                {
                    block.Data = new byte[block.Length];
                    s.Read(block.Data, 0, block.Data.Length);
                }
                return block;
            }

            public override string ToString()
            {
                return "BglBlock: t=" + Type + " len=" + Length + "\r\n"
                  + "---" + Encoding.ASCII.GetString(Data) + "\r\n";
            }

            public String GetString(int offset, int length, Encoding enc)
            {
                MemoryStream ms = new MemoryStream();
                ms.Write(Data, offset, length);
                return GetString(ms, enc);
            }

            // TODO: not tested in full
            public String GetDefString(int offset, int length, Encoding enc)
            {
                MemoryStream ms = new MemoryStream();

                for (int i = offset; i < offset + length; i++)
                {
                    byte b = Data[i];
                    if (b == 0x0a) { ms.WriteByte((byte)'\n'); }
                    else if (Data[i] < 0x20)
                    {
                        if (i < (offset + length - 2) &&
                            Data[i] == 0x14 && Data[i + 1] == 0x12)
                        {
                            int posIndex = Data[i + 2] - 0x30;
                            String posS = "/POS:" + BabylonBglParser.PartOfSpeech[posIndex] + ". ";

                            byte[] posBytes = Encoding.ASCII.GetBytes(posS);
                            ms.Write(posBytes, 0, posBytes.Length);
                        }
                        i += 2;
                    }
                    else
                    {
                        ms.WriteByte(b);
                    }
                }

                return GetString(ms, enc);
            }

            static String GetString(MemoryStream ms, Encoding enc)
            {
                ms.Seek(0, SeekOrigin.Begin);

                // Thai by default
                using (StreamReader sr = new StreamReader(ms, enc))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}
