using System;
using System.IO;

namespace FileCopyLib
{
   /// <summary>
   /// Copies file data using the FileStream API asynchronously.
   /// </summary>
   internal class AsyncFileStreamDataCopier : FileDataCopier
   {
      /// <summary>
      /// Buffers used to transfer data from the source file to the target file.
      /// </summary>
      private byte[] copyBuffer1;
      private byte[] copyBuffer2;

      /// <summary>
      /// Creates an object of type AsyncFileStreamDataCopier.
      /// </summary>
      /// <param name="bufferSize">Size of buffer (in bytes) used to transfer data from the source file to the target file.</param>
      public AsyncFileStreamDataCopier( int bufferSize )
      {
         this.copyBuffer1 = new byte[bufferSize];
         this.copyBuffer2 = new byte[bufferSize];
      }

      /// <summary>
      /// Creates an object of type AsyncFileStreamDataCopier.
      /// </summary>
      public AsyncFileStreamDataCopier()
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
            using ( var sourceStream = new FileStream( source.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
               using ( var targetStream = new FileStream( target.FilePath, FileMode.Create, FileAccess.Write, FileShare.None ) )
               {
                  var bytesRead = 0;
                  IAsyncResult asyncResult = null;
                  var readBuffer = this.copyBuffer1;

                  do
                  {
                     // Read in data from the source file.
                     bytesRead = sourceStream.Read( readBuffer, 0, readBuffer.Length );

                     if ( bytesRead > 0 )
                     {
                        if ( asyncResult != null )
                        {
                           // Wait for the write operation to finish.
                           asyncResult.AsyncWaitHandle.WaitOne();
                           targetStream.EndWrite( asyncResult );
                           asyncResult = null;
                        }

                        // Set the buffer to write to the target file as the read buffer.
                        var writeBuffer = readBuffer;

                        // Set the read buffer to be the other copy buffer because the buffer that is currently
                        // being pointed to by readBuffer will be written to the target file next.
                        readBuffer = ( readBuffer == this.copyBuffer1 ) ? this.copyBuffer2 : copyBuffer1;

                        // Write the data to the target file.
                        asyncResult = targetStream.BeginWrite( writeBuffer, 0, bytesRead, null, null );

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

                  if ( asyncResult != null )
                  {
                     asyncResult.AsyncWaitHandle.WaitOne();
                     targetStream.EndWrite( asyncResult );
                  }
               }
            }
         }
         catch ( Exception ex )
         {
            result.Fail( ex );
         }

         // If the file copy was cancelled and the file has not been completely
         // copied, delete the partially copied file.
         if ( ( result.Outcome == CopyOutcome.Cancelled ) || ( result.Outcome == CopyOutcome.Failed ) )
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
