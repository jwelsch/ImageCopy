using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FileCopyLib
{
   /// <summary>
   /// Represents a filter for what files to copy based on their name.
   /// </summary>
   public abstract class FilePathFilter
   {
      /// <summary>
      /// Takes a string containing multiple Windows wildcard filters and parses them into an
      /// IEnumerable&lt;string&gt; containing regular expressions.
      /// </summary>
      /// <param name="filterString">String containing multiple Windows wildcard filters.</param>
      /// <returns>IEnumerable&lt;string&gt; containing regular expressions.</returns>
      public static IEnumerable<string> ParseFilterString( string filterString )
      {
         var patterns = new List<string>();
         var filters = filterString.Split( '|' );

         var questionReplacement = "dot361Operator";
         var asterisksReplacement = "matchAll516Operator";

         foreach ( var filter in filters )
         {
            var s1 = filter.Replace( "?", questionReplacement );
            var s2 = filter.Replace( "*", asterisksReplacement );

            s2 = Regex.Escape( s2 );
            s1 = s2.Replace( questionReplacement, "." );
            s2 = s1.Replace( asterisksReplacement, ".*" );
            patterns.Add( s2 );
         }

         return filters;
      }

      private List<Regex> regexList = new List<Regex>();
      private bool emptyMapping;

      /// <summary>
      /// Creates an object of type FilePathFilter.
      /// </summary>
      /// <param name="type">The type of filter.</param>
      /// <param name="patterns">The file name patterns.</param>
      public FilePathFilter( IEnumerable<string> patterns )
      {
         if ( patterns != null )
         {
            foreach ( var pattern in patterns )
            {
               this.regexList.Add( new Regex( pattern ) );
            }
         }

         if ( this.regexList.Count == 0 )
         {
            this.emptyMapping = this.MapEmpty();
         }
      }

      public virtual bool Check( string path )
      {
         if ( this.regexList.Count == 0 )
         {
            return this.emptyMapping;
         }

         var match = false;

         foreach ( var regex in this.regexList )
         {
            if ( regex.IsMatch( path ) )
            {
               match = true;
               break;
            }
         }

         return this.MapRegexMatch( match );
      }

      /// <summary>
      /// Maps a call to Regex.IsMatch to what that means for a subclassed filter.
      /// </summary>
      /// <param name="regexMatch">Result from Regex.IsMatch</param>
      /// <returns>True if the path should be included, false otherwise.</returns>
      protected abstract bool MapRegexMatch( bool regexMatch );

      /// <summary>
      /// Maps what should happen when there are no patterns to match.
      /// </summary>
      /// <returns>True if all paths should be included, false otherwise.</returns>
      protected abstract bool MapEmpty();
   }

   #region WhitelistFilePathFilter

   public class WhitelistFilePathFilter : FilePathFilter
   {
      public WhitelistFilePathFilter( IEnumerable<string> patterns )
         : base( patterns )
      {
      }

      protected override bool MapRegexMatch( bool regexMatch )
      {
         return regexMatch;
      }

      protected override bool MapEmpty()
      {
         return false;
      }
   }

   #endregion

   #region BlacklistFilePathFilter

   public class BlacklistFilePathFilter : FilePathFilter
   {
      public BlacklistFilePathFilter()
         : this( null )
      {
      }

      public BlacklistFilePathFilter( IEnumerable<string> patterns )
         : base( patterns )
      {
      }

      protected override bool MapRegexMatch( bool regexMatch )
      {
         return !regexMatch;
      }

      protected override bool MapEmpty()
      {
         return true;
      }
   }

   #endregion
}
