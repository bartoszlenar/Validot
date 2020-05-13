# Changelog
All notable changes to Validot project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- Replaced `IsValid` property in `IValidationResult` (a flag set to `true` if no errors) with `AnyErrors` property (similar, but negation) to avoid confusion with `IsValid` method in `IValidator` interface.

## [1.0.0-preview1] - 2020-04-25
Initial development point. Everything starts here.
