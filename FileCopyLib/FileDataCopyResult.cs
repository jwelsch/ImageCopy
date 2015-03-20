using System;

namespace FileCopyLib
{
   public enum CopyOutcome
   {
      None,
      Successful,
      Failed,
      Skipped,
      Cancelled
   }

   public enum SkipReason
   {
      None,
      Overwrite,
      LastModifiedDate,
      Size,
      Filter
   }

   public interface IFileDataCopyResult
   {
      string SourcePath { get; }
      string TargetPath { get; }
      CopyOutcome Outcome { get; }
      Exception Error { get; }
      SkipReason SkipReason { get; }
      Int64 BytesCopied { get; }
   }

   public class FileDataCopyResult : IFileDataCopyResult
   {
      private CopyOutcome outcome = CopyOutcome.Successful;
      private SkipReason skipReason = SkipReason.None;

      public void Fail( Exception error )
      {
         this.Error = error;
         this.outcome = CopyOutcome.Failed;
      }

      public void Cancel()
      {
         this.outcome = CopyOutcome.Cancelled;
      }

      public void Skip( SkipReason reason )
      {
         this.skipReason = reason;
         this.outcome = CopyOutcome.Skipped;
      }

      #region IFileDataCopyResult Members

      public string SourcePath
      {
         get;
         set;
      }

      public string TargetPath
      {
         get;
         set;
      }

      public CopyOutcome Outcome
      {
         get { return this.outcome; }
      }

      public Exception Error
      {
         get;
         private set;
      }

      public SkipReason SkipReason
      {
         get;
         private set;
      }

      public Int64 BytesCopied
      {
         get;
         set;
      }

      #endregion
   }
}
