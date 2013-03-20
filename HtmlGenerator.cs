using System;
using BabylonToHtml.BabylonReader;
using System.IO;

namespace BabylonToHtml
{
    class HtmlGenerator
    {
        internal static void GenerateHtml(XDict dict, string outputPath)
        {
            StreamWriter outFileStream = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);
            int numOfEntries = dict.Entries.Count;
            int curEntry = 1;

            int top = Console.CursorTop;
            int left = Console.CursorLeft;

            //header
            outFileStream.WriteLine("<html>\n\t<head>\n\t\t<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\" />\n\t\t<title>Babylon Dictionary</title>\n\t</head>\n\t<body>");

            foreach(XDictEntry entry in dict.Entries)
            {
                Console.SetCursorPosition(left, top);
                Console.Write("Entry #{0}/{1} ({2})...", curEntry.ToString(), numOfEntries.ToString(), (double.Parse(curEntry.ToString()) / double.Parse(numOfEntries.ToString())).ToString("P"));
                curEntry++;

                outFileStream.WriteLine("\t\t<idx:entry name=\"dic\">");

                outFileStream.WriteLine("\t\t\t\t<idx:orth>" + entry.Word.Text);

                if (entry.Comments.Count > 0)
                {
                    outFileStream.WriteLine("\t\t\t\t<idx:infl>");

                    foreach (XWordInfo comment in entry.Comments)
                    {
                        if (!string.IsNullOrWhiteSpace(comment.Text))
                            outFileStream.WriteLine("\t\t\t\t\t<idx:iform value=\"" + comment.Text + "\" />");
                    }
                    outFileStream.WriteLine("\t\t\t\t</idx:infl>");
                }

                outFileStream.WriteLine("\t\t\t</idx:orth>");
                foreach (XWordInfo definition in entry.Definitions)
                {
                    outFileStream.WriteLine("\t\t\t<p><ul><li><blockquote>" + definition.Text + "</blockquote></li></ul></p>");
                }
                outFileStream.WriteLine("\t\t</idx:entry>");
            }
            outFileStream.WriteLine("\t</body>\n</html>");
            outFileStream.Close();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n\nGenerated HTML file \"{0}\".", Path.GetFileName(outputPath), numOfEntries);
        }
    }
}
