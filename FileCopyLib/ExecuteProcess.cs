using System;
using System.Diagnostics;

namespace FileCopyLib
{
   #region ProcessEnded

   public class ProcessEndedArgs : EventArgs
   {
      public int ExitCode
      {
         get;
         private set;
      }

      public ProcessEndedArgs( int exitCode )
      {
         this.ExitCode = exitCode;
      }
   }

   public delegate void ProcessedEndedEventHandler( object sender, ProcessEndedArgs e );

   #endregion

   public class ExecuteProcess
   {
      public event DataReceivedEventHandler ProcessHasOutput;
      public event DataReceivedEventHandler ProcessHasError;
      public event ProcessedEndedEventHandler ProcessEnded;

      public int Run( string filePath, string arguments )
      {
         var startInfo = new ProcessStartInfo()
         {
            FileName = filePath,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardInput = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
         };

         var process = new Process();
         process.StartInfo = startInfo;

         this.AddDataEventHandlers( process, this.ProcessHasOutput, this.ProcessHasError );

         process.Start();
         process.BeginOutputReadLine();
         process.BeginErrorReadLine();
         process.WaitForExit();

         if ( this.ProcessEnded != null )
         {
            this.ProcessEnded( this, new ProcessEndedArgs( process.ExitCode ) );
         }

         this.RemoveDataEventHandlers( process, this.ProcessHasOutput, this.ProcessHasError );

         return process.ExitCode;
      }

      private void AddDataEventHandlers( Process process, DataReceivedEventHandler processHasOutput, DataReceivedEventHandler processHasError )
      {
         foreach ( var d in processHasOutput.GetInvocationList() )
         {
            process.OutputDataReceived += (DataReceivedEventHandler) d;
         }

         foreach ( var d in processHasError.GetInvocationList() )
         {
            process.ErrorDataReceived += (DataReceivedEventHandler) d;
         }
      }

      private void RemoveDataEventHandlers( Process process, DataReceivedEventHandler processHasOutput, DataReceivedEventHandler processHasError )
      {
         foreach ( var d in processHasOutput.GetInvocationList() )
         {
            process.OutputDataReceived -= (DataReceivedEventHandler) d;
         }

         foreach ( var d in processHasError.GetInvocationList() )
         {
            process.ErrorDataReceived -= (DataReceivedEventHandler) d;
         }
      }
   }
}
