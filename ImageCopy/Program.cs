using System;
using CommandLineLib;
using FileCopyLib;
using ExifLib;
using System.IO;

namespace ImageCopy
{
   class Program
   {
      static void Main( string[] args )
      {
         CommandLine<CLArguments> commandLine = null;

         try
         {
            var arguments = commandLine.Parse( args );

            var sorter = new Sorter( arguments );
            var copier = new FileCopier();
            copier.NextFile += ( sender, e ) =>
               {
                  e.TargetFile = sorter.Sort( e.SourceFile, e.TargetFile );
               };

            FileCopyOptions fileCopyOptions = new FileCopyOptions()
            {
              CreateTarget = true,
              Difference = FileDifference.Ignore,
              FilePathFilter = new WhitelistFilePathFilter( new string[] { "*.jpg", "*.jpeg", "*.jpe", "*.tif" } ),
              Overwrite = arguments.Overwrite,
              Recursive = true,
              SourcePath = arguments.Source,
              TargetDirectory = arguments.Target
            };

            Console.WriteLine( "Starting copy..." );

            var result = copier.Copy( fileCopyOptions );
         }
         catch ( CommandLineException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );

            if ( commandLine == null )
            {
               Console.WriteLine( ex );
            }
            else
            {
               Console.WriteLine( commandLine.Help() );
            }
         }
         catch ( CommandLineDeclarationException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );

            if ( commandLine == null )
            {
               Console.WriteLine( ex );
            }
            else
            {
               Console.WriteLine( commandLine.Help() );
            }
         }
         catch ( Exception ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( ex );
         }
      }
   }
}
