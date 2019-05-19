using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using kuujinbo.iTextInAction2Ed;

public class CommandLine {
  public static void Main (string[] args) {
    string resultDir = Utility.ResultDirectory;
// recreate result files every time!    
    if (Directory.Exists(resultDir)) {
      Directory.Delete(resultDir, true);
    }
    else {
      Directory.CreateDirectory(resultDir);
    }
    Console.WriteLine("");
    Console.WriteLine("========================================");
    Console.WriteLine("RESULT DIRECTORY: {0}", resultDir);
    Console.WriteLine("========================================");
    Console.WriteLine("");
    foreach (string chapter in Chapters.Examples.Keys) {
      bool madeSubDir = false;
      foreach ( string example in Chapters.Examples[chapter].Keys ) {
        Chapters c = new Chapters() {
          ChapterName = chapter, ExampleName = example
        };
        if (c.IsPdfResult || c.IsZipResult || c.IsOtherResult) {
          if (!madeSubDir) {
            string resultSubDir = Path.Combine(resultDir, chapter);
            Directory.CreateDirectory(resultSubDir);
            madeSubDir = true;
          }
          Console.WriteLine("Creating output file: {0} => {1} ", 
            chapter, example
          );
          c.SendOutput();
        }
      }
    }
  }
// ---------------------------------------------------------------------------  
}
