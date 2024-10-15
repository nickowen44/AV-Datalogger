# Get the git root directory
$gitRoot = git rev-parse --show-toplevel

# Define the target folders
$windowsTargetFolder = "$gitRoot\release\windows"
$linuxTargetFolder = "$gitRoot\release\linux"

# Define the target project to build
$targetProject = "$gitRoot\Dashboard"

# Publish for Windows (Release)
dotnet publish $targetProject -c Release -r win-x64 --self-contained -o $windowsTargetFolder -p:PublishSingleFile=true -f net8.0

# Publish for Linux (Release)
dotnet publish $targetProject -c Release -r linux-x64 --self-contained -o $linuxTargetFolder -p:PublishSingleFile=true -f net8.0