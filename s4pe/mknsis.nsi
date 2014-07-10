;!include "MUI.nsh"
!include "FileAssociation.nsh"

!define tla "s3pe"
!ifndef INSTFILES
  !error "Caller didn't define INSTFILES"
!endif
!ifndef UNINSTFILES
  !error "Caller didn't define UNINSTFILES"
!endif
!ifndef VSN
  !error "Caller didn't define VSN"
!endif

Var wasInUse
Var wantAll
Var wantSM
Var wantAssoc
Var wantSendTo

InstallDir "$PROGRAMFILES64\${tla}"
!define PROGRAM_NAME "s3pe - Sims3 Package Editor"
!define INSTREGKEY "${tla}"
!define SMDIR "$SMPROGRAMS\${tla}"
!define EXE ${tla}.exe
!define LNK "${tla}.lnk"

SetCompressor /SOLID LZMA
XPStyle on
Name "${PROGRAM_NAME}"
AddBrandingImage top 0
Icon Resources\${tla}.ico
UninstallIcon Resources\${tla}.ico

; Request application privileges for Windows Vista and above
RequestExecutionLevel admin

LicenseData "gpl-3.0.txt"
Page license
;!insertmacro MUI_PAGE_LICENSE "gpl-3.0.txt"

PageEx components
  ComponentText "Select the installation options.  Click Next to continue." " " " "
PageExEnd
Page directory
;Var StartMenuFolder
;!insertmacro MUI_PAGE_STARTMENU "Application" $StartMenuFolder
Page instfiles

Section "Install for all users"
  StrCpy $wantAll "Y"
SectionEnd

Section "Create Start Menu entry"
  StrCpy $wantSM "Y"
SectionEnd

Section /o "Associate *.package with s3pe?"
  StrCpy $wantAssoc "Y"
SectionEnd

Section /o "Create SendTo for Import? (current user only)"
  StrCpy $wantSendTo "Y"
SectionEnd


Section
  SetShellVarContext all
  StrCmp "Y" $wantAll gotAll
  SetShellVarContext current
gotAll:  

  SetOutPath $INSTDIR
  
  !include ${INSTFILES}
  IntOp $0 $0 / 1024

  WriteUninstaller uninst-${tla}.exe
  
  ; Write the uninstall keys for Windows
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "DisplayIcon" "$INSTDIR\${EXE}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "DisplayName" "${PROGRAM_NAME}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "DisplayVersion" "${VSN}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "HelpLink" "http://www.den.simlogical.com/denforum/index.php?board=19.0"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "Publisher" "Peter L Jones"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "UninstallString" '"$INSTDIR\uninst-${tla}.exe"'
  ; $0 is set in ${INSTFILES} by the batch file...
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "EstimatedSize" $0
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "NoModify" 1
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "NoRepair" 1

  StrCmp "Y" $wantSM wantSM noWantSM
wantSM:
  CreateDirectory "${SMDIR}"
  CreateShortCut "${SMDIR}\${LNK}" "$INSTDIR\${EXE}" "" "" "" SW_SHOWNORMAL "" "${PROGRAM_NAME}"
  CreateShortCut "${SMDIR}\Uninstall.lnk" "$INSTDIR\uninst-${tla}.exe" "" "" "" SW_SHOWNORMAL "" "Uninstall"
  CreateShortCut "${SMDIR}\${tla}-Version.lnk" "$INSTDIR\${tla}-Version.txt" "" "" "" SW_SHOWNORMAL "" "Show version"
noWantSM:

  StrCmp "Y" $wantAssoc wantAssoc noWantAssoc
wantAssoc:
  ${registerExtension} "$INSTDIR\${EXE}" ".package" "Sims3 package"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "RemoveAssociation" "Y"
noWantAssoc:

  StrCmp "Y" $wantSendTo wantSendTo noWantSendTo
wantSendTo:
  CreateShortCut "$SENDTO\Import to Sims3 package.lnk" "$INSTDIR\${EXE}" "-import" "" "" SW_SHOWNORMAL "" "Import files to a new Sims3 package"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "RemoveSendTo" "Y"
noWantSendTo:

SectionEnd

Function .onGUIInit
  SetOutPath $TEMP
  File ..\Resources\${tla}.ico
  SetBrandingImage $TEMP\${tla}.ico
  Delete $TEMP\${tla}.ico
  Call GetInstDir
  Call CheckInUse
  Call CheckOldVersion
FunctionEnd

Function GetInstDir
  Push $0
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"
  StrCmp $0 "" gidNotCU
  IfFileExists "$0${EXE}" gidSetINSTDIR
gidNotCU:
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"
  StrCmp $0 "" gidDone
  IfFileExists "$0${EXE}" gidSetINSTDIR gidDone
gidSetINSTDIR:
  StrCpy $INSTDIR $0
gidDone:
  Pop $0
  ClearErrors
FunctionEnd

Function CheckInUse
  StrCpy $wasInUse 0
cuiRetry:
  IfFileExists "$INSTDIR\${EXE}" cuiExists
  Return
cuiExists:
  ClearErrors
  FileOpen $0 "$INSTDIR\${EXE}" a
  IfErrors cuiInUse
  FileClose $0
  Return
cuiInUse:
  StrCpy $wasInUse 1

  MessageBox MB_RETRYCANCEL|MB_ICONQUESTION \
    "${EXE} is running.$\r$\nPlease close it and retry.$\r$\n$INSTDIR\${EXE}" \
    IDRETRY cuiRetry

  MessageBox MB_OK|MB_ICONSTOP "Cannot continue to install if ${EXE} is running."
  Quit
FunctionEnd

Function CheckOldVersion
  Push $0

  StrCpy $0 "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}"
  StrCpy $R0 ""
  Call CheckOldVersionHKCU
  Call CheckOldVersionHKLM

  StrCpy $0 "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla} (x64)"
  StrCpy $R0 "64-bit "
  Call CheckOldVersionHKCU
  Call CheckOldVersionHKLM

  ClearErrors
  Pop $0
FunctionEnd

Function CheckOldVersionHKCU
  ReadRegStr $R1 HKCU $0 "UninstallString"
  ReadRegStr $R2 HKCU $0 "InstallLocation"
  StrCmp $R1 "" covNotCU
  Call UninstallOldVersion
  IfErrors covNotCU
  DeleteRegKey HKCU $0
covNotCU:
FunctionEnd

Function CheckOldVersionHKLM
  ReadRegStr $R1 HKLM $0 "UninstallString"
  ReadRegStr $R2 HKLM $0 "InstallLocation"
  StrCmp $R1 "" covNotLM
  Call UninstallOldVersion
  IfErrors covNotLM
  DeleteRegKey HKLM $0
covNotLM:
FunctionEnd


Function UninstallOldVersion
  Push $0

  StrCpy $0 $R1 -1 1
  IfFileExists $0 uovExists
  MessageBox MB_OK "'$0' not found.$\n$\nPlease clean your registry$\n(or reinstall the old version and retry)." IDOK
  Goto uovDone

uovExists:
  MessageBox MB_OKCANCEL|MB_ICONQUESTION \
    "An old $R0version of ${PROGRAM_NAME} is installed.$\n$\nClick [OK] to remove it or [Cancel] to abort." \
    IDOK uovUninstall
  Quit

uovUninstall:
  ClearErrors
  ExecWait '$R1 /S _?=$R2'
  IfErrors uovFail
  Delete $0
  Goto uovDone
uovFail:
  MessageBox MB_OK "Failed to uninstall $R0version from\n$R2\n\nPlease manually tidy up." IDOK

uovDone:
  Pop $0
FunctionEnd



Function un.onInit
  Call un.GetInstDir
  StrCmp $INSTDIR "" 0 unoiGotInstDir
  Abort
unoiGotInstDir:
  IfSilent 0 unoiNotSilent1
  Call un.testInUse
  StrCmp $wasInUse 1 0 unoiNotSilent1
  Abort
unoiNotSilent1:
  Call un.GetWantAssoc
  Call un.GetWantSendTo
FunctionEnd

Function un.OnGUIInit
  StrCmp $INSTDIR "" 0 unogiGotInstDir
  MessageBox MB_OK|MB_ICONSTOP "Cannot find Install Location."
  Abort

unogiGotInstDir:
  Call un.CheckInUse
FunctionEnd

Function un.GetInstDir
  Push $0

  SetShellVarContext all

  ClearErrors
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"

  IfErrors notCU
  SetShellVarContext current
notCU:  
  ClearErrors

  ReadRegStr $0 SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"

  StrCmp $0 "" ungidDone
  IfFileExists "$0" 0 ungidDone
  StrCpy $INSTDIR $0

ungidDone:
  Pop $0
FunctionEnd

Function un.CheckInUse
uncuiRetry:
  Call un.testInUse
  StrCmp $wasInUse 1 0 ciuNotInUse

  MessageBox MB_RETRYCANCEL|MB_ICONQUESTION \
    "${EXE} is running.$\r$\nPlease close it and retry.$\r$\n$INSTDIR\${EXE}" \
    IDRETRY uncuiRetry

  MessageBox MB_OK|MB_ICONSTOP "Cannot continue to uninstall if ${EXE} is running."
  Abort

ciuNotInUse:
FunctionEnd

Function un.testInUse
  StrCpy $wasInUse 0

  ClearErrors
  FileOpen $0 "$INSTDIR\${EXE}" a
  IfErrors untiuInUse
  FileClose $0
  Return

untiuInUse:
  StrCpy $wasInUse 1
FunctionEnd

Function un.GetWantAssoc
  Push $0
  ReadRegStr $0 SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "RemoveAssociation"
  StrCmp $0 "" ungwaDone
  StrCpy $wantAssoc $0
ungwaDone:
  Pop $0
FunctionEnd

Function un.GetWantSendTo
  Push $0
  ReadRegStr $0 SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "RemoveSendTo"
  StrCmp $0 "" ungwstDone
  StrCpy $wantSendTo $0
ungwstDone:
  Pop $0
FunctionEnd

UninstPage uninstConfirm
PageEx un.components
  ComponentText "Select the uninstallation options.  Click Next to continue." " " " "
PageExEnd
UninstPage instfiles

Section "Uninstall"

  DeleteRegKey SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}"
  DeleteRegKey SHCTX Software\s3pi\${tla}

  RMDir /r "${SMDIR}"

  StrCmp "Y" $wantAssoc wantAssoc noWantAssoc
wantAssoc:
  ${unregisterExtension} ".package" "Sims3 package"
noWantAssoc:

  StrCmp "Y" $wantSendTo wantSendTo noWantSendTo
wantSendTo:
  Delete "$SENDTO\Import to Sims3 package.lnk"
noWantSendTo:

  !include ${UNINSTFILES}
  Delete $INSTDIR\uninst-${tla}.exe
  RMDir $INSTDIR ; safe - will not delete unless folder empty
SectionEnd