<h1 align="center">
  <br />
    <img src="../assets/logo/validot-logo.svg" height="256px" width="256px" />
  <br />
  Validot
  <br />
</h1>

 <p align="center">Validot is a performance-first, compact library for advanced model validation. Using a simple declarative fluent interface, it efficiently handles classes, structs, nested members, collections, nullables, plus any relation or combination of them. It also supports translations, custom logic extensions with tests, and DI containers.</p>

  <br />
<p align="center">
  <a href="https://www.nuget.org/packages/Validot">
      <img src="https://img.shields.io/nuget/v/Validot?style=for-the-badge&logo=nuget&logoColor=white&logoWidth=20&label=CURRENT%20VERSION">
  </a>
  <a href="https://www.nuget.org/packages/Validot">
      <img src="https://img.shields.io/github/release-date/bartoszlenar/Validot?include_prereleases&style=for-the-badge&label=RELEASED">
  </a>
</p>
<p align="center">
  <a href="https://github.com/bartoszlenar/Validot/commits/main">
    <img src="https://img.shields.io/github/commits-since/bartoszlenar/Validot/v2.3.0/main?logo=git&logoColor=white&style=flat-square">
  </a>
  <a href="https://github.com/bartoszlenar/Validot/commits/main">
    <img src="https://img.shields.io/github/last-commit/bartoszlenar/Validot/main?style=flat-square">
  </a>
  <a href="https://github.com/bartoszlenar/Validot/actions?query=branch%3Amain+workflow%3ACI">
    <img src="https://img.shields.io/github/workflow/status/bartoszlenar/Validot/CI/main?style=flat-square&label=build&logo=github&logoColor=white&logoWidth=20">
  </a>
  <a href="https://codecov.io/gh/bartoszlenar/Validot/branch/main">
    <img src="https://img.shields.io/codecov/c/gh/bartoszlenar/Validot/main?style=flat-square&logo=codecov&logoColor=white&logoWidth=20">
  </a>
</p>
<!--
<p align="center">
  <a href="https://github.com/bartoszlenar/Validot/releases">
    <img src="https://img.shields.io/github/v/release/bartoszlenar/Validot?include_prereleases&style=for-the-badge&label=latest%20pre-release%20version&logo=nuget&logoColor=white&logoWidth=20">
  </a>
</p>
 -->

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
    <a href="#project-info">
      Project info
    </a>
    |
    <a href="https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md">
      Documentation
    </a>
    |
    <a href="../docs/CHANGELOG.md">
      Changelog
    </a>
</div>

<p align="center">
    <a href="#validot-vs-fluentvalidation">
        🔥⚔️ Validot vs FluentValidation ⚔️🔥
    </a>
    </br>
    </br>
    <a href="https://lenar.dev/posts/validots-performance-explained">
        📜 Blogged: Validot's performance explained
    </a>
    </br>
    <a href="https://lenar.dev/posts/crafting-model-specifications-using-validot">
        📜 Blogged: Crafting model specifications using Validot
    </a>
</p>

<p align="center">
    Built with 🤘🏻by <a href="https://lenar.dev">Bartosz Lenar</a>
    </br>
    Supported by <a href="https://www.jetbrains.com/community/opensource/#support" target="_blank">JetBrains'</a> and <a href="https://opensource.microsoft.com/program/" target="_blank">Microsoft's</a> Open Source Programs (thanks!)
</p>
    </br>

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
        .Email()
        .WithExtraCode("ERR_EMAIL")
        .And()
        .MaxLength(100)
    )
    .Member(m => m.Name, m => m
        .Optional()
        .And()
        .LengthBetween(8, 100)
        .And()
        .Rule(name => name.All(char.IsLetterOrDigit))
        .WithMessage("Must contain only letter or digits")
    )
    .And()
    .Rule(m => m.Age >= 18 || m.Name != null)
    .WithPath("Name")
    .WithMessage("Required for underaged user")
    .WithExtraCode("ERR_NAME");
```

The next step is to create a [validator](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#validator). As its name stands - it validates objects according to the [specification](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#specification). It's also thread-safe so you can seamlessly register it as a singleton in your DI container.

``` csharp
var validator = Validator.Factory.Create(specification);
```

Validate the object!

``` csharp
var model = new UserModel(email: "inv@lidv@lue", age: 14);

var result = validator.Validate(model);
```

The [result](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#result) object contains all information about the [errors](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#error-output). Without retriggering the validation process, you can extract the desired form of an output.

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

* [See this example's real code](../tests/Validot.Tests.Functional/Readme/QuickStartFuncTests.cs)

## Features

### Advanced fluent API, inline

No more obligatory if-ology around input models or separate classes wrapping just validation logic. Write [specifications](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#specification) inline with simple, human-readable [fluent API](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#fluent0api). Native support for properties and fields, structs and classes, [nullables](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#asnullable), [collections](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#ascollection), [nested members](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#member), and possible combinations.

``` csharp
Specification<string> nameSpecification = s => s
    .LengthBetween(5, 50)
    .SingleLine()
    .Rule(name => name.All(char.IsLetterOrDigit));

Specification<string> emailSpecification = s => s
    .Email()
    .And()
    .Rule(email => email.All(char.IsLower))
    .WithMessage("Must contain only lower case characters");

Specification<UserModel> userSpecification = s => s
    .Member(m => m.Name, nameSpecification)
    .WithMessage("Must comply with name rules")
    .And()
    .Member(m => m.PrimaryEmail, emailSpecification)
    .And()
    .Member(m => m.AlternativeEmails, m => m
        .Optional()
        .And()
        .MaxCollectionSize(3)
        .WithMessage("Must not contain more than 3 addresses")
        .And()
        .AsCollection(emailSpecification)
    )
    .And()
    .Rule(user => {

        return user.PrimaryEmail is null || user.AlternativeEmails?.Contains(user.PrimaryEmail) == false;

    })
    .WithMessage("Alternative emails must not contain the primary email address");
```

* [Blog post about constructing specifications in Validot](https://lenar.dev/posts/crafting-model-specifications-using-validot)
* [Guide through Validot's fluent API](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#fluent-api)
* [If you prefer the approach of having a separate class for just validation logic, it's also fully supported](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#specification-holder)

### Validators

Compact, highly optimized, and thread-safe objects to handle the validation.

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

* [What Validator is and how it works](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#validator)
* [More about template and how to use it](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#template)

### Results

Whatever you want. [Error flag](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#anyerrors), compact [list of codes](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#codes), or detailed maps of [messages](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#messagemap) and [codes](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#codemap). With sugar on top: friendly [ToString() printing](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#tostring) that contains everything, nicely formatted.


``` csharp
var validationResult = validator.Validate(signUpModel);

if (validationResult.AnyErrors)
{
    // check if a specific code has been recorded for Email property:
    if (validationResult.CodeMap["Email"].Contains("DOMAIN_BANNED"))
    {
        _actions.NotifyAboutDomainBanned(signUpModel.Email);
    }

    var errorsPrinting = validationResult.ToString();

    // save all messages and codes printing into the logs
    _logger.LogError("Errors in incoming SignUpModel: {errors}", errorsPrinting);

    // return all error codes to the frontend
    return new SignUpActionResult
    {
        Success = false,
        ErrorCodes = validationResult.Codes,
    };
}
```

* [Validation result types](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#result)

### Rules

Tons of [rules available out of the box](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#rules). Plus, an easy way to [define your own](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#custom-rules) with the full support of Validot internal features like [formattable message arguments](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#message-arguments).

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

* [List of built-in rules](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#rules)
* [Writing custom rules](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#custom-rules)
* [Message arguments](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#message-arguments)

### Translations

Pass errors directly to the end-users in the language of your application.

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

At the moment Validot delivers the following translations out of the box: [Polish](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#withpolishtranslation), [Spanish](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#withspanishtranslation), [Russian](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#withrussiantranslation), [Portuguese](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#withportuguesetranslation) and [German](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#withgermantranslation).

* [How translations work](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#translations)
* [Custom translation](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#custom-translation)
* [How to selectively override built-in error messages](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#overriding-messages)

### Dependency injection

Although Validot doesn't contain direct support for the dependency injection containers (because it aims to rely solely on the .NET Standard 2.0), it includes helpers that can be used with any DI/IoC system.

For example, if you're working with ASP.NET Core and looking for an easy way to register all of your validators with a single call (something like `services.AddValidators()`), wrap your specifications in the [specification holders](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#specification-holder), and use the following snippet:

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    // ... registering other dependencies ...

    // Registering Validot's validators from the current domain's loaded assemblies
    var holderAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    var holders = Validator.Factory.FetchHolders(holderAssemblies)
        .GroupBy(h => h.SpecifiedType)
        .Select(s => new
        {
            ValidatorType = s.First().ValidatorType,
            ValidatorInstance = s.First().CreateValidator()
        });
    foreach (var holder in holders)
    {
        services.AddSingleton(holder.ValidatorType, holder.ValidatorInstance);
    }

    // ... registering other dependencies ...
}
```

