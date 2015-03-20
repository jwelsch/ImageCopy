using System;

namespace FileCopyLib
{
   /// <summary>
   /// Interface for copying the data of a file.
   /// </summary>
   internal interface IFileDataCopier
   {
      /// <summary>
      /// Copies the data of a file.
      /// </summary>
      /// <param name="source">Information on the source file.</param>
      /// <param name="target">Information on the target file.</param>
      IFileDataCopyResult Copy( FileDataInfo source, FileDataInfo target, IFileCopyOptions options );

      /// <summary>
      /// Fired when a chunk of data has been copied.
      /// </summary>
      event EventHandler<ChunkCopiedArgs> ChunkCopied;

      /// <summary>
      /// Cancels the current file copy operation.
      /// </summary>
      void Cancel();
   }
}
