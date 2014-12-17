using System;
using System.IO;
using ExifLib;

namespace ImageCopy
{
   internal class Sorter
   {
      private ISorterOptions options;
      private string dateFormat;

      public Sorter( ISorterOptions options )
      {
         this.options = options;

         switch ( this.options.DateFormat )
         {
            case DateFormat.Y:
            {
               this.dateFormat = "yyyy";
               break;
            }
            case DateFormat.YM:
            {
               this.dateFormat = "yyyy-MM";
               break;
            }
            case DateFormat.YMD:
            {
               this.dateFormat = "yyyy-MM-dd";
               break;
            }
            default:
            {
               this.dateFormat = "yyyy-MM-dd";
               break;
            }
         }
      }

      public string Sort( string sourcePath, string targetPath )
      {
         using ( var reader = new ExifReader( sourcePath ) )
         {
            DateTime dateTaken;
            if ( reader.GetTagValue<DateTime>( ExifTags.DateTimeDigitized, out dateTaken ) )
            {
               var directory = Path.Combine( Path.GetDirectoryName( targetPath ), dateTaken.ToString( this.dateFormat ) );
               return Path.Combine( directory, Path.GetFileName( targetPath ) );
            }
         }

         return targetPath;
      }
   }
}
