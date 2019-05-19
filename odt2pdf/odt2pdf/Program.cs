using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using DocuLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odt2pdf
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() >= 2)
            {
                var plik = args[0];
                var outputFile = args[1];
                File.Delete(outputFile);
                
                var converter = new PdfConverter();
                converter.ConvertDocumentToPdf(plik, outputFile);

                System.Diagnostics.Process.Start(outputFile);
            }
            else
            {
                Console.WriteLine("Błedne wywołanie");
                Console.WriteLine("Przykład odt2pdf.exe 123.odt 123.pdf");
                Console.ReadLine();
            }
        }


    }
}
