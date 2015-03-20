using System;
using CommandLineLib;

namespace ImageCopy
{
   internal class CLArguments : ISorterOptions
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

      [Switch( "-recursive", Optional = true, Description = "Include to recurse through subdirectories contained in \"-source\"." )]
      public bool Recursive
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

      [EnumCompound( "-dateFormat", Optional = true, Description = "Specifies the format of the date in the directory name.  Default is YMD." )]
      public DateFormat DateFormat
      {
         get;
         private set;
      }

      [Switch( "-overwrite", Optional = true, Description = "Include to automatically overwrite files in the target directory.  Default is to not overwrite." )]
      public bool Overwrite
      {
         get;
         private set;
      }

      [Switch( "-noChange", Optional = true, Description = "Instructs the program to do everything except do the actual copy.  Useful to testing the output before actually copying files." )]
      public bool NoChange
      {
         get;
         private set;
      }
   }
}
