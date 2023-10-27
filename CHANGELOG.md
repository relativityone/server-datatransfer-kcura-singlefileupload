# Changelog

All notable changes to this project will be documented in this file.

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