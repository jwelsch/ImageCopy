using System;

namespace FileCopyLib
{
   /// <summary>
   /// Thrown when there is an unrecoverable problem during a file copy operation.
   /// </summary>
   [Serializable]
   public class FileCopyException : Exception
   {
      /// <summary>
      /// Constructs an object of type FileCopyException.
      /// </summary>
      public FileCopyException()
      {
      }

      /// <summary>
      /// Constructs an object of type FileCopyException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      public FileCopyException( string message )
         : this( message, null )
      {
      }

      /// <summary>
      /// Constructs an object of type FileCopyException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="innerException">Another exception associated with the FileCopyException.</param>
      public FileCopyException( string message, Exception innerException )
         : base( message, innerException )
      {
      }

      /// <summary>
      /// Constructs a FileCopyException object.
      /// </summary>
      /// <param name="info">The serialization information.</param>
      /// <param name="context">The streaming context.</param>
      protected FileCopyException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
         : base( info, context )
      {
      }
   }
}
