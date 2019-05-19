!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
2012-03-26
This is a work in progress; still need to get around providing examples for
Chapter 12. If you find bugs in any of the examples, please post to the
iText mailing list so everyone benefits:

http://itextpdf.com/contact.php
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

############################################################################
Examples developed with Visual Studio 2008 (VS2008)/ASP.NET 3.5, so your
mileage may vary. Steps to get the examples up and running: 

[1] Create a new web site with VS2008.

[2] Unpack the iTextSharp-examples.zip files into the project directory.

[3] Download the following .dlls, or build yourself:
    [a] iTextSharp.dll => 5.2.0.0
        http://sourceforge.net/projects/itextsharp/files/

        as announced in the 5.1.0 changelog:
        http://itextpdf.com/history/?branch=51&node=510

        "RichMedia dictionaries to xtra.jar"

        for us .NET developers, this means you __also__ need the
        itextsharp.xtra.dll for the examples in Chapter 16.

    [b] iTextAsian.dll => 2.1.0.0
        http://sourceforge.net/projects/itextsharp/files/extras/
    [c] System.Data.SQLite.dll => 1.0.79.0:
        http://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki

        !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
        if you are running the examples under a web context download the
        32-bit precompiled binaries.
        !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 

    [d] Ionic.Zip.Reduced.dll => 1.9.1.8:
        http://dotnetzip.codeplex.com/

[4] To run the examples under a web context:
    [a] Copy each .dll from step [3] to ~/bin.
    [b] Download the resources from:
        http://itextpdf.com/examples/
        OR
        http://itext.svn.sourceforge.net/viewvc/itext/book/resources/
        (look for a hyperlink titled 'Download GNU tarball' at the
        bottom of the page)
        then copy to ~/iTextInAction2Ed/resources/
    [c] Create your own index page or copy the HTML source from:
        http://kuujinbo.info/iTextInAction2Ed/
    [d] Set 'Virtual Path' for the project's 'Web Site Properties' - VS2008
        default is to set the path to the project name.

[5] To run the examples from the command line:
    [a] Copy each .dll from step [3] to ~/__COMMAND_LINE__
    [b] Download the resources from:
        http://itextpdf.com/examples/
        OR
        http://itext.svn.sourceforge.net/viewvc/itext/book/resources/
        (look for a hyperlink titled 'Download GNU tarball' at the
        bottom of the page)
        then copy to ~/iTextInAction2Ed/resources/
    [c] Run BUILD.bat to build the .NET executable 'CommandLine'. see
        .cs file by same name.
    [d] Run CommandLine.exe to create PDF files. individual source files
        are in ~/app_code/iTextInAction2Ed
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
NOTE: **BEFORE** running the batch file in step 5b above make sure 
you're using the correct csc.exe:

%WINDIR%\Microsoft.NET\Framework\v3.5\

This library is using Linq. otherwise the build will fail and you'll
see a number of exceptions like:

"The type or namespace name 'Linq' does not exist in the namespace 'System'
(are you missing an assembly"
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
NOTES WHEN RUNNING FROM COMMAND LINE:
  ** obviously chapter 9 examples need to be run on a web server, so you
      **WILL NOT** see some Chapter09 files
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

############################################################################
MISCELLANEOUS NOTES:
** chapter 11 deals with fonts, which are **OS DEPENDENT**. so your 
   mileage __WILL__ vary.
############################################################################
SQLite (http://www.sqlite.org/) chosen for data access because of its
simplicity; no additional software required to run the examples. in the summer
of 2001 the core SQLite team themselves have taken over the project :)

    verify the iTextInAction2Ed.db3 database file is in ~/app_data; it's
    included with the source code.

If you want to use SQL Server or other ADO.NET data provider:

[1] If running under a web context, update <connectionStrings> in web.config

[2] If running command-line, edit:
    ~/app_codeiTextInAction2Ed/Intro_1_2/AdoDB.cs
      [a] CS property
      [b] GetProvider()
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
If running from the command line DO NOT EVER, EVER DELETE the file named:
  
  CommandLine.exe.config

I WASTED A COUPLE OF HOURS TRYING TO FIGURE OUT WHY THINGS DID NOT WORK
because i accidentally deleted the file. THE FILE __MUST__ BE in the same
directory as the executable or the ADO access code will __NOT__ be able
to find SQLite! if you do accidentally delete the file, here's it is:

<configuration>
  <system.data>
    <DbProviderFactories>
      <remove invariant='System.Data.SQLite'/>
      <add 
        name='SQLite Data Provider' 
        invariant='System.Data.SQLite' 
        description='.Net Framework Data Provider for SQLite' 
        type='System.Data.SQLite.SQLiteFactory, System.Data.SQLite'
      />
    </DbProviderFactories>
  </system.data>
</configuration>

If you see the following exception:

    "Unable to find the requested .Net Framework Data Provider. It may not be installed."

you probably acidentally deleted the file "CommandLine.exe.config" 
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


[3] to create your database of choice, you may need to update:
    ~/app_data/filmfestival.sql
    ~/app_data/filmfestival-schema.sql
############################################################################
