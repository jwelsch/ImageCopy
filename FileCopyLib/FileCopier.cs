using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using LogSystem;

namespace FileCopyLib
{
   /***************************************************************************
    * 
    * Supported options:
    * - Overwrite
    * - Copying and overwriting read-only files
    * - Copying only changed files
    * - Copying only files on a whitelist
    * - Not copying files on a blacklist
    * - Recursive copying of subdirectories
    * - Automatic creation of target directory
    * - Logging
    * - Silent operation
    * - Copy progress
    * - Report skipped files
    * - Asynchronous operation
    * 
    * TODO:
    * - Logging
    * - Silent operation
    * - Asynchronous operation
    * 
    * ************************************************************************/

   #region NextFileArgs

   /// <summary>
   /// Arguments associated switching to the next file to copy.
   /// </summary>
   public class NextFileArgs : EventArgs
   {
      /// <summary>
      /// Gets the path of the source file being copied.
      /// </summary>
      public string SourceFile
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the path of the target file that is being copied to.
      /// </summary>
      public string TargetFile
      {
         get;
         set;
      }

      /// <summary>
      /// Gets the zero-based index of the file that will be copied.
      /// </summary>
      public int FileNumber
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the total number of files that will be copied.
      /// </summary>
      public int TotalFiles
      {
         get;
         private set;
      }

      /// <summary>
      /// Creates an object of type NextFileArgs.
      /// </summary>
      /// <param name="sourceFile">The path of the source file being copied.</param>
      /// <param name="targetFile">The path of the target file that is being copied to.</param>
      /// <param name="fileNumber">The zero-based index of the file that will be copied.</param>
      /// <param name="totalFiles">The total number of files that will be copied.</param>
      public NextFileArgs( string sourceFile, string targetFile, int fileNumber, int totalFiles )
      {
         this.SourceFile = sourceFile;
         this.TargetFile = targetFile;
         this.FileNumber = fileNumber;
         this.TotalFiles = totalFiles;
      }
   }

   #endregion

   #region ChunkCopiedArgs

   /// <summary>
   /// Events passed when a chunk of a file is copied.
   /// </summary>
   public class ChunkCopiedArgs : EventArgs
   {
      private Int64 fileSize;
      /// <summary>
      /// Gets the size of the file being copied.
      /// </summary>
      public Int64 FileSize
      {
         get { return this.fileSize; }
      }

      private Int64 bytesCopied;
      /// <summary>
      /// Gets the bytes that have been copied so far.
      /// </summary>
      public Int64 BytesCopied
      {
         get { return this.bytesCopied; }
      }

      /// <summary>
      /// Creates an object of type ChunkCopiedArgs.
      /// </summary>
      /// <param name="fileSize">Size of the file being copied in bytes.</param>
      /// <param name="bytesCopied">Number of bytes copied so far.</param>
      public ChunkCopiedArgs( Int64 fileSize, Int64 bytesCopied )
      {
         this.fileSize = fileSize;
         this.bytesCopied = bytesCopied;
      }
   }

   #endregion

   #region FileCopyEndedArgs

   public class FileCopyEndedArgs : EventArgs
   {
      public IFileCopyResult Result
      {
         get;
         private set;
      }

      public FileCopyEndedArgs( IFileCopyResult result )
      {
         this.Result = result;
      }
   }

   #endregion

   /// <summary>
   /// Copies items in a file system.
   /// </summary>
   public class FileCopier
   {
      private object sync = new object();

      /// <summary>
      /// Fired when the next file has started being copied.
      /// </summary>
      public event EventHandler<NextFileArgs> NextFile;

      /// <summary>
      /// Fired when a chunk of a file has been copied.
      /// </summary>
      public event EventHandler<ChunkCopiedArgs> ChunkCopied;

      /// <summary>
      /// Fired when the file copy operation has ended.
      /// </summary>
      public event EventHandler<FileCopyEndedArgs> FileCopyEnded;

      public event System.Diagnostics.DataReceivedEventHandler ProcessHasOutput;

      public event System.Diagnostics.DataReceivedEventHandler ProcessHasError;

      public event EventHandler<ProcessEndedArgs> ProcessEnded;

      /// <summary>
      /// Whether or not to cancel the copy process.
      /// </summary>
      private bool cancel;

      /// <summary>
      /// Gets whether or not the file copy operation was cancelled.
      /// </summary>
      public bool Cancelled
      {
         get { return this.cancel; }
      }

