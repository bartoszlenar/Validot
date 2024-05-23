# Changelog
All notable changes to the [Validot project](https://github.com/bartoszlenar/Validot) will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [2.5.0] - 2024-05-24
### Added
- Added `AsDictionary` command. [#26](https://github.com/bartoszlenar/Validot/issues/26)

## [2.4.1] - 2023-03-16
### Fixed
- Fixed invalid placeholders in messages. [#32](https://github.com/bartoszlenar/Validot/issues/32)

## [2.4.0] - 2022-10-01
### Added
- Added `AsType` command. [#4](https://github.com/bartoszlenar/Validot/issues/24)

### Fixed
- Inline XML documentation for `AsConverted`.

## [2.3.0] - 2022-08-13
### Added
- Added `AsConverted` command. [#3](https://github.com/bartoszlenar/Validot/issues/3)

### Removed
- Official support for not supported dotnet versions (.NET Core 2.1, .NET 5.0).

## [2.2.0] - 2021-11-05
### Added
- German translation (along with `WithGermanTranslation` extension to the settings builder). [#12](https://github.com/bartoszlenar/Validot/issues/12)
- Portuguese translation (along with `WithPortugueseTranslation` extension to the settings builder). [#13](https://github.com/bartoszlenar/Validot/issues/13)

### Fixed
- Fix to Spanish translation in `Times.BeforeOrEqualTo` message key. [#20](https://github.com/bartoszlenar/Validot/pull/20/commits/6a68dcdc17589f3c9bd524bc2266238b5245ff50)
- Minor performance fixes and code improvements. [#21](https://github.com/bartoszlenar/Validot/pulls/21) [#22](https://github.com/bartoszlenar/Validot/pulls/22)

## [2.1.0] - 2021-06-07
### Added
- Spanish translation (along with `WithSpanishTranslation` extension to the settings builder). [#11](https://github.com/bartoszlenar/Validot/issues/11)
- Russian translation (along with `WithRussianTranslation` extension to the settings builder). [#14](https://github.com/bartoszlenar/Validot/issues/14)
- Translation template with script. To add a new translation all you need to do is call the build script, e.g. to add Korean, execute `pwsh build.ps1 --target AddTranslation --translationName Korean` (you can use `bash build.sh` instead of `pwsh build.ps1`) and the template with phrases will be created at `src/Validot/Translations/Korean` (plus unit tests in their own location). The only thing that is left to do is to enter translated phrases into the dictionary and make a PR!
- A preview version of .NET 6 in the CI pipeline for all unit and functional tests.

## [2.0.0] - 2021-02-01
### Added
- `FetchHolders` method in the factory that helps [fetching specification holders](DOCUMENTATION.md#fetching-holders) from the assemblies and delivers a handy way to create the validators and [register them in the dependency injection containers](DOCUMENTATION.md#dependency-injection). [#10](https://github.com/bartoszlenar/Validot/issues/10)
- Method in the factory that accepts the settings (in form of `IValidatorSettings`) directly, so the settings (e.g. from another validator) could be reused. This method compensates the lack of validator's public constructor.
- [Settings holders](DOCUMENTATION.md#settings-holder) (`ISettingsHolder` interface), a mechanism similar to specification holders. This feature compensates the lack of `ITranslationHolder`.

### Fixed
- Fixed inline XML code documentation, so it's visible from IDEs when referencing a nuget package.

### Changed
- Introduced `IValidatorSettings` as a public interface for read-only access to the `ValidatorSettings` instance. `ValidatorSettings` class isn't public anymore, and validator's `Settings` property is now of type `IValidatorSettings`. This is a breaking change.
- Renamed `ReferenceLoopProtection` flag to `ReferenceLoopProtectionEnabled`. This is a breaking change.
- `IsValid` method uses a dedicated validation context that effectively doubles the speed of the operation
- Ported all test projects to .NET 5.
- Replaced ruleset-based code style rules with editorconfig and `Microsoft.CodeAnalysis.CSharp.CodeStyle` roslyn analyzers.
- Renamed `master` git branch to `main`.

### Removed
- Validator's public constructor. Please use the factory to create validators. If you want to reuse the settings, factory has new method that accepts `IValidatorSettings` instance. This is a breaking change.
- Translation holders (`ITranslationHolder` interface). You can easily replace them with the newly introduced settings holders (`ISettingsHolder` interface). This is a breaking change.
- CapacityInfo feature. It wasn't publicly available anyway and ultimately didn't prove to boost up the performance.

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
