@echo off
set TargetName=s3pi
set ConfigurationName=Release
set base=%TargetName%
rem -%ConfigurationName%
set src=%TargetName%-Source
set help=..\s3pi Doc\Help\s3piHelp.chm

set out=S:\Sims3\Tools\s3pi\

set mydate=%date: =0%
set dd=%mydate:~0,2%
set mm=%mydate:~3,2%
set yy=%mydate:~8,2%
set mytime=%time: =0%
set h=%mytime:~0,2%
set m=%mytime:~3,2%
set s=%mytime:~6,2%
set suffix=%yy%-%mm%%dd%-%h%%m%
	
set nsisv=/V3

if x%ConfigurationName%==xRelease goto REL
set pdb=
goto noREL
:REL:
set pdb=-xr!*.pdb
:noREL:


rem there shouldn't be any to delete...
del %out%%TargetName%*%suffix%.*

pushd ..
7za a -r -t7z -mx9 -ms -xr!.?* -xr!*.suo -xr!bin -xr!obj -xr!xml -xr!Makefile "%out%%src%_%suffix%.7z" "CS System Classes" s3pi "s3pi Extras" "s3pi Wrappers"
popd


mkdir %base%
copy "build\bin\%ConfigurationName%\*" %base%
if exist "%help%" copy "%help%" %base%

pushd %base%
echo %suffix% >%TargetName%-Version.txt
attrib +r %TargetName%-Version.txt
7za a -r -t7z -mx9 -ms -xr!.?* -xr!build.* %pdb% "%out%%base%_%suffix%.7z" *
del /f %TargetName%-Version.txt
popd

rem 7za x -o"%base%-%suffix%" "%out%%base%_%suffix%.7z"
rem No point copying anything but Release as we don't install this:
rem copy ..\GetAssemblyInfo\bin\Release\GetAssemblyInfo.exe "%base%-%suffix%"
rem "%PROGRAMFILES%\nsis\makensis" "/DTARGET=%base%-%suffix%" %nsisv% mknsis.nsi "/XOutFile %out%%base%_%suffix%.exe"

rmdir /s/q %base%
rem rmdir /s/q %base%-%suffix%
pause
