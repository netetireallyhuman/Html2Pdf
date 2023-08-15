using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Html2Pdf
{
    ///<summary>Provides the various selenium-locator types.</summary>
    public enum LocatorType
    {
        ///<summary>default</summary>
        None,
        ///<summary>Locates elements whose class name contains the search value(compound class names are not permitted)</summary>
        ClassName,
        ///<summary>Locates elements matching a CSS selector</summary>
        CssSelector,
        ///<summary>Locates elements whose ID attribute matches the search value</summary>
        Id,
        ///<summary>Locates elements whose NAME attribute matches the search value</summary>
        Name,
        ///<summary>Locates anchor elements whose visible text matches the search value</summary>
        LinkText,
        ///<summary>Locates anchor elements whose visible text contains the search value.If multiple elements are matching, only the first one will be selected.</summary>
        PartialLinkText,
        ///<summary>Locates elements whose tag name matches the search value</summary>
        TagName,
        ///<summary>Locates elements matching an XPath expression</summary>
        Xpath
    }

    /// <summary>
    /// Generates a pdf file from a website, a locally saved html file or a string-list.
    /// </summary>
    /// <remarks>
    /// 03.06.2022 Erik Nagel: created.
    /// 15.08.2023 Erik Nagel: Module ChromeDriverInstaller is theoretically obsolet since Selenium.WebDriver 4.6.0.
    ///            But during the implicite call of the new selenium-manager.exe a short flicker of a console window appears.
    ///            Therefore the existing implementation is retained. Because google did not issue a downloadble driver for
    ///            chrome 115, ChromeDriverInstaller had to be extended anywhere (see ChromeDriverInstaller for details).
    /// </remarks>
    public static class Generator
    {
        private static bool _isChromeDriverInstalled = false;

        /// <summary>
        /// Generates a pdf-file from a locally saved html-file.
        /// </summary>
        /// <param name="htmlFileName">Path to a local html file.</param>
        /// <param name="pdfFileName">Path to the output-pdf file.</param>
        /// <param name="noHeaderFooter">If true, no site-headers and site-footers are added.</param>
        /// <param name="pageSettings">A container with print-parameters.</param>
        /// <returns>A async Task.</returns>
        public static async Task Convert(string htmlFileName, string pdfFileName, bool noHeaderFooter = false, PageSettings pageSettings = null)
        {
            Uri uri = new System.Uri(Path.GetFullPath(htmlFileName));
            await Convert(htmlFileName, pdfFileName, noHeaderFooter, LocatorType.None, null);
        }

        /// <summary>
        /// Generates a pdf-file from a locally saved html-file.
        /// </summary>
        /// <param name="htmlFileName">>Path to a local html file.</param>
        /// <param name="pdfFileName">Path to the output-pdf file.</param>
        /// <param name="noHeaderFooter">If true, no site-headers and site-footers are added.</param>
        /// <param name="locatorType">Type of a selenium-locator.</param>
        /// <param name="locatorString">Text of the locator.</param>
        /// <param name="pageSettings">A container with print-parameters.</param>
        /// <returns>A async Task.</returns>
        public static async Task Convert(string htmlFileName, string pdfFileName, bool noHeaderFooter = false,
            LocatorType locatorType = LocatorType.None, string locatorString = null, PageSettings pageSettings = null)
        {
            Uri uri = new System.Uri(Path.GetFullPath(htmlFileName));
            await Convert(uri, pdfFileName, noHeaderFooter, locatorType, locatorString, pageSettings);
        }

        /// <summary>
        /// Generates a pdf file from a string-list.
        /// </summary>
        /// <param name="htmlFileLines">A string list with valid html.</param>
        /// <param name="pdfFileName">Path to the output-pdf file.</param>
        /// <param name="noHeaderFooter">If true, no site-headers and site-footers are added.</param>
        /// <param name="pageSettings">A container with print-parameters.</param>
        /// <returns>A async Task.</returns>
        public static async Task Convert(List<string> htmlFileLines, string pdfFileName, bool noHeaderFooter = false, PageSettings pageSettings = null)
        {
            string htmlFileName = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "index.html"));
            File.WriteAllLines(htmlFileName, htmlFileLines);
            Uri uri = new System.Uri(htmlFileName);
            await Convert(uri, pdfFileName, noHeaderFooter, pageSettings);
        }

        /// <summary>
        /// Generates a pdf file from a website.
        /// </summary>
        /// <param name="uri">The website address (uri/url).</param>
        /// <param name="pdfFileName">Path to the output-pdf file.</param>
        /// <param name="noHeaderFooter">If true, no site-headers and site-footers are added.</param>
        /// <param name="pageSettings">A container with print-parameters.</param>
        /// <returns>A async Task.</returns>
        public static async Task Convert(Uri uri, string pdfFileName, bool noHeaderFooter = false, PageSettings pageSettings = null)
        {
            await Convert(uri, pdfFileName, noHeaderFooter, LocatorType.None, null, pageSettings);
        }

        /// <summary>
        /// Generates a pdf file from a website.
        /// </summary>
        /// <param name="uri">The website address (uri/url).</param>
        /// <param name="pdfFileName">Path to the output-pdf file.</param>
        /// <param name="noHeaderFooter">If true, no site-headers and site-footers are added.</param>
        /// <param name="locatorType">Type of a selenium-locator.</param>
        /// <param name="locatorString">Text of the locator.</param>
        /// <param name="pageSettings">A container with print-parameters.</param>
        /// <returns>A async Task.</returns>
        public static async Task Convert(Uri uri, string pdfFileName, bool noHeaderFooter = false,
            LocatorType locatorType = LocatorType.None, string locatorString = null, PageSettings pageSettings = null)
        {
            await InstallChromeDriver();

            var absoluteUri = uri.AbsoluteUri;
            Print2Pdf(absoluteUri, pdfFileName, noHeaderFooter, locatorType, locatorString, pageSettings);
        }

        private static void Print2Pdf(string uri, string pdfName, bool noHeaderFooter = false,
            LocatorType locatorType = LocatorType.None, string locatorString = null, PageSettings pageSettings = null)
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory());
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string>() {
                 "headless", "no-sandbox", "disable-gpu" /* unsatisfactory: "log-level=3"*, senseless: "--silent" "log-level=OFF" */
            });
            // chromeOptions.AddArgument("enable-print-browser");
            if (pageSettings == null)
            {
                pageSettings = new PageSettings();
            }
            using (var driver = new ChromeDriver(service, chromeOptions))
            {
                driver.Navigate().GoToUrl(uri);
                By locator = null;
                if (locatorType != LocatorType.None)
                {
                    switch (locatorType)
                    {
                        case LocatorType.ClassName:
                            locator = By.ClassName(locatorString);
                            break;
                        case LocatorType.CssSelector:
                            locator = By.CssSelector(locatorString);
                            break;
                        case LocatorType.Id:
                            locator = By.Id(locatorString);
                            break;
                        case LocatorType.Name:
                            locator = By.Name(locatorString);
                            break;
                        case LocatorType.LinkText:
                            locator = By.LinkText(locatorString);
                            break;
                        case LocatorType.PartialLinkText:
                            locator = By.PartialLinkText(locatorString);
                            break;
                        case LocatorType.TagName:
                            locator = By.TagName(locatorString);
                            break;
                        case LocatorType.Xpath:
                            locator = By.XPath(locatorString);
                            break;
                        default:
                            break;
                    }
                    new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.FindElements(locator).Count == 1);
                }
                else
                {
                    new WebDriverWait(driver, TimeSpan.FromMilliseconds(100));
                }
                Dictionary<string, object> printOptions;
                double paperHeight = pageSettings.PaperHeight;
                double paperWidth = pageSettings.PaperWidth;
                if (pageSettings.Landscape)
                {
                    double tmp = paperWidth;
                    paperWidth = paperHeight;
                    paperHeight = tmp;
                }
                const double mm2inch = 25.4; // mm to inch factor
                printOptions = new Dictionary<string, object>
                {
                      { "paperWidth", paperWidth / mm2inch }
                    , { "paperHeight", paperHeight / mm2inch }
                    , { "marginLeft", pageSettings.MarginLeft / mm2inch }
                    , { "scale", pageSettings.Scale }
                    , { "displayHeaderFooter", !noHeaderFooter }
                };
                if (!String.IsNullOrEmpty(pageSettings.HeaderTemplate))
                {
                    printOptions.Add("headerTemplate", pageSettings.HeaderTemplate);
                }
                if (!String.IsNullOrEmpty(pageSettings.FooterTemplate))
                {
                    printOptions.Add("footerTemplate", pageSettings.FooterTemplate);
                }
                if (!String.IsNullOrEmpty(pageSettings.PageRanges))
                {
                    printOptions.Add("pageRanges", pageSettings.PageRanges);
                }
                Dictionary<string, object> printOutput;
                printOutput = driver.ExecuteCdpCommand("Page.printToPDF", printOptions) as Dictionary<string, object>;
                byte[] pdf = System.Convert.FromBase64String(printOutput["data"] as string);
                File.WriteAllBytes(pdfName, pdf);
            }
        }

        private static async Task InstallChromeDriver()
        {
            if (_isChromeDriverInstalled)
            {
                return;
            }

            ChromeDriverInstaller chromeDriverInstaller = new ChromeDriverInstaller();

            // not necessary, but added for logging purposes
            var chromeVersion = await chromeDriverInstaller.GetChromeVersion();
            // Console.WriteLine($"Chrome version {chromeVersion} detected");

            await chromeDriverInstaller.Install(chromeVersion, Directory.GetCurrentDirectory());
            // Console.WriteLine("ChromeDriver installed");

            _isChromeDriverInstalled = true;
        }

    }
}