      private bool isBusy;
      /// <summary>
      /// Gets whether or not the object is busy with a copy operation.
      /// </summary>
      public bool IsBusy
      {
         get { return this.isBusy; }
      }

      /// <summary>
      /// Object used to copy the file data.
      /// </summary>
      private IFileDataCopier fileDataCopier;

      /// <summary>
      /// Creates an object of type FileCopyLibEngine.
      /// </summary>
      public FileCopier()
      {
         //this.fileDataCopier = new AsyncFileStreamDataCopier();
         this.fileDataCopier = new FileStreamDataCopier();

         this.fileDataCopier.ChunkCopied += ( sender, e ) =>
         {
            if ( this.ChunkCopied != null )
            {
               this.ChunkCopied( sender, e );
            }
         };
      }

      /// <summary>
      /// Cancels the copy process.
      /// </summary>
      public void Cancel()
      {
         lock ( this.sync )
         {
            this.cancel = true;

            if ( this.fileDataCopier != null )
            {
               this.fileDataCopier.Cancel();
            }
         }
      }

      /// <summary>
      /// Copies the file system items.
      /// </summary>
      /// <param name="options">Options controlling the copy operation.</param>
      public IFileCopyResult Copy( IFileCopyOptions options )
      {
         lock ( this.sync )
         {
            if ( this.isBusy )
            {
               options.Logger.WriteLine( LogLevel.Fatal, "Another copy is in progres." );
               throw new FileCopyException( "Another copy is in progress." );
            }

            this.isBusy = true;
            this.cancel = false;
         }

         options.Logger.WriteLine( "Copy operation started." );
         var result = new FileCopyResult();
         result.MarkBegin();

         try
         {
            var absoluteSourcePath = Path.GetFullPath( options.SourcePath );
            var absoluteTargetPath = Path.GetFullPath( options.TargetDirectory );

            string sourcePathRoot = null;
            string[] sourceFiles = null;

            if ( FileEx.DoesFileHaveAttribute( absoluteSourcePath, FileAttributes.Directory ) )
            {
               var searchOption = options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
               sourceFiles = Directory.GetFiles( absoluteSourcePath, "*", searchOption );
               sourcePathRoot = absoluteSourcePath;
            }
            else
            {
               sourceFiles = new string[] { absoluteSourcePath };
               sourcePathRoot = Path.GetDirectoryName( absoluteSourcePath );
            }

            var filtered = new List<string>();

            for ( var i = 0; i < sourceFiles.Length; i++ )
            {
               if ( options.FilePathFilter.Check( sourceFiles[i] ) )
               {
                  filtered.Add( sourceFiles[i] );
               }
            }

            sourceFiles = filtered.ToArray();

            for ( var i = 0; i < sourceFiles.Length; i++ )
            {
               var sourceFile = sourceFiles[i];

               // Get the full path of the target file using the root target folder and part of the source file's directory.
               var targetFile = this.GetFullTargetPath( sourceFile, sourcePathRoot, absoluteTargetPath );

               if ( this.NextFile != null )
               {
                  var arg = new NextFileArgs( sourceFile, targetFile, i, sourceFiles.Length );
                  this.NextFile( this, arg );

                  if ( String.Compare( arg.TargetFile, targetFile ) != 0 )
                  {
                     targetFile = Path.GetFullPath( arg.TargetFile );
                  }
               }

               var targetDirectory = Path.GetDirectoryName( targetFile );

               if ( !options.NoChange )
               {
                  if ( !Directory.Exists( targetDirectory ) && options.CreateTarget )
                  {
                     Directory.CreateDirectory( targetDirectory );
                  }

                  var sourceFileDataInfo = new FileDataInfo( sourceFile );
                  var targetFileDataInfo = new FileDataInfo( targetFile );

                  // Copy the file bytes.
                  var dataResult = this.fileDataCopier.Copy( sourceFileDataInfo, targetFileDataInfo, options );

                  this.LogFileDataResult( options.Logger, dataResult );

                  result.AddFileDataResult( dataResult );

                  if ( this.cancel )
                  {
                     options.Logger.WriteLine( "Cancelled by user." );
                     break;
                  }
               }
            }
         }
         finally
         {
            result.MarkEnd();

            lock ( this.sync )
            {
               if ( this.FileCopyEnded != null )
               {
                  this.FileCopyEnded( this, new FileCopyEndedArgs( result ) );
               }

               this.isBusy = false;
            }

            options.Logger.WriteLine( "{0} file{1} copied successfully, {2} skipped, {3} failed.", result.Successes.Count, result.Successes.Count == 1 ? string.Empty : "s", result.Skips.Count, result.Failures.Count );
            options.Logger.WriteLine( "Copy operation finished." );
         }

         if ( !result.Cancelled )
         {
            if ( !String.IsNullOrEmpty( options.DoneExecutePath ) )
            {
               this.ExecuteProcess( options );
            }
         }

         return result;
      }

      /// <summary>
      /// Gets the full path of the target file based on the source file path.
      /// </summary>
      /// <param name="sourceFile">The source file to copy.</param>
      /// <param name="targetFolder">The initial target folder.</param>
      /// <returns>The full path of the target file.</returns>
      private string GetFullTargetPath( string sourceFile, string sourceRoot, string targetFolder )
      {
         var relativeFilePath = new StringBuilder( sourceFile.Substring( sourceRoot.Length ) );

         // Remove any slashes at the beginning or they will cause Path.Combine()
         // to not return the correct result.
         while ( relativeFilePath[0] == '\\' )
         {
            relativeFilePath.Remove( 0, 1 );
         }

         // Remove the file name from the relative path so only the folders remain.
         var relativeFolderPath = Path.GetDirectoryName( relativeFilePath.ToString() );

         // Create the target folders if they do not exist and copy over the attributes and times
         // from the source folders.
         while ( relativeFolderPath.Length > 0 )
         {
            // Get the absolute path of the target folder.
            var targetAbsoluteFolderPath = Path.Combine( targetFolder, relativeFolderPath );

            // Create the target folder if it doesn't exist already.
            if ( !Directory.Exists( targetAbsoluteFolderPath ) )
            {
               Directory.CreateDirectory( targetAbsoluteFolderPath );
            }

            // Get the absolute path of the source folder.
            var sourceAbsoluteFolderPath = Path.Combine( sourceRoot, relativeFolderPath );

            // Copy the source folder's attributes and times to the target folder.
            FileEx.CopyAttributes( sourceAbsoluteFolderPath, targetAbsoluteFolderPath );
            FileEx.CopyTimes( sourceAbsoluteFolderPath, targetAbsoluteFolderPath );

            // "Go up" one level in the folder hierarchy.
            var lastSlash = relativeFolderPath.LastIndexOf( '\\' );

            if ( lastSlash < 0 )
            {
               lastSlash = 0;
            }

            relativeFolderPath = relativeFolderPath.Remove( lastSlash );
         }

         return Path.Combine( targetFolder, relativeFilePath.ToString() );
      }

      private void LogFileDataResult( Logger logger, IFileDataCopyResult result )
      {
         if ( result.Outcome == CopyOutcome.Successful )
         {
            logger.WriteLine( "Copied \"{0}\".", result.SourcePath );
         }
         else if ( result.Outcome == CopyOutcome.Skipped )
         {
            logger.WriteLine( "Skipped ({0}) \"{1}\".", result.SkipReason, result.SourcePath );
         }
         else if ( result.Outcome == CopyOutcome.Failed )
         {
            logger.WriteLine( LogLevel.Error, "Failed ({0}) \"{1}\".", result.Error.Message, result.SourcePath );
         }
      }

      private void ExecuteProcess( IFileCopyOptions options )
      {
         var execute = new ExecuteProcess();
         execute.ProcessHasOutput += ( sender, e ) =>
            {
               if ( this.ProcessHasOutput != null )
               {
                  if ( !String.IsNullOrEmpty( e.Data ) )
                  {
                     this.ProcessHasOutput( sender, e );
                  }
               }
            };
         execute.ProcessHasError += ( sender, e ) =>
         {
            if ( this.ProcessHasError != null )
            {
               if ( !String.IsNullOrEmpty( e.Data ) )
               {
                  this.ProcessHasError( sender, e );
               }
            }
         };
         execute.ProcessEnded += ( sender, e ) =>
            {
               if ( this.ProcessEnded != null )
               {
                  this.ProcessEnded( sender, e );
               }
            };

         options.Logger.WriteLine( "Executing done process." );
         var exitCode = execute.Run( options.DoneExecutePath, options.DoneArguments );
         options.Logger.WriteLine( "Done process execution finished with exit code: {0}.", exitCode );
      }
   }
}
