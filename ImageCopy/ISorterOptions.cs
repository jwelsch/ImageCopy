using System;

namespace ImageCopy
{
   internal interface ISorterOptions
   {
      string Source
      {
         get;
      }

      string Target
      {
         get;
      }

      SortCriteria Sort
      {
         get;
      }

      DateFormat DateFormat
      {
         get;
      }
   }
}