* [What specification holders are and how to create them](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#specification-holder)
* [Fetching specification holders from assemblies](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#fetching-holders)
* [Writing the fully-featured `AddValidators` extension step-by-step](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#dependency-injection)


## Validot vs FluentValidation

A short statement to start with - [@JeremySkinner](https://twitter.com/JeremySkinner)'s [FluentValidation](https://fluentvalidation.net/) is an excellent piece of work and has been a huge inspiration for this project. True, you can call Validot a direct competitor, but it differs in some fundamental decisions, and lot of attention has been focused on entirely different aspects. If - after reading this section - you think you can bear another approach, api and [limitations](#fluentValidations-features-that-validot-is-missing), at least give Validot a try. You might be positively surprised. Otherwise, FluentValidation is a good, safe choice, as Validot is certainly less hackable, and achieving some particular goals might be either difficult or impossible.

### Validot is faster and consumes less memory

This document shows oversimplified results of [BenchmarkDotNet](https://benchmarkdotnet.org/) execution, but the intention is to present the general trend only. To have truly reliable numbers, I highly encourage you to [run the benchmarks yourself](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#benchmarks).

There are three data sets, 10k models each; `ManyErrors` (every model has many errors), `HalfErrors` (circa 60% have errors, the rest are valid), `NoErrors` (all are valid) and the rules reflect each other as much as technically possible. I did my best to make sure that the tests are just and adequate, but I'm a human being and I make mistakes. Really, if you spot errors [in the code](https://github.com/bartoszlenar/Validot/tree/cdca31a2588bf801288ef73e8ca50bfd33be8049/tests/Validot.Benchmarks), framework usage, applied methodology... or if you can provide any counterexample proving that Validot struggles with some particular scenarios - I'd be very very very happy to accept a PR and/or discuss it on [GitHub Issues](https://github.com/bartoszlenar/Validot/issues).

To the point; the statement in the header is true, but it doesn't come for free. Wherever possible and justified, Validot chooses performance and less allocations over [flexibility and extra features](#fluentvalidations-features-that-validot-is-missing). Fine with that kind of trade-off? Good, because the validation process in Validot might be **~1.6x faster while consuming ~4.7x less memory** (in the most representational, `Validate` tests using `HalfErrors` data set). Especially when it comes to memory consumption, Validot could be even 13.3x better than FluentValidation (`IsValid` tests with `HalfErrors` data set) . What's the secret? Read my blog post: [Validot's performance explained](https://lenar.dev/posts/validots-performance-explained).

| Test | Data set | Library | Mean [ms] | Allocated [MB] |
| - | - | - | -: | -: |
| Validate | `ManyErrors` | FluentValidation | `703.83` | `453` |
| Validate | `ManyErrors` | Validot | `307.04` | `173` |
| FailFast | `ManyErrors` | FluentValidation | `21.63` | `21` |
| FailFast | `ManyErrors` | Validot | `16.76` | `32` |
| Validate | `HalfErrors` | FluentValidation | `563.92` | `362` |
| Validate | `HalfErrors` | Validot | `271.62` | `81` |
| FailFast | `HalfErrors` | FluentValidation | `374.90` | `249` |
| FailFast | `HalfErrors` | Validot | `173.41` | `62` |
| Validate | `NoErrors` | FluentValidation | `559.77` | `354` |
| Validate | `NoErrors` | Validot | `260.99` | `75` |

* [Validate benchmark](../tests/Validot.Benchmarks/Comparisons/ValidationBenchmark.cs) - objects are validated.
* [FailFast benchmark](../tests/Validot.Benchmarks/Comparisons/ValidationBenchmark.cs) - objects are validated, the process stops on the first error.

FluentValidation's `IsValid` is a property that wraps a simple check whether the validation result contains errors or not. Validot has [AnyErrors](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#anyerrors) that acts the same way, and [IsValid](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#isvalid) is a special mode that doesn't care about anything else but the first rule predicate that fails. If the mission is only to verify the incoming model whether it complies with the rules (discarding all of the details), this approach proves to be better up to one order of magnitude:

| Test | Data set | Library | Mean [ms] | Allocated [MB] |
| - | - | - | -: | -: |
| IsValid | `ManyErrors` | FluentValidation | `20.91` | `21` |
| IsValid | `ManyErrors` | Validot | `8.21` | `6` |
| IsValid | `HalfErrors` | FluentValidation | `367.59` | `249` |
| IsValid | `HalfErrors` | Validot | `106.77` | `20` |
| IsValid | `NoErrors` | FluentValidation | `513.12` | `354` |
| IsValid | `NoErrors` | Validot | `136.22` | `24` |

* [IsValid benchmark](../tests/Validot.Benchmarks/Comparisons/ValidationBenchmark.cs) - objects are validated, but only to check if they are valid or not.

Combining these two methods in most cases could be quite beneficial. At first, [IsValid](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#isvalid) quickly verifies the object, and if it contains errors - only then [Validate](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#validate) is executed to report the details. Of course in some extreme cases (megabyte-size data? millions of items in the collection? dozens of nested levels with loops in reference graphs?) traversing through the object twice could neglect the profit. Still, for the regular web api input validation, it will undoubtedly serve its purpose:

``` csharp
if (!validator.IsValid(model))
{
    errorMessages = validator.Validate(model).ToString();
}
```

| Test | Data set | Library | Mean [ms] | Allocated [MB] |
| - | - | - | -: | -: |
| Reporting | `ManyErrors` | FluentValidation | `768.00` | `464` |
| Reporting | `ManyErrors` | Validot | `379.50` | `294` |
| Reporting | `HalfErrors` | FluentValidation | `592.50` | `363` |
| Reporting | `HalfErrors` | Validot | `294.60` | `76` |

* [Reporting benchmark](../tests/Validot.Benchmarks/Comparisons/ReportingBenchmark.cs):
  * FluentValidation validates model, and `ToString()` is called if errors are detected.
  * Validot processes the model twice - at first, with its special mode, [IsValid](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#isvalid). Secondly - in case of errors detected - with the standard method, gathering all errors and printing them with `ToString()`.

Benchmarks environment: Validot 2.3.0, FluentValidation 11.2.0, .NET 6.0.7, i7-9750H (2.60GHz, 1 CPU, 12 logical and 6 physical cores), X64 RyuJIT, macOS Monterey.

### Validot handles nulls on its own

In Validot, null is a special case [handled by the core engine](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#null-policy). You don't need to secure the validation logic from null as your predicate will never receive it.

``` csharp
Member(m => m.LastName, m => m
    .Rule(lastName => lastName.Length < 50) // 'lastName' is never null
    .Rule(lastName => lastName.All(char.IsLetter)) // 'lastName' is never null
)
```

### Validot treats null as an error by default

All values are marked as required by default. In the above example, if `LastName` member were null, the validation process would exit `LastName` scope immediately only with this single error message:

```
LastName: Required
```

The content of the message is, of course, [customizable](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#withmessage).

If null should be allowed, place [Optional](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#optional) command at the beginning:

``` csharp
Member(m => m.LastName, m => m
    .Optional()
    .Rule(lastName => lastName.Length < 50) // 'lastName' is never null
    .Rule(lastName => lastName.All(char.IsLetter)) // 'lastName' is never null
)
```

Again, no rule predicate is triggered. Also, null `LastName` member doesn't result with errors.

* [Read more about how Validot handles nulls](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#null-policy)

### Validot's Validator is immutable

Once [validator](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#validator) instance is created, you can't modify its internal state or [settings](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#settings). If you need the process to fail fast (FluentValidation's `CascadeMode.Stop`), use the flag:

``` csharp
validator.Validate(model, failFast: true);
```

### FluentValidation's features that Validot is missing

Features that might be in the scope and are technically possible to implement in the future:

* failing fast only in a single scope ([discuss it on GitHub Issues](https://github.com/bartoszlenar/Validot/issues/5))
* validated value in the error message ([discuss it on GitHub Issues](https://github.com/bartoszlenar/Validot/issues/6))

Features that are very unlikely to be in the scope as they contradict the project's principles, and/or would have a very negative impact on performance, and/or are impossible to implement:

* Full integration with ASP.NET or other frameworks:
  * Validot tries to remain a single-purpose library, depending only on .NET Standard 2.0. Thus all integrations need to be done individually.
  * However, Validot delivers [FetchHolders method](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#fetching-holders) that makes such integrations possible to wrap within a few lines of code. The quick example is in the [Dependency Injection section of this readme file](#dependency-injection), more advanced solution with explanation is contained [in the documentation](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#dependency-injection).
* Access to any stateful context in the rule condition predicate:
  * It implicates a lack of support for dynamic message content and/or amount.
* Callbacks:
  * Please react on [failure/success](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#anyerrors) after getting [validation result](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#result).
* Pre-validation:
  * All cases can be handled by additional validation and a proper if-else.
  * Also, the problem of the root being null doesn't exist in Validot (it's a regular case, [covered entirely with fluent api](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#presence-commands))
* Rule sets
  * workaround; multiple [validators](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#validator) for different parts of the object.
* `await`/`async` support
  * only support for large collections is planned ([more details on GitHub Issues](https://github.com/bartoszlenar/Validot/issues/2))
* severities  ([more details on GitHub Issues](https://github.com/bartoszlenar/Validot/issues/4))
  * workaround: multiple [validators](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#validator) for error groups with different severities.

## Project info

### Requirements

Validot is a dotnet class library targeting .NET Standard 2.0. There are no extra dependencies.

Please check the [official Microsoft document](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0) that lists all the platforms that can use it on.

### Versioning

[Semantic versioning](https://semver.org/) is being used very strictly. The major version is updated only when there is a breaking change, no matter how small it might be (e.g., adding extra method to the public interface). On the other hand, a huge pack of new features will bump the minor version only.

Before every major version update, at least one preview version is published.

### Reliability

Unit tests coverage hits 100% very close, and it can be detaily verified on [codecov.io](https://codecov.io/gh/bartoszlenar/Validot/branch/main).

Before publishing, each release is tested on the ["latest" version](https://help.github.com/en/actions/reference/virtual-environments-for-github-hosted-runners#supported-runners-and-hardware-resources) of the following operating systems:

* macOS
* Ubuntu
* Windows Server

using the upcoming, the current and all also the supported [LTS versions](https://dotnet.microsoft.com/platform/support/policy/dotnet-core) of the underlying frameworks:

* .NET 7.0 (preview 7)
* .NET 6.0
* .NET Core 3.1
* .NET Framework 4.8 (Windows only)

### Performance

Benchmarks exist in the form of  [the console app project](https://github.com/bartoszlenar/Validot/tree/5219a8da7cc20cd5b9c5c49dd5c0940e829f6fe9/tests/Validot.Benchmarks) based on [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet). Also, you can trigger performance tests [from the build script](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md#benchmarks).

### Documentation

The documentation is hosted alongside the source code, in the git repository, as a single markdown file: [DOCUMENTATION.md](./../docs/DOCUMENTATION.md).

Code examples from the documentation live as [functional tests](https://github.com/bartoszlenar/Validot/tree/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/tests/Validot.Tests.Functional).

### Development

The entire project ([source code](https://github.com/bartoszlenar/Validot), [issue tracker](https://github.com/bartoszlenar/Validot/issues), [documentation](https://github.com/bartoszlenar/Validot/blob/1a744d1021b40c84f9ce27d0f60f073e3bc1ed06/docs/DOCUMENTATION.md), and [CI workflows](https://github.com/bartoszlenar/Validot/actions)) is hosted here on github.com.

Any contribution is more than welcome. If you'd like to help, please don't forget to check out the [CONTRIBUTING](./../docs/CONTRIBUTING.md) file and [issues page](https://github.com/bartoszlenar/Validot/issues).

### Licencing

Validot uses the [MIT license](../LICENSE). Long story short; you are more than welcome to use it anywhere you like, completely free of charge and without oppressive obligations.