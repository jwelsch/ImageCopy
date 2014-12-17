using System;
using CommandLineLib;
using FileCopyLib;
using ExifLib;

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

            var copier = new FileCopier();
            copier.NextFile += ( sender, e ) =>
               {
                  using ( var reader = new ExifReader( e.SourceFile ) )
                  {
                     DateTime dateTaken;
                     if ( reader.GetTagValue<DateTime>( ExifTags.DateTimeDigitized, out dateTaken ) )
                     {
                     }
                  }
               };
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
