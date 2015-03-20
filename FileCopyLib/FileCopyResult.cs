using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCopyLib
{
   public interface IFileCopyResult
   {
      ReadOnlyCollection<IFileDataCopyResult> Successes { get; }
      ReadOnlyCollection<IFileDataCopyResult> Failures { get; }
      ReadOnlyCollection<IFileDataCopyResult> Skips { get; }
      Int64 TotalBytesCopied { get; }
      DateTime BeginTime { get; }
      DateTime EndTime { get; }
      TimeSpan ElapsedTime { get; }
      bool Cancelled { get; }
   }

   public class FileCopyResult : IFileCopyResult
   {
      private bool successListChanged;
      private ReadOnlyCollection<IFileDataCopyResult> successes;
      private List<IFileDataCopyResult> successList = new List<IFileDataCopyResult>();

      private bool failureListChanged;
      private ReadOnlyCollection<IFileDataCopyResult> failures;
      private List<IFileDataCopyResult> failureList = new List<IFileDataCopyResult>();

      private bool skipListChanged;
      private ReadOnlyCollection<IFileDataCopyResult> skips;
      private List<IFileDataCopyResult> skipList = new List<IFileDataCopyResult>();

      private DateTime beginTime;
      private DateTime endTime;
      private TimeSpan elapsedTime;

      public void MarkBegin()
      {
         this.beginTime = DateTime.Now;
      }

      public void MarkEnd()
      {
         this.endTime = DateTime.Now;
      }

      public void AddFileDataResult( IFileDataCopyResult result )
      {
         switch ( result.Outcome )
         {
            case CopyOutcome.Successful:
            {
               this.AddSuccess( result );
               break;
            }
            case CopyOutcome.Skipped:
            {
               this.AddSkip( result );
               break;
            }
            case CopyOutcome.Failed:
            {
               this.AddFailure( result );
               break;
            }
            default:
            {
               throw new FileCopyException( String.Format( "Unknown copy outcome: {0}.", result.Outcome.ToString() ) );
            }
         }
      }

      private void AddSuccess( IFileDataCopyResult result )
      {
         this.successList.Add( result );
         this.successListChanged = true;
         this.TotalBytesCopied += result.BytesCopied;
      }

      private void AddFailure( IFileDataCopyResult result )
      {
         this.failureList.Add( result );
         this.failureListChanged = true;
      }

      private void AddSkip( IFileDataCopyResult result )
      {
         this.skipList.Add( result );
         this.skipListChanged = true;
      }

      #region IFileCopyResult Members

      public ReadOnlyCollection<IFileDataCopyResult> Successes
      {
         get
         {
            if ( this.successListChanged || ( this.successes == null ) )
            {
               this.successes = this.successList.AsReadOnly();
               this.successListChanged = false;
            }

            return this.successes;
         }
      }

      public ReadOnlyCollection<IFileDataCopyResult> Failures
      {
         get
         {
            if ( this.failureListChanged || ( this.failures == null ) )
            {
               this.failures = this.failureList.AsReadOnly();
               this.failureListChanged = false;
            }

            return this.failures;
         }
      }

      public ReadOnlyCollection<IFileDataCopyResult> Skips
      {
         get
         {
            if ( this.skipListChanged || ( this.skips == null ) )
            {
               this.skips = this.skipList.AsReadOnly();
               this.skipListChanged = false;
            }

            return this.skips;
         }
      }

      public Int64 TotalBytesCopied
      {
         get;
         set;
      }

      public DateTime BeginTime
      {
         get { return this.beginTime; }
      }

      public DateTime EndTime
      {
         get { return this.endTime; }
      }

      public TimeSpan ElapsedTime
      {
         get
         {
            if ( this.EndTime == DateTime.MinValue )
            {
               return DateTime.Now - this.BeginTime;
            }

            if ( this.elapsedTime == TimeSpan.Zero )
            {
               this.elapsedTime = this.EndTime - this.BeginTime;
            }

            return this.elapsedTime;
         }
      }

      public bool Cancelled
      {
         get;
         set;
      }

      #endregion
   }
}
