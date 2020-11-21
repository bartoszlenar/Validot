# Changelog
All notable changes to the [Validot project](https://github.com/bartoszlenar/Validot) will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Changed
- Fixed inline XML code documentation, so it's visible from IDEs when referencing a nuget package.

## [1.2.0] - 2020-11-04
### Added
- `And` - a fluent API method that [helps to visually separate the rules](DOCUMENTATION.md#And) within the specification. [#9](https://github.com/bartoszlenar/Validot/issues/9)
- Inline documentation (XML comments)

## [1.1.0] - 2020-09-01
### Added
- Email rule now operates in two modes: ComplexRegex (which covers the previous, regex-based behavior, and is still set as default) and DataAnnotationsCompatible (compatible with the dotnet's [EmailAddressAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.emailaddressattribute?view=netcore-3.1)).
- Title case support for the name argument, so name `SuperImportantValue123` when inserted with `{_name|format=titleCase}` is converted into `Super Important Value 123`. [#1](https://github.com/bartoszlenar/Validot/issues/1)

## [1.0.0] - 2020-06-23

First stable and public release. The reference point for all further changes.
