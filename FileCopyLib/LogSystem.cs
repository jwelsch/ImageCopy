using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogSystem
{
   #region LogSystem Support

   public enum LogLevel
   {
      Information,
      Warning,
      Error,
      Fatal
   }

   public class TimestampOptions
   {
      public bool Enable
      {
         get;
         set;
      }

      public bool UTC
      {
         get;
         set;
      }

      public string Format
      {
         get;
         set;
      }

      public TimestampOptions()
      {
         this.Enable = false;
         this.UTC = true;
         this.Format = "u";
      }
   }

   public class LogLevelOptions
   {
      public bool Enable
      {
         get;
         set;
      }

      public LogLevel Default
      {
         get;
         set;
      }

      public LogLevelOptions()
      {
         this.Enable = false;
         this.Default = LogLevel.Information;
      }
   }

   public interface ILogOutput
   {
      void Open();
      void Close();
      void Write( string text );
      void WriteLine( string text );
   }

   public class LogOutputCollection : FileCopyLib.BaseCollection<ILogOutput>
   {
   }

   #endregion

   #region Logger

   public class Logger //: IDisposable
   {
      public LogOutputCollection Outputs
      {
         get;
         private set;
      }

      public bool AutoCloseOutput
      {
         get;
         set;
      }

      public bool Enabled
      {
         get;
         set;
      }

      public LogLevelOptions LogLevel
      {
         get;
         private set;
      }

      public TimestampOptions Timestamp
      {
         get;
         private set;
      }

      private static string TimestampLogLevelTemplate = @"{0} [{1}]: {2}"; // 0 = Timestamp, 1 = Log Level
      private static string TimestampTemplate = @"{0}: {1}";
      private static string LogLevelTemplate = @"[{0}]: {1}";

      public Logger()
      {
         this.Outputs = new LogOutputCollection();
         this.LogLevel = new LogLevelOptions();
         this.Timestamp = new TimestampOptions();
      }

      ~Logger()
      {
         if ( this.AutoCloseOutput )
         {
            this.Close();
         }
      }

      private void AllOutputs( Action<ILogOutput> action )
      {
         if ( this.Outputs.Count > 0 )
         {
            this.Outputs.ForEach( i => action( i ) );
         }
      }

      public void Open()
      {
         this.AllOutputs( i => i.Open() );
      }

      public void Close()
      {
         this.AllOutputs( i => i.Close() );
      }

      public void Write( LogLevel level, string text, params object[] arguments )
      {
         if ( ( this.Outputs.Count > 0 ) && this.Enabled )
         {
            this.AllOutputs( i => i.Write( this.FormatText( level, text, arguments ) ) );
         }
      }

      public void Write( string text, params object[] arguments )
      {
         this.Write( this.LogLevel.Default, text, arguments );
      }

      public void WriteLine( LogLevel level, string text, params object[] arguments )
      {
         if ( ( this.Outputs.Count > 0 ) && this.Enabled )
         {
            this.AllOutputs( i => i.WriteLine( this.FormatText( level, text, arguments ) ) );
         }
      }

      public void WriteLine( string text, params object[] arguments )
      {
         this.WriteLine( this.LogLevel.Default, text, arguments );
      }

      public string FormatText( LogLevel level, string text, params object[] arguments )
      {
         var formattedText = String.Format( text, arguments );

         if ( this.LogLevel.Enable && this.Timestamp.Enable )
         {
            return String.Format( Logger.TimestampLogLevelTemplate, this.GetTimestamp(), this.MapLogLevel( level ), formattedText );
         }
         else if ( this.LogLevel.Enable )
         {
            return String.Format( Logger.LogLevelTemplate, this.MapLogLevel( level ), formattedText );
         }
         else if ( this.Timestamp.Enable )
         {
            return String.Format( Logger.TimestampTemplate, this.GetTimestamp(), formattedText );
         }

         return String.Format( text, arguments );
      }

      public string GetTimestamp()
      {
         return ( this.Timestamp.UTC ? DateTime.Now.ToUniversalTime() : DateTime.Now ).ToString( this.Timestamp.Format );
      }

      public string MapLogLevel( LogLevel level )
      {
         switch ( level )
         {
            case LogSystem.LogLevel.Information:
            {
               return "INFO";
            }
            case LogSystem.LogLevel.Warning:
            {
               return "WARN";
            }
            case LogSystem.LogLevel.Error:
            {
               return "ERROR";
            }
            case LogSystem.LogLevel.Fatal:
            {
               return "FATAL";
            }
            default:
            {
               return "UNKNOWN";
            }
         }
      }

      //#region IDisposable Members

      ///// <summary>
      ///// Implements the IDisposable.Dispose() method.
      ///// Provides an opportunity to execute clean up code.
      ///// </summary>
      //public void Dispose()
      //{
      //   // Clean up both managed and unmanaged resources.
      //   this.Dispose( true );

      //   // Prevent the garbage collector from calling Object.Finalize since
      //   // the clean up has already been done by the call to Dispose() above.
      //   GC.SuppressFinalize( this );
      //}

      ///// <summary>
      ///// Releases managed and optionally unmanaged resources.
      ///// </summary>
      ///// <param name="disposing">Pass true to release both managed and unmanaged resources,
      ///// false to release only unmanaged resources.</param>
      //protected virtual void Dispose( bool disposing )
      //{
      //   if ( disposing )
      //   {
      //      // Release managed resources here.
      //      if ( this.AutoCloseOutput )
      //      {
      //         this.Close();
      //      }
      //   }

      //   // Release unmanaged (native) resources here.
      //}

      //#endregion
   }

   #endregion

   #region FileLogOutput

   public class FileLogOutput : ILogOutput
   {
      public string FilePath
      {
         get;
         private set;
      }

      public bool Append
      {
         get;
         set;
      }

      public Encoding Encoding
      {
         get;
         set;
      }

      private StreamWriter writer;

      public FileLogOutput( string filePath, Encoding encoding, bool append )
      {
         this.FilePath = filePath;
         this.Append = append;
         this.Encoding = encoding;
      }

      public FileLogOutput( string filePath )
         : this( filePath, Encoding.UTF8, true )
      {
      }

      #region ILogOutput Members

      public void Open()
      {
         if ( this.writer == null )
         {
            this.writer = new StreamWriter( this.FilePath, this.Append, this.Encoding );
            this.writer.AutoFlush = true;
         }
      }

      public void Close()
      {
         if ( this.writer != null )
         {
            try
            {
               this.writer.Close();
            }
            finally
            {
               this.writer.Dispose();
               this.writer = null;
            }
         }
      }

      public void Write( string text )
      {
         if ( this.writer != null )
         {
            this.writer.Write( text );
         }
      }

      public void WriteLine( string text )
      {
         if ( this.writer != null )
         {
            this.writer.WriteLine( text );
         }
      }

      #endregion
   }

   #endregion

   #region ConsoleLogOutput

   public class ConsoleLogOutput : ILogOutput
   {
      #region ILogOutput Members

      public void Open()
      {
      }

      public void Close()
      {
      }

      public void Write( string text )
      {
         Console.Write( text );
      }

      public void WriteLine( string text )
      {
         Console.WriteLine( text );
      }

      #endregion
   }

   #endregion
}