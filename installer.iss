; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "ARGB Control"
#define MyAppVersion "0.3"
#define MyAppPublisher "KrikCo"
#define MyAppURL "https://krik.co"
#define MyAppExeName "ARGBControl.exe"
#define PublishPath "src\argb-control\bin\Release\netcoreapp3.1\publish\win-x64"
#define StartupTaskName "Start KrikHome ARGB Control"

[Setup]
AppId={{44498B5D-1DCA-462D-8FB2-EE2B5CC1D99A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir={#PublishPath}\setup
OutputBaseFilename=argb-control-setup-v{#MyAppVersion}
SetupIconFile={#PublishPath}\bulb.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked  
Name: "startwithwindows"; Description: "Start app when Windows starts";

[Files]
Source: "{#PublishPath}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#PublishPath}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; \
  AfterInstall: SetElevationBit('{autoprograms}\{#MyAppName}.lnk')
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; \
  AfterInstall: SetElevationBit('{autodesktop}\{#MyAppName}.lnk'); \
  Tasks: desktopicon

[Run]
Filename: "schtasks"; \
   Parameters: "/Create /F /SC ONLOGON /RL HIGHEST /TN ""{#StartupTaskName}"" /TR ""'{app}\{#MyAppExeName}'"""; \
   Flags: runhidden; \
   Tasks: startwithwindows
Filename: "{app}\{#MyAppExeName}"; \
   Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; \
   Flags: nowait postinstall skipifsilent runascurrentuser

[UninstallRun]
Filename: "schtasks"; \
   Parameters: "/Delete /TN ""{#StartupTaskName}"" /F"; \
   Flags: runhidden

[Code]
procedure SetElevationBit(Filename: string);
  var
    Buffer: string;
    Stream: TStream;
  begin
    Filename := ExpandConstant(Filename);
    Log('Setting elevation bit for ' + Filename);

    Stream := TFileStream.Create(FileName, fmOpenReadWrite);
    try
      Stream.Seek(21, soFromBeginning);
      SetLength(Buffer, 1);
      Stream.ReadBuffer(Buffer, 1);
      Buffer[1] := Chr(Ord(Buffer[1]) or $20);
      Stream.Seek(-1, soFromCurrent);
      Stream.WriteBuffer(Buffer, 1);
    finally
      Stream.Free;
    end;
end;