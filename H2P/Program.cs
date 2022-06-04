using Html2Pdf;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace H2P
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool forceOverwrite;
            bool noHeaderFooter;
            string urlOrFilename;
            bool isUrl;
            string pdfFilename;
            LocatorType locatorType;
            string locatorString;
            PageSettings pageSettings;
            EvaluateArgumentsOrExit(args, out forceOverwrite, out noHeaderFooter, out urlOrFilename, out isUrl, out pdfFilename,
                out locatorType, out locatorString, out pageSettings);
            if (!forceOverwrite && File.Exists(pdfFilename))
            {
                Console.Write($"overwrite {pdfFilename} y(es)|N(o)? ");
                string answer = Console.ReadLine();
                if (String.IsNullOrEmpty(answer) || (answer.ToLower().Trim() != "y" && answer.ToLower().Trim() != "yes"))
                {
                    Console.WriteLine("Operation aborted.");
                    Environment.Exit(0);
                }
            }
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

        private static void EvaluateArgumentsOrExit(string[] args, out bool forceOverwrite, out bool noHeaderFooter,
            out string urlOrFilename, out bool isUrl, out string pdfFilename, out LocatorType locatorType, out string locatorString, out PageSettings pageSettings)
        {
            forceOverwrite = false;
            noHeaderFooter = false;
            urlOrFilename = null;
            isUrl = false;
            pdfFilename = null;
            locatorType = LocatorType.None;
            locatorString = null;
            pageSettings = new PageSettings();
            foreach (string argument in args)
            {
                if (argument[0] == '-' || argument[0] == '/')
                {
                    string arg = argument.Substring(1).Trim();
                    if (arg[0] == '-')
                    {
                        arg = arg.Substring(1).ToLower();
                    }
                    switch (arg[0])
                    {
                        case 'f':
                            forceOverwrite = true;
                            break;
                        case 'n':
                            noHeaderFooter = true;
                            break;
                        case 'l':
                            arg = arg.Substring(1);
                            if (arg[0] == ':')
                            {
                                arg = arg.Substring(1) + "=";
                                string[] locatorAndString = arg.Split('=');
                                string locator = locatorAndString[0].Trim();
                                locatorAndString[0] = "";
                                locatorString = String.Join("=", locatorAndString).TrimStart('=').TrimEnd('=');
                                if (!String.IsNullOrEmpty(locator))
                                {
                                    if (!Enum.TryParse(locator, true, out locatorType))
                                    {
                                        Syntax(String.Format($"Unknown locator-type '{locator}'!"));
                                    }
                                }
                            }
                            break;
                        case 'p':
                            arg = arg.Substring(1);
                            if (arg[0] == ':')
                            {
                                arg = arg.Substring(1) + ",";
                                string[] pageSettingsStrings = arg.Split(',');
                                foreach (string pageSettingString in pageSettingsStrings)
                                {
                                    if (!String.IsNullOrEmpty(pageSettingString))
                                    {
                                        string pageSetting = pageSettingString.Trim() + "=";
                                        string pageSettingKey = pageSetting.ToLower().Split('=')[0];
                                        string pageSettingValue = pageSetting.Replace(pageSettingKey, "").TrimStart('=').TrimEnd('=');
                                        switch (pageSettingKey)
                                        {
                                            case "l":
                                            case "landscape":
                                                pageSettings.Landscape = true;
                                                break;
                                            case "pw":
                                            case "paperWidth":
                                                double width;
                                                if (double.TryParse(pageSettingValue.Replace('.', ','), out width))
                                                {
                                                    pageSettings.PaperWidth = width;
                                                }
                                                break;
                                            case "ph":
                                            case "paperHeight":
                                                double height;
                                                if (double.TryParse(pageSettingValue.Replace('.', ','), out height))
                                                {
                                                    pageSettings.PaperHeight = height;
                                                }
                                                break;
                                            case "ml":
                                            case "marginLeft":
                                                double left;
                                                if (double.TryParse(pageSettingValue.Replace('.', ','), out left))
                                                {
                                                    pageSettings.MarginLeft = left;
                                                }
                                                break;
                                            case "ht":
                                            case "headerTemplate":
                                                if (!String.IsNullOrEmpty(pageSettingValue))
                                                {
                                                    pageSettings.HeaderTemplate = pageSettingValue;
                                                }
                                                break;
                                            case "ft":
                                            case "footerTemplate":
                                                if (!String.IsNullOrEmpty(pageSettingValue))
                                                {
                                                    pageSettings.FooterTemplate = pageSettingValue;
                                                }
                                                break;
                                            case "sc":
                                            case "scale":
                                                double scale;
                                                if (double.TryParse(pageSettingValue.Replace('.', ','), out scale))
                                                {
                                                    pageSettings.Scale = scale;
                                                }
                                                break;
                                            case "pr":
                                            case "pageRanges":
                                                if (!String.IsNullOrEmpty(pageSettingValue))
                                                {
                                                    pageSettings.PageRanges = pageSettingValue;
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(urlOrFilename))
                    {
                        urlOrFilename = argument.Trim();
                        Uri uriResult;
                        isUrl = Uri.TryCreate(urlOrFilename, UriKind.Absolute, out uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        if (!isUrl && !File.Exists(urlOrFilename))
                        {
                            Syntax(String.Format($"File '{urlOrFilename}' not found!"));
                        }
                    }
                    else
                    {
                        pdfFilename = argument.Trim();
                    }
                }
            }
            if (string.IsNullOrEmpty(urlOrFilename) || string.IsNullOrEmpty(pdfFilename))
            {
                Syntax(String.Format($"Insufficient arguments ({String.Join(" ", args)})"));
            }
            if (!pdfFilename.ToLower().EndsWith(".pdf"))
            {
                pdfFilename += ".pdf";
            }
        }

        private static void MessageAndOut(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(127);
        }

        private static void Syntax(string message)
        {
            StringBuilder fullMessage = new StringBuilder(message + Environment.NewLine);
            fullMessage.Append("Syntax:\t" + Assembly.GetExecutingAssembly().GetName().Name.ToLower());
            fullMessage.AppendLine(" [options] url(website address) pdf-filename[.pdf]");
            fullMessage.Append("\t" + Assembly.GetExecutingAssembly().GetName().Name.ToLower());
            fullMessage.AppendLine(" [options] html-filename(locally saved html-file) pdf-filename[.pdf]");
            fullMessage.AppendLine("\tOptions: -f(force overwriting)");
            fullMessage.AppendLine("\t         -n(no header, no footer)");
            fullMessage.AppendLine("\t         -p:page-setting[=value]");
            fullMessage.AppendLine("\t         page-settings: ");
            fullMessage.AppendLine("\t\t\tl or landscape: din-A4 landscape (default: din-A4 portrait).");
            fullMessage.AppendLine("\t\t\tpw or paperWidth=paper-width (default:210.0mm).");
            fullMessage.AppendLine("\t\t\tph or paperHeight=paper-height  (default:297.0mm).");
            fullMessage.AppendLine("\t\t\tml or marginLeft=margin-left (default:10.0 mm).");
            fullMessage.AppendLine("\t\t\tht or headerTemplate=optional: \"header-template\"");
            fullMessage.AppendLine("\t\t\tft or footerTemplate=optional: \"footer-template\"");
            fullMessage.AppendLine("\t\t\tsc or scale=scale (default:0.9).");
            fullMessage.AppendLine("\t\t\tpr or pageRanges=\"e.g. 1 or 2-4\"");
            fullMessage.AppendLine("\t         -l:locator-type=\"locator-string\"");
            fullMessage.AppendLine("\t         locator-types: ");
            fullMessage.AppendLine("\t\t\tClassName: Locates elements whose class name contains the search value.");
            fullMessage.AppendLine("\t\t\tCssSelector: Locates elements matching a CSS selector.");
            fullMessage.AppendLine("\t\t\tId: Locates elements whose ID attribute matches the search value.");
            fullMessage.AppendLine("\t\t\tName: Locates elements whose NAME attribute matches the search value.");
            fullMessage.AppendLine("\t\t\tLinkText: Locates anchor elements whose visible text matches the search value.");
            fullMessage.AppendLine("\t\t\tPartialLinkText: Locates anchor elements whose visible text contains the search value.");
            fullMessage.AppendLine("\t\t\tTagName: Locates elements whose tag name matches the search value.");
            fullMessage.AppendLine("\t\t\tXpath: Locates elements matching an XPath expression.");
            fullMessage.AppendLine("Examples:");
            fullMessage.AppendLine("-f -l:Xpath=\"//div[@class='VWDcomp WE021']//span[@class='price']\" -p:l -p:sc=0.95 -p:ml=20 -p:pr=\"1-3\" \"https://www.tagesschau.de/wirtschaft/boersenkurse/basf-aktie-basf11/\" basf");
            fullMessage.AppendLine("-f demo/index.html converted.pdf");
            MessageAndOut(fullMessage.ToString());
        }

    }
}
