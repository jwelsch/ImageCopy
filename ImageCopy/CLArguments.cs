using System;
using CommandLineLib;

namespace ImageCopy
{
   internal enum SortCriteria
   {
      DateTaken
   }

   internal class CLArguments
   {
      [DirectoryPathCompound( "-source", MustExist = true, Description = "Path to the directory containing image files to copy." )]
      public string Source
      {
         get;
         private set;
      }

      [DirectoryPathCompound( "-target", Description = "Path to the directory where the images will be copied." )]
      public string Target
      {
         get;
         private set;
      }

      [EnumCompound( "-sort", Description = "Criteria by which to sort the images." )]
      public SortCriteria Sort
      {
         get;
         private set;
      }

      [Switch( "-overwrite", Optional = true, Description = "Include to automatically overwrite files in the target directory." )]
      public bool Overwrite
      {
         get;
         private set;
      }
   }
}
