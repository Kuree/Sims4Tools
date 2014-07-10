!define PROGRAM_NAME "C# Custom Classes"
!define tla "cscustom"
!ifndef TARGET
  !error "Caller didn't define TARGET"
!endif
!cd ${TARGET}

XPStyle on
SetCompressor /SOLID LZMA

Var wasInUse
Var wantAll


Name "${PROGRAM_NAME}"
InstallDir $COMMONFILES\s3pi


; Request application privileges for Windows Vista
RequestExecutionLevel admin

LicenseData "gpl-3.0.txt"

Page license
PageEx components
  ComponentText "Select the installation options.  Click Next to continue." " " " "
PageExEnd
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

Section "Install for all users"
  StrCpy $wantAll "Y"
SectionEnd

Section "-C# Custom Classes"
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
  
  File /a /x GetAssemblyInfo.exe *

  Call InstallTemplateConfig
SectionEnd


Section "Uninstall"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}"
  DeleteRegKey HKCU Software\s3pi\${tla}
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}"
  DeleteRegKey HKLM Software\s3pi\${tla}

  Delete $INSTDIR\System.Custom.dll
  Delete $INSTDIR\System.Custom.Template.Config
  Delete $INSTDIR\${tla}-Version.txt

  Delete $INSTDIR\uninst-${tla}.exe

SectionEnd


Function .onGUIInit
  Call GetInstDir
  Call CheckInUse
  Call CheckOldVersion
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

  IfFileExists "$INSTDIR\System.Custom.dll" Exists
  Return
Exists:
  ClearErrors
  FileOpen $0 "$INSTDIR\System.Custom.dll" a
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
  Exec "$R0"
NotInstalled:
  ClearErrors
FunctionEnd

!macro GetAssemblyInfo assembly badlabel handle
  nsExec::ExecToStack `GetAssemblyInfo.exe /dependentassembly "$INSTDIR\${assembly}.dll"`
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
  FileOpen $R0 System.Custom.Template.Config w
  FileWrite $R0 `<configuration>$\r$\n`
  FileWrite $R0 `  <runtime>$\r$\n`
  FileWrite $R0 `    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">$\r$\n`
  ${gai} System.Custom broken $R0
  FileWrite $R0 `    </assemblyBinding>$\r$\n`
  FileWrite $R0 `  </runtime>$\r$\n`
  FileWrite $R0 `</configuration>$\r$\n`
  FileClose $R0
  CopyFiles /SILENT System.Custom.Template.Config $INSTDIR
  Goto okay
broken:
  SetDetailsView show
  MessageBox MB_OK|MB_ICONEXCLAMATION "Something went wrong getting assembly info: $0"
okay:
  Delete System.Custom.Template.Config
  Delete GetAssemblyInfo.exe
  SetOutPath -
  Pop $0
  Pop $R0
FunctionEnd
