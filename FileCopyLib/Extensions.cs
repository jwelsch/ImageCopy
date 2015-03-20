using System;

namespace FileCopyLib
{
   public static class Extensions
   {
      public static void ForEach<T>( this T[] array, Action<T> proc )
      {
         if ( proc != null )
         {
            foreach ( var i in array )
            {
               proc( i );
            }
         }
      }
   }
}
