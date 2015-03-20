using System;
using System.IO;

namespace FileCopyLib
{
   /// <summary>
   /// Copies file data using the FileStream API.
   /// </summary>
   internal class FileStreamDataCopier : FileDataCopier
   {
      /// <summary>
      /// Buffer used to transfer data from the source file to the target file.
      /// </summary>
      private byte[] copyBuffer;

      /// <summary>
      /// Creates an object of type FileStreamDataCopier.
      /// </summary>
      /// <param name="bufferSize">Size of buffer (in bytes) used to transfer data from the source file to the target file.</param>
      public FileStreamDataCopier( int bufferSize )
      {
         this.copyBuffer = new byte[bufferSize];
      }

      /// <summary>
      /// Creates an object of type DotNetFileCopyDataCopier.
      /// </summary>
      public FileStreamDataCopier()
         : this( DefaultBufferSize )
      {
      }

      #region CallbackFileDataCopier Members

      /// <summary>
      /// Copies the data of a file.
      /// </summary>
      /// <param name="source">Information on the source file.</param>
      /// <param name="target">Information on the target file.</param>
      protected override FileDataCopyResult DoCopy( FileDataInfo source, FileDataInfo target, IFileCopyOptions options, FileDataCopyResult result )
      {
         try
         {
            using ( var sourceStream = new FileStream( source.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
            {
               using ( var targetStream = new FileStream( target.FilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite ) )
               {
                  var bytesRead = 0;

                  do
                  {
                     bytesRead = sourceStream.Read( copyBuffer, 0, copyBuffer.Length );

                     if ( bytesRead > 0 )
                     {
                        targetStream.Write( copyBuffer, 0, bytesRead );

                        result.BytesCopied += (Int64) bytesRead;

                        this.FireChunkCopied( new ChunkCopiedArgs( sourceStream.Length, result.BytesCopied ) );
                     }

                     if ( this.Cancelled )
                     {
                        result.Cancel();
                        break;
                     }
                  }
                  while ( bytesRead > 0 );
               }
            }
         }
         catch ( Exception ex )
         {
            result.Fail( ex );
         }

         if ( ( result.Outcome == CopyOutcome.Failed ) || ( result.Outcome == CopyOutcome.Cancelled ) )
         {
            if ( File.Exists( target.FilePath ) )
            {
               File.Delete( target.FilePath );
            }
         }

         return result;
      }

      #endregion
   }
}
