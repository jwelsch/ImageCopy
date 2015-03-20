using System;
using System.IO;
using LogSystem;

namespace FileCopyLib
{
   /// <summary>
   /// Implements functionality common to IFileDataCopier implementers that have callbacks.
   /// </summary>
   internal abstract class FileDataCopier : IFileDataCopier
   {
      protected const Int32 DefaultBufferSize = 1000 * 1024; // 1 MB

      /// <summary>
      /// Determines whether or not to cancel the operation.
      /// </summary>
      private bool cancel;

      /// <summary>
      /// Gets whether or not the operation was cancelled.
      /// </summary>
      public bool Cancelled
      {
         get { return this.cancel; }
      }

      /// <summary>
      /// Creates an object of type FileDataCopier.
      /// </summary>
      public FileDataCopier()
      {
      }

      #region IFileDataCopier Members

      /// <summary>
      /// Copies the data of a file.
      /// </summary>
      /// <param name="source">Information on the source file.</param>
      /// <param name="target">Information on the target file.</param>
      public IFileDataCopyResult Copy( FileDataInfo source, FileDataInfo target, IFileCopyOptions options )
      {
         this.cancel = false;

         var result = new FileDataCopyResult()
         {
            SourcePath = source.FilePath,
            TargetPath = target.FilePath
         };

         if ( !options.FilePathFilter.Check( source.FilePath ) )
         {
            result.Skip( SkipReason.Filter );
            return result;
         }

         if ( target.Exists )
         {
            if ( !options.Overwrite )
            {
               // Do not overwrite existing files.  Continue to the next file.
               result.Skip( SkipReason.Overwrite );
               return result;
            }
            else if ( options.Difference != FileDifference.Ignore )
            {
               if ( ( options.Difference == FileDifference.LastModifiedDate ) && ( source.LastWriteTime == target.LastWriteTime ) )
               {
                  result.Skip( SkipReason.LastModifiedDate );
                  return result;
               }
               else if ( ( options.Difference == FileDifference.Size ) && ( source.Size == target.Size ) )
               {
                  result.Skip( SkipReason.Size );
                  return result;
               }
            }
            else
            {
               // Remove the read-only attribute of the target file in order to allow writing to the file.
               // This will get set back to read-only when the source file's attributes are copied to the
               // target file (if the source file is still read-only).
               if ( target.IsReadOnly )
               {
                  var attributes = target.Attributes;
                  attributes &= ~FileAttributes.ReadOnly;
                  File.SetAttributes( target.FilePath, attributes );
               }
            }
         }

         result = this.DoCopy( source, target, options, result );

         if ( result.Outcome == CopyOutcome.Successful )
         {
            try
            {
               // Set the target file's times to be the same as the source file's.
               FileEx.CopyTimes( source.FilePath, target.FilePath );

               // Set the target file's attributes to be the same as the source file's.
               FileEx.CopyAttributes( source.FilePath, target.FilePath );
            }
            catch ( Exception ex )
            {
               result.Fail( ex );
            }
         }

         return result;
      }

      /// <summary>
      /// Fired when a chunk of a file has been copied.
      /// </summary>
      public event EventHandler<ChunkCopiedArgs> ChunkCopied;

      /// <summary>
      /// Cancels the current file copy operation.
      /// </summary>
      public void Cancel()
      {
         this.cancel = true;
      }

      #endregion

      /// <summary>
      /// Fires the ChunkCopied event.
      /// </summary>
      /// <param name="e">Parameters associated with the ChunkCopied event.</param>
      protected void FireChunkCopied( ChunkCopiedArgs e )
      {
         if ( this.ChunkCopied != null )
         {
            this.ChunkCopied( this, e );
         }
      }

      /// <summary>
      /// Copies the data of a file.
      /// </summary>
      /// <param name="source">Information on the source file.</param>
      /// <param name="target">Information on the target file.</param>
      protected abstract FileDataCopyResult DoCopy( FileDataInfo source, FileDataInfo target, IFileCopyOptions options, FileDataCopyResult result );
   }
}
