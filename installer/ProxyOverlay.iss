#define MyAppName "ProxyOverlay"
#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#define MyAppPublisher "Nierhain"
#define MyAppExeName "ProxyOverlay.exe"

[Setup]
AppId={{7B0F5B0B-4D60-4F91-9DB8-6B1D7B1D7E5A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL=https://github.com/Nierhain/ProxyOverlay
AppSupportURL=https://github.com/Nierhain/ProxyOverlay/issues
AppUpdatesURL=https://github.com/Nierhain/ProxyOverlay/releases
AppComments=Batch image overlay tool for Magic proxies and other image-processing purposes.
VersionInfoDescription=ProxyOverlay installer
VersionInfoProductName=ProxyOverlay
VersionInfoCompany={#MyAppPublisher}
VersionInfoCopyright=Copyright (C) 2026 Nierhain
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=..\publish\installer
OutputBaseFilename=ProxyOverlay-Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
LicenseFile=..\LICENSE.md
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64compatible
PrivilegesRequired=admin
UninstallDisplayIcon={app}\{#MyAppExeName}

[Files]
Source: "..\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
