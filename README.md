# Html2Pdf
Provides a library for generating pdf files from websites, locally saved html files or string-lists plus a standalone commandline tool.
Use the library 'Html2Pdf.dll' in your c#-programs or use directly the commandline-tool 'h2p'.

You can call the commandline-tool without arguments to get help:
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