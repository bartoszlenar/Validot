<h1 align="center">
  <br />
    <img src="../assets/logo/validot-logo.svg" height="256px" width="256px" />
  <br />
  Validot
  <br />
</h1>


 <h3 align="center">Tiny lib for advanced model validation, with performance in mind</h3>

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
    <img src="https://img.shields.io/github/v/release/bartoszlenar/Validot?include_prereleases&style=flat-square&label=last%20release%20version">
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
    <a href="#validot-vs-fluentvalidation">
      Validot vs FluentValidation
    </a>
    |
    <a href="#project-info">
      Project info
    </a>
    |
    <a href="../docs/DOCUMENTATION.md">
      Documentation
    </a>
  </h3>
</div>

<p align="center">
    Built with 🤘🏻by <a href="https://lenar.dev">Bartosz Lenar</a>
</p>

## Quickstart

Add the Validot nuget package to your project using dotnet CLI:

```
dotnet add package Validot
```

All the features are accessible after referencing single namespace:


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
        .WithPath("Name")
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

The result object contains all information about the errors. Without retriggering the validation process you can extract the desired form of an output.

``` csharp
result.AnyErrors; // bool flag:
// true

result.MessageMap["Email"] // collection of messages for "Email":
// [ "Must be a valid email address" ]

result.Codes; // collection of all the codes from the model:
// [ "ERR_EMAIL", "ERR_NAME" ]

result.ToString(); // compact printing of codes and messages:
// ERR_EMAIL, ERR_NAME
//
// Email: Must be a valid email address
// Name: Required for underaged user
```

* [See this example's real code](../tests/Validot.Tests.Functional/Readme/QuickStartTest.cs)

## Features

### Advanced fluent API, inline

No more obligatory if-ology around input models or separate classes wrapping just validation logic. Write specifications inline with simple, human-readable fluent API. Native support for properties and fields, structs and classes, nullables, collections, nested members and all of the possible combinations.

``` csharp
Specification<string> nameSpecification = s => s
    .LengthBetween(5, 50)
    .SingleLine()
    .Rule(name => name.All(char.IsLetterOrDigit));

Specification<string> emailSpecification = s => s
    .Email()
    .Rule(email => email.All(char.IsLower)).WithMessage("Must contain only lower case characters");

Specification<UserModel> userSpecification = s => s
    .Member(m => m.Name, nameSpecification).WithMessage("Must comply with name rules")
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
Specification<BookModel> bookSpecification = s => s
    .Optional()
    .Member(m => m.AuthorEmail, m => m.Optional().Email())
    .Member(m => m.Title, m => m.NotEmpty().LengthBetween(1, 100))
    .Member(m => m.Price, m => m.NonNegative());

var bookValidator =  Validator.Factory.Create(bookSpecification);

services.AddSingleton<IValidator<BookModel>>(bookValidator);
```

``` csharp
var bookModel = new BookModel() { AuthorEmail = "inv@lid_em@il", Price = 10 };

bookValidator.IsValid(bookModel);
// false

bookValidator.Validate(bookModel).ToString();
// AuthorEmail: Must be a valid email address
// Title: Required

bookValidator.Validate(bookModel, failFast: true).ToString();
// AuthorEmail: Must be a valid email address

bookValidator.Template.ToString(); // Template contains all of the possible errors:
// AuthorEmail: Must be a valid email address
// Title: Required
// Title: Must not be empty
// Title: Must be between 1 and 100 characters in length
// Price: Must not be negative
```

* [What Validator is and how it works](../docs/DOCUMENTATION.md#validator)
* [More about template and how to use it](../docs/DOCUMENTATION.md#template)

### Results

A flag, list of codes or full error paths with messages. Additionally - access to the raw errors data so you can produce your own kind of report.


``` csharp
var validationResult = validator.Validate(signUpModel);

if (validationResult.AnyErrors)
{
    // check if a specific code has been recorded for Email property:
    if (validationResult.CodeMap["Email"].Contains("DOMAIN_BANNED"))
    {
        _actions.RecordBannedDomainRegistrationAttempt(signUpModel.Email);
    }

    // save all messages and codes printing into the logs
    _logger.LogError("Errors in incoming SignUpModel: {errors}", validationResult.ToString());

    // return all error codes to the frontend
    return new SignUpActionResult
    {
        Success = false,
        ErrorCodes = validationResult.Codes,
    };
}
```

* [Reports and their types](../docs/DOCUMENTATION.md#reports)
* [Building custom report](../docs/DOCUMENTATION.md#custom-report)

### Rules

Tons of rules available out of the box. Plus an easy way to define your own with full support of Validot internal features like formattable message arguments.

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

.ExactLinesCount(4).WithMessage("Required lines count: {count|format=000.00|culture=pl-PL}")
// Required lines count: 004,00
```

* [List of built-in rules](../docs/DOCUMENTATION.md#rule-list)
* [Writing custom rules](../docs/DOCUMENTATION.md#custom-rules)
* [Parametrized messages](../docs/DOCUMENTATION.md#parameterized-messages)

### Translations

Pass errors directly to the end users in the language of your application.

``` csharp
Specification<UserModel> specification = s => s
    .Member(m => m.PrimaryEmail, m => m.Email())
    .Member(m => m.Name, m => m.LengthBetween(3, 50));

var validator =  Validator.Factory.Create(specification, settings => settings.WithPolishTranslation());

var model = new UserModel() { PrimaryEmail = "in@lid_em@il", Name = "X" };

var result = validator.Validate(model);

result.ToString();
// Email: Must be a valid email address
// Name: Must be between 3 and 50 characters in length

result.ToString(translationName: "Polish");
// Email: Musi być poprawnym adresem email
// Name: Musi być długości pomiędzy 3 a 50 znaków
```

* [How translations work](../docs/DOCUMENTATION.md#translations)
* [Custom translation](../docs/DOCUMENTATION.md#custom-translation)
* [How to selectively override built-in error messages](../docs/DOCUMENTATION.md#overriding-messages)

## Validot vs FluentValidation

A short statement to start with - [@JeremySkinner](https://twitter.com/JeremySkinner)'s [FluentValidation](https://fluentvalidation.net/) is a great piece of work and has been a huge inspiration for this project. True, you can call Validot a direct competitor, but it differs in some fundamental decisions and lot of attention has been focused on completely different aspects. If after reading this section you think you can bear another approach, api and [limitations](#fluentValidations-features-that-validot-is-missing), at least give Validot a try. You might be positivly surprised. Otherwise, FluentValidation is a good, safe choice, as Validot is certainly less hackable and achieving some very specific goals might be either difficult or impossible.

### Validot is faster and consumes less memory

Of course, it doesn't come for free. Wherever possible and justified, Validot chooses performance and less allocations over flexibility and extra features. For instance, Validot relies heavingly on caching and output size predictions. You'll never receive validation context to dynamically create the message during the validation process. Messages are very customizable, but ultimately the content needs to be deterministic (_the workaround for including validated value in the message is technically possible, but will be released later_). Fine with that kind of trade-off? Good, because in validation and gathering detailed error messages from thousands of objects Validot might be ~2.5x faster while consuming ~3.5x less memory:

<details>
  <summary>Click here to expand important comment about the benchmarks.</summary>

The input data set of 100000 objects is the same for both libraries, so are the error messages and the rules reflect each other as much as technically possible. Of course, the below tables show terribly oversimplified results of BenchmarkDotNet execution, but the intention is to present the general trend only. All benchmarks live in the [separate project](https://github.com/bartoszlenar/Validot/tree/master/tests/Validot.Benchmarks) that you can [run yourself](../docs/DOCUMENTATION.md#benchmarks), which is highly recommended if you want to have truly reliable numbers.

Of course, any help with making these benchmarks more accurate and covering more use cases would be very very welcomed - especially errors in the code, usage or methodology, as well as counterexamples proving that Validot struggles with some particular scenarios (please make a pull request and/or [rise an issue](https://github.com/bartoszlenar/Validot/issues/new)).

* [ErrorMessages benchmark](../tests/Validot.Benchmarks/Comparisons/ErrorMessagesBenchmark.cs) - objects with all possible member types, simple logic and custom error messages
* [NoErrors benchmark](../tests/Validot.Benchmarks/Comparisons/NoErrorsBenchmark.cs) - similar to Messages benchmark, but all objects are valid
* [NoLogic benchmark](../tests/Validot.Benchmarks/Comparisons/NoLogicBenchmark.cs) - very simple model, validating multiple members, but no validation logic (testing core engines only)
* [Initialization benchmark](../tests/Validot.Benchmarks/Comparisons/InitializationBenchmark.cs) - creating validators only

</details>


| Test name | Library | Environment | Mean [ms] | Allocated [MB] |
| - | - | - | -: | -: |
| ErrorMessages, FullReport | FluentValidation | .NET Core 3.1, i7-9750H, X64 RyuJIT | `7513.665` | `7201.38` |
| ErrorMessages, FullReport | Validot | .NET Core 3.1, i7-9750H, X64 RyuJIT | `3699.273` | `2502.20` |

FluentValidation's `IsValid` is a property that wraps a simple check whether the validation result contains errors or not. Validot has `AnyErrors` that acts the same way, but `IsValid` is a dedicated special mode that doesn't care about anything else but the first rule predicate that fails. If the mission is only to verify incoming model whether it complies with the rules (discarding all of the details), this approach proves to be better up to one order of magnitude:

| Test name | Library | Environment | Mean [ms] | Allocated [MB] |
| - | - | - | -: | -: |
| ErrorMessages, IsValid | FluentValidation | .NET Core 3.1, i7-9750H, X64 RyuJIT | `7385.523` | `6859.70` |
| ErrorMessages, IsValid | Validot | .NET Core 3.1, i7-9750H, X64 RyuJIT | `147.647` | `312.13` |
| NoErrors, IsValid | FluentValidation | .NET Core 3.1, i7-9750H, X64 RyuJIT | `6253.320` | `6689.79` |
| NoErrors, IsValid | Validot | .NET Core 3.1, i7-9750H, X64 RyuJIT | `2404.450` | `788.60` |

In fact, combining these two methods in most cases could be quite beneficial. At first `IsValid` quickly verifies the object and if it contains errors - only then `Validate` is executed to report the details. Of course in some extreme cases (megabyte-size data? milions of items in the collection? dozens of nested levels with loops in reference graphs?) traversing through the object twice could neglate the profit, but for the regular web api input validation it will certainly serve its purpose:

``` csharp
if (validator.IsValid(model))
{
    errorMessages = validator.Validate(model).ToString();
}
```

| Test name | Library | Environment | Mean [ms] | Allocated [MB] |
| - | - | - | -: | -: |
| ErrorMessages, IsValidAndValidate | FluentValidation | .NET Core 3.1, i7-9750H, X64 RyuJIT | `6446.736` | `6863.09` |
| ErrorMessages, IsValidAndValidate | Validot | .NET Core 3.1, i7-9750H, X64 RyuJIT | `2805.867` | `964.45` |

### Validot handles nulls by default

In Validot, null is a special case handled by the core engine. You don't need to secure the validation logic from null as your predicate will never receive it.

``` csharp
Member(m => m.LastName, m => m
    .Rule(lastName => lastName.Length < 50) // 'lastName' is never null
    .Rule(lastName => lastName.All(char.IsLetter)) // 'lastName' is never null
)
```

Unless explicitly commanded, the model is marked as required by default. In the above example, if `LastName` member were null, the validation process would exit `LastName` scope immediately only with this single error message (content can be changed):

```
LastName: Required
```

If null should be allowed, place `Optional` command at the beginning:

``` csharp
Member(m => m.LastName, m => m
    .Optional()
    .Rule(lastName => lastName.Length < 50) // 'lastName' is never null
    .Rule(lastName => lastName.All(char.IsLetter)) // 'lastName' is never null
)
```

Again, no rule predicate is triggered. Also null `LastName` member doesn't result with errors.

The strategy of handling nulls is deeply baked in Validot's foundations and can't be altered. Although some scenarios could be achieved by mixing `Rule` (the logic) + `WithMessage` (error message) + `WithName` (target path) commands at the higher level.

* [Read more about how Validot handles nulls](../docs/DOCUMENTATION.md#handling-nulls)

### Validot's Validator is immutable

Once Validator is created, you can't modify its internal state or options. If you need the process to fail fast (FluentValidation's `CascadeMode.StopOnFirstFailure`), use the flag:

``` csharp
validator.Validate(model, failFast: true);
```

### FluentValidation's features that Validot is missing

Features that might be in the scope and are technically possible to implement in the future:

* `await`/`async` support
* dependent rules
* transforming values
* severities
* failing fast only in a single scope
* validated value in the error message
* "smart paths" in the error message (`RootUserCollection` member becomes `Root User Collection`)

Features that are very unlikely to be in the scope as they contradict with the project's principles, and/or would have very negative impact on performance, and/or are impossible to implement:

* access to any stateful context in the rule condition predicate
  * it implicates already mentioned lack of support for dynamic message content and/or amount
* integration with ASP.NET or other frameworks
  * making such a thing wouldn't be a difficult task at all, but Validot tries to remain a single-purpose library and all integrations need to be done individually
* callbacks
  * please react on failure/success after getting validation result
* pre-validation
  * all cases can be handled by additional validation and a proper if-else
  * also, the problem of root being null doesn't exist in Validot (it's a regular case, covered entirely with fluent api)
* rule sets
  * workaround; multiple validators for different sets of properties

## Project info

### Requirements

Validot is a dotnet class library targeting .NET Standard 2.0. There are no extra dependencies.

Please check the [official Microsoft document](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md) that lists all the platforms that can use it on.

### Reliability

Unit tests coverage hits 100% very close, it can be detaily verified on [codecov.io](https://codecov.io/gh/bartoszlenar/Validot/branch/master).

[Semantic versioning](https://semver.org/) is being used very strictly. Major version is updated only when there is a breaking change, no matter how small it might be (e.g. adding extra function to the public interface). On the other hand, huge pack of new features will bump the minor version only.

Before publishing, each release is tested on the [latest versions](https://help.github.com/en/actions/reference/virtual-environments-for-github-hosted-runners#supported-runners-and-hardware-resources) of operating systems:

* macOS
* Ubuntu
* Windows Server

using the [LTS versions](https://dotnet.microsoft.com/platform/support/policy/dotnet-core) of the underlying frameworks:

* .NET Core 3.1
* .NET Core 2.1
* .NET Framework 4.8 (Windows only)

### Performance

Benchmarks exist in the form of  [the console app project](https://github.com/bartoszlenar/Validot/tree/master/tests/Validot.Benchmarks) based on [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet). Also, you can trigger performance tests [from the build script](../docs/DOCUMENTATION.md#benchmarks).

### Documentation

The documentation is hosted alongside the source code, in the git repository, as a single markdown file: [DOCUMENTATION.md](./../docs/DOCUMENTATION.md).

Code examples from the documentation live as [functional tests](https://github.com/bartoszlenar/Validot/tree/master/tests/Validot.Tests.Functional).

### Development

The entire project ([source code](https://github.com/bartoszlenar/Validot), [issue tracker](https://github.com/bartoszlenar/Validot/issues), [documentation](./../docs/DOCUMENTATION.md) and [CI workflows](https://github.com/bartoszlenar/Validot/actions)) is hosted here on github.com.

Any contribution is more than welcome. If you'd like to help, please don't forget to check out the [CONTRIBUTING](docs/CONTRIBUTING.md) file and [issues page](https://github.com/bartoszlenar/Validot/issues).

### Licencing

Validot uses [MIT licence](../LICENCE). Long story short; you are more than welcome to use it anywhere you like, completely free of charge and without opressive obligations.