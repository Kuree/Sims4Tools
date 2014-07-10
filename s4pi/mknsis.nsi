!define PROGRAM_NAME "Sims3 Package Interface"
!define tla "s3pi"
!ifndef TARGET
  !error "Caller didn't define TARGET"
!endif
!cd ${TARGET}

XPStyle on
SetCompressor /SOLID LZMA

Var wasInUse
Var wantAll
Var haveCustom

Name "${PROGRAM_NAME}"
InstallDir $COMMONFILES\s3pi


; Request application privileges for Windows Vista
RequestExecutionLevel admin

LicenseData "gpl-3.0.txt"

Page license
PageEx components
  ComponentText "Select the installation options and components you want to install and uncheck those you don't want.  Click Next to continue." "" "Or, select the options and components:"
PageExEnd
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

InstType "Full (recommended)"
InstType "Minimal"

Section "Install for all users"
  SectionIn 1 2
  StrCpy $wantAll "Y"
SectionEnd

Section "-Core s3pi Library"
  SectionIn 1 2 RO

  SetShellVarContext all
  StrCmp "Y" $wantAll gotAll
  SetShellVarContext current
gotAll:  

  SetOutPath $INSTDIR
  ; Write the installation path into the registry
  WriteRegStr HKLM Software\s3pi\${tla} "InstallDir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "DisplayName" "${PROGRAM_NAME}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "UninstallString" '"$INSTDIR\uninst-${tla}.exe"'
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "NoModify" 1
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "NoRepair" 1

  WriteUninstaller uninst-${tla}.exe

  StrCmp "" $haveCustom NotHaveCustom HaveCustom
NotHaveCustom:
  File /a System.Custom.dll
HaveCustom:
  File /a /x *Resources.txt /x *Resource.dll DemoPlugins.txt Extensions.txt s3pi.* ${tla}-Version.txt
  File /a s3pi.DefaultResource.dll

  Call InstallTemplateConfig
SectionEnd

Section "Additional Resource Wrappers"
  SectionIn 1
  File /a /x s3pi.DefaultResource.dll *Resources.txt *Resource.dll
SectionEnd


Section "Uninstall"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}"
  DeleteRegKey HKCU Software\s3pi\${tla}
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}"
  DeleteRegKey HKLM Software\s3pi\${tla}
 
  Delete $INSTDIR\*Resources.txt
  Delete $INSTDIR\DemoPlugins.txt
  Delete $INSTDIR\Extensions.txt
  Delete $INSTDIR\s3pi.*.dll
  Delete $INSTDIR\s3pi.Template.Config
  Delete $INSTDIR\${tla}-Version.txt

  Delete $INSTDIR\uninst-${tla}.exe

SectionEnd


Function .onGUIInit
  Call GetInstDir
  Call CheckInUse
  Call CheckOldVersion
  Call CheckCustom
FunctionEnd

Function GetInstDir
  Push $0
  ReadRegStr $0 HKLM Software\s3pi\${tla} "InstallDir"
  StrCmp $0 "" NotInstalledLM
  StrCpy $INSTDIR $0
  Goto InstDirDone
NotInstalledLM:
  ReadRegStr $R0 HKCU Software\s3pi\${tla} "InstallDir"
  StrCmp $0 "" InstDirDone
  StrCpy $INSTDIR $0
InstDirDone:
  Pop $0
FunctionEnd

Function CheckInUse
  StrCpy $wasInUse 0

  IfFileExists "$INSTDIR\s3pi.Interfaces.dll" Exists
  Return
Exists:
  ClearErrors
  FileOpen $0 "$INSTDIR\s3pi.Interfaces.dll" a
  IfErrors InUse
  FileClose $0
  Return
InUse:
  StrCpy $wasInUse 1

  MessageBox MB_RETRYCANCEL|MB_ICONQUESTION \
    "A program is running that uses the library.$\r$\nPlease close it and retry." \
    IDRETRY Exists

  MessageBox MB_OK|MB_ICONSTOP "Cannot continue to install if the library is in use."
  Quit
FunctionEnd

Function CheckOldVersion
  ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "UninstallString"
  StrCmp $R0 "" NotInstalledCU Installed
NotInstalledCU:
  ReadRegStr $R0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "UninstallString"
  StrCmp $R0 "" NotInstalled
Installed:
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
    "${PROGRAM_NAME} is already installed. $\n$\nClick [OK] to remove the previous version or [Cancel] to abort this upgrade." \
    IDOK UnInstall
  Quit
UnInstall:
  ExecWait "$R0"
NotInstalled:
  ClearErrors
FunctionEnd

Function CheckCustom
  Push $0
  StrCpy $haveCustom ""
  ReadRegStr $0 HKCU Software\s3pi\cscustom "InstallDir"
  StrCmp "" $0 NoCustomLM
  IfFileExists "$0\System.Custom.dll" HaveCustom DoneCheckCustom
NoCustomLM:
  ReadRegStr $0 HKLM Software\s3pi\cscustom "InstallDir"
  StrCmp "" $0 DoneCheckCustom
  IfFileExists "$0\System.Custom.dll" HaveCustom DoneCheckCustom
HaveCustom:
  StrCpy $haveCustom "$0\System.Custom.dll"
DoneCheckCustom:
  Pop $0
FunctionEnd

!macro GetAssemblyInfo assembly badlabel handle
  nsExec::ExecToStack `GetAssemblyInfo.exe /bindingredirect /dependentassembly "$INSTDIR\${assembly}.dll"`
  Pop $0
  StrCmp "error" $0 ${badlabel}
  IntCmpU 1 $0 ${badlabel} ${badlabel}
  Pop $0
  FileWrite ${handle} $0
  DetailPrint "GetAssemblyInfo ${assembly} OK"
!macroend
!define gai "!insertmacro GetAssemblyInfo"
;; $INSTDIR\s3pi.Template.Config
Function InstallTemplateConfig
  Push $R0
  Push $0
  SetOutPath $TEMP
  File GetAssemblyInfo.exe
  FileOpen $R0 s3pi.Template.Config w
  FileWrite $R0 `<configuration>$\r$\n`
  FileWrite $R0 `  <runtime>$\r$\n`
  FileWrite $R0 `    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">$\r$\n`
  ${gai} System.Custom broken $R0
  ${gai} s3pi.DemoPlugins broken $R0
  ${gai} s3pi.Extensions broken $R0
  ${gai} s3pi.Interfaces broken $R0
  ${gai} s3pi.Package broken $R0
  ${gai} s3pi.Settings broken $R0
  ${gai} s3pi.WrapperDealer broken $R0
  FileWrite $R0 `    </assemblyBinding>$\r$\n`
  FileWrite $R0 `  </runtime>$\r$\n`
  FileWrite $R0 `</configuration>$\r$\n`
  FileClose $R0
  CopyFiles /SILENT s3pi.Template.Config $INSTDIR
  Goto okay
broken:
  SetDetailsView show
  Pop $0
  DetailPrint "Something went wrong getting assembly info: $0"
  MessageBox MB_OK|MB_ICONEXCLAMATION "Something went wrong getting assembly info: $0"
okay:
  Delete s3pi.Template.Config
  Delete GetAssemblyInfo.exe
  SetOutPath -
  Pop $0
  Pop $R0
FunctionEnd
