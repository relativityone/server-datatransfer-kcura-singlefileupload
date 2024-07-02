# Changelog

All notable changes to this project will be documented in this file.

## [24000.0.10] - 29-June-2024

### Changed

- [REL-931886](https://jira.kcura.com/browse/REL-931886) - Upload/Replace Image uses deprecated non-versioned API - Backported [REL-905669](https://jira.kcura.com/browse/REL-905669) ticket
- Backported [REL-790560](https://jira.kcura.com/browse/REL-790560) - Simple File upload placing documents into root folder in latest version
- [REL-975487](https://jira.kcura.com/browse/REL-975487) - All binaries should populate correct File version for SFU.
- [REL-974405](https://jira.kcura.com/browse/REL-974405) - Removed OTEL assemblies from the SFU RAPs

## [24000.0.9] - 28-June-2024

### Changed

- [REL-974405](https://jira.kcura.com/browse/REL-974405) - Removed OTEL assemblies from the SFU RAPs

## [24000.0.8] - 24-June-2024

### Changed

- [REL-973479](https://jira.kcura.com/browse/REL-973479) - Upgrade latest IAPI into SFU, Sync and RIP

## [24000.0.7] - 11-June-2024

### Changed

- [REL-944165](https://jira.kcura.com/browse/REL-944165)
- Consumed oauth2 client into SFU
- Revved latest IAPI that has unified RDC and SDK version
- Revved latest relativity outsidein upgraded to match IAPI version

## [24000.0.6] - 27-May-2024

### Changed

- [REL-948521](https://jira.kcura.com/browse/REL-948521) - Increase the code coverage - SFU

## [24000.0.5] - 21-May-2024

### Changed

- [REL-946043](https://jira.kcura.com/browse/REL-946043) - Revved latest package for Relativity.DataExchange.Client.SDK and Relativity.Transfer.Client

## [24000.0.4] - 08-May-2024

### Changed

- [REL-942148](https://jira.kcura.com/browse/REL-942148) - [Server 2024] RAP Schema Update.

## [24000.0.3] - 22-April-2024

### Changed
 
- [REL-931883](https://jira.kcura.com/browse/REL-931883) Update OTEL Dependencies and Verify Kepler Services.

## [24000.0.2] - 25-March-2024

### Changed
 
- [REL-925106](https://jira.kcura.com/browse/REL-925106) Verify Nightly pipeline jobs for SingleFileUpload migrated repo.

## [24000.0.1] - 11-10-2023

### Changed
 
- [REL-886791](https://jira.kcura.com/browse/REL-886791) Cloned repo from BitBucket to GitHub and created pipeline in AzDO.

## [24000.0.0] - 10-16-2023

### Changed
 
- Prepared branch for the next official Relativity 2024 release.
- Use the latest SUT release image.

## [23012.8.1001] - 10-06-2023

### Changed

- [REL-866456](https://jira.kcura.com/browse/REL-866456) - Revved latest package for Relativity.DataTransfer.Legacy.SDK and Relativity.Transfer.Client

## [23012.8.3] - 09-08-2023

### Changed

- Bumped the application version to align with the Server 2023 application versioning strategy ADR.

## [23012.8.2] - 09-04-2023

### Changed

- Bumped the application version to align with the Server 2023 application versioning strategy ADR.

## [12.8.2] - 09-02-2023

### Changed 
- [REL-871666](https://jira.kcura.com/browse/REL-871666) - Improve Import API error message when Kepler and WebAPI endpoints are not found.

## [12.8.1] - 08-15-2023
 
### Changed

- Updated latest IAPI pre package

## [12.8.0] - 07-31-2023
 
### Changed
 
- [REL-862766](https://jira.kcura.com/browse/REL-862766) - Create release branch for RAPCD
- Official Relativity 2023 12.3 release.
- The SUT configuration upgrades the previous release image to the latest release image.

## [12.7.0] - 07-19-2023

### Changed

- [REL-848356](https://jira.kcura.com/browse/REL-848356) - Revved OutsideIn - Backported [REL-712106](https://jira.kcura.com/browse/REL-712106) ticket from Server 2022 release.

## [12.6.0] - 07-17-2023

### Changed

- [REL-848356](https://jira.kcura.com/browse/REL-848356) - Revved IAPI Latest Package - Backported [REL-712106](https://jira.kcura.com/browse/REL-712106) ticket from Server 2022 release.

## [12.5.1] - 05-24-2023

### Added

- Added a new Directory.Packages.Props file.

### Changed

- Converted nuget to centrally managed packages.
- The Directory.Packages.Props file has been created and all references have been added.
- In the csproj file reference tag was replaced with package reference tag.
- Deleted the packages.config file.

## [12.5.0] - 05-16-2023

### Added

- Added a new changelog.md

### Changed

- Archived the existing changelog.md file
- Removed ##maintainers section from README.md 
- SlackChannel has been updated to ci-server-delta in trident file
- The code owners details has been updated to show all server delta team members
- Bumped the minor version and zeroed out the patch number