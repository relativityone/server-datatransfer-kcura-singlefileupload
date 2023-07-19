# Changelog

All notable changes to this project will be documented in this file.

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