using System;
using System.IO;

namespace FileCopyLib
{
   /// <summary>
   /// Contains information used by IFileDataCopier implementers.
   /// </summary>
   internal class FileDataInfo
   {
      /// <summary>
      /// Gets the file path.
      /// </summary>
      public string FilePath
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the attributes of the file.
      /// </summary>
      public FileAttributes Attributes
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets whether or not the file is read only.
      /// </summary>
      public bool IsReadOnly
      {
         get { return ( this.Attributes & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly; }
      }

      /// <summary>
      /// Gets whether or not the file is hidden.
      /// </summary>
      public bool IsHidden
      {
         get { return ( this.Attributes & FileAttributes.Hidden ) == FileAttributes.Hidden; }
      }

      /// <summary>
      /// Gets whether or not the file is a system file.
      /// </summary>
      public bool IsSystem
      {
         get { return ( this.Attributes & FileAttributes.System ) == FileAttributes.System; }
      }

      /// <summary>
      /// Gets the size of the file in bytes.
      /// </summary>
      public Int64 Size
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the creation time of the file.
      /// </summary>
      public DateTime CreationTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the last write time of the file.
      /// </summary>
      public DateTime LastWriteTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the last access time of the file.
      /// </summary>
      public DateTime LastAccessTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets whether or not the file exists in the file system.
      /// </summary>
      public bool Exists
      {
         get;
         private set;
      }

      /// <summary>
      /// Creates an object of type FileDataInfo.
      /// </summary>
      /// <param name="filePath">The path of the file.</param>
      public FileDataInfo( string filePath )
      {
         this.FilePath = filePath;
         this.LoadFilePath();
      }

      /// <summary>
      /// Refreshes the file information.
      /// </summary>
      public void Refresh()
      {
         this.LoadFilePath();
      }

      /// <summary>
      /// Loads the object with information on the file.
      /// </summary>
      private void LoadFilePath()
      {
         this.Exists = File.Exists( this.FilePath );

         if ( this.Exists )
         {
            this.Attributes = File.GetAttributes( this.FilePath );

            this.CreationTime = File.GetCreationTime( this.FilePath );
            this.LastWriteTime = File.GetLastWriteTime( this.FilePath );
            this.LastAccessTime = File.GetLastAccessTime( this.FilePath );

            this.Size = ( new FileInfo( this.FilePath ) ).Length;
         }
      }
   }
}
