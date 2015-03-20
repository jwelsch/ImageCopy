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
            commandLine = new CommandLine<CLArguments>();
            var arguments = commandLine.Parse( args );

            var sorter = new Sorter( arguments );
            var copier = new FileCopier();
            var first = true;
            copier.NextFile += ( sender, e ) =>
               {
                  if ( first )
                  {
                     Console.WriteLine( "{0} files found to copy.", e.TotalFiles );
                     first = false;
                  }

                  e.TargetFile = sorter.Sort( e.SourceFile, e.TargetFile );
                  Console.WriteLine( "{0}/{1} \"{2}\" =>\n  \"{3}\"", e.FileNumber + 1, e.TotalFiles, e.SourceFile, e.TargetFile );
               };

            FileCopyOptions fileCopyOptions = new FileCopyOptions()
            {
              CreateTarget = true,
              Difference = FileDifference.Ignore,
              FilePathFilter = new WhitelistFilePathFilter( new string[] { @".*\.jpg", @".*\.jpeg", @".*\.jpe", @".*\.tif" } ),
              Overwrite = arguments.Overwrite,
              Recursive = arguments.Recursive,
              SourcePath = arguments.Source,
              TargetDirectory = arguments.Target,
              NoChange = arguments.NoChange
            };

            Console.WriteLine( "Starting copy..." );

            var result = copier.Copy( fileCopyOptions );

            Console.WriteLine( "Copy completed." );
            if ( result.Cancelled )
            {
               Console.WriteLine( "  Operation was CANCELLED." );
            }
            Console.WriteLine( "  Successful: {0}", result.Successes.Count );
            Console.WriteLine( "  Skipped: {0}", result.Skips.Count );
            Console.WriteLine( "  Failed: {0}", result.Failures.Count );
            Console.WriteLine( "  Start time: {0}", result.BeginTime.ToString( "yyyy-MM-dd HH:mm:ss" ) );
            Console.WriteLine( "  Finish time: {0}", result.EndTime.ToString( "yyyy-MM-dd HH:mm:ss" ) );
            Console.WriteLine( "  Elapsed time: {0}", result.ElapsedTime );
            Console.WriteLine( "  Total bytes: {0:N0}", result.TotalBytesCopied );
            Console.WriteLine( "  Bytes per second: {0:N2}", (double) result.TotalBytesCopied / (double) result.ElapsedTime.TotalSeconds );
         }
         catch ( CommandLineException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( commandLine == null ? ex.ToString() : commandLine.Help() );
         }
         catch ( CommandLineDeclarationException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( commandLine == null ? ex.ToString() : commandLine.Help() );
         }
         catch ( Exception ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine( ex );
         }
      }
   }
}
