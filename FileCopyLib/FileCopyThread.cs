using System;
using System.Threading;

namespace FileCopyLib
{
   /// <summary>
   /// Represents the thread that does the file copying.
   /// </summary>
   internal class FileCopyThread
   {
      /// <summary>
      /// The thread that does the work.
      /// </summary>
      private Thread thread;

      /// <summary>
      /// Does the work of copying files.
      /// </summary>
      private FileCopier FileCopyLib;

      /// <summary>
      /// Parameters to pass to the FileCopyLib.
      /// </summary>
      private FileCopyOptions parameters;

      /// <summary>
      /// Creates an object of type FileCopyThread.
      /// </summary>
      public FileCopyThread()
      {
      }

      /// <summary>
      /// Starts the copy operation.
      /// </summary>
      /// <param name="FileCopyLib">Does the work of copying files.</param>
      /// <param name="parameters">Parameters to control the file copying.</param>
      public void StartCopy( FileCopier FileCopyLib, FileCopyOptions parameters )
      {
         this.FileCopyLib = FileCopyLib;
         this.parameters = parameters;

         this.thread = new Thread( new ParameterizedThreadStart( FileCopyThread.DoCopy ) );
         this.thread.Start( this );
      }

      /// <summary>
      /// Thread procedure.
      /// </summary>
      /// <param name="param">The object that called Thread.Start().</param>
      private static void DoCopy( object param )
      {
         var fileCopyThread = (FileCopyThread) param;

         fileCopyThread.FileCopyLib.Copy( fileCopyThread.parameters );
      }
   }
}
