<h1 align="center">
  <br />
    <img src="../docs/logo/Validot-logo.svg" height="256px" width="256px" />
  <br />
  Validot
  <br />
</h1>


 <h3 align="center">Tiny lib for great validations</h3>

  <br />
<p align="center">
  <a href="https://github.com/bartoszlenar/Validot/actions?query=branch%3Amaster+workflow%3ACI">
    <img src="https://img.shields.io/github/workflow/status/bartoszlenar/Validot/CI/master?style=for-the-badge&label=CI&logo=github&logoColor=white&logoWidth=20">
  </a>
  <a href="https://codecov.io/gh/bartoszlenar/Validot/branch/master">
    <img src="https://img.shields.io/codecov/c/gh/bartoszlenar/Validot/master?style=for-the-badge&logo=codecov&logoColor=white&logoWidth=20">
  </a>
  <a href="https://www.nuget.org/packages/Validot">
      <img src="https://img.shields.io/nuget/v/Validot?style=for-the-badge&logo=nuget&logoColor=white&logoWidth=20&label=STABLE%20VERSION">
  </a>
</p>
<p align="center">
  <a href="https://github.com/bartoszlenar/Validot/commits/master">
    <img src="https://img.shields.io/github/last-commit/bartoszlenar/Validot/master?style=flat-square">
  </a>
  <a href="https://github.com/bartoszlenar/Validot/releases">
    <img src="https://img.shields.io/github/release-date-pre/bartoszlenar/Validot?include_prereleases&style=flat-square&label=last%20release">
  </a>
  <a href="https://github.com/bartoszlenar/Validot/releases">
    <img src="https://img.shields.io/github/v/release/bartoszlenar/Validot?include_prereleases&style=flat-square&label=last%20release">
  </a>
  <a>
  </a>
  <a href="https://github.com/bartoszlenar/Validot/milestone/1">
    <img alt="GitHub milestone" src="https://img.shields.io/github/milestones/progress/bartoszlenar/Validot/1?label=milestone%20v1.0&style=flat-square">
  </a>
</p>

<div align="center">
  <h3>
    <a href="#quickstart">
      Quickstart
    </a>
    |
    <a href="#features">
      Features
    </a>
    |
    <a href="#tech-info">
      Project info
    </a>
    |
    <a href="../docs/DOCUMENTATION.md">
      Documentation
    </a>
    |
    <a href="../docs/CHANGELOG.md">
      Changelog
    </a>
  </h3>
</div>

<p align="center">
    Built with 🤘🏻by <a href="https://lenar.dev">Bartosz Lenar</a>
</p>

## Quickstart

Add the the Validot nuget package to your project using dotnet CLI:

```
dotnet add package Validot
```

All features are accessible after referencing single namespace:


``` csharp
using Validot;
```

And you're good to go! At first, create a specification for your model with the fluent api.

``` csharp
Specification<UserModel> specification = _ => _
    .Member(m => m.Email, m => m
        .Email().WithExtraCode("ERR_EMAIL")
        .MaxLength(100)
    )
    .Member(m => m.Name, m => m
        .Optional()
        .LengthBetween(8, 100)
        .Rule(name => name.All(char.IsLetterOrDigit)).WithMessage("Must contain only letter or digits")
    )
    .Rule(m => m.Age >= 18 || m.Name != null)
        .WithName("Name")
        .WithMessage("Required for underaged user")
        .WithExtraCode("ERR_NAME");
```

The next step is to create a validator. As its name stands - it validates objects according to the specification. It's also thread-safe so you can seamlessly register it as a singleton in your DI container.

``` csharp
var validator = Validator.Factory.Create(specification);
```

Validate the object!

``` csharp
var model = new UserModel(email: "inv@lidv@lue", age: 14);

var result = validator.Validate(model);
```

The result contains all errors information. Without retriggering the validation process you can extract the desired form of an output.

``` csharp
result.ToMessagesString();
// Email: Must be a valid email address
// Name: Required for underaged user

result.ToCodesList();
// [ "ERR_EMAIL", "ERR_NAME" ]

result.IsValid;
// false
```

* [See this example's real code](../tests/Validot.Tests.Functional/Readme/QuickStartTest.cs)

## Features

### Advanced fluent API, inline

No more obligatory if-ology around input models or separate classes wrapping just validation logic. Write specifications inline with simple, human-readable fluent API. Native support for properties and fields, structs and classes, nullables, collections, nested members... and all of the possible combinations!

``` csharp
Specification<string> nameSpecification = _ => _
    .LengthBetween(5, 50)
    .SingleLine()
    .Rule(name => name.All(char.IsLetterOrDigit));

Specification<string> emailSpecification = _ => _
    .Email()
    .Rule(email => email.All(char.IsLowerCase)).WithMessage("Must contain only lower case characters");

Specification<UserModel> userSpecification = _ => _
    .Member(m => Name, nameSpecification).WithMessage("Must comply with name rules")
    .Member(m => m.PrimaryEmail, emailSpecification)
    .Member(m => m.AlternativeEmails, m => m
        .Optional()
        .MaxCollectionSize(3).WithMessage("Must not contain more than 3 addresses")
        .AsCollection(emailSpecification)
    )
    .Rule(user => {

      return user.PrimaryEmail == null || user.AlternativeEmails?.Contains(user.PrimaryEmail) == false;

    }).WithMessage("Alternative emails must not contain the primary email address");
```

* [Guide through Validot's fluent API](../docs/DOCUMENTATION.md#specification)
* [If you prefer approach of having separate class for just validation logic, it's also fully supported](../docs/DOCUMENTATION.md#specification-holder)

### Validators

Compact, highly optimized and thread-safe objects to handle the validation.

``` csharp
var bookValidator =  Validator.Factory.Create(bookSpecification);

services.AddSingleton<IValidator<BookModel>>(bookValidator);
```

``` csharp
bookValidator.Validate(bookModel).ToMessagesString();
// AuthorsEmail: Must be a valid email address
// Title: Required

bookValidator.Validate(bookModel, failFast: true).ToMessagesString();
// AuthorsEmail: Must be a valid email address

bookValidator.ErrorMap.ToMessagesString(); // ErrorMap contains all of the possible errors
// AuthorsEmail: Must be a valid email address
// Title: Required
// Title: Must not be empty
// Title: Must be between 1 and 100 characters in length
// Price: Must not be negative
```

* [What Validator is and how it works](../docs/DOCUMENTATION.md#validator)
* [More about error map and how to use it](../docs/DOCUMENTATION.md#error-map)


### Results

A flag, list of codes or full error paths with messages. Additionally - access to the raw errors data so you can produce your own kind of report.


``` csharp
var validationResult = validator.Validate(signUpModel);

if (!validationResult.IsValid)
{
    _logger.LogError("Errors in incoming SignUpModel: {errors}", validationResult.ToMessagesString());

    return new SignUpFailureResult
    {
        Success = false,
        ErrorCodes = validationResult.ToCodesList(),
    }
}
```

* [Reports and their types](../docs/DOCUMENTATION.md#reports)
* [Building custom report](../docs/DOCUMENTATION.md#custom-report)

### Rules

Tons of rules available out of the box. Plus an easy way to define your own with full support of Validot internal features like parametrized messages.

``` csharp
public static IRuleOut<string> ExactLinesCount(this IRuleIn<string> @this, int count)
{
    return @this.RuleTemplate(
        value => value.Split(Environment.NewLine).Length == count,
        "Must contain exactly {count} lines",
        Arg.Number("count", count)
    );
}
```

``` csharp
.ExactLinesCount(4)
// Must contain exactly 4 lines

.ExactLinesCount(4).WithMessage("Required lines count: {count}")
// Required lines count: 4

.ExactLinesCount(4).WithMessage("Required lines count: {count|format=000}")
// Required lines count: 004
```

* [List of built-in rules](../docs/DOCUMENTATION.md#rule-list)
* [Writing custom rules](../docs/DOCUMENTATION.md#custom-rules)
* [Parametrized messages](../docs/DOCUMENTATION.md#parameterized-messages)

### Translations

Pass errors directly to the end users in the language of your application.

``` csharp
var validator =  Validator.Factory.Create(specification, settings => settings.WithPolishTranslation());

var result = validator.Validate(model);

result.ToMessagesString();
// Email: Must be a valid email address
// Name: Must be between 3 and 50 characters in length

result.ToMessagesString(translation: "Polish");
// Email: Musi być poprawnym adresem email
// Name: Musi być długości pomiędzy 3 a 50 znaków
```

* [How translations work](../docs/DOCUMENTATION.md#translations)
* [Custom translation](../docs/DOCUMENTATION.md#custom-translation)
* [How to selectively override built-in error messages](../docs/DOCUMENTATION.md#overriding-messages)

## Project info

### Requirements

Validot is a dotnet class library targeting .NET Standard 2.0. There are no extra dependencies.

Please check the [official Microsoft document](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md) that lists all the platforms that can use it on.

### Reliability

Unit tests coverage hits 100% very close, it can be detaily verified on [codecov.io](https://codecov.io/gh/bartoszlenar/Validot/branch/master).

Functional tests are in [the separate project](https://github.com/bartoszlenar/Validot/tree/master/tests/Validot.Tests.Functional), which stands also as a base of usage examples and helps with following [semantic versioning](https://semver.org/).

Before publishing, each release is tested on the [latest versions](https://help.github.com/en/actions/reference/virtual-environments-for-github-hosted-runners#supported-runners-and-hardware-resources) of operating systems:

* macOS
* Ubuntu
* Windows Server

using the [LTS versions](https://dotnet.microsoft.com/platform/support/policy/dotnet-core) of the underlying framework:

* .NET Core 3.1
* .NET Core 2.1
* .NET Framework 4.8 (Windows only)

### Performance

Benchmarks are based on [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) and are focused on comparison with [@JeremySkinner](https://twitter.com/JeremySkinner)'s [FluentValidation](https://github.com/FluentValidation/FluentValidation). Which is a great piece of work and has been inspiration for this project.

### Development

The entire project (source code, issue tracker, documentation and CI workflows) is hosted here on github.com.

Any contribution is more than welcome. If you'd like to help, don't forget to check out [CONTRIBUTING](docs/CONTRIBUTING.md) file.

### Licencing

Validot uses [MIT licence](../LICENCE). Long story short; you are more than welcome to use it anywhere you like, completely free of charge and without opressive obligations.