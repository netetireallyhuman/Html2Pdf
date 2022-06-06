namespace Html2Pdf
{
    /// <summary>
    /// Funktion: Container for print-parameters
    /// </summary>
    /// <remarks>
    /// 03.06.2022 "Erik Nagel, NetEti": erstellt
    /// </remarks>
    public class PageSettings
    {
        public bool Landscape { get; set; } ///<summary>If true, Din-A4 landscape format is selected; default: din-a4 portrait</summary>
        public double PaperWidth { get; set; } ///<summary>Paper width in mm; default: 210 mm</summary>
        public double PaperHeight { get; set; } /// <summary>Paper height in mm; default: 297 mm</summary>
        public double MarginLeft { get; set; } ///<summary>Left margin in mm; default: 10 mm</summary>
        public string HeaderTemplate { get; set; } ///<summary>Optional HTML header-template</summary>
        public string FooterTemplate { get; set; } ///<summary>Optional HTML footer-template</summary>
        public double Scale { get; set; } ///<summary>factor for shrinking the page from 0.2 to 2.0 (1.0 = original size); default: 0.95</summary>
        public string PageRanges { get; set; } ///<summary>optional: e.g. 1 or 2-4</summary>

        public PageSettings()
        {
            this.Landscape = false;
            this.PaperWidth = 210.0;
            this.PaperHeight = 297.0;
            this.MarginLeft = 10.0;
            this.HeaderTemplate = null;
            this.HeaderTemplate = null;
            this.Scale = 0.95;
            this.PageRanges = null;
        }
    }
}
