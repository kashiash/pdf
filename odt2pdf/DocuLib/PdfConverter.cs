using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using System;
using System.IO;

namespace DocuLib
{
    public class PdfConverter
    {

        public void ConvertDocumentToPdf(string plik, string outputFile)
        {
            var server = new RichEditDocumentServer();
            server.LoadDocument(plik, DocumentFormat.OpenDocument);
            //Specify export options: 
            PdfExportOptions options = new PdfExportOptions();
            options.DocumentOptions.Author = "Mediqus";
            options.Compressed = false;
            options.ImageQuality = PdfJpegImageQuality.Highest;
            //Export the document to the stream:  


            using (FileStream pdfFileStream = new FileStream(outputFile, FileMode.Create))
            {
                server.ExportToPdf(pdfFileStream, options);
            }
        }
    }
}
