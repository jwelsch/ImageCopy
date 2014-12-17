@ECHO OFF
SET ConfigurationName=%~1
SET SolutionDir=%~2
SET ProjectDir=%~3
SET TargetDir=%~4
SET TargetPath=%~5
SET TargetName=%~6
IF /I "%ConfigurationName%" == "Release" (
  @ECHO ON
  "%SolutionDir%packages\ilmerge.2.14.1208\tools\ILMerge.exe" /out:"%TargetDir%%TargetName%M.exe" "%TargetPath%" "%TargetDir%CommandLineLib.dll" "%SolutionDir%packages\ExifLib.1.6.3.0\lib\net45\ExifLib.dll" "%SolutionDir%FileCopyLib-1.0.0.0\FileCopyLib.dll"
  move /Y "%TargetDir%%TargetName%M.exe" "%TargetPath%"
  move /Y "%TargetDir%%TargetName%M.pdb" "%TargetDir%%TargetName%.pdb"
  del "%TargetDir%CommandLineLib.dll"
)