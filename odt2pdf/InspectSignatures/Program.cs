using iTextLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspectSignatures
{
    class Program
    {

        public const String EXAMPLE1 = @"c:\Users\jkosinski\source\repos\pdf\I8_V2_G47.pdf";
        public const String EXAMPLE2 = @"c:\Users\jkosinski\source\repos\pdf\Synchronizacja XL.pdf";
        public const String EXAMPLE3 = @"c:\Users\jkosinski\source\repos\pdf\I153_V4_G620.pdf";

        static void Main(String[] args)
        {

            ITextLib app = new ITextLib();
            //  app.InspectSignatures(EXAMPLE1);
            var list = app.InspectSignatures(EXAMPLE1);
            var json = JsonConvert.SerializeObject(list);
            Console.Write(json);
            var list2 = app.InspectSignatures(EXAMPLE2);

            var json2 = JsonConvert.SerializeObject(list2);
            Console.Write(json2);
            var list3 = app.InspectSignatures(EXAMPLE3);

            var json3 = JsonConvert.SerializeObject(list3);
            Console.Write(json3);

            Console.ReadLine();
        }
    }
}
