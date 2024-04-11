# Changelog

## 0.0.3
- feat: add paramter to New-NuGetPackage to set repository meta data
-
## 0.0.2
- fix: downgrade NuGet client SDK to 6.6.1 to avoid assembly binding problem on
  powershel 5.1 to System.Buffers 4.0.2.0
- fix: correct handling of feed config when using `NuGet.config` file
- feat: improve New-NuGetPackage cmdlet
  - manifest file & command line arguments can be used together
  - hash table added as input for files mapping
  - dependencies can be provided in short from (version only) or full (with includes/excluds)

## 0.0.1
- logging infrastructure in place but disabled due to threading issues
- initial release - basic functionality for creating, searching, resolving and installing
  nuget packages based on Nuget Client SDK
- module documentation mainly stubs
- test tbd
