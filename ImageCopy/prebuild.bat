@ECHO OFF
SET ConfigurationName=%~1
SET SolutionDir=%~2
SET ProjectDir=%~3
SET AviVer=1.0.6.0
SET VersionPart=Build
IF /I "%ConfigurationName%" == "Release" (
   SET VersionPart=Maintenance
)
@ECHO ON
"%SolutionDir%AssemblyVersionIncrement-%AviVer%\AssemblyVersionIncrement.exe" %VersionPart% "%ProjectDir%Properties\AssemblyInfo.cs"