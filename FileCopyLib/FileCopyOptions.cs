using System;
using LogSystem;

namespace FileCopyLib
{
   #region IFileCopyOptions

   public enum FileDifference
   {
      Ignore,
      LastModifiedDate,
      Size
   }

   public interface IFileCopyOptions
   {
      /// <summary>
      /// Gets the path of the source directory or file being copied.
      /// </summary>
      string SourcePath
      {
         get;
      }

      /// <summary>
      /// Gets the path of the target path that is being copied to.
      /// </summary>
      string TargetDirectory
      {
         get;
      }

      /// <summary>
      /// Gets whether or not to overwrite existing files.
      /// </summary>
      bool Overwrite
      {
         get;
      }

      FileDifference Difference
      {
         get;
      }

      bool Recursive
      {
         get;
      }

      bool CreateTarget
      {
         get;
      }

      /// <summary>
      /// Gets file name filter.
      /// </summary>
      FilePathFilter FilePathFilter
      {
         get;
      }

      Logger Logger
      {
         get;
      }

      string DoneExecutePath
      {
         get;
      }

      string DoneArguments
      {
         get;
      }

      bool NoChange
      {
         get;
      }
   }

   #endregion

   #region FileCopyOptions

   /// <summary>
   /// Parameters passed to the FileCopyLib.
   /// </summary>
   public class FileCopyOptions : IFileCopyOptions
   {
      /// <summary>
      /// Gets the path of the source directory or file being copied.
      /// </summary>
      public string SourcePath
      {
         get;
         set;
      }

      /// <summary>
      /// Gets the path that the source files will be copied to.
      /// </summary>
      public string TargetDirectory
      {
         get;
         set;
      }

      /// <summary>
      /// Gets whether or not to overwrite existing files.
      /// </summary>
      public bool Overwrite
      {
         get;
         set;
      }

      public FileDifference Difference
      {
         get;
         set;
      }

      public bool Recursive
      {
         get;
         set;
      }

      public bool CreateTarget
      {
         get;
         set;
      }

      /// <summary>
      /// Gets file name filter.
      /// </summary>
      public FilePathFilter FilePathFilter
      {
         get;
         set;
      }

      public bool NoChange
      {
         get;
         set;
      }

      public Logger Logger
      {
         get;
         private set;
      }

      public string DoneExecutePath
      {
         get;
         private set;
      }

      public string DoneArguments
      {
         get;
         private set;
      }

      public FileCopyOptions()
      {
         this.FilePathFilter = new BlacklistFilePathFilter();
         this.Logger = new Logger();
      }
   }

   #endregion
}