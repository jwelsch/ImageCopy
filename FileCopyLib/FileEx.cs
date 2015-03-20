using System;
using System.IO;

namespace FileCopyLib
{
   /// <summary>
   /// Provides supplements to the System.IO.File class.
   /// </summary>
   internal static class FileEx
   {
      /// <summary>
      /// Copies the attributes of the source path to the target path.
      /// The sourcePath and the targetPath must both point to either a directory or a file.
      /// </summary>
      /// <param name="sourcePath">Path containing the attributes to copy.</param>
      /// <param name="targetPath">Path containing the attributes to copy to.</param>
      internal static void CopyAttributes( string sourcePath, string targetPath )
      {
         var sourceAttributes = File.GetAttributes( sourcePath );
         var targetAttributes = File.GetAttributes( targetPath );
         var sourceIsDirectory = ( sourceAttributes & FileAttributes.Directory ) == FileAttributes.Directory;
         var targetIsDirectory = ( targetAttributes & FileAttributes.Directory ) == FileAttributes.Directory;

         if ( sourceIsDirectory != targetIsDirectory )
         {
            throw new ArgumentException( "The sourcePath and the targetPath must both either be directories or files." );
         }

         try
         {
            File.SetAttributes( targetPath, sourceAttributes );
         }
         catch ( IOException )
         {
            System.Diagnostics.Trace.WriteLine( "!!!*** Is Windows Explorer open to the folder containing the targetPath? ***!!!" );
            throw;
         }
      }

      /// <summary>
      /// Copies the creation, last write, and last access times of the source path to the target path.
      /// The sourcePath and the targetPath must both point to either a directory or a file.
      /// </summary>
      /// <param name="sourcePath">Path containing the times to copy.</param>
      /// <param name="targetPath">Path containing the times to copy to.</param>
      internal static void CopyTimes( string sourcePath, string targetPath )
      {
         var sourceAttributes = File.GetAttributes( sourcePath );
         var targetAttributes = File.GetAttributes( targetPath );
         var sourceIsDirectory = ( sourceAttributes & FileAttributes.Directory ) == FileAttributes.Directory;
         var targetIsDirectory = ( targetAttributes & FileAttributes.Directory ) == FileAttributes.Directory;

         if ( sourceIsDirectory != targetIsDirectory )
         {
            throw new ArgumentException( "The sourcePath and the targetPath must both either be directories or files." );
         }

         try
         {
            if ( sourceIsDirectory )
            {
               Directory.SetCreationTime( targetPath, Directory.GetCreationTime( sourcePath ) );
               Directory.SetLastWriteTime( targetPath, Directory.GetLastWriteTime( sourcePath ) );
               Directory.SetLastAccessTime( targetPath, Directory.GetLastAccessTime( sourcePath ) );
            }
            else
            {
               File.SetCreationTime( targetPath, File.GetCreationTime( sourcePath ) );
               File.SetLastWriteTime( targetPath, File.GetLastWriteTime( sourcePath ) );
               File.SetLastAccessTime( targetPath, File.GetLastAccessTime( sourcePath ) );
            }
         }
         catch ( IOException )
         {
            System.Diagnostics.Trace.WriteLine( "!!!*** Is Windows Explorer open to the folder containing the targetPath? ***!!!" );
            throw;
         }
      }

      /// <summary>
      /// Determines if the given file has the given attribute.
      /// </summary>
      /// <param name="filePath">Path of the file to check.</param>
      /// <param name="attribute">Attribute to check for.</param>
      /// <returns>True if the file has the attribute, false otherwise.</returns>
      internal static bool DoesFileHaveAttribute( string filePath, FileAttributes attribute )
      {
         return ( ( File.GetAttributes( filePath ) & attribute ) == attribute );
      }
   }
}
