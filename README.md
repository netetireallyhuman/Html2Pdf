# Html2Pdf
### Provides a C#-library for generating PDF files from websites, locally stored HTML files, or string lists, as well as a standalone command-line tool.
Use the library 'Html2Pdf.dll' in your c#-programs
or use directly the commandline-tool 'h2p' (https://github.com/netetireallyhuman/Html2Pdf/blob/master/h2p.zip?raw=true)

### You can invoke the commandline-tool without arguments to get help:
```html
h2p
Insufficient arguments ()
Syntax: h2p [options] url(website address) pdf-filename[.pdf]
        h2p [options] html-filename(locally saved html-file) pdf-filename[.pdf]
        Options: -f(force overwriting)
                 -n(no header, no footer)
                 -p:page-setting[=value]
                 page-settings:
                        l or landscape: din-A4 landscape (default: din-A4 portrait).
                        pw or paperWidth=paper-width (default:210.0mm).
                        ph or paperHeight=paper-height  (default:297.0mm).
                        ml or marginLeft=margin-left (default:10.0 mm).
                        ht or headerTemplate=optional: "header-template"
                        ft or footerTemplate=optional: "footer-template"
                        sc or scale=scale (default:0.9).
                        pr or pageRanges="e.g. 1 or 2-4"
                 -l:locator-type="locator-string"
                 locator-types:
                        ClassName: Locates elements whose class name contains the search value.
                        CssSelector: Locates elements matching a CSS selector.
                        Id: Locates elements whose ID attribute matches the search value.
                        Name: Locates elements whose NAME attribute matches the search value.
                        LinkText: Locates anchor elements whose visible text matches the search value.
                        PartialLinkText: Locates anchor elements whose visible text contains the search value.
                        TagName: Locates elements whose tag name matches the search value.
                        Xpath: Locates elements matching an XPath expression.
Examples:
-f -l:Xpath="//div[@class='VWDcomp WE021']//span[@class='price']" -p:l -p:sc=0.95 -p:ml=20 -p:pr="1-3" "https://www.tagesschau.de/wirtschaft/boersenkurse/basf-aktie-basf11/" basf
-f demo/index.html converted.pdf
```

### Requirements
Html2Pdf downloads and uses 'chromedriver.exe', which in turn requires a locally installed Chrome browser. In addition, an internet connection is required.

### Special thanks to
Niels Swimberghe
https://swimburger.net/blog/dotnet/download-the-right-chromedriver-version-and-keep-it-up-to-date-on-windows-linux-macos-using-csharp-dotnet

I ported his solution from .Net Core to .Net Framework and made some minor adjustments, but largely adopted it unchanged.

### Third-Party software used by Html2Pdf
Selenium.WebDriver, Selenium.Support - license: Apache 2.0
https://www.selenium.dev/documentation/about/copyright/#license

### Using the library Html2Pdf in your C# code
It is best to look at the source code of h2p.exe.
Below is an excerpt from the H2P project's Program.cs file:

```C#
public class Program
{
	...
	static async Task Main(string[] args)
	{
		...
		string urlOrFilename;
		bool isUrl;
		string pdfFilename;
		LocatorType locatorType;
		string locatorString;
		PageSettings pageSettings;
		EvaluateArgumentsOrExit(args, out forceOverwrite, out noHeaderFooter, out urlOrFilename, out isUrl, out pdfFilename,
			out locatorType, out locatorString, out pageSettings);
		...
		Console.WriteLine("generating pdf " + pdfFilename);
		try
		{
			if (!isUrl)
			{
				await Generator.Convert(urlOrFilename, pdfFilename, noHeaderFooter, locatorType, locatorString, pageSettings);
			}
			else
			{
                
				await Html2Pdf.Generator.Convert(new System.Uri(urlOrFilename), pdfFilename, noHeaderFooter, locatorType, locatorString, pageSettings);
			}

		}
		catch (Exception ex)
		{
			MessageAndOut(ex.Message);
		}
		Console.WriteLine("done.");
	}
	...
}
```

### Contributing
If you find bugs or want to suggest improvements, please open a "New issue".<br/>

### have fun!