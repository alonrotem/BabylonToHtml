#BabylonToHtml#

This is tool for converting Babylon .bgl dictionaries to structured HTML, for creation of a Kindle-compatible dictionaries.

##The Idea in Brief##
BabylonToHtml takes a Babylon .bgl file (which can be downloaded freely from [Babylon's free content section](http://www.babylon.com/free-dictionaries/ "Free Glossaries"), and converts it to a well-structured HTML.

The produced HTML can be used with [MobiPocket Creator](http://www.mobipocket.com/en/downloadsoft/productdetailscreator.asp "MobiPocket Creator") to create a .prc formatted eBook, which can be used natively as a dictionary on Kindle devices.


See my blog post [Using Babylon-based dictionaries on your Kindle](http://www.alonintheworld.com/2012/06/using-babylon-based-dictionaries-on.html "AlonInTheWorld.com") for additional information about this tool's purpose.

##About the Structure of the BabylonToHtml Tool##
The binary structure of .BGL files has already been cracked (not by me). This knowledge is commonly out in the open and shared across various open-source projects. I have combined a few of those resources into one easy-to-use command-line utility.

+ One source was [dictconv](http://freecode.com/projects/dictconv "dictconv"), a dictionary conversion tool for Linux which comes with its full C++ source. I used parts of this code (ported by me into C#) in order to analyse the meta-data of the dictionary file (text encoding, author etc).

+ Another resource is is an open-source project named [ThaiLanguageTools](http://code.google.com/p/thailanguagetools/source/browse/trunk/ThaiLanguageTools/?r=15 "ThaiLanguageTools"). It's written in C# but the contents of the code looks suspiciously similar to the code of dictconv mentioned above (similar variable names, comments etc) which suggests it's a porting as well.

+ The content of Babylon's .BGL files is encoded in compressed GZip format. In order to decompress the data, I have incorporated the free open-source [SharpZipLib](http://www.icsharpcode.net/opensource/sharpziplib/ "SharpZipLib") into the project as well (as source code, so there is only one executable needed to run my app in the end. no additional DLLs).


+ To all the above I added my very own simple HTML generator. It structures the entries from the dictionary file in a markup compatible with the next step (converting it into an eBook).

##Basic Usage##

BabylonToHtml tool runs in a command prompt window. Running it without any additional parameters, you'll receive some basic help:<br/>
![Runtime help](http://2.bp.blogspot.com/-sgs1NUKZIE8/T-tjX-dCcpI/AAAAAAAAOWc/wN9Z-szk2-U/s1600/BabylonToHtmlHelp.png "A handy message for the perplexed user..")

###Command line parameters:###
+ In most cases all you have to provide is the name (and potentially the path) of your .BGL file. 
+ The output .HTML is encoded in UTF-8 (Unicode).
However, the entries read from the .BGL dictionary are encoded with specific character sets (and sometimes with more than one).For example: in a Chinese - Bulgarian dictionary the source language entries are encoded with Chinese characters and the target language entries are encoded in Cyrillic. 
+ BabylonToHtml will try, by default, to get the right encoding (this info is available in the meta-data of the .BGL file in most cases), but it may make mistakes.
+ These encodings can be enforced:<br/>
It is possible to set the codepage of the source language by specifying the -se command line argument.<br/>
It is possible to set the codepage of the target language by specifying the -te command line argument. 

So something like the following should be sufficient in most cases:
<table style="width:100%; background-color:black; color:white;"><tr><td>BabylonToHtml.exe English_Bulgarian.BGL</td></tr></table\>
If your .BGL file does not reside in the same folder with the .EXE, a full path should be specified (may be wrapped with double-quotes if needed).<br/>
The encoding (and other information about your dictionary) is be parsed and progress of the process is presented...<br/>
![Running...](http://2.bp.blogspot.com/-Ywq_tZ4d_lE/T-tngEbRf0I/AAAAAAAAOWo/xis9uE4H4Pk/s1600/BabylonToHtmlRunning.png "Running...")

Once the process is done, a new HTML file resides next to the original .BGL file.
The new file's name matches the original .BGL file (just with .HTML extension):<br/>
![All done. A new HTML file is generated. Magic!!](http://4.bp.blogspot.com/-Ei8IYaHpWqk/T-tnhBtXPyI/AAAAAAAAOWw/pHRpvDO1O8g/s1600/BabylonToHtmlDone.png "All done. A new HTML file is generated. Magic!!")

##Why is this project on GitHub?##

###Known issues###
My blog-post presenting this project, "[Using Babylon-based dictionaries on your Kindle](http://www.alonintheworld.com/2012/06/using-babylon-based-dictionaries-on.html "AlonInTheWorld.com")", got a lot of attention. It seems many people liked the idea of importing Babylon dictionaries to Kindle. Many people asked about an option to produce an English-to-Hebrew dictionary, but not just.

The outputs of this project at runtime, the HTML (and then the .prc file), are not perfect. There are a few key problems which need to be adderssed:

1. There are unresolved portions of the produced dictionary, which are wrapped by  &lt;charset c=T&gt;****;&lt;/charset&gt; blocks. Those are probably unicode characters which need to be resolved.
2. There are unresolved portions of the produced dictionary, which are delimited with dollar-signs, e.g: $506274$ or cos$531761$. Not 100% sure how those should be treated/resolved.
3. The encoding is actually force-resolved in the code, no matter what the user says (it gets overridden by the dictionary analysis code). This needs to be fixed.
4. Specifically for Hebrew dictionaries: Kindle (at least Kindle v2) always aligs the text to left-to-right. Not just the letters (that's easy), but also the order of the words themselves. To know which word will appear where is impossible (this depends on the screen's size and selected unicode font (in the case of Kindle 2, a hacked font needs to be installed, as explained [here](http://blogkindle.com/unicode-fonts-hack/ "Unicode Fonts Hack | Amazon Kindle, Kindle 2 And Kindle DX Blog")).

Having not much time to address these issues, I've come to a decision to open this project to the public, for whoever wants and can improve it. <br/>
So there you have it!

###Additional resources for contributors###
As I stated above (and in my blog post), Babylon's .bgl format is well known and there are other projects which parse it. Some were suggested by commentators of the post directly and some by email.
Here are a few, which may come-in handy for anyone who'd like to contribute to this project and its known issues listed above:

* Forum thread: [BGL (babylon glossary) to GLS (babylon glossary source)](http://www.woodmann.com/forum/showthread.php?7028#post44981 "BGL (babylon glossary) to GLS (babylon glossary source)")
 
* [BGL-Reverse](https://github.com/mgreen/bgl-reverse "BGL-Reverse") -  another open-source reverse engineered BGL parser.

* [PyGlossary](https://github.com/ilius/pyglossary "PyGlossary") - yet another open-source reverse engineered BGL parser, this time in Python.
