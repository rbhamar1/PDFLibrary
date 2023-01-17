using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PDFGenerator
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadPdf();
        }

        private void LoadPdf()
        {

            try
            {
                var docFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "PDFGenerator");
                if (!Directory.Exists(docFolderPath))
                {
                    Directory.CreateDirectory(docFolderPath);
                }
                var filenamePath = Path.Combine(docFolderPath, "Sample.pdf");
                var testPrintPdfFactory = new TestPrintPdfFactory();
                var pdf = testPrintPdfFactory.Create();
                File.WriteAllBytes(filenamePath, pdf.GetBytes());
                PdfData = new Uri(filenamePath);
                PdfView.Source = PdfData;
            }
            catch (Exception ex)
            {
                // ignored
                Debug.WriteLine("Exception :{0}", ex);
            }
        }

        public WebViewSource PdfData
        {
            get => _pdfData;
            set => _pdfData = value;
        }

        private WebViewSource _pdfData;
    }
}

