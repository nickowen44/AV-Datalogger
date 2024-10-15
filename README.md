# AV-Datalogger
AV Datalogger used for the technical inspection of Formula Student Autonomous Vehicles 

# Building the AV Datalogger for release
Pre-requisites:
- dotnet 8.0 (or higher) SDK

To build the AV Datalogger, run the `release-build.ps1` script in the root directory of the repository. This will create a release build of the AV Datalogger in the `release` directory.
```powershell
.\release-build.ps1
```

Two builds will be created:
- `/release/linux` for Linux
- `/release/windows` for Windows