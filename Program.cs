using System.Text;
using BabylonToHtml.BabylonReader;
using System;
using System.IO;

namespace BabylonToHtml
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Dictionary Babylon -> HTML v2.0");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Alon Rotem, 2012\n");

            Encoding sourceEnc = null;
            Encoding targetEnc = null;
            string fileName = string.Empty;

            for (int arg = 0; arg < args.Length; arg++)
            {
                if (args[arg].ToUpper().Trim() == "-SE")
                {
                    if (args.Length > arg + 1)
                    {
                        try
                        {
                            sourceEnc = System.Text.Encoding.GetEncoding(args[arg + 1]);
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("\nError interpreting encoding numerical code \"" + args[arg + 1] + "\": ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(ex.Message + "\n");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        arg++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nWarning: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("-se argument requested but no valid encoding numerical code given.\n");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }

                else if (args[arg].ToUpper().Trim() == "-TE")
                {
                    if (args.Length > arg + 1)
                    {
                        try
                        {
                            targetEnc = System.Text.Encoding.GetEncoding(args[arg + 1]);
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("\nError interpreting encoding numerical code \"" + args[arg + 1] + "\": ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(ex.Message + "\n");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        arg++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\nWarning: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("-te argument requested but no valid encoding numerical code given.\n");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }

                else
                {
                    fileName = args[arg];
                    if (!File.Exists(fileName))
                    {
                        //input file doesn't exist
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Error: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("File \"" + fileName + "\" not found.\n\n");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Help();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                //input file doesn't exist
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("No input file provided.\n\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                Help();
            }
            else
            {
                string outputFile = Path.GetFullPath(Path.GetFileNameWithoutExtension(fileName)) + ".html";

                WriteKeyValue("Path", Path.GetFullPath(fileName) + "\n", "", 0);
                WriteKeyValue("Input file", fileName);
                WriteKeyValue("Output file", Path.GetFileName(outputFile) + "\n");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Processing input...\n\n");

                BabylonBglParser parser;
                parser = new BabylonBglParser(sourceEnc, targetEnc);
                XDict dict = parser.Parse(fileName);

                WriteKeyValue("Dictionary title", parser.Title);
                WriteKeyValue("Author", parser.Author);
                WriteKeyValue("Languages", parser.SrcLng + " -> " + parser.DstLng + "");
                if (sourceEnc == null)
                {
                    if (!string.IsNullOrWhiteSpace(parser.SrcEnc))
                        WriteKeyValue("Source encoding", parser.SrcEnc + " (" + parser.SrcEncName + ")","*detected");
                    else
                        WriteKeyValue("Source encoding", "Failed to detect!");
                }
                else
                {
                    WriteKeyValue("Source encoding", sourceEnc.WebName + " (" + sourceEnc.EncodingName + ")", "*provided by user");
                }

                if (targetEnc == null)
                {
                    if (!string.IsNullOrWhiteSpace(parser.DstEnc))
                        WriteKeyValue("Target encoding", parser.DstEnc + " (" + parser.DstEncName + ")","*detected\n");
                    else
                        WriteKeyValue("Target encoding","", "Failed to detect!\n");
                }
                else
                {
                    WriteKeyValue("Target encoding", targetEnc.WebName + " (" + targetEnc.EncodingName + ")", "*provided by user\n");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Generating output...\n");

                Console.ForegroundColor = ConsoleColor.Yellow;
                HtmlGenerator.GenerateHtml(dict, outputFile);
            }
            Console.ResetColor();
        }

        static void WriteKeyValue(string key, string value, string remark = "", int indent = 18)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("{0,-"+indent+"}", key + ": ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0}", value);

            if (string.IsNullOrWhiteSpace(remark))
                Console.Write("\n");
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" {0}\n", remark);
            }
        }

        static void Help()
        {
            #region Console help

            string thisExe = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Usage: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("  {0} ", thisExe);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[Filename]");
            Console.Write(" [");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("-se ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[SourceLangEnc]]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" [");
            Console.ForegroundColor = ConsoleColor.Gray; 
            Console.Write("-te ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[TargetLangEnc]]");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n\n[Filename]");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("* Mandatory");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n - Name of the input file to transform.");
            Console.WriteLine("\n   Input file should be a Babylon dictionary file with .BGL extension.");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n[SourceLangEnc]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" (Optional parameter)\n - Character encoding of the source language of the dictionary.");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n\n[TargetLangEnc]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" (Optional parameter)\n - Character encoding of the target language of the dictionary.");


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\nExample: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   {0} English_Bulgarian.BGL -se \"ISO-8859-1\" -te \"windows-1251\"", thisExe);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   (this will encode the file into html, with cyrillic encoding)");

            Console.ResetColor();

            #endregion
        }
    }
}
