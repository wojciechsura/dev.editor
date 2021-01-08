; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Dev.Editor"
#define MyAppVersion "1.0.1.0"
#define MyAppPublisher "Spooksoft"
#define MyAppURL "http://www.spooksoft.pl"
#define MyAppExeName "dev.editor.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{6FED44DF-FB8B-4AB4-9B81-C4DADF7D4A45}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\Spooksoft\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=.
OutputBaseFilename=setupDevEditor
SetupIconFile=Setup.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\Dev.Editor\bin\Release\Dev.Editor.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\ControlzEx.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Dev.Editor.BinAnalyzer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Dev.Editor.BusinessLogic.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Dev.Editor.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Dev.Editor.Configuration.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Dev.Editor.Dependencies.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Dev.Editor.Resources.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Fluent.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Geometry.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\HexEditor.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\ICSharpCode.AvalonEdit.Dev.Editor.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Irony.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Microsoft.Xaml.Behaviors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Unity.Abstractions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Unity.Container.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Spooksoft.VisualStateManager.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\Dev.Editor.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Dev.Editor\bin\Release\pl-PL\Dev.Editor.Resources.resources.dll"; DestDir: "{app}\pl-PL\"; Flags: ignoreversion
Source: "..\Changelog.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Dirs]
Name: "{app}\pl-PL"