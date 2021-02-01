# Documentation

## Table of contents

- [Documentation](#documentation)
  - [Table of contents](#table-of-contents)
  - [Introduction](#introduction)
  - [Specification](#specification)
    - [Scope commands](#scope-commands)
    - [Parameter commands](#parameter-commands)
    - [Presence commands](#presence-commands)
    - [Error output](#error-output)
      - [Message](#message)
      - [Code](#code)
    - [Path](#path)
    - [Fluent api](#fluent-api)
      - [Rule](#rule)
      - [RuleTemplate](#ruletemplate)
      - [Member](#member)
      - [AsModel](#asmodel)
      - [AsCollection](#ascollection)
      - [AsNullable](#asnullable)
      - [WithCondition](#withcondition)
      - [WithPath](#withpath)
      - [WithMessage](#withmessage)
      - [WithExtraMessage](#withextramessage)
      - [WithCode](#withcode)
      - [WithExtraCode](#withextracode)
      - [Optional](#optional)
      - [Required](#required)
      - [Forbidden](#forbidden)
      - [And](#and)
    - [Null policy](#null-policy)
    - [Reference loop](#reference-loop)
  - [Validator](#validator)
    - [Validate](#validate)
    - [IsValid](#isvalid)
    - [Factory](#factory)
      - [Specification holder](#specification-holder)
      - [Settings holder](#settings-holder)
      - [Reusing settings](#reusing-settings)
      - [Fetching holders](#fetching-holders)
      - [Dependency injection](#dependency-injection)
    - [Settings](#settings)
      - [WithReferenceLoopProtection](#withreferenceloopprotection)
      - [WithTranslation](#withtranslation)
    - [Template](#template)
  - [Result](#result)
    - [AnyErrors](#anyerrors)
    - [Paths](#paths)
    - [Codes](#codes)
    - [CodeMap](#codemap)
    - [MessageMap](#messagemap)
    - [GetTranslatedMessageMap](#gettranslatedmessagemap)
    - [TranslationNames](#translationnames)
    - [ToString](#tostring)
  - [Rules](#rules)
    - [Global rules](#global-rules)
    - [Bool rules](#bool-rules)
    - [Char rules](#char-rules)
    - [Collections rules](#collections-rules)
    - [Numbers rules](#numbers-rules)
    - [Texts rules](#texts-rules)
    - [Times rules](#times-rules)
    - [Guid rules](#guid-rules)
    - [TimeSpan rules](#timespan-rules)
    - [Custom rules](#custom-rules)
  - [Message arguments](#message-arguments)
    - [Enum argument](#enum-argument)
    - [Guid argument](#guid-argument)
    - [Number argument](#number-argument)
    - [Text argument](#text-argument)
    - [Time argument](#time-argument)
    - [Translation argument](#translation-argument)
    - [Type argument](#type-argument)
    - [Path argument](#path-argument)
    - [Name argument](#name-argument)
  - [Translations](#translations)
    - [Overriding messages](#overriding-messages)
    - [Custom translation](#custom-translation)
  - [Development](#development)
    - [Build](#build)
    - [Tests](#tests)
    - [Benchmarks](#benchmarks)

## Introduction

- This documentation is written in short points.
  - Sometimes a point contains a subpoint.
  - Occasionally, a point could have a source code following it.
    - It's for demonstration, and the code is also commented in italic font.
- Most code examples in this documentation are using the following set of models:

``` csharp
public class BookModel
{
    public string Title { get; set; }
    public IEnumerable<AuthorModel> Authors { get; set; }
    public IEnumerable<Language> Languages { get; set; }
    public int YearOfFirstAnnouncement { get; set; }
    public int? YearOfPublication { get; set; }
    public PublisherModel Publisher { get; set; }
    public bool IsSelfPublished { get; set; }
}

public class AuthorModel
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class PublisherModel
{
    public string CompanyId { get; set; }
    public string Name { get; set; }
}

public enum Language
{
    English,
    Polish
}
```

_Comments are usually placed below the code sample, but that's not the rock-solid principle. The important thing is that they are related to the preceding point, while the next point starts the new thing._

- Vast majority of the code snippets live as functional tests in the [separate project](../tests/Validot.Tests.Functional/).

---

## Specification

- Specification is an expression that uses [fluent api](#fluent-api) to describe all conditions of a valid object.
- Technically, [specification is a generic delegate](../src/Validot/Specification/Specification.cs), and in most cases, you'll see it in the form of a lambda function.
  - If you prefer the approach of wrapping validation logic into a separate class, use the [specification holder](#specification-holder).
- Specification - considered purely as a C# function - is executed by the [validator](#validator) during its construction (directly or through the [factory](#factory)).
  - However the validation logic (that specification contains in the form of predicates) is triggered only when [validator](#validator) calls [Validate](#validate) method.
- Fluent api consist of commands called in so-called method chain:

``` csharp
Specification<int> yearSpecification = m => m
    .GreaterThan(-10000)
    .NotEqualTo(0).WithMessage("There is no such year as 0")
    .LessThan(3000);
```

_Above; four chained commands: `GreaterThan`, `NotEqualTo`, `WithMessage`, `LessThan`. All of them - the entire specification - is the single scope that validates value of type `int`._

- Logically, specification consist of scopes. And the scope could be explained as:
  - Set of commands that describe validation rules for the same value.
    - This value is often referred to in this documentation as "scope value".
  - If the value is null, scope acts according to the [null policy](#null-policy).

``` csharp
Specification<int> yearSpecification = s => s
    .GreaterThan(-10000)
    .NotEqualTo(0).WithMessage("There is no such year as 0")
    .LessThan(3000);

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.YearOfFirstAnnouncement, yearSpecification)
    .Member(m => m.YearOfPublication, m => m
        .Positive()
    )
    .Rule(m => m.YearOfPublication == m.YearOfFirstAnnouncement).WithMessage("Same year in both places is invalid");
```

_Above; `yearSpecification` contains four commands in its scope, all validating the value of type `int`._

_Next one, `bookSpecification`, is more complex. Let's analyse it:_

_First [Member](#member) command steps into the `BookModel`'s member of type `int` named `YearOfFirstAnnouncement` and in its scope validates the value using the `yearSpecification` defined earlier._

_Second [Member](#member) command opens scope that validates `YearOfPublication`; this scope contains single rule, `Positive`. Also, according to the [null policy](#null-policy), it requires the nullable member `YearOfPublication` to have a value._

_The last [scope command](#scope-commands), [Rule](#rule) contains a piece of logic for `BookModel` and [parameter command](#parameter-commands) [WithMessage](#withmessage) defines the error message if the predicate fails._

- You can also say that specification is a scope. A "root level" scope.
  - All commands and their logic are related to a single value (of type `T` in `Specification<T>`).
  - The [null policy](#null-policy) is followed here as well.
  - Commands that validate parts of the model are using... specification to describe the scope rules.
    - Even the root scope behaves as it was placed in [AsModel](#asmodel) command.
- There are three types of commands:
  - [Scope commands](#scope-commands) - contain validation logic and produce [error output](#error-output).
  - [Parameter commands](#paramter-commands) - changes the behavior of the preceding [scope command](#scope-commands).
  - [Presence commands](#presence-commands) - sets the scope behavior in case of null value.

---

### Scope commands

- Scope command is a command that validates the model by:
  - executing the validation logic directly:
    - [Rule](#rule) - executes a custom predicate.
    - [RuleTemplate](#ruletemplate) and all of the [built-in rules](#rules) - executes a predefined piece of logic.
  - executing the validation logic wrapped in another [specification](#specification), in the way dependent on the scope value type:
    - [Member](#member) - executes specification on the model's member.
    - [AsModel](#asmodel) - executes specification on the model.
    - [AsCollection](#ascollection) - executes specification on each item of the collection type model.
    - [AsNullable](#asnullable) - executes specification on the value of the nullable type model.

``` csharp
Specification<AuthorModel> authorSpecification = m => m
    .Member(m => m.Name, m => m.NotWhiteSpace().MaxLength(100))
    .Member(m => m.Email, m => m.Email())
    .Rule(m => m.Email != m.Name);
```

_In the above code you can see the specification containing only scope commands._


- Scope command produces [error output](#error-output) if - by any bit of a validation logic - the scope value is considered as invalid.
- How is "scope" term related with scope command?
  - Good to read; [Specification](#specification) - also tries to describe what is a scope.
  - All scope commands (except for [Rule](#rule) and [RuleTemplate](#ruletemplate)) validate the value by executing a specification (which is a scope).
  - [Rule](#rule) and [RuleTemplate](#ruletemplate) are slightly different. They contain the most atomic part of validation logic - a predicate. They are still [scope commands](#scope-commands), because:
    - They determine if the value is valid or not. The only difference is that they execute the logic directly instead of wrapped within another scope.
    - They produce [error output](#error-output) in case of validation error.

---

### Parameter commands

- Parameter command is a command that affects (parametrizes) the closest [scope command](#scope-commands) placed before it.
  - [WithCondition](#withcondition) - sets execution condition.
  - [WithPath](#withpath) - sets the path for the [error output](#error-output).
  - [WithMessage](#withmessage) - overwrites the entire [error output](#error-output) with a single message.
  - [WithExtraMessage](#withextramessage) - appends a single message to the [error output](#error-output).
  - [WithCode](#withcode) - overwrites the entire [error output](#error-output) with a single code.
  - [WithExtraCode](#withextracode) - appends a single code to the [error output](#error-output).
- Parameter commands have their order strictly defined and enforced by the language constructs.
  - So you might notice that some commands are not available from certain places.
    - Example: [AsNullable](#asnullable) can't be called in the scope that validates `int`.
    - Example: [WithCode](#withcode) can't be called after [WithMessage](#withmessage), because that doesn't make much sense (double overwrite...).
  - To know what other commands are allowed to be placed before/after, read the section about the particular command.
- It doesn't matter how many parameter commands are defined in the row - they are all related to the closest preceding [scope command](#scope-command) (or [presence command](#presence-commands)).
    - All the parameter commands start with `With...`, so it's easy to group them visually:


``` csharp
Specification<AuthorModel> authorSpecification = s => s
    .Member(m => m.Name, m => m.NotWhiteSpace().MaxLength(100))
    .WithCondition(m => !string.IsNullOrEmpty(m.Name))
    .WithPath("AuthorName")
    .WithCode("AUTHOR_NAME_ERROR")

    .Member(m => m.Email, m => m.Email())
    .WithMessage("Invalid email!")
    .WithExtraCode("EMAIL_ERROR")

    .Rule(m => m.Email != m.Name)
    .WithCondition(m => m.Email != null && m.Name != null)
    .WithPath("Email")
    .WithMessage("Name can't be same as Email");
```

_Above, you can see that the first [Member](#member) command is configured with the following parameters commands: [WithCondition](#withcondition), [WithPath](#withpath) and [WithCode](#withcode)._

_The second [Member](#member) command is configured with [WithMessage](#withmessage), and [WithExtraCode](#withextracode) commands._

_The third scope command - [Rule](#rule) - is configured with [WithCondition](#withcondition), [WithPath](#withpath), and [WithMessage](#withmessage) commands_

---

### Presence commands

- Presence command is the command that defines the behavior of the entire scope in case of null scope value:
  - [Required](#required) - scope value must not be null.
    - if no presence command exists in the scope, this behavior is set implicitly, by default.
  - [Forbidden](#forbidden) - scope value must be null.
  - [Optional](#optional) - scope value can be null.
    - Value gets validated normally if it isn't null, but nothing happens if it is.
- Only one presence command is allowed within the scope.
- Presence command needs to be the first command in the scope.
- Presence commands produce [error output](#error-output) that can be modified with some of the [parameter commands](#parameter-commands).
  - Not all of them, because e.g. you can't change their [path](#path) or set an [execution condition](#withcondition).
- Good to read: [Handling nulls](#null-policy) - details about the null value validation strategy.

``` csharp
Specification<AuthorModel> authorSpecification = m => m
    .Optional()
    .Member(m => m.Name, m => m
        .Optional()
        .NotWhiteSpace()
        .MaxLength(100)
    )
    .Member(m => m.Email, m => m
        .Required().WithMessage("Email is obligatory.")
        .Email()
    )
    .Rule(m => m.Email != m.Name);
```

_In the example above the entire model is allowed to be null. Similarly - `Name` member. `Email` is required, but the error output will contain a custom message (`Email is obligatory.`) in case of null._

---

### Error output

- Error output is everything that is returned from the scope if - according to the internal logic - the scope value is invalid.
  - Therefore, the absence of error output means that the value is valid.
- Error output can contain:
  - [Error messages](#message) - human-readable messages explaining what went wrong.
  - [Error codes](#code) - flags that help to organize the logic around specific errors.
  - Both. There are no limitations around that. The error output can contain only messages, only codes, or a mix.
- The validation process assigns every error output to the [path](#path) where it was produced.
  - The [path](#path) shows the location where the error occurred.
  - Sometimes this documentation refers to this action as "saving error output _under the path_"
- Good to read:
  - [Result](#result) - how to get the error output from the validation process.
  - [Path](#path) - how the paths are constructed.

---

#### Message

- Messages are primarily targeted to humans.
  - Use case; logs and the details about invalid models incoming from the frontend.
  - Use case; rest api returning messages that frontend shows in the pop up.
- [Error output](#error-output) can contain one or more error messages.
- Good to read:
  - [Translations](#translations) - how to translate a message or [overwrite](#overriding-messages) the default one.
  - [Message arguments](#message-arguments) - how to use message arguments.
  - [MessageMap](#messagemap) - how to read messages from the [validation result](#result).
- Message can be set using [WithMessage](#withmessage), [WithExtraMessage](#withmessage), and [RuleTemplate](#ruletemplate) commands.

``` csharp
Specification<int> yearSpecification = s => s
    .Rule(year => year > -300)
        .WithMessage("Minimum year is 300 B.C.")
        .WithExtraMessage("Ancient history date is invalid.")
    .Rule(year => year != 0)
        .WithMessage("The year 0 is invalid.")
        .WithExtraMessage("There is no such year as 0.")
    .Rule(year => year < 10000)
        .WithMessage("Maximum year is 10000 A.D.");

var validator = Validator.Factory.Create(yearSpecification);

var result = validator.Validate(-500);

result.MessageMap[""][0] // Minimum year is 300 B.C.
result.MessageMap[""][1] // Ancient history date is invalid.

validator.ToString();
// Minimum year is 300 B.C.
// Ancient history date is invalid.
```

_In the above code, [MessageMap](#messagemap) holds the messages assigned to their paths. Empty string as a path means that the error is recorded for the root model._

- Printing returned by [ToString](#tostring) method includes the path before each message.

``` csharp
Specification<int> yearSpecification = s => s
    .Rule(year => year > -300)
        .WithMessage("Minimum year is 300 B.C.")
        .WithExtraMessage("Ancient history date is invalid.")
    .Rule(year => year != 0)
        .WithMessage("The year 0 is invalid.")
        .WithExtraMessage("There is no such year as 0.")
    .Rule(year => year < 10000)
        .WithMessage("Maximum year is 10000 A.D.");

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.YearOfFirstAnnouncement, yearSpecification)
    .Member(m => m.YearOfPublication, m => m.AsNullable(yearSpecification))
    .Rule(m => m.YearOfFirstAnnouncement <= m.YearOfPublication)
        .WithCondition(m => m.YearOfPublication.HasValue)
        .WithMessage("Year of publication must be after the year of first announcement");

var validator = Validator.Factory.Create(bookSpecification);

var book = new BookModel() { YearOfFirstAnnouncement = 0, YearOfPublication = -100 };

var result = validator.Validate(book);

result.MessageMap[""][0]; // Year of publication must be after the year of first announcement
result.MessageMap["YearOfFirstAnnouncement"][0]; // "The year 0 is invalid.
result.MessageMap["YearOfFirstAnnouncement"][1]; // There is no such year as 0.

result.ToString();
// Year of publication must be after the year of first announcement
// YearOfFirstAnnouncement: The year 0 is invalid.
// YearOfFirstAnnouncement: There is no such year as 0.
```

---

#### Code

- Codes are primarily for the parsers and interpreters - they should be short flags, easy to process.
- Code cannot contain white space characters.
- Good to read:
  - [CodeMap](#codemap) - how to read codes from the validation result.
  - [Codes](#codes) - a quick list of all codes from the result.

``` csharp
Specification<int> yearSpecification = s => s
    .Rule(year => year > -300)
        .WithCode("MAX_YEAR")
    .Rule(year => year != 0)
        .WithCode("ZERO_YEAR")
        .WithExtraCode("INVALID_VALUE")
    .Rule(year => year < 10000)
        .WithCode("MIN_YEAR");

var validator = Validator.Factory.Create(yearSpecification);

var result = validator.Validate(0);

result.Codes; // [ "ZERO_YEAR", "INVALID_VALUE" ]

result.CodeMap[""][0]; // [ "ZERO_YEAR" ]
result.CodeMap[""][1]; // [ "INVALID_VALUE" ]

result.ToString();
// ZERO_YEAR, INVALID_VALUE
```

_In the above example, [CodeMap](#codemap) acts similarly to [MessageMap](#messagemap). Also, for your convenience, [Codes](#codes) holds all the error codes in one place. [ToString()](#tostring) called on the result prints error codes, coma separated, in the first line._

---

### Path

- Path is a string that shows the way of reaching the value that is invalid.
  - "The way" means which members need to be traversed through in order to reach the particular value.
  - Example; `Author.Email` path describes the value of `Email` that is inside `Author`.
- Path contains segments, and each one stands for one member that the validation context needs to enter in order to reach the value.
  - Path segments are separated with `.` (dot character).
  - [Member](#member), which is the way of stepping into the nested level uses the member's name as a segment.

``` csharp
model.Member.NestedMember.MoreNestedMember.Email = "invalid_email_value";

var result = validator.Validate(model);

result.MessageMap["Member.NestedMember.MoreNestedMember.Email"][0]; // Must be a valid email address

result.ToString();
// Member.NestedMember.MoreNestedMember.Email: Must be a valid email address
```

- When it comes to collections (validated with [AsCollection](#ascollection), n-th (counting from zero) item is considered as the member named `#n`.

``` csharp
model.MemberCollection[0].NestedMember.MoreNestedMemberCollection[23].Email = "invalid_email_value";

var result = validator.Validate(model);

result.MessageMap["MemberCollection[0].NestedMember.MoreNestedMemberCollection[23].Email"][0]; // Must be a valid email address

result.ToString();
// MemberCollection[0].NestedMember.MoreNestedMemberCollection[23]: Must be a valid email address
```

_Above, `MemberCollection.#0.NestedMember.MoreNestedMemberCollection.#23.Email:` is the path that leads through 1st item of `MemberCollection` and 24th item of `MoreNestedMemberCollection`._

- You are free to modify the path of every error output using [WithPath](#withpath).

---

### Fluent api

- The order the commands in the specification is strictly enforced by the language constructs. Invalid order means compilation error.

---

#### Rule

- `Rule` is a [scope command](#scope-commands).
  - Can be placed after:
    - any command except [Forbidden](#forbidden).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - any of the [parameter commands](#parameter-commands).
- `Rule` defines a single, atomic bit of validation logic with a predicate that accepts the scope value and returns:
  - `true`, if the scope value is valid.
  - `false`, if the scope value in invalid.

``` csharp
Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

Specification<int> ageSpecification = m => m.Rule(isAgeValid);

var ageValidator = Validator.Factory.Create(ageSpecification);

ageValidator.IsValid(12); // true
ageValidator.IsValid(20); // false

ageValidator.Validate(32).ToString();
// Error
```

- If the predicate returns `false`, the `Rule` scope returns [error output](#error-output).
  - The default error output of `Rule` command is a single [message](#message) key `Global.Error`
    - Default English translation for it is just `Error`.
  - It can be altered with [WithMessage](#withmessage) command.

``` csharp
Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

Specification<int> ageSpecification = m => m.Rule(isAgeValid).WithMessage("The age is invalid");

var ageValidator = Validator.Factory.Create(ageSpecification);

ageValidator.Validate(32).ToString();
// The age is invalid
```

_This is just a regular usage of [WithMessage](#withmessage) command that overwrites the entire [error output](#error-output) of the preceding [scope command](#scope-commands) (in this case - `Rule`)._

- `Rule` can be used to validate dependencies between the scope object's members.
  - If the [error output](#error-output) of such validation should be placed in the member scope rather than its parent, use [WithPath](#withpath) command.

``` csharp
Specification<BookModel> bookSpecification = m => m
    .Rule(book => book.IsSelfPublished == (book.Publisher is null)).WithMessage("Book must have a publisher or be self-published.");

var bookValidator = Validator.Factory.Create(bookSpecification);

bookValidator.Validate(new BookModel() { IsSelfPublished = true, Publisher = new PublisherModel() }).ToString();
// Book must have a publisher or be self-published.

bookValidator.Validate(new BookModel() { IsSelfPublished = true, Publisher = null }).AnyErrors; // false
```

- The value received in the predicate as an argument is never null.
  - All null-checks on it are redundant, no matter what code analysis has to say about it.
  - Although the received value is never null, its members could be!

``` csharp
Specification<PublisherModel> publisherSpecification = m => m
    .Rule(publisher =>
    {
        if (publisher.Title.Contains(publisher.CompanyId))
        {
            return false;
        }

        return true;
    });

var validator = Validator.Factory.Create(publisherSpecification);

validator.Validate(new PublisherModel()); // throws NullReferenceException
```

_In the above example, `publisher` argument is never null, but `Title` and `CompanyId` could be, thus it's high a risk of `NullReferenceException`._

- All unhandled exceptions are bubbled up to the surface and can be caught from `Validate` method.
  - Exceptions are unmodified and are not wrapped.

``` csharp
var verySpecialException = new VerySpecialException();

Specification<BookModel> bookSpecification = m => m.Rule(book => throw verySpecialException);

var bookValidator = Validator.Factory.Create(bookSpecification);

try
{
    bookValidator.Validate(new BookModel());
}
catch(VerySpecialException exception)
{
    object.ReferenceEquals(exception, verySpecialException); // true
}
```

- After processing the [Specification](#specification), the [validator](#validator) stores the predicate in its internals.
  - This is the very reason to be double-cautious when "capturing" variables in the predicate function as you're risking memory leak. Especially when the [validator](#validator) is registered as a singleton in a DI container.

---

#### RuleTemplate

- `RuleTemplate` is a [scope command](#scope-commands).
  - Can be placed after:
    - any command except [Forbidden](#forbidden).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - any of the [parameter commands](#parameter-commands).
- `RuleTemplate` is a special version of [Rule](#rule).
  - All of the details described in the [Rule](#rule) section also apply to `RuleTemplate`.
- The purpose of `RuleTemplate` is to deliver a convenient foundation for predefined, reusable rules.
  - All [built-in rules](#rules) use `RuleTemplate` under the hood. There are no exceptions, hacks, or special cases.
  - So if you decide to write your own [custom rules](#custom-rules), you're using the exact same api that the Validot uses.
- Technically, there is nothing wrong in placing `RuleTemplate` in the specification directly, but it's not considered as a good practice.
  - You should rather limit the usage of `RuleTemplate` to its purpose; [custom rules](#custom-rules).
- `RuleTemplate` accepts three parameters:
    - `Predicate<T>` - predicate that tells if the value is valid or not (exactly the same meaning as in [Rule](#rule)).
    - `message` - error message content. Required.
    - `args` - a collection of [arguments](#message-arguments) that can be used in the message content. Optional.
- `message` sets the single [error message](#message) that will be in the [error output](#error-output) if the predicate returns `false`.
  - So the result is the same as when using `Rule` followed by `WithMessage`. Below example presents that:

``` csharp
Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

Specification<int> ageSpecification1 = m => m.Rule(isAgeValid).WithMessage("The age is invalid");

Specification<int> ageSpecification2 = m => m.RuleTemplate(isAgeValid, "The age is invalid");

var ageValidator1 = Validator.Factory.Create(ageSpecification1);

var ageValidator2 = Validator.Factory.Create(ageSpecification2);

ageValidator1.Validate(32).ToString();
// The age is invalid

ageValidator2.Validate(32).ToString();
// The age is invalid
```

_The above code presents that there is no difference between the basic usage of [Rule](#rule) and [RuleTemplate](#ruletemplate)._

- `args` parameter is optional, and it's a collection of [arguments](#message-arguments) that can be used in placeholders within the error message.
  - Each argument needs to be created with `Arg` static factory
      - Ok, technically it doesn't _need_ to be created by the factory, but it's highly recommended as implementing `IArg` yourself could be difficult and more support for it is planned, but not in the very nearly future.
  - Factory contains helper methods to create arguments related with enums, types, texts, numbers, and guids.
  - When creating an argument, factory needs:
      - `name` - needs to be unique across the collection of arguments.
        - it's the base part of the placeholder: `{name}`
      - value - value that the message can use
  - `Arg.Number("minimum", 123)` - creates a number argument named `minimum` with `int` value of `123`
  - `Arg.Text("title", "Star Wars")` - creates text argument named `title` with `string` value of `"Star Wars"`
  - Good to read: [Message arguments](#message-arguments) - how to use arguments in messages
- Placeholders in the [error message](#message) will be replaced with the value of the related argument.
  - Name must be the same
  - Placeholder needs follow the pattern: `{argumentName}`

``` csharp
Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

Specification<int> ageSpecification = m => m
    .RuleTemplate(isAgeValid, "Age must be between {minAge} and {maxAge}", Arg.Number("minAge", 0), Arg.Number("maxAge", 18));

var ageValidator = Validator.Factory.Create(ageSpecification);

ageValidator.Validate(32).ToString();
// Age must be between 0 and 18
```

- Optionally, placeholders can contain additional parameters:
  - Good to read: [Message arguments](#message-arguments)

``` csharp
Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

Specification<int> ageSpecification = m => m
    .RuleTemplate(
        isAgeValid,
        "Age must be between {minAge|format=0.00} and {maxAge|format=0.00|culture=pl-PL}",
        Arg.Number("minAge", 0),
        Arg.Number("maxAge", 18)
    );

var ageValidator = Validator.Factory.Create(ageSpecification);

ageValidator.Validate(32).ToString();
// Age must be between 0.00 and 18,00
```

_Notice that the format follows dotnet [custom numeric format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings). The `maxAge` argument also has a different culture set (`pl-PL`, so `,` as a divider instead of `.`)._

- Not all arguments need to be used.
- One argument can be used more than once in the same message.
- If there is any error (like invalid name of the argument or parameter), no exception is thrown in the code, but the string, unformatted, goes directly to the error output.

``` csharp
Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

Specification<int> ageSpecification = m => m
    .RuleTemplate(
        isAgeValid,
        "Age must be between {minAge|format=0.00} and {maximumAge|format=0.00|culture=pl-PL}",
        Arg.Number("minAge", 0),
        Arg.Number("maxAge", 18)
    );

var ageValidator = Validator.Factory.Create(ageSpecification);

ageValidator.Validate(32).ToString();
// "Age must be between 0.00 and {maximumAge|format=0.00|culture=pl-PL}"
```

_In the above example, `maximumAge` is invalid argument name (`maxAge` would be OK in this case) and therefore - the placeholder stays as it is._

- `RuleTemplate` exposes its arguments to all [messages](#message) in its [error output](#error-output).
  - Each message can contain only a subset of arguments.
  - Each message is free to use any formatting it wants.

``` csharp
Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

Specification<int> ageSpecification = m => m
    .RuleTemplate(
        isAgeValid,
        "Age must be between {minAge|format=0.00} and {maxAge|format=0.00|culture=pl-PL}",
        Arg.Number("minAge", 0),
        Arg.Number("maxAge", 18)
    )
    .WithExtraMessage("Must be more than {minAge}")
    .WithExtraMessage("Must be below {maxAge|format=0.00}! {maxAge}!");

var ageValidator = Validator.Factory.Create(ageSpecification);

ageValidator.Validate(32).ToString();
// Age must be between 0.00 and 18,00
// Must be more than 0
// Must be below 18.00! 18!
```

- Arguments passed to `RuleTemplate` are also available in [WithMessage](#withmessage) and [WithExtraMessage](#withextramessage).

``` csharp
Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

Specification<int> ageSpecification = m => m
    .RuleTemplate(
        isAgeValid,
        "Age must be between {minAge|format=0.00} and {maxAge|format=0.00|culture=pl-PL}",
        Arg.Number("minAge", 0),
        Arg.Number("maxAge", 18)
    )
    .WithMessage("Only {minAge}-{maxAge}!");

var ageValidator = Validator.Factory.Create(ageSpecification);

ageValidator.Validate(32).ToString();
// Only 0-18!
```

- Because all the [built-in rules](#rules) are based on `RuleTemplate`, this is the magic behind altering their error message and still having access to the arguments.

``` csharp
Specification<int> ageSpecification = m => m.Between(min: 0, max: 18).WithMessage("Only {min}-{max|format=0.00}!");

var ageValidator = Validator.Factory.Create(ageSpecification);

ageValidator.Validate(32).ToString();
// Only 0-18!
```

_In the above example, `Between` is a built-in rule for `int` type values that exposes `min` and `max` parameters to be used in the error messages._

- Good to read:
    - [Message arguments](#message-arguments) - everything about the available arguments, their types, and parameters.
    - [Custom rules](#custom-rules) - how to create a custom rule, step by step.
    - [Rules](#rules) - the detailed list of all arguments available in each of the built-in rule.

---

#### Member

- `Member` is a [scope command](#scope-commands).
  - Can be placed after:
    - any command except [Forbidden](#forbidden).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - any of the [parameter commands](#parameter-commands).
- `Member` executes a specification upon a scope object's member.
- `Member` command accepts:
  - member selector - a lambda expression pointing at a scope object's member.
  - specification - [specification](#specification) to be executed upon the selected member.
- Member selector serves two purposes:
  - It points at the member that will be validated with the passed [specification](#specification).
    - So technically it determines type `T` in `Specification<T>` that `Member` accepts as a second parameter.
  - It defines the nested path under which the entire [error output](#error-output) from the passed [specification](#specification) will be saved.
    - By default, if the member selector is `m => m.Author`, the [error output](#error-output) will be saved under the path `Author` (as a next segment).

``` csharp
Specification<string> nameSpecification = s => s
    .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
    .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!");

var nameValidator = Validator.Factory.Create(nameSpecification);

nameValidator.Validate("Adam !!!").ToString();
// Must consist of letters only!
// Must not contain whitespace!
```

_In the above example, you can see specification and validation of a string value. Let's use this exact specification inside `Member` command and observe how the entire output is saved under a nested path:_

``` csharp
Specification<PublisherModel> publisherSpecification = s => s
    .Member(m => m.Name, nameSpecification);

var publisherValidator = Validator.Factory.Create(publisherSpecification);

var publisher = new PublisherModel()
{
    Name = "Adam !!!"
};

publisherValidator.Validate(publisher).ToString();
// Name: Must consist of letters only!
// Name: Must not contain whitespace!
```

_Let's add one more level:_


``` csharp
Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Publisher, publisherSpecification);

var bookValidator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Publisher = new PublisherModel()
    {
        Name = "Adam !!!"
    }
};

authorValidator.Validate(book).ToString();
// Publisher.Name: Must consist of letters only!
// Publisher.Name: Must not contain whitespace!
```

- Whether to define a [specification](#specification) upfront and pass it to the `Member` command or define everything inline - it's totally up to you. It doesn't make any difference.
  - The only thing that is affected is the source code readability.
  - However, in some particular situations, reusing predefined specifications could lead to having an infinite reference loop in the object. This topic is covered in [Reference loop](#reference-loop) section.

``` csharp
Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Publisher, m => m
        .Member(m1 => m1.Name, m1 => m1
            .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
            .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!")
        )
    );

var bookValidator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Publisher = new PublisherModel()
    {
        Name = "Adam !!!"
    };
};

authorValidator.Validate(book).ToString();
// Publisher.Name: Must consist of letters only!
// Publisher.Name: Must not contain whitespace!
```

- Selected member can be only one level from the scope object!
  - No language construct prevents you from stepping into more nested levels (so no compilation errors), but then, during runtime, [validator](#validator) throws the exception from its constructor (or [factory](#factory)).
  - This behavior is very likely to be updated in the future versions, so such selectors might be allowed someday... but not now.

``` csharp
Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Publisher.Name, nameSpecification);

Validator.Factory.Create(bookSpecification); // throws exception
```

_In the above example, the exception is thrown because member selector goes two levels down (`Publisher.Name`). Please remember that one level down is allowed (just `Publisher` would be totally OK)._

- Selected member can be either property or variable.
  - It can't be a function.
- Type of selected member doesn't matter (can be a reference type, value type, string, enum, or whatever...).
- The default path for the [error output](#error-output) (determined by the member selector) can be altered using [WithPath](#withpath) command.
- If the selected member contains null, the member scope is still executed and the [error output](#error-output) entirely depends on the [specification](#specification).
  - It means that null member is not anything special. It's a normal situation, and the behavior relies on the passed [specification](#specification), its [presence commands](#presence-commands), and the [null handling strategy](#null-policy).

``` csharp
Specification<PublisherModel> publisherSpecification = s => s
    .Member(m => m.Name, m => m
        .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
        .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!")
    );

Specification<PublisherModel> publisherSpecificationRequired = s => s
    .Member(m => m.Name, m => m
        .Required().WithMessage("Must be filled in!")
        .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
        .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!")
    );

Specification<PublisherModel> publisherSpecificationOptional = s => s
    .Member(m => m.Name, m => m
        .Optional()
        .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
        .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!")
    );

var publisherValidator = Validator.Factory.Create(publisherSpecification);
var publisherValidatorRequired = Validator.Factory.Create(publisherSpecificationRequired);
var publisherValidatorOptional = Validator.Factory.Create(publisherSpecificationOptional);

var publisher = new PublisherModel()
{
    Name = null
};

publisherValidator.Validate(publisher).ToString();
// Name: Required

publisherValidatorRequired.Validate(publisher).ToString();
// Name: Must be filled in!

publisherValidatorOptional.Validate(publisher).AnyErrors; // false
```

_Without any [presence command](#presence-commands) in `publisherSpecification`, the default behavior is to require the scope value to be non-null. The [error message](#message) can be customized (`publisherSpecificationRequired`) with [Required](#required) command followed by [WithMessage](#withmessage)._

_If the specification starts with `Optional`, no error is returned from the member scope._

---

#### AsModel

- `AsModel` is a [scope command](#scope-commands).
  - Can be placed after:
    - any command except [Forbidden](#forbidden).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - any of the [parameter commands](#parameter-commands).
- `AsModel` executes a specification upon the scope value.
- `AsModel` command accepts only one argument; a specification `Specification<T>`, where `T` is the current scope type.
- Technically `AsModel` executes specification in the same scope that it lives itself.
  - So you can say it's like [Member](#member) command, but it doesn't step into any member.

``` csharp
Specification<string> emailSpecification = s => s
    .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!");

Specification<string> emailAsModelSpecification = s => s
    .AsModel(emailSpecification);

var emailValidator = Validator.Factory.Create(emailSpecification);
var emailAsModelValidator = Validator.Factory.Create(emailAsModelSpecification);

emailValidator.Validate("invalid email").ToString();
// Must contain @ character!

emailAsModelValidator.Validate("invalid email").ToString();
// Must contain @ character!
```

_In the above code you can see that it doesn't matter whether specification is used directly or through `AsModel` - the validation logic is the same and the [error output](#error-output) is saved under the same [path](#path)._

``` csharp
Specification<string> emailSpecification = s => s
    .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!");

Specification<string> emailNestedAsModelSpecification = s => s
    .AsModel(s1 => s1
        .AsModel(s2 => s2
            .AsModel(emailSpecification)
        )
    );

var emailValidator = Validator.Factory.Create(emailSpecification);
var emailNestedAsModelValidator = Validator.Factory.Create(emailNestedAsModelSpecification);

emailValidator.Validate("invalid email").ToString();
// Must contain @ character!

emailAsModelValidator.Validate("invalid email").ToString();
// Must contain @ character!
```

_The above example presents that even several levels of nested `AsModel` commands don't make any difference._

- `AsModel` can be used to execute many independent [specifications](#specification) on the same value.
  - Effectively, it's like merging [specifications](#specification) into one.

``` csharp
Specification<string> atRequiredSpecification = s => s
    .Rule(text => text.Contains('@')).WithMessage("Must contain @ character!");

Specification<string> allLettersLowerCaseSpecification = s => s
    .Rule(text => !text.Any(c => !char.IsLetter(c) || char.IsLower(c))).WithMessage("All letters need to be lower case!");

Specification<string> lengthSpecification = s => s
    .Rule(text => text.Length > 5).WithMessage("Must be longer than 5 characters")
    .Rule(text => text.Length < 20).WithMessage("Must be shorter than 20 characters");

Specification<string> emailSpecification = s => s
    .AsModel(atRequiredSpecification)
    .AsModel(allLettersLowerCaseSpecification)
    .AsModel(lengthSpecification);

var emailValidator = Validator.Factory.Create(emailSpecification);

emailValidator.Validate("Email").ToString();
// Must contain @ character!
// All letters need to be lower case!
// Must be longer than 5 characters
```

_In the above example, you can see how three separate [specifications](#specification) are - practically - combined into one._

- `AsModel` can be used to mix predefined specifications with inline rules.
  - Thanks to this, you might "modify" the presence rule in the predefined specification.

``` csharp
Specification<string> atRequiredSpecification = s => s
    .Rule(text => text.Contains('@')).WithMessage("Must contain @ character!");

Specification<string> allLettersLowerCaseSpecification = s => s
    .Rule(text => !text.Any(c => !char.IsLetter(c) || char.IsLower(c))).WithMessage("All letters need to be lower case!");

Specification<string> emailSpecification = s => s
    .Optional()
    .AsModel(atRequiredSpecification)
    .AsModel(allLettersLowerCaseSpecification)
    .Rule(text => text.Length > 5).WithMessage("Must be longer than 5 characters")
    .Rule(text => text.Length < 20).WithMessage("Must be shorter than 20 characters");

var emailValidator = Validator.Factory.Create(emailSpecification);

emailValidator.Validate("Email").ToString();
// Must contain @ character!
// All letters need to be lower case!
// Must be longer than 5 characters

emailValidator.Validate(null).AnyErrors; // false
```

_The example above shows that predefined [specification](#specification) can be expanded with more rules (`AsModel` and subsequent [Rule](#rule) commands)._

_Also, you can observe the interesting behavior that can be described as [presence rule](#presence-commands) alteration. Please notice that `emailSpecification` starts with [Optional](#optional) command that makes the entire model optional (null is allowed) and no error is returned even though both `atRequiredSpecification` and `allLettersLowerCaseSpecification` require model to be not null. Of course, technically it is NOT a modification of their presence settings, but the specification execution would never reach them. Why? The scope value is null, and the scope presence rule `Optional` allows this. And in case of null, as always, no further validation is performed in the scope. Not a big deal, but the example gives an overview of how to play with fluent-api bits to "modify" presence rule._

_Naturally, this works the other way around. Below a short demo of how to make a model required while only using specification that allows the model to be null:_


``` csharp
Specification<string> emailOptionalSpecification = s => s
    .Optional()
    .Rule(text => text.Contains('@')).WithMessage("Must contain @ character!");

Specification<string> emailSpecification = s => s
    .AsModel(emailOptionalSpecification);

var emailOptionalValidator = Validator.Factory.Create(emailOptionalSpecification);

var emailValidator = Validator.Factory.Create(emailSpecification);

emailOptionalValidator.Validate(null).AnyErrors; // false

emailOptionalValidator.Validate("Email").ToString();
// Must contain @ character!

emailValidator.Validate(null).ToString();
// Required

emailValidator.Validate("Email").ToString();
// Must contain @ character!
```

_As you can notice, null passed to `emailOptionalValidator` doesn't produce any validation errors (and it's okay, because the specification allows that with `Optional` command). Having the same specification in `AsModel` effectively changes this behavior. True, null passed to `AsModel` would not return any error output, but null never gets there. The root scope (`emailSpecification`) doesn't allow nulls and it terminates the validation before reaching `AsModel`._

- `AsModel` can be very helpful if you want to bundle many commands and want a single [error message](#message) if any of them indicates validation error.
  - Saying that, `AsModel` can wrap the entire [specification](#specification) and return single [error message](#message) out of it.
  - This is just a regular usage of [WithMessage](#withmessage) command and applies to all [scope commands](#scope-commands), not only `AsModel`. It's mentioned here only to present this very specific use case. For more details, please read the [WithMessage](#withmessage) section.

``` csharp
Specification<string> emailSpecification = s => s
    .Rule(text => text.Contains('@')).WithMessage("Must contain @ character!")
    .Rule(text => !text.Any(c => !char.IsLetter(c) || char.IsLower(c))).WithMessage("All letters need to be lower case!")
    .Rule(text => text.Length > 5).WithMessage("Must be longer than 5 characters")
    .Rule(text => text.Length < 20).WithMessage("Must be shorter than 20 characters");

Specification<string> emailWrapperSpecification = s => s
    .AsModel(emailSpecification).WithMessage("This value is invalid as email address");

var emailValidator = Validator.Factory.Create(emailSpecification);

var emailWrapperValidator = Validator.Factory.Create(emailWrapperSpecification);

emailValidator.Validate("Email").ToString();
// Must contain @ character!
// All letters need to be lower case!
// Must be longer than 5 characters

emailWrapperValidator.Validate("Email").ToString();
// This value is invalid as email address
```

_Above, `emailSpecification` contains multiple rules and - similarly - can have several [messages](#message) in its [error output](#error-output). When wrapped within `AsModel` followed by `WithMessage` command, any validation failure results with just a single error message._

_The advantage of this combination is even more visible when you define [specification](#specification) inline and skip all of the error messages attached to the rules - they won't ever be in the output anyway._


``` csharp
Specification<string> emailSpecification = s => s
    .AsModel(s1 => s1
        .Rule(text => text.Contains('@'))
        .Rule(text => !text.Any(c => !char.IsLetter(c) || char.IsLower(c)))
        .Rule(text => text.Length > 5)
        .Rule(text => text.Length < 20)
    ).WithMessage("This value is invalid as email address");

var emailValidator = Validator.Factory.Create(emailSpecification);

emailValidator.Validate("Email").ToString();
// This value is invalid as email address
```

---

#### AsCollection

- `AsCollection` is a [scope command](#scope-commands).
  - Can be placed after:
    - any command except [Forbidden](#forbidden).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - any of the [parameter commands](#parameter-commands).
- `AsCollection` command has two generic type parameters: `AsCollection<T, TItem>`, where:
  - `TItem` - is a type of the single item in the collection.
  - `T` - is derived from `IEnumerable<TItem>`.
- `AsCollection` has dedicated versions for some dotnet native collections, so you don't need to specify a pair of `IEnumerable<TItem>` and `TItem` while dealing with:
  - `T[]`
  - `IEnumerable<T>`
  - `ICollection<T>`
  - `IReadOnlyCollection<T>`
  - `IList<T>`
  - `IReadOnlyList<T>`
  - `List<T>`
- `AsCollection` accepts one parameter; item [specification](#specification) `Specification<TItem>`.
- `AsCollection` executes the passed [specification](#specification) upon each item in the collection.
  - Internally, getting the items out of the collection is done using `foreach` loop.
    - Validation doesn't materialize the collection. Elements are picked up using enumerator (as in standard `foreach` loop).
    - So it might get very tricky when you implement IEnumerable yourself; there is no protection against an infinite stream of objects coming from the enumerator, etc.
  - Items are validated one after another, sequentially.
      - Support for async collection validation is coming in the future releases.
- [Error output](#error-output) from the n-th item in the collection is saved under the path `#n`.
  - The counting starts from zero (the first item in the collection is `0` and its [error output](#error-output) will be saved under `#0`).
  - Validation uses the standard `foreach` loop over the collection, so "n-th item" really means "n-th item received from enumerator".
    - For some types, the results won't be deterministic, simple because the collection itself doesn't guarantee to keep the order. It might happen that the error output saved under path `#1` next time will be saved under `#13`. This could be a problem for custom collections or some particular use cases, like instance of `HashSet<TItem>` that gets modified between the two validations. But it will never happen for e.g. array or `List<T>`.


``` csharp
Specification<int> evenNumberSpecification = s => s
    .Rule(number => (number % 2) == 0).WithMessage("Number must be even");

Specification<int[]> specification = s => s
    .AsCollection(evenNumberSpecification);

var validator = Validator.Factory.Create(specification);

var numbers = new[] { 1, 2, 3, 4, 5 };

validator.Validate(numbers).ToString();
// #0: Number must be even
// #2: Number must be even
// #4: Number must be even
```

_`AsCollection` is able to automatically resolve the type parameters for array. In this case, `AsCollection` is `AsCollection<int[], int>` under the hood._

- `AsCollection` makes sense only if the type validated in the scope is a collection
  - Well... technically, that's not entirely true, because the only requirement is that it implements `IEnumerable<TItem>` interface.
  - Code completion tools (IntelliSense, Omnisharp, etc.) will show `AsCollection` as always available, but once inserted you'll need to define `T` and `TItem`, so effectively - `AsCollection` works only for collections.

_Let's consider a custom class holding two collections:_

``` csharp
class NumberCollection : IEnumerable<int>, IEnumerable<double>
{
    public IEnumerable<int> Ints { get; set; }
    public IEnumerable<double> Doubles { get; set; }

    IEnumerator<double> IEnumerable<double>.GetEnumerator() => Doubles.GetEnumerator();

    IEnumerator<int> IEnumerable<int>.GetEnumerator() => Ints.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<int>)this).GetEnumerator();
}
```

_You can use `AsCollection` to validate an object as a collection of any type; as long as you are able to specify both generic type parameters:_

``` csharp
Specification<int> evenNumberSpecification = s => s
    .Rule(number => (number % 2) == 0).WithMessage("Number must be even");

Specification<double> smallDecimalSpecification = s => s
    .Rule(number => Math.Floor(number) < 0.5).WithMessage("Decimal part must be below 0.5");

Specification<NumberCollection> specification = s => s
    .AsCollection<NumberCollection, int>(evenNumberSpecification)
    .AsCollection<NumberCollection, double>(smallDecimalSpecification);

var validator = Validator.Factory.Create(specification);

var numberCollection = new NumberCollection()
{
    Ints = new [] { 1, 2, 3, 4, 5 },
    Doubles = new [] { 1.1, 2.8, 3.3, 4.6, 5.9 }
}

validator.Validate(numberCollection).ToString();
// #0: Number must be even
// #1: Decimal part must be below 0.5
// #2: Number must be even
// #3: Decimal part must be below 0.5
// #4: Number must be even
// #4: Decimal part must be below 0.5
```

_Above, `AsCollection` command triggers validation of `NumberCollection` as a collection of `int` and `double` items, each with their own [specification](#specification)._

- `AsCollection` doesn't treat the null item as anything special. The behavior is described by the passed [specification](#specification).
  - `AsCollection` is like [Member](#member) command, but the member selector is pointing at the collection items and the path is dynamic.

``` csharp
Specification<AuthorModel> authorSpecification = s => s
    .Member(m => m.Email, m => m
        .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!")
    );

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Authors, m => m.AsCollection(authorSpecification));

var bookValidator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Authors = new[]
    {
        null,
        new AuthorModel() { Email = "foo@bar" },
        new AuthorModel() { Email = null },
        null,
        new AuthorModel() { Email = "InvalidEmail" },
        null,
    }
};

bookValidator.Validate(book).ToString();
// Authors.#0: Required
// Authors.#2.Email: Required
// Authors.#3: Required
// Authors.#4.Email: Must contain @ character!
// Authors.#5: Required
```

_In the code above you can see that null items in the collection result with the default [error message](#message). This is because `authorSpecification` doesn't allow nulls._

_Let's change this and see what happens:_


``` csharp
Specification<AuthorModel> authorSpecification = s => s
    .Optional()
    .Member(m => m.Email, m => m
        .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!")
    );

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Authors, m => m.AsCollection(authorSpecification));

var bookValidator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Authors = new[]
    {
        null,
        new AuthorModel() { Email = "foo@bar" },
        new AuthorModel() { Email = null },
        null,
        new AuthorModel() { Email = "InvalidEmail" },
        null,
    }
};

validator.Validate(book).ToString();
// Authors.#2.Email: Required
// Authors.#4.Email: Must contain @ character!
```

_Above, `authorSpecification` starts with [Optional](#optional) command, and therefore null items in the collection are allowed._

- `AsCollection` validates the collection items, but the collection itself (as an object) can be normally validated in its own scope normally, as any other value.
  - One of the widespread use cases is to verify the collection size:

``` csharp
Specification<AuthorModel> authorSpecification = s => s
    .Optional()
    .Member(m => m.Email, m => m
        .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!")
    );

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Authors, m => m
        .AsCollection(authorSpecification)
        .Rule(authors => authors.Count() <= 5).WithMessage("Book can have max 5 authors.")
    );

var bookValidator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Authors = new[]
    {
        null,
        new AuthorModel() { Email = "foo@bar" },
        new AuthorModel() { Email = null },
        null,
        new AuthorModel() { Email = "InvalidEmail" },
        null,
    }
};

bookValidator.Validate(book).ToString();
// Authors.#2.Email: Required
// Authors.#4: Must contain @ character!
// Authors: Book can have max 5 authors.
```

---

#### AsNullable

- `AsNullable` is a [scope command](#scope-commands).
  - Can be placed after:
    - any command except [Forbidden](#forbidden).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - any of the [parameter commands](#parameter-commands).
- `AsNullable` "unwraps" the nullable value and provides the way to validate it with a [specification](#specification).
- `AsNullable` accepts a single parameter; `Specification<T>`, where `T` is a value type wrapped in `Nullable<T>` (`T?`).
- Null value never reaches `AsNullable`, exactly as [handling nulls policy](#null-policy) states.
  - The passed [specification](#specification) describes `T` that is a value type, so [Optional](#optional) command is not even available.
  - Null must be handled one level higher (in the [specification](#specification) that contains `AsNullable`).

``` csharp
Specification<int> numberSpecification = s => s
    .Rule(number => number < 10).WithMessage("Number must be less than 10");

Specification<int?> nullableSpecification = s => s
    .AsNullable(numberSpecification);

var validator = Validator.Factory.Create(nullableSpecification);

validator.Validate(5).AnyErrors; // false

validator.Validate(15).ToString();
// Number must be less than 10

validator.Validate(null).ToString();
// Required
```

_In the above code, `Validate` method accepts `int?`. You can observe that the value is unwrapped by `AsNullable` and validated with `numberSpecification` (that describes just `int`)._

_If the nullable value is null, it is stopped at the level of `nullableSpecification`, which doesn't allow nulls. Of course, you can change this behavior:_

``` csharp
Specification<int> numberSpecification = s => s
    .Rule(number => number < 10).WithMessage("Number must be less than 10");

Specification<int?> nullableSpecification = s => s
    .Optional()
    .AsNullable(numberSpecification);

var validator = Validator.Factory.Create(nullableSpecification);

validator.Validate(5).AnyErrors; // false

validator.Validate(null).AnyErrors; // false

validator.Validate(15).ToString();
// Number must be less than 10
```

_Now, `nullableSpecification` starts with [Optional](#optional) command, and therefore - null doesn't result with an error. On the other hand - if nullable has a value, it is passed and validated with `numberSpecification`._

- [Every built-in rule](#rules) for a value type has an extra variant for the nullable of this type.
  - So you don't need to provide `AsNullable` in the most popular and simple cases.

``` csharp
Specification<int> numberSpecification = s => s.GreaterThan(0).LessThan(10);

Specification<int?> nullableSpecification = s => s.GreaterThan(0).LessThan(10);

var numberValidator = Validator.Factory.Create(numberSpecification);
var nullableValidator = Validator.Factory.Create(nullableSpecification);

numberValidator.Validate(5).AnyErrors; // false
nullableValidator.Validate(5).AnyErrors; // false

numberValidator.Validate(15).ToString();
// Must be less than 10

nullableValidator.Validate(15).ToString();
// Must be less than 10
```

_In the above code, `GreaterThan` and `LessThan` can be applied to both `Specification<int?>` and `Specification<int>`. Technically, they are two separate rules with same names. The consistency of their inner logic is verified by the unit tests._

- `AsNullable` can be handy when you have two versions of the same type (nullable and non-nullable) that can be validated with the same specification.

``` csharp
Specification<int> yearSpecification = s => s
    .Rule(year => year >= -3000).WithMessage("Minimum year is 3000 B.C.")
    .Rule(year => year <= 3000).WithMessage("Maximum year is 3000 A.D.");

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.YearOfFirstAnnouncement, yearSpecification)
    .Member(m => m.YearOfPublication, m => m
        .Optional()
        .AsNullable(yearSpecification)
    );

var bookValidator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    YearOfFirstAnnouncement = -4000,
    YearOfPublication = 4000
};

bookValidator.Validate(book).ToString()
// YearOfFirstAnnouncement: Minimum year is 3000 B.C.
// YearOfPublication: Maximum year is 3000 A.D.
```

_Above the example how two members - nullable `YearOfPublication` and non-nullable `YearOfFirstAnnouncement` - can be validated with the same specification `yearSpecification`._

---

#### WithCondition

- `WithCondition` is a [parameter command](#parameter-commands).
  - Can be placed after:
    - the related [scope command](#scope-commands).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - other [parameter commands](#parameter-commands):  [WithPath](#withpath), [WithMessage](#withmessage), [WithExtraMessage](#withextramessage), [WithCode](#withcode), [WithExtraCode](#withextracode).
- `WithCondition` sets the execution condition to the related (preceding) [scope command](#scope-commands).
- `WithCondition` accepts single argument; a predicate `Predicate<T>`, where `T` is the current scope type.
  - So `T` is the same as in `Specification<T>` where the command lives.
  - The received argument is never null.
- If the predicate returns:
  - `true` - the related [scope command](#scope-commands) is going to be executed.
    - same behavior as if `When` wasn't there at all.
  - `false` - the related [scope command](#scope-commands) is skipped.
    - no validation logic defined in the [scope command](#scope-commands) is triggered.
    - no [error output](#error-output) is returned.

``` csharp
Predicate<string> isValidEmail = email => email.Substring(0, email.IndexOf('@')).All(char.IsLetterOrDigit);

Specification<string> emailSpecification = s => s
    .Rule(isValidEmail)
        .WithCondition(email => email.Contains('@'))
        .WithMessage("Email username must contain only letters and digits.");

var validator = Validator.Factory.Create(emailSpecification);

validator.Validate("John.Doe-at-gmail.com").AnyErrors; // false

validator.Validate("John.Doe@gmail.com").ToStringMessages();
// Email username must contain only letters and digits.
```

_Above, the predicate in `WithCondition` checks if the scope value contains `@` character. If true, then the related command scope ([Rule](#rule)) is executed._

_The code shows also that `WithCondition` can makes the code look more clean and readable, as `isValidEmail` predicate doesn't need to contain any logic around `email.IndexOf('@')` returning `-1`. It always has `@` at some position, because otherwise the condition in `WithCondition` prevents the entire `Rule` scope from execution._

- `WithCondition` can be used in pre-verification.
    - Example; it can ensure that all elements are non-null before validating the relation between them.

``` csharp
Predicate<BookModel> isAuthorAPublisher = book =>
{
    return book.Authors.Any(a => a.Name == book.Publisher.Name);
};

Specification<BookModel> bookSpecification = s => s
    .Rule(isAuthorAPublisher)
        .WithCondition(book =>
            book.IsSelfPublished &&
            book.Authors?.Any() == true &&
            book.Publisher?.Name != null
        )
        .WithMessage("Self-published book must have author as a publisher.");

var validator = Validator.Factory.Create(bookSpecification);

// 1: Condition is met, but the rule fails:
var bookModel1 = new BookModel()
{
    IsSelfPublished = true,
    Authors = new[] { new AuthorModel() { Name = "Bart" } },
    Publisher = new PublisherModel() { Name = "Adam" }
};

// 2: Condition is met, and the rule doesn't fail:
var bookModel2 = new BookModel()
{
    IsSelfPublished = true,
    Authors = new[] { new AuthorModel() { Name = "Bart" } },
    Publisher = new PublisherModel() { Name = "Bart" }
};

// 3: Condition is not met:
var bookModel3 = new BookModel()
{
    IsSelfPublished = false,
    Authors = new[] { new AuthorModel() { Name = "Bart" } },
    Publisher = null
};

validator.Validate(bookModel1).ToString();
// Self-published book must have author as a publisher.

validator.Validate(bookModel2).AnyErrors; // false

validator.Validate(bookModel3).AnyErrors; // false
```

_Validot never passes null into predicates, but in the above code `isAuthorAPublisher` doesn't care at all about null also at the nested levels (`Publisher` and `Publisher.Name`). The logic in `WithCondition` makes sure that the values are always going to be there._

- `WithCondition` allows you to define many [specifications](#specification) (each validating different case) and execute them selectively, based on some logic. Either exclusively (one at the time) or using any way of mixing them.

``` csharp
Specification<string> gmailSpecification = s => s
    .Rule(email => {

        var username = email.Substring(0, email.Length - "@gmail.com".Length);

        return !username.Contains('.');

    }).WithMessage("Gmail username must not contain dots.");

Specification<string> outlookSpecification = s => s
    .Rule(email => {

        var username = email.Substring(0, email.Length - "@outlook.com".Length);

        return username.All(char.IsLower);

    }).WithMessage("Outlook username must be all lower case.");


Specification<string> emailSpecification = s => s
    .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!");

Predicate<AuthorModel> hasGmailAddress = a => a.Email?.EndsWith("@gmail.com") == true;

Predicate<AuthorModel> hasOutlookAddress = a => a.Email?.EndsWith("@outlook.com") == true;

Specification<AuthorModel> authorSpecification = s => s
    .Member(m => m.Email, gmailSpecification).WithCondition(hasGmailAddress)
    .Member(m => m.Email, outlookSpecification).WithCondition(hasOutlookAddress)
    .Member(m => m.Email, emailSpecification)
        .WithCondition(author => !hasGmailAddress(author) && !hasOutlookAddress(author));

var validator = Validator.Factory.Create(authorSpecification);

var outlookAuthor = new AuthorModel() { Email = "John.Doe@outlook.com" };

var gmailAuthor = new AuthorModel() { Email = "John.Doe@gmail.com" };

var author1 = new AuthorModel() { Email = "JohnDoe" };

var author2 = new AuthorModel() { Email = "John.Doe@yahoo.com" };

validator.Validate(outlookAuthor).ToString();
// Email: Outlook username must be all lower case.

validator.Validate(gmailAuthor).ToString();
// Email: Gmail username must not contain dots.

validator.Validate(author1).ToString();
// Email: Must contain @ character!

validator.Validate(author2).AnyErrors; // false
```

_The above code shows how to validate a member with three different specifications, depending on the the email provider._

---

#### WithPath

- `WithPath` is a [parameter command](#parameter-commands).
  - Can be placed after:
    - the related [scope command](#scope-commands).
    - other [parameter commands](#parameter-commands): [WithCondition](#withcondition).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - other [parameter commands](#parameter-commands): [WithMessage](#withmessage), [WithExtraMessage](#withextramessage), [WithCode](#withcode), [WithExtraCode](#withextracode).
- `WithPath` sets the [path](#path) for the related scope's [error output](#error-output).
- `WithPath` accepts one parameter; a path relative to the current scope path.
  - Example 1; at `FirstLevel.SecondLevel`, setting `ThirdLevel` as path results with `FirstLevel.SecondLevel.ThirdLevel`, not just `ThirdLevel`.
  - Example 2; at root level, placing setting `Characters` as path results with `Characters`:

``` csharp
Specification<string> specification1 = s => s
    .Rule(email => email.Contains('@'))
        .WithMessage("Must contain @ character!");

Specification<string> specification2 = s => s
    .Rule(email => email.Contains('@'))
        .WithName("Characters")
        .WithMessage("Must contain @ character!");

var validator1 = Validator.Factory.Create(specification1);
var validator2 = Validator.Factory.Create(specification2);

validator1.Validate("invalidemail").ToStringMessages();
// Must contain @ character!

validator2.Validate("invalidemail").ToStringMessages();
// Characters: Must contain @ character!
```

_You can observe that the [error output](#error-output) coming from the `Rule` scope command is saved under `Characters` path._

- `WithPath` can move the [error output](#error-output) between levels:
  - To move it down to the nested level, just use `.` (dot) as a separator, e.g. `FirstLevel.SecondLevel`
    - Effectively it works like appending the path to the current one.
  - To move to the upper level, place as many `<` (less-than) as many levels you want to go up
    - Single `<` works and moves the error output one level up.
    - Passing `<<<` would move the error output three levels up, etc.
  - To move it to the upper level, and to the nested level (but e.g. different branch), combine the two methods described above.
    - Passing `<<Test` would go two levels up and then step into `Test`
    - Going up always stops at the root level, so don't worry if you put too many of `<`.
      - This wouldn't result with an exception, but it could be very misleading if you use such [specification](#specification) in another specification. Please be careful because Validot won't warn you about this problem.

| Current path | WithName parameter | Final path |
| - | - | - |
| root level | `FirstLevel` | `FirstLevel` |
| root level | `FirstLevel.SecondLevel` | `FirstLevel.SecondLevel` |
| `FirstLevel` | `SecondLevel` | `FirstLevel.SecondLevel` |
| `FirstLevel` | `SecondLevel.ThirdLevel` | `FirstLevel.SecondLevel.ThirdLevel` |
| `FirstLevel.SecondLevel.ThirdLevel` | `<` | `FirstLevel.SecondLevel` |
| `FirstLevel.SecondLevel.ThirdLevel` | `<<` | `FirstLevel` |
| `FirstLevel.SecondLevel.ThirdLevel` | `<<<` | root level |
| `FirstLevel.SecondLevel.ThirdLevel` | `<3rdLvl` | `FirstLevel.SecondLevel.3rdLvl` |
| `FirstLevel.SecondLevel.ThirdLevel` | `<<2ndLvl` | `FirstLevel.2ndLvl` |
| `FirstLevel.SecondLevel.ThirdLevel` | `<<<1stLvl` | `1stLvl` |
| `FirstLevel.SecondLevel.ThirdLevel` | `<<<1stLvl.2ndLvl` | `1stLvl.2ndLvl` |
| `FirstLevel.SecondLevel.ThirdLevel` | `<<<<` | root level |
| `FirstLevel.SecondLevel.ThirdLevel` | `<<<<<<` | root level |
| `FirstLevel` | `<<<<<<` | root level |
| root level | `<<<<<<` | root level |
| root level | `<<<<<<FirstLevel` | `FirtLevel` |
| `FirstLevel.SecondLevel.ThirdLevel` | `<<<<<<A.B.C` | `A.B.C` |

``` csharp
Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Publisher, m => m
        .Member(m1 => m1.Name, m1 => m1
            .Rule(name => name.All(char.IsLetter)).WithPath("<<NameOfPublisher").WithMessage("Must consist of letters only!")
        )
    );

var bookValidator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Publisher = new PublisherModel()
    {
        Name = "Adam !!!"
    }
};

bookValidator.Validate(book).ToString();
// NameOfPublisher: Must consist of letters only!
```

_[Rule](#rule) would normally save the message within its scope's path (`Publisher.Name`), but `<<NameOfPublisher` moves its location two levels up and then down to `NameOfPublisher`_

- Path passed to `WithPath` has few restrictions related to `.` (dot) being a special character used to separate levels:
  - you can't start the path with `.`
  - you can't end the path with `.`
  - you can't have two dots next to each other (`..`)

``` csharp
Specification<string> specification = s => s
    .Rule(email => email.Contains('@'))
    .WithPath("Characters.")
    .WithMessage("Must contain @ character!");

var validator = Validator.Factory.Create(specification); // throws ArgumentExceptions
```

- `WithPath` is often used to configure [Member](#member) command.
  - By default, [Member](#member) uses member selector to resolve the next level where the [error output](#error-output) from the passed specification will be saved under.
    - So if the member selector is `m => m.DescriptionDetails`, then by default the [error output](#error-output) is saved under `DescriptionDetails`
  - `WithPath` can alter this default value.

``` csharp
Specification<PublisherModel> publisherSpecification = s => s
    .Member(m => m.Name, nameSpecification).WithName("FirstName");

var publisherValidator = Validator.Factory.Create(publisherSpecification);

var publisher = new PublisherModel()
{
    Name = "Adam !!!"
};

publisherValidator.Validate(publisher).ToString();
// FirstName: Must consist of letters only!
// FirstName: Must not contain whitespace!
```

_The default location set by the [Member](#member) command - `Name` - has been changed to `FirstName`._

- `WithPath` can be used to merge error outputs from many scopes into a single path.

``` csharp
Specification<string> nameSpecification = s => s
    .Rule(name => name.All(char.IsLetter)).WithMessage("Name must consist of letters only!")
    .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Name must not contain whitespace!");

Specification<string> companyIdSpecification = s => s
    .Rule(name => name.Any()).WithMessage("Company Id must not be empty!");

Specification<PublisherModel> publisherSpecification = s => s
    .Member(m => m.Name, nameSpecification).WithPath("<Info")
    .Member(m => m.CompanyId, companyIdSpecification).WithPath("<Info");

var publisherValidator = Validator.Factory.Create(publisherSpecification);

var publisher = new PublisherModel()
{
    Name = "Adam !!!",
    CompanyId = ""
};

publisherValidator.Validate(publisher).ToString();
// Info: Name must consist of letters only!
// Info: Name must not contain whitespace!
// Info: Company Id must not be empty!
```

_[Error messages](#message) from two scopes (members `Name` and `CompanyId`) are both placed under `Info` path._

- `WithPath` can be used to split [error output](#error-output) and distribute errors from a single scope into distinct [paths](#path).

``` csharp
Specification<string> nameSpecification = s => s
    .Rule(name => name.All(char.IsLetter))
    .WithPath("Characters")
    .WithMessage("Must consist of letters only!")

    .Rule(name => char.IsUpper(name.First()))
    .WithPath("Grammar")
    .WithMessage("First letter must be capital!");

Specification<PublisherModel> publisherSpecification = s => s
    .Member(m => m.Name, nameSpecification);

var publisherValidator = Validator.Factory.Create(publisherSpecification);

var publisher = new PublisherModel()
{
    Name = "adam !!!",
};

publisherValidator.Validate(publisher).ToString();
// Name.Characters: Must consist of letters only!
// Name.Grammar: First letter must be capital!
```

_Above, two rules from the same scope are saving [error messages](#message) into entirely different [paths](#path) (`Characters` and `Grammar`)._

---

#### WithMessage

- `WithMessage` is a [parameter command](#parameter-commands).
  - Can be placed after:
    - the related [scope command](#scope-commands).
    - the related [presence commands](#presence-commands): [Required](#required), [Forbidden](#forbidden).
    - other [parameter commands](#parameter-commands): [WithCondition](#withcondition), [WithPath](#withpath).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - other [parameter commands](#parameter-commands): [WithExtraMessage](#withextramessage), [WithExtraCode](#withextracode).
- `WithMessage` overwrites the entire [error output](#error-output) of the related (preceding) [scope command](#scope-commands) with an error output that contains a single [error message](#message).
  - Effectively it's overwriting all errors with a single message.
- `WithMessage` accepts single parameters: message content.
- `WithMessage` is the only way to override the default message (`"Error"`) recorded if the predicate in [Rule](#rule) fails:

``` csharp
Specification<int> specification = s => s
    .Rule(year => year != 0);

var validator = Validator.Factory.Create(specification);

validator.Validate(0).ToString();
// Error

Specification<int> specificationWithMessage = s => s
    .Rule(year => year != 0)
    .WithMessage("Year 0 is invalid");

var validatorWithMessage = Validator.Factory.Create(specificationWithMessage);

Validator.Factory.Create(specificationWithMessage).Validate(0).ToString();
// Year 0 is invalid
```

- It doesn't matter how many nested levels or messages/codes the [error output](#error-output) has. If any of the inner validation rules indicates failure, the entire related scope returns a single message passed to `WithMessage`.
  - If there is no error - there is no [error output](#error-output), and of course, no message as well.

``` csharp
Specification<AuthorModel> authorSpecification = s => s.Member(m => m.Email, m => m.Email());

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Authors, m => m
        .AsCollection(authorSpecification).WithMessage("Contains author with invalid email")
    );

var validator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Authors = new[]
    {
        new AuthorModel() { Email = "InvalidEmail1" },
        new AuthorModel() { Email = "InvalidEmail2" },
        new AuthorModel() { Email = "john.doe@gmail.com" },
        new AuthorModel() { Email = "InvalidEmail3" },
    }
};

validator.Validate(book).ToString();
// Authors: Contains author with invalid email
```

_Above, [AsCollection](#ascollection) would return messages under multiple different paths. When followed by `WithMessage` even a single error coming from [AsCollection](#ascollection) results with just a single error message._

- When overwriting the [error output](#error-output) of [RuleTemplate](#ruletemplate), `WithMessage` has full access to their [message arguments](#message-arguments) and can use them in its content.
  - Good to read;
    - [built-in rules](#rules) - list of the rules and the arguments available
    - [message args](#message-arguments) - how to use args and placeholders

``` csharp
Specification<int> specification = s => s
    .Between(min: 10, max: 20)
    .WithMessage("Minimum value is {min}. Maximum value is {max}");

var validator = Validator.Factory.Create(specification);

validator.Validate(0).ToString();
// Minimum value is 10. Maximum value is 20
```

_`Between` rule takes two arguments; `max` and `min`. These values can be used within the message - just use the placeholders._

- `WithMessage` combined with [AsModel](#asmodel) can be used to group multiple rules and define one error message for them.
  - Good to read: [AsModel](#asmodel) - in this section, you can find code example for such a scenario.
- Validation result presents messages in:
  - [ToString](#tostring) - prints messages preceded by their paths, each in a separate line.
  - [MessageMap](#messagemap) - a dictionary that holds collections of messages grouped by the paths.

---

#### WithExtraMessage

- `WithExtraMessage` is a [parameter command](#parameter-commands).
  - Can be placed after:
    - the related [scope command](#scope-commands).
    - the related [presence commands](#presence-commands): [Required](#required), [Forbidden](#forbidden).
    - other [parameter commands](#parameter-commands): [WithCondition](#withcondition), [WithPath](#withpath), [WithMessage](#withmessage).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - other [parameter commands](#parameter-commands): [WithExtraMessage](#withextramessage), [WithExtraCode](#withextracode).
- `WithExtraMessage` adds a single [message](#message) to the [error output](#error-output) of the related scope.
- `WithExtraMessage` accepts a single parameter: message content.
- `WithExtraMessage` is the only way to add additional messages to the [error output](#error-output).
  - `WithExtraMessage` can be used multiple times, in a row:

``` csharp
Specification<int> specification = s => s
    .Rule(year => year != 0)
    .WithMessage("Year 0 is invalid")
    .WithExtraMessage("Year 0 didn't exist")
    .WithExtraMessage("Please change to 1 B.C. or 1 A.D.");

var validator = Validator.Factory.Create(specification);

validator.Validate(0).ToString();
// Year 0 is invalid
// Year 0 didn't exist
// Please change to 1 B.C. or 1 A.D.
```

- `WithExtraMessage` acts very similar to [WithMessage](#withmessage), with one important difference; in case of error, it appends message to the [error output](#error-output) of the related scope, instead of overwriting it (as [WithMessage](#withmessage) would do).
  - Message is added only if the related scope has [error output](#error-output). No error output - no extra message.

``` csharp
Specification<AuthorModel> authorSpecification = s => s.Member(m => m.Email, m => m.Email());

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Authors, m => m
        .AsCollection(authorSpecification).WithExtraMessage("Contains author with invalid email")
    );

var validator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Authors = new[]
    {
        new AuthorModel() { Email = "InvalidEmail1" },
        new AuthorModel() { Email = "InvalidEmail2" },
        new AuthorModel() { Email = "john.doe@gmail.com" },
        new AuthorModel() { Email = "InvalidEmail3" },
    }
};

validator.Validate(book).ToString();
// Authors.#0.Email: Must be a valid email address
// Authors.#1.Email: Must be a valid email address
// Authors.#3.Email: Must be a valid email address
// Authors: Contains author with invalid email
```

_A similar example to the above one is in the [WithMessage](#withmessage) section. Here, [AsCollection](#ascollection) command returns messages under multiple different paths. When followed by `WithExtraMessage` even a single error coming from [AsCollection](#ascollection) results with an extra message appended to the entire scope._

- When overwriting the [error output](#error-output) of [RuleTemplate](#ruletemplate), `WithMessage` has full access to their [message arguments](#message-arguments) and can use them in its content.
  - Good to read;
    - [built-in rules](#rules) - a list of the rules and the arguments available.
    - [message arguments](#message-arguments) - how to use args and placeholders.

``` csharp
Specification<int> specification = s => s
    .Between(min: 10, max: 20)
    .WithExtraMessage("Minimum value is {min}. Maximum value is {max}.");

var validator = Validator.Factory.Create(specification);

validator.Validate(0).ToString();
// Must be between 10 and 20 (exclusive)
// Minimum value is 10. Maximum value is 20.
```

_`Between` rule takes two arguments; `max` and `min`. These values can be used within the message set with both [WithMessage](#withmessage) and [WithExtraMessage](#withextramessage) - just use the placeholders._

- Validation result presents messages in:
  - [ToString](#tostring) - prints messages preceded by their paths, each in a separate line.
  - [MessageMap](#messagemap) - a dictionary that holds collections of messages grouped by the paths.

#### WithCode

- `WithCode` is a [parameter command](#parameter-commands).
  - Can be placed after:
    - the related [scope command](#scope-commands).
    - the related [presence commands](#presence-commands): [Required](#required), [Forbidden](#forbidden).
    - other [parameter commands](#parameter-commands): [WithCondition](#withcondition), [WithPath](#withpath).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - other [parameter commands](#parameter-commands): [WithExtraCode](#withextracode).
- `WithCode` overwrites the [entire output](#error-output) of the related scope with a single [error code](#code).
- `WithCode` accepts one parameter: [code](#code).
  - [Error code](#code) can't contain white space characters.
- `WithCode` acts very similar to [WithMessage](#withmessage), with one important difference; in case of error, it overrides the entire [error output](#error-output) with the [error code](#code), instead of the [error message](#message) (as [WithMessage](#withmessage) would do).
   - Error code is only if the related scope has any error output. No error output - no error code.
   - The entire error output is overridden, including the messages! If you want to have both [messages](#message) AND [codes](#code), you should use [WithExtraCode](#withextracode) command.

``` csharp
 Specification<int> specification = s => s
    .Rule(year => year != 0)
    .WithCode("YEAR_ZERO");

var validator = Validator.Factory.Create(specification);

var result = validator.Validate(0);

result.ToString();
// YEAR_ZERO
```

_Normally, [Rule](#rule) would return error message, but in the above code, the entire error output is replaced with a single code._

- [Validation result](#result) presents codes in:
  - [Codes](#codes) - a collection of all error codes, from all paths, without duplications.
  - [ToString()](#tostring) - prints all the codes from [Codes](#codes) collection in the first line, coma separated.
  - [CodeMap](#codemap) - a dictionary that holds collections of codes grouped by the paths.

``` csharp
Specification<int[]> specification = s => s
    .AsCollection(m => m
        .Rule(year => year % 2 == 0).WithCode("IS_EVEN")
        .Rule(year => year % 2 != 0).WithCode("IS_ODD")
    );

var validator = Validator.Factory.Create(specification);

var result = validator.Validate(new[] { 0, 1, 2, 3, 4 });

result.ToString();
// IS_EVEN, IS_ODD

result.Codes; // collection containing two items:
// ["IS_EVEN", "IS_ODD"]

result.CodeMap["#0"]; // collection with single item: ["IS_EVEN"]
result.CodeMap["#1"]; // collection with single item: ["IS_ODD"]
result.CodeMap["#2"]; // collection with single item: ["IS_EVEN"]
result.CodeMap["#3"]; // collection with single item: ["IS_ODD"]
result.CodeMap["#4"]; // collection with single item: ["IS_EVEN"]
```

_In the above example, [ToString](#tostring) prints all [error codes](#code) in the first line. [Codes](#codes) contains all the codes and [CodeMap](#codemap) allows to check exactly where the codes has been recorded._

- `WithCode` can be used to group multiple rules and define one code for any failure among them.

``` csharp
Specification<AuthorModel> authorSpecification = s => s.Member(m => m.Email, m => m.Email());

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Authors, m => m
        .AsCollection(authorSpecification).WithCode("INVALID_AUTHORS")
    );

var validator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Authors = new[]
    {
        new AuthorModel() { Email = "InvalidEmail1" },
        new AuthorModel() { Email = "InvalidEmail2" },
        new AuthorModel() { Email = "john.doe@gmail.com" },
        new AuthorModel() { Email = "InvalidEmail3" },
    }
};

validator.Validate(book).ToString();
// INVALID_AUTHORS

result.Codes; // collection with single item: ["INVALID_AUTHORS"]

result.CodeMap["Authors"]; // collection with single item: ["INVALID_AUTHORS"]
```

_Above, [AsCollection](#ascollection) would return messages under multiple different paths. When followed by `WithCode` even a single error coming from [AsCollection](#ascollection) results with just a single error code._

---

#### WithExtraCode

- `WithExtraCode` is a [parameter command](#parameter-commands).
  - Can be placed after:
    - the related [scope command](#scope-commands).
    - the related [presence commands](#presence-commands): [Required](#required), [Forbidden](#forbidden).
    - other [parameter commands](#parameter-commands): [WithCondition](#withcondition), [WithPath](#withpath), [WithMessage](#withmessage), [WithExtraMessage](#withextramessage), [WithCode](#withcode).
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - other [parameter commands](#parameter-commands): [WithExtraCode](#withextracode).
- `WithExtraCode` adds a single [error code](#code) to the [error output](#error-output) of the related (preceding) [command scope](#scope-commands).
- `WithExtraCode` accepts a single parameter; [code](#code).
  - Reminder; error code can't contain white space characters.
- `WithExtraCode` is for [WithCode](#withcode) what [WithExtraMessage](#withextramessage) is for [WithMessage](#withmessage).

``` csharp
Specification<int> specification = s => s
    .Rule(year => year != 0)
    .WithCode("YEAR_ZERO")
    .WithExtraCode("INVALID_YEAR");

var validator = Validator.Factory.Create(specification);

var result = validator.Validate(0);

result.ToString();
// YEAR_ZERO, INVALID_YEAR
```

- `WithExtraCode` acts very similar to [WithCode](#withcode), with one important difference; in case of error it appends the [error code](#code) to the [error output](#error-output) of the related scope, instead of overwriting it (as [WithCode](#withcode) would do).
    - [Error code](#code) is added only if the related scope has error output. No error output - no extra code.
- `WithExtraCode` is the only way to mix error [messages](#message) and [codes](#code) in one [error output](#error-output):

``` csharp
Specification<AuthorModel> authorSpecification = s => s.Member(m => m.Email, m => m.Email());

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Authors, m => m
        .AsCollection(authorSpecification).WithExtraCode("INVALID_AUTHORS")
    );

var validator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Authors = new[]
    {
        new AuthorModel() { Email = "InvalidEmail1" },
        new AuthorModel() { Email = "InvalidEmail2" },
        new AuthorModel() { Email = "john.doe@gmail.com" },
        new AuthorModel() { Email = "InvalidEmail3" },
    }
};

var result = validator.Validate(book);

result.Codes; // collection with single item: ["INVALID_AUTHORS"]

result.CodeMap["Authors"]; // collection with single item: ["INVALID_AUTHORS"]

result.ToString();
// INVALID_AUTHORS
//
// Authors.#0.Email: Must be a valid email address
// Authors.#1.Email: Must be a valid email address
// Authors.#3.Email: Must be a valid email address
```

_In the above example, you can observe how [ToString()][#tostring] prints codes and messages. Of course, both can be detaily examined using [Codes](#codes), [CodeMap](#codemap), and [MessageMap](#messagemap) properties of validation result._

#### Optional

- `Optional` is a [presence command](#presence-commands).
  - Needs to be placed as the first on in the scope.
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
- `Optional` makes the current scope value optional (null is allowed).
  - `Optional` is the only way to avoid errors in case of null scope value.

``` csharp
Specification<string> specification1 = s => s
    .Optional()
    .Rule(title => title.Length > 3)
    .WithMessage("The minimum length is 3");

var validator1 = Validator.Factory.Create(specification1);

validator1.Validate(null).AnyErrors; // false
```

_Above, `Optional` placed as the first command in the [specification](#specification) makes null a valid case. If we remove it, the null value will result with validation error:_

``` csharp
Specification<string> specification2 = s => s
    .Rule(title => title.Length > 3)
    .WithMessage("The minimum length is 3");

var validator2 = Validator.Factory.Create(specification2);

var result2 = validator2.Validate(null);

result2.AnyErrors; // true

result2.ToString();
// Required
```

_In both cases (with and without `Optional`), when the value is provided - there is no difference in the [error output](#error-output):_

``` csharp
validator1.Validate("a").ToString();
// The minimum length is 3

validator2.Validate("a").ToString();
// The minimum length is 3

validator1.Validate("abc").AnyErrors; // false
validator2.Validate("abc").AnyErrors; // false
```

- Using [presence commands](#presence-commands) in the root scope is absolutely correct, but the most common use case for `Optional` is marking members as optional:

``` csharp
Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Title, m => m
        .Optional()
        .Rule(title => title.Length > 3).WithMessage("The minimum length is 3")
    );

var validator = Validator.Factory.Create(bookSpecification);

var book1 = new BookModel() { Title = null };

validator.Validate(book1).AnyErrors; // false

var book2 = new BookModel() { Title = "a" };

validator.Validate(book2).ToString();
// Title: The minimum length is 3
```

- Good to read; [null policy](#null-policy) - the entire logic of handling nulls.

---

#### Required

- `Required` is a [presence command](#presence-commands).
  - Needs to be placed as the first in the scope.
  - Can be followed by:
    - any of the [scope commands](#scope-commands).
    - [parameter commands](#parameter-commands): [WithMessage](#withmessage), [WithExtraMessage](#withextramessage), [WithCode](#withcode), [WithExtraCode](#withextracode).
- `Required` makes the current scope value required (null is not allowed).
    - Every scope by default requires the incoming value to be non-null, and inserting single `Required` doesn't change anything:

``` csharp
Specification<string> specification1 = s => s
    .Required()
    .Rule(title => title.Length > 3)
    .WithMessage("The minimum length is 3");

var validator1 = Validator.Factory.Create(specification1);

var result1 = validator1.Validate(null);

result1.AnyErrors; // true

result1.ToString();
// Required
```

_Above, `Required` placed as the first command in the specification. If we remove it, literally nothing changes:_

``` csharp
Specification<string> specification2 = s => s
    .Rule(title => title.Length > 3)
    .WithMessage("The minimum length is 3");

var validator2 = Validator.Factory.Create(specification2);

var result2 = validator2.Validate(null);

result2.AnyErrors; // true

result2.ToString();
// Required
```

_Similarly to [Optional](#optional), in both cases (with and without `Required`), when the value is provided - there is no difference ns the [error output](#error-output):_

``` csharp
validator1.Validate("a").ToString();
// The minimum length is 3

validator2.Validate("a").ToString();
// The minimum length is 3

validator1.Validate("abc").AnyErrors; // false
validator2.Validate("abc").AnyErrors; // false
```

- `Required` can be used to modify the error output that the scope returns if the scope value is null.
    - [WithMessage](#withmessage) overrides the default error message.
    - [WithExtraMessage](#withextramessage) adds the error message to the default one.
    - [WithCode](#withcode) overrides the default error message with error code.
    - [WithExtraCode](#withextracode) adds the error code to the default error output.

``` csharp
Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Title, m => m
        .Required().WithMessage("Title is required").WithExtraCode("MISSING_TITLE")
        .Rule(title => title.Length > 3).WithMessage("The minimum length is 3")
    );

var validator = Validator.Factory.Create(bookSpecification);

var book = new BookModel() { Title = null };

var result = validator.Validate(book);

result.Codes; // collection with single item: ["MISSING_TITLE"]

result.ToString();
// MISSING_TITLE
//
// Title: Title is required
```

_Above, `Title` member has the default error replaced with message `Title is required` and additional code `MISSING_TITLE`.

- Presence errors are special, and you can't move them with [WithPath](#withpath), but there are workarounds:
    - Check null with [Rule](#rule) command at the upper level and then save the output somewhere else using [WithPath](#withpath).

``` csharp
Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Title, m => m
        .Optional()
        .Rule(title => title.Length > 3).WithMessage("The minimum length is 3")
    )

    .Rule(m => m.Title != null)
    .WithPath("BookTitle")
    .WithMessage("Title is required")
    .WithExtraCode("MISSING_TITLE");

var validator = Validator.Factory.Create(bookSpecification);

var book = new BookModel() { Title = null };

var result = validator.Validate(book);

result.Codes; // [ "MISSING_TITLE" ]

result.ToString();
// MISSING_TITLE
//
// BookTitle: Title is required
```

_Above, `Title` is optional, so no presence error is saved under `Title` path. If `Title` is null, the [error output](#error-output) from [Rule](#rule) is saved under `BookTitle` path._

- Good to read; [null policy](#null-policy) - the entire logic of handling nulls.

---

#### Forbidden

- `Forbidden` is a [presence command](#presence-commands).
  - Needs to be placed as the first on in the scope.
  - Can be followed by:
    - [parameter commands](#parameter-commands): [WithMessage](#withmessage), [WithExtraMessage](#withextramessage), [WithCode](#withcode), [WithExtraCode](#withextracode).
- `Forbidden` makes the current scope forbidden.
  - Non-null is not allowed, or in other words, the value must be null.
  - `Forbidden` is exactly opposite to [Required](#required).

``` csharp
Specification<string> specification = s => s
    .Forbidden();

var validator = Validator.Factory.Create(specification);

validator.Validate(null).AnyErrors; // false

validator.Validate("some value").ToString();
// Forbidden
```

- Similarly to [Required](#required), you can alter the default [error output](#error-output) using parameter commands:

``` csharp
Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Title, m => m
        .Forbidden().WithMessage("Title will be autogenerated").WithExtraCode("TITLE_EXISTS")
    );

var validator = Validator.Factory.Create(bookSpecification);

var book = new BookModel() { Title = null };

var result = validator.Validate(book);

result.Codes; // [ "TITLE_EXISTS" ]

result.ToString();
// TITLE_EXISTS
//
// Title: Title will be autogenerated
```

- Good to read; [null policy](#null-policy) - the entire logic of handling nulls.


---

#### And

- `And` contains no validation logic, it's purpose is to visually separate rules in the fluent API method chain.
- `And` is a special case - from the technical point of view, `And` could be described as a [scope command](#scope-commands) that doesn't do anything.
- The only difference between `And` and a [Rule](#rule) that doesn't do anything are the position restrictions:
  - `And` can't be placed at the beginning of the specification.
  - `And` can't be placed at the end of the specification.
- `And` helps with automatic formatters that could visually spoil the code:

``` csharp
Specification<BookModel> bookSpecificationPlain = s => s
  .Member(m => m.Title, m => m
      .Optional()
      .Rule(title => title.Length > 5)
        .WithMessage("The minimum length is 5")
      .Rule(title => title.Length < 10)
        .WithMessage("The maximum length is 10")
  )
  .Rule(m => !m.Title.Contains("title"))
    .WithPath("Title")
    .WithCode("TITLE_IN_TITLE")
  .Rule(m => m.YearOfFirstAnnouncement < 3000)
    .WithMessage("Maximum year value is 3000");
```

_Above, the example of specification where fluent API methods are separated using indentations. Autoformatting (e.g., when pasting this code) could align all methods like this:_

``` csharp
Specification<BookModel> bookSpecificationPlain = s => s
  .Member(m => m.Title, m => m
      .Optional()
      .Rule(title => title.Length > 5).WithMessage("The minimum length is 5")
      .Rule(title => title.Length < 10).WithMessage("The maximum length is 10")
  )
  .Rule(m => !m.Title.Contains("title"))
  .WithPath("Title")
  .WithCode("TITLE_IN_TITLE")
  .Rule(m => m.YearOfFirstAnnouncement < 3000)
  .WithMessage("Maximum year value is 3000");
```

_`And` helps to maintain the readability by visually separating the rules:_

``` csharp
Specification<BookModel> bookSpecificationAnd = s => s
  .Member(m => m.Title, m => m
      .Optional()
      .And()
      .Rule(title => title.Length > 5).WithMessage("The minimum length is 5")
      .And()
      .Rule(title => title.Length < 10).WithMessage("The maximum length is 10")
  )
  .And()
  .Rule(m => !m.Title.Contains("title"))
  .WithPath("Title")
  .WithCode("TITLE_IN_TITLE")
  .And()
  .Rule(m => m.YearOfFirstAnnouncement < 3000)
  .WithMessage("Maximum year value is 3000");
```

_`And` within the fluent API method chain doesn't affect the logic. Both above specifications always produce equal results_.

---

### Null policy

- If the value is entering the scope, presence commands are the first to take action.
- If the value entering the scope is null, [scope commands](#scope-commands) are not executed.
    - It doesn't matter how many rules, commands and logic the scope has - it is skipped, and the validation process leaves the scope.
    - This is why you don't need to secure your code from `NullReferenceException` in the predicates passed to the [Rule](#rule) (and [RuleTemplate](#ruletemplate)) commands. Validot will never pass null to a predicate.
- If the scope doesn't contain any presence command, it acts as it had a single [Required](#required) command at the beginning.
    - Therefore, every specification by default marks the validated input as required (non-null).
- [Required](#required) command itself doesn't do anything extra comparing to the specification without it, however it gives a possibility to change the [error output](#error-output) returned in case the incoming value is null.
    - By default, the error output contains the single error message key `Global.Required`.
- [Optional](#optional) command allows the value to be null. In such a case, validation leaves the scope immediately, and no error output is recorded.
- [Forbidden](#forbidden) command requires the value to be null.
    - By default, the error output contains the single error message key `Global.Forbidden`.
- To know how you can modify the error outputs of the presence commands, read their sections: [Required](#required), [Optional](#optional), [Forbidden](#forbidden)


### Reference loop

- The reference loop is a loop in a reference graph of your incoming model.
    - In other words; reference loop exists in a model if you traverse through its members and can reach some reference twice at some point.
    - On a simple example (imagine a classic linked list, letters represent references):
        - `A->A`, a direct self-reference; type defines a member of the same type and the object has itself assigned there.
        - `A->B->C->A`, no direct self-reference, but `A` has member `B`, that has member `C`, that has member `A`, so same reference as at the beginning.

``` csharp
public class A
{
    public B B { get; set; }
}

public class B
{
    public A A { get; set; }
}
```

_Above; simple structure `A->B->A`._

- If you're traversing through the object graph and have a reference loop, you can end up in infinite loop and stack overflow exception.
- Reference loops are visible in the [Template](#template).
  - The root of the loop is marked with message key `Global.ReferenceLoop`.
- Reference loop is the only case where the [Template](#template) doesn't reflect what ultimately lands in the validation [result](#result).
    - The validation process inside the loop is running normally. However, the lack of caching might slightly affect performance.

``` csharp
Specification<B> specificationB = null;

Specification<A> specificationA = s => s
    .Member(m => m.B, specificationB);

specificationB = s => s
    .Member(m => m.A, specificationA);

var validator = Validator.Factory.Create(specificationA);

var a = new A()
{
    B = new B()
    {
        A = new A()
        {
            B = new B()
            {
                A = null
            }
        }
    }
};

validator.Validate(a).ToString();
// B.A.B.A: Required
```

- Validot has protection against reference loop.
    - When reference loop is detected in the validated object, `ReferenceLoopException` is thrown from the `Validate` function, with information like:
        - `Type` - what type was at the beginning of the loop
        - `Path` - path where the loop starts
        - `NestedPath` - path where the loop ends (so where the object (of type described in `Type`) has same reference as the object under path visible in `Path`).
        - `ScopeId` - the id of the scope where the loop happens. This is the information from Validot's internals, not useful in the outside world. However please include it when raising an issue, as it will help the dev team.


``` csharp
Specification<B> specificationB = null;

Specification<A> specificationA = s => s
    .Member(m => m.B, specificationB);

specificationB = s => s
    .Member(m => m.A, specificationA);

var validator = Validator.Factory.Create(specificationA);

var a = new A()
{
    B = new B()
    {
        A = new A()
        {
            B = new B()
            {
                A = null
            }
        }
    }
};

a.B.A.B.A = a.B.A;

try
{
    validator.Validate(a);
}
catch(ReferenceLoopException exception)
{
    exception.Path; // "B.A"
    exception.NestedPath; // "B.A.B.A"
    exception.Type; // typeof(A)
}
```

- Protection against the reference loop is enabled automatically - but only when the risk of such a case is detected.
    - The protection uses certain resources (validation needs to track the visited references), but performance drop shouldn't be that much noticeable. Please bear that in mind in case you encounter some extreme corner case.
    - You can explicitly enable/disable the protection in the [settings](#settings).
        - Please do know what you're doing; e.g. if disabled, there is no protection from stack overflow exception.
- There is a risk of reference loop and stack overflow if:
    - There is a loop in the type graph, and the same types are using the same specification.
        - It is true that the loop in the type graph indicates possibility of having the loop in the reference graph, but as long as the same types don't use the same specification - it's totally fine because the validation would never end up in the endless loop.
    - Reference loop is reachable at all.
        - Validation is based on the specification. If the specification doesn't even step into the members that are in the loop, there is no risk.

---

## Validator

- Validator is the object that performs validation process.
- Validator validates the object according to the [specification](#specification) `Specification<T>`.
  - Validator is a generic class `Validator<T>` where `T` is the type of objects it can validate.
  - Type `T` comes from [specification](#specification) `Specification<T>`
- Validator can be created only using  can be initialized using the [factory](#factory).
  - Constructor receives two parameters:
    - the [specification](#specification).
    - the [settings](#settings).

``` csharp
Specification<BookModel> specification = s => s
    .Member(m => m.Title, m => m.NotEmpty());

var validator = Validator.Factory.Create(specification);
```

_The code above presents that `Validator` can be created with just a [specification](#specification). The code below presents how to apply [settings](#settings) using a fluent api:_

``` csharp
Specification<BookModel> specification = s => s
    .Member(m => m.Title, m => m.NotEmpty())
    .And()
    .Rule(m => m.YearOfPublication > m.YearOfFirstAnnouncement)
    .WithCondition(m => m.YearOfPublication.HasValue);

var validator = Validator.Factory.Create(
    specification,
    s => s.WithPolishTranslation()
);
```

- On creation, [factory](#factory) executes the [specification](#specification) function and performs an in-depth analysis of all of the commands that it has.
  - All of the [error messages](#message) (along with their [translations](#translations)) are pre-generated and cached.
    - They are exposed in the form of a regular [validation result](#result) ([Template](#template) property).
  - [Reference loops](#reference-loop) are detected.
    - If [reference loops](#reference-loop) are possible, [reference loop protection](#withreferenceloopprotection) is automatically enabled, unless you explicitly disable using [WithReferenceLoopProtectionDisabled](#withreferenceloopprotection).
    - The [reference loop protection](#withreferenceloopprotection) slightly decreases the validation performance. It's because the validator needs to track all visited references in order to prevent stack overflow.
- Validation process always executes the commands in the same order as they appear in the specification.
- Validation process always executes as few commands as possible in order to satisfy the specification.
  - Example; if the scope is followed with [WithMessage](#withmessage) or [WithCode](#withcode), internally the validation executes the rules until the first error is found. This is because it doesn't matter how many of the rules inside fails, they're all going to be overridden by [WithMessage](#withmessage) or [WithCode](#withcode).
  - Example; if the validation process triggered with `failFast` flag, it terminates after detecting the first error.

``` csharp
Specification<BookModel> specification = s => s
    .Member(m => m.Title, m => m
        .NotEmpty()
        .NotWhiteSpace()
        .NotEqualTo("blank")
        .And()
        .Rule(t => !t.StartsWith(" ")).WithMessage("Can't start with whitespace")
    )
    .WithMessage("Contains errors!");

var validator = Validator.Factory.Create(specification);

var book = new BookModel() { Title = "     " };

validator.Validate(book).ToString();
// Title: Contains errors!
```

_Above, the `Title` value is checked by `NotEmpty` and `NotWhiteSpace` rules. `NotWhiteSpace` reports an error, therefore there is no need of executing `NotEqualTo` and `Rule` - as the entire error output is replaced with the message defined in [WithMessage](#withmessage)._

### Validate

- `Validate` is the very function that triggers the full validation process.
  - It accepts two parameters:
    - Model of type `T` - the object to validate.
    - `failFast` (default value: `false`) - a flag indicating whether the process should terminate immediately after detecting the first [error](#error-output).
  - It returns [validation result](#result).

``` csharp
Specification<BookModel> specification = s => s
    .Member(m => m.Title, m => m.NotEmpty())
    .And()
    .Member(m => m.YearOfFirstAnnouncement, m => m.BetweenOrEqualTo(1000, 3000))
    .And()
    .Rule(m => m.YearOfPublication >= m.YearOfFirstAnnouncement)
    .WithCondition(m => m.YearOfPublication.HasValue)
    .WithMessage("Year of publication needs to be after the first announcement");

var validator = Validator.Factory.Create(specification);

var book = new BookModel()
{
    Title = "",
    YearOfPublication = 600,
    YearOfFirstAnnouncement = 666
};

var result = validator.Validate(book);

result.ToString();
// Title: Must not be empty
// YearOfFirstAnnouncement: Must be between 1000 and 3000 (inclusive)
// Year of publication needs to be after the first announcement

var failFastResult = validator.Validate(book, failFast: true);

failFastResult.ToString();
// Title: Must not be empty
```

_In the code above, you can observe that the validation process triggered with `failFast` set to `true` returns only the first [error message](#message) from the regular run. It's always going to be the same message - because validation executes the rules in the same order as they appear in the specification._

### IsValid

- `IsValid` is the highly-optimized version of [Validate](#validate) to check if the model is valid or not.
  - It's super-fast, but it has its price: no [error output](#error-output) and no [paths](#path).
    - So you don't know what value is wrong and where it is.
  - It returns a bool - if `true`, then no error found. Otherwise, `false`.

``` csharp
Specification<BookModel> specification = s => s
    .Member(m => m.Title, m => m.NotEmpty())
    .And()
    .Member(m => m.YearOfFirstAnnouncement, m => m.BetweenOrEqualTo(1000, 3000))
    .And()
    .Rule(m => m.YearOfPublication >= m.YearOfFirstAnnouncement)
    .WithCondition(m => m.YearOfPublication.HasValue)
    .WithMessage("Year of publication needs to be after the first announcement");

var validator = Validator.Factory.Create(specification);

var book1 = new BookModel()
{
    Title = "",
    YearOfPublication = 600,
    YearOfFirstAnnouncement = 666
};

validator.IsValid(book1); // false

var book2 = new BookModel()
{
    Title = "test",
    YearOfPublication = 1666,
    YearOfFirstAnnouncement = 1600
};

validator.IsValid(book2); // true
```

- In fact, `IsValid` is so fast that it might be a good idea to call it first and then - if model is invalid - trigger `Validate` to get all of the details.

``` csharp
if (!validator.IsValid(heavyModel))
{
    _logger.Log("Errors found: " + validator.Validate(heavyModel).ToString());
}
```

### Factory

- Factory is the way to create the [validator](#validator) instances.
- Factory is exposed through the static member `Factory` of the static class `Validator`:

``` csharp
var validator = Validator.Factory.Create(specification);
```

- Factory contains several methods that allows to create [validator](#validator) instances by receiving:
    - [Specification](#specification) and [settings](#settings) builder
      - the most popular way
      - [Validator](#validator) created using this method can validate objects described by the given [specification](#specification), using the [settings](#settings) constructed inline with the fluent API.
    - [Specification holder](#specification-holder) and [settings](#settings) builder
      - Similar to the first option, however the [specification](#specification) is acquired from the [specification holder](#specification-holder)
      - (along with the settings, if it's also a [settings holder](#settings-holder))
    - [Specification](#specification) and [settings](#settings)
      - Similar to the first option, but allows to use [settings](#settings) from somewhere else (e.g. another [validator](#validator)).

_Code presenting the usage of [specification holder](#specification-holder) and [validator settings holder](#settings-holder) is placed in their sections._

_Below; simple scenario of creating the [validator](#validator) out the [specification](#specification) and [settings](#settings):_

``` csharp
// specifications:
Specification<AuthorModel> authorSpecification = s => s
    .Member(m => m.Email, m => m
        .Email()
        .And()
        .EndsWith("@gmail.com")
        .WithMessage("Only gmail accounts are accepted")
    );

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Title, m => m.NotEmpty().NotWhiteSpace())
    .Member(m => m.Authors, m => m.AsCollection(authorSpecification));

// data:
var book = new BookModel()
{
    Title = "   ",
    Authors = new[]
    {
        new AuthorModel() { Email = "john.doe@gmail.com" },
        new AuthorModel() { Email = "john.doe@outlook.com" },
        new AuthorModel() { Email = "inv@lidem@il" },
    }
};

// validator:
var validator = Validator.Factory.Create(bookSpecification, s => s
    .WithTranslation("English", "Texts.Email", "This is not a valid email address!")
);

validator.Validate(book).ToString();
// Title: Must not consist only of whitespace characters
// Authors.#1.Email: Only gmail accounts are accepted
// Authors.#2.Email: This is not a valid email address!
// Authors.#2.Email: Only gmail accounts are accepted
```

_Above you can observe that `validator` respects the rules described in the `bookSpecification` as well as the settings (notice the custom error message in `Authors.#2.Email`)._

_Below, let's take a look at the continuation of the previous snippet, showing that we can reuse the settings already built for the other [validator](#validator):_

``` csharp
var validator2 = Validator.Factory.Create(bookSpecification, validator.Settings);

validator2.Validate(book).ToString();
// Title: Must not consist only of whitespace characters
// Authors.#1.Email: Only gmail accounts are accepted
// Authors.#2.Email: This is not a valid email address!
// Authors.#2.Email: Only gmail accounts are accepted
```

#### Specification holder

- Logically, specification holder is a class that holds [specification](#specification) that [factory](#factory) will fetch and initialize the [validator](#validator) with.
- Technically, specification holder is a class that implements `ISpecificationHolder<T>` generic interface.
  - This interface exposes single member of type `Specification<T>`.

``` csharp
interface ISpecificationHolder<T>
{
    Specification<T> Specification { get; }
}
```

- [Factory](#factory) has a `Create` method that accepts `ISpecificationHolder<T>` instead of `Specification<T>`.
  - Specification is taken directly from `Specification` interface member.
- Specification holder is a way to wrap the entire specification within a single class.

``` csharp
class BookSpecificationHolder : ISpecificationHolder<BookModel>
{
    public BookSpecificationHolder()
    {
        Specification<string> titleSpecification = s => s
            .NotEmpty()
            .NotWhiteSpace();

        Specification<string> emailSpecification = s => s
            .Email()
            .EndsWith("@gmail.com").WithMessage("Only gmail accounts are accepted");

        Specification<AuthorModel> authorSpecification = s => s
            .Member(m => m.Email, emailSpecification);

        Specification<BookModel> bookSpecification = s => s
            .Member(m => m.Title, titleSpecification)
            .Member(m => m.Authors, m => m.AsCollection(authorSpecification));

        Specification = bookSpecification;
    }

    public Specification<BookModel> Specification { get; }
}
```

_Above; example of [specification](#specification) wrapped in the holder. Below; example of usage._

``` csharp
var validator = Validator.Factory.Create(new BookSpecificationHolder());

var book = new BookModel()
{
    Title = "   ",
    Authors = new[]
    {
        new AuthorModel() { Email = "john.doe@gmail.com" },
        new AuthorModel() { Email = "john.doe@outlook.com" },
        new AuthorModel() { Email = "inv@lidem@il" },
    }
};

validator.Validate(book).ToString();
// Title: Must not consist only of whitespace characters
// Authors.#1.Email: Only gmail accounts are accepted
// Authors.#2.Email: Must be a valid email address
// Authors.#2.Email: Only gmail accounts are accepted
```

#### Settings holder

- Logically, a settings holder is a class that holds [settings](#settings) that the [factory](#factory) will fetch and initialize the [validator](#validator) with.
- Technically, settings holder is a class that implements `ISettingsHolder`:

``` csharp
interface ISettingsHolder
{
    Func<ValidatorSettings, ValidatorSettings> Settings { get; }
}
```

- Settings holder needs to expose `Settings` member which - practically - is a fluent API builder. Same as the one used in `Validate.Factory.Create` method.
- Settings holder is very similar to [specification holder](#specification-holder), but its purpose is to wrap the [settings](#settings).
- If the [specification holder](#specification-holder) passed to the [Factory](#factory) implements settings holder as well, the created [validator](#validator) instance will have [settings](#settings) from the holder applied.

``` csharp
public class AuthorSpecificationHolder : ISpecificationHolder<AuthorModel>, ISettingsHolder
{
    public AuthorSpecificationHolder()
    {
        Specification<string> emailSpecification = s => s
            .Email()
            .EndsWith("@gmail.com");

        Specification<AuthorModel> authorSpecification = s => s
            .Member(m => m.Email, emailSpecification).WithMessage("Invalid email")
            .Member(m => m.Name, m => m.NotEmpty()).WithMessage("Name.EmptyValue");

        Specification = authorSpecification;

        Settings = s => s
            .WithReferenceLoopProtection()
            .WithPolishTranslation()
            .WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["English"] = new Dictionary<string, string>()
                {
                    ["Name.EmptyValue"] = "Name must not be empty"
                },
                ["Polish"] = new Dictionary<string, string>()
                {
                    ["Invalid email"] = "Nieprawidłowy email",
                    ["Name.EmptyValue"] = "Imię nie może być puste"
                }
            });
    }

    public Specification<AuthorModel> Specification { get; }

    public Func<ValidatorSettings, ValidatorSettings> Settings { get; }
}
```

_In the above code, [specification](#specification) exposed from the holder internally uses message keys that are resolved in the translations provided in the `Settings` builder. The usage would look like:_

``` csharp
var validator = Validator.Factory.Create(new AuthorSpecificationHolder());

var author = new AuthorModel()
{
    Name = "",
    Email = "john.doe@outlook.com",
};

var result = validator.Validate(author);

result.ToString();
// Name: Name must not be empty
// Email: Invalid email

result.ToString("Polish");
// Name: Imię nie może być puste
// Email: Nieprawidłowy email
```

_And the validator's `Settings` proves that settings holder has been used:_

``` csharp
validator.Settings.Translations.Keys // ["English", "Polish"]
validator.Settings.Translations["English"]["Name.EmptyValue"] // "Name must not be empty"
validator.Settings.Translations["Polish"]["Invalid email"] // "Nieprawidłowy email"

validator.Settings.ReferenceLoopProtection // true
```

- The [factory](#factory)'s `Create` method (`Validator.Factory.Create`) that accepts the specification holder, allows to inline modify [settings](#settings) as well.
  - The inline [settings](#settings) builder overrides the [settings](#settings) from the holder.

_Let's see this behavior in the below code:_

``` csharp
var validator = Validator.Factory.Create(
    new AuthorSpecificationHolder(),
    s => s
        .WithReferenceLoopProtectionDisabled()
        .WithTranslation("English", "Invalid email", "The email address is invalid")
);

var author = new AuthorModel()
{
    Name = "",
    Email = "john.doe@outlook.com",
};

validator.Validate(author).ToString();
// Name: Name must not be empty
// Email: The email address is invalid

validator.Settings.ReferenceLoopProtection; // false
```

#### Reusing settings

- Factory can create the [validator](#validator) instance using settings taken from another.
- Use the overloaded `Create` method that accepts [specification](#specification) and `IValidatorSettings` instance.
  - You must pass `IValidatorSettings` instance acquired from a validator. Using custom implementations is not supported and will end up with an exception.

_Below, `validator2` uses settings taken from the previously created `validator1`:_

``` csharp
Specification<AuthorModel> authorSpecification = s => s
  .Member(m => m.Email, m => m.Email().EndsWith("@gmail.com"))
  .WithMessage("Invalid email")
  .And()
  .Member(m => m.Name, m => m.NotEmpty())
  .WithMessage("Name.EmptyValue");

var validator1 = Validator.Factory.Create(
        authorSpecification,
        s => s
        .WithTranslation("English", "Invalid email", "The email address is invalid")
        .WithTranslation("English", "Name.EmptyValue", "Name must not be empty")
);

var validator2 = Validator.Factory.Create(authorSpecification, validator1.Settings);

var author = new AuthorModel()
{
    Name = "",
    Email = "john.doe@outlook.com",
};

validator1.Validate(author).ToString()
// Name: Name must not be empty
// Email: The email address is invalid

validator2.Validate(author).ToString()
// Name: Name must not be empty
// Email: The email address is invalid

object.ReferenceEquals(validator1.Settings, validator2.Settings) // true
```

#### Fetching holders

- Factory has `FetchHolders` method that scans the provided assemblies for [specification holders](#specification-holder).
   -  You can get all loaded assemblies by calling `AppDomain.CurrentDomain.GetAssemblies()`, or anything else that in your specific case would produce an array of `System.Reflection.Assembly` objects.
   -  You can also be more precise and pick only the desired assemblies. For example, by calling `typeof(TypeInTheAssembly).Assembly`.
  - [Specification holder](#specification-holder) is included in the result collection if it:
    -  is a class that implements `ISpecificationHolder<T>` interface.
    -  contains a parameterless constructor.
-  `FetchHolders` returns a collection of `HolderInfo` objects, each containing following members:
   - `HolderType` - type of the holder, the class that implements `ISpecificationHolder<T>`
   -  `SpecifiedType` - the type that is covered by the [specification](#specification), it's `T` from `ISpecificationHolder<T>` and its member `Specification<T>`.
   - `HoldsSettings` - a flag, `true` if the class is also a [settings holder](#settings-holder) (implements `ISettingsHolder` interface).
   - `CreateValidator` - a method that using reflection creates instance of `HolderType` (with its parametless constructor) and then - the validator out of it.
     - If you want to use it directly, you need to cast it, as the return type is just top-level `object`.
   - `ValidatorType` - the type of the validator created by `CreateValidator` method. It's always `IValidator<T>` where `T` is `SpecifiedType`.

_Let's have a [specification holder](#specification-holder) that holds also the settings:_

``` csharp
public class HolderOfIntSpecificationAndSettings : ISpecificationHolder<int>, ISettingsHolder
{
    public Specification<int> Specification { get; } = s => s
        .GreaterThanOrEqualTo(1).WithMessage("Min value is 1")
        .LessThanOrEqualTo(10).WithMessage("Max value is 10");

    public Func<ValidatorSettings, ValidatorSettings> Settings { get; } = s => s
        .WithTranslation("English", "Min value is 1", "The minimum value is 1")
        .WithTranslation("English", "Max value is 10", "The maximum value is 10")
        .WithTranslation("BinaryEnglish", "Min value is 1", "The minimum value is 0b0001")
        .WithTranslation("BinaryEnglish", "Max value is 10", "The maximum value is 0b1010")
        .WithReferenceLoopProtection();

}
```

_It will be detected by `FetchHolders` method:_

``` csharp
var holder = Validator.Factory.FetchHolders(assemblies).Single(h => h.HolderType == typeof(HolderOfIntSpecificationAndSettings));

var validator = (Validator<int>)holder.CreateValidator();

validator.Validate(11).ToString(translationName: "BinaryEnglish");
// The maximum value is 0b1010
```

_Above, we can observe that the created validator respects the rules and the settings acquired from `HolderOfIntSpecificationAndSettings`._

- `FetchHolders` outputs `HolderInfo` in the following order:
  - Assemblies are analyzed in the order they are provided.
    - Or, if called without parameters, it's the order returned by `AppDomain.CurrentDomain.GetAssemblies()`.
  - For each assembly, holders are analyzed in the order they appear in the output of `assembly.GetTypes()`.
  - For each [specification holder](#specification-holder), the types are analyzed in the order returned by `type.GetInterfaces()`.

#### Dependency injection

- Validot doesn't have any dependencies (apart of the pure .NET Standard 2.0), and therefore - there is no direct support for third-party dependency injection containers.
- However, the [factory](#factory) is able to [fetch the holders](#fetching-holders) from the referenced assemblies and provides helpers to create [validators](#validator) out of them.
- For example, if you want your validators to be automatically registered within the DI container, you can implement the following strategy:
  - Define [specifications](#specification) for your models in [specification holders](#specification-holder)
    - Each in a separate class or everything in the single one - it doesn't matter.
  - Call `Validator.Factory.FetchHolders(AppDomain.CurrentDomain.GetAssemblies())` to get the information about the holders and group the results by the `SpecifiedType`.
    - instead of `AppDomain.CurrentDomain.GetAssemblies()` you can pass the array of `System.Reflection.Assembly` that the function will scan for `ISpecificationHolder` implementations.
    - Theoretically, you could define more than one specification for a single type. Let's assume it's not the case here, but as you will notice, the entire operation is merely a short LINQ call. You can easily adjust it to your needs and/or the used DI container's requirements.
  - Out of every group, take the `ValidatorType` (this is your registered type) and the result of `CreateValidator` (this is your implementation instance).
  - It's safe to register validators as singletons.


_In ASP.NET Core the services registration by default takes place in the ConfigureServices method. Something like `AddValidators` is desirable._

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    // it would be great if this line would scan all referenced projects ...
    // ... and register validators based on the detected ISpecificationHolder implementations

    // services.AddValidators();
}
```

_Instead of `AddValidators` you can copy-paste the following lines of code:_

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

_You can easily specify the exact assemblies for the Validot to scan (by setting up `holderAssemblies` collection). Validators are created only from the first  `ISpecificationHolder` implementation found for each type. To change this logic, adjust the LINQ statement that creates `holders` collection._

_Of course, you can create the fully-featured `AddValidators` extension in the code by saving the following snippet as a new file somewhere in your namespace:_

``` csharp
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Validot;

static class AddValidatorsExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection @this, params Assembly[] assemblies)
    {
        var assembliesToScan = assemblies.Length > 0
                ? assemblies
                : AppDomain.CurrentDomain.GetAssemblies();

        var holders = Validator.Factory.FetchHolders(assembliesToScan)
            .GroupBy(h => h.SpecifiedType)
            .Select(s => new
            {
                ValidatorType = s.First().ValidatorType,
                ValidatorInstance = s.First().CreateValidator()
            });

        foreach (var holder in holders)
        {
            @this.AddSingleton(holder.ValidatorType, holder.ValidatorInstance);
        }

        return @this;
    }
}
```

_So it can be used in the ASP.NET Core's `Startup.cs` as below:_

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    // ... registering other dependencies ...

    services.AddValidators();

    // ... registering other dependencies ...
}
```

### Settings

- Settings is the object that holds configuration of the validation process that [validator](#validator) will perform:
  - [Translations](#translations) - values for the message keys used in specification.
  - [Reference loop](#reference-loop) protection - prevention against stack overflow exception.
- Settings are represented by `IValidatorSettings` interface (namespace `Validot.Settings`).
- [Validator](#validator) exposes `Settings` property.
  - `Settings` property is of type `IValidationSettings`, so you can [reuse it](#reusing-settings) in [Factory](#factory) to initialize a new [validator](#validator) instance with the same settings.
- All properties in `IValidatorSettings` are read-only, but under the hood there is an instance of `ValidatorSettings` class that has fluent API methods to change the values
- You can't create `ValidatorSettings` object directly, but there is no reason to do it. Use the builder pattern exposed by the [factory](#factory).
  - [Factory](#factory) initializes the settings object with the default values and exposes it through the fluent API:

``` csharp
var validator = Validator.Factory.Create(specification, settings => settings
    .WithReferenceLoopProtection()
);

validator.Settings.ReferenceLoopProtectionEnabled; // true
```

#### WithReferenceLoopProtection

- `WithReferenceLoopProtection` enables the protection against the [reference loop](#reference-loop).
  - If not explicitly set, the [validator](#validator) turns it on automatically if the [reference loop](#reference-loop) is theoretically possible according to the [specification](#specification).
- `WithReferenceLoopProtectionDisabled` disables the protection against the [reference loop](#reference-loop).
  - One scenario when this protection is redundant is when you're absolutely sure that the object won't have [reference loops](#reference-loop), because the model is e.g., deserialized from the string.
- Settings' property `ReferenceLoopProtectionEnabled` holds to final value.

#### WithTranslation

- `WithTranslation` accepts three parameters:
  - `name` - translation name
  - `messageKey` - message key
  - `translation` - the content for the given message key

``` csharp
settings => settings
    .WithTranslation("English", "Global.Error", "Error found")
    .WithTranslation("English", "Global.Required", "Value is required")
    .WithTranslation("Polish", "Global.Required", "Wartość wymagana");
```

- Called with keys (`name` or `messageKey`) for the first time, `WithTranslation` creates the underlying dictionaries.
- Called multiple times with the same keys (`name` and `messageKey`), `WithTranslation` overwrites the previous value with the provided `translation` value.
- `WithTranslation` can also be used to overwrite the existing values (like the default ones or those added before, with another `WithTranslation` method, in whatever form).
  - In order to overwrite the default value, you need to check the message key that the rule uses.
  - Good to read;
    - [Translations](#translations) - how translations work.
    - [Rules](#rules) - the list of rules and their message keys.

``` csharp
Specification<AuthorModel> specification = s => s
    .Member(m => m.Email, m => m
        .NotEmpty()
        .Email()
    )
    .Member(m => m.Name, m => m
        .Required().WithMessage("Name is required")
    );

var author = new AuthorModel()
{
    Email = ""
};

var validator1 = Validator.Factory.Create(specification);

validator1.Validate(author).ToString();
// Email: Must not be empty
// Email: Must be a valid email address
// Name: Name is required

var validator2 = Validator.Factory.Create(specification, settings => settings
    .WithTranslation("English", "Name is required", "You must fill out the name")
    .WithTranslation("English", "Texts.NotEmpty", "Text value cannot be empty")
);

validator2.Validate(author).ToString();
// Email: Text value cannot be empty
// Email: Must be a valid email address
// Name: You must fill out the name
```

_In the above code, the default value for `NotEmpty` (message key `Texts.NotEmpty`) has been overridden with the content `Text value cannot be empty`_

- `WithTranslation` has a version (via extension method) that wraps the base method and accepts:
    - `name` - translation name
    - `translation` - dictionary; its keys are set as `messageKey` and the related values as `translations`.


``` csharp
settings => settings
    .WithTranslation("English", new Dictionary<string, string>()
    {
        ["Global.Error"] = "Error found",
        ["Global.Required"] = "Value is required",
    })
    .WithTranslation("Polish", new Dictionary<string, string>()
    {
        ["Global.Required"] = "Wartość wymagana",
    });
```

- `WithTranslation` has a version (via extension method) that wraps the base method and accepts `IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>`:
  - the keys is passed as `name`
  - the value is another dictionary; its keys are set as `messageKey` and the related values as `translations`.

``` csharp
settings => settings
    .WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
    {
        ["English"] = new Dictionary<string, string>()
        {
            ["Global.Error"] = "Error found",
            ["Global.Required"] = "Value is required",
        },
        ["Polish"] = new Dictionary<string, string>()
        {
            ["Global.Required"] = "Wartość wymagana",
        }
    });
```

- `WithTranslation` also has extension methods that wrap the base method and add entries for a specific translation:
  - `WithEnglishTranslation` - adds [English translation](../src/Validot/Translations/English/EnglishTranslation.cs), by default always present in the settings.
  - `WithPolishTranslation` - adds [Polish translation](../src/Validot/Translations/Polish/PolishTranslation.cs), by default, always present in the settings.


### Template

- `Template` is a byproduct of the analysis that the [validator](#validator) performs during the initialization.
  - Validator traverses through all of the commands in [specification](#specification), determines and caches all the possible [paths](#path), [messages](#message) and [codes](#code).
  - `Template` is the object of the same type as [results](#result) (`IValidationResult`), so you can check all of the cached data with the same properties, verify the translations, error codes, etc.

``` csharp
Specification<string> specification = s => s
    .NotEmpty()
    .NotWhiteSpace().WithMessage("White space is not allowed")
    .Rule(m => m.Contains('@')).WithMessage("Must contain @ character");

var validator = Validator.Factory.Create(specification);

validator.Template.ToString();
// Required
// Must not be empty
// White space is not allowed
// Must contain @ character
```

- The first difference between the actual [validation result](#result) and the `Template` is that the `Template` doesn't have indexes in the [paths](#path).
    - It doesn't make any sense because `Template` isn't related to any particular object.
    - Example; `Collection.#.NestedCollection.#.Something` instead of `Collection.#5.NestedCollection.#0.Something` that would appear in the [result](#result) of the [Validate](#validate) method.

``` csharp
Specification<BookModel> specification = s => s
    .Member(m => m.Authors, m => m
        .AsCollection(m1 => m1
            .Member(m2 => m2.Name, m2 => m2.NotEmpty())
        )
    );

var validator = Validator.Factory.Create(specification);

validator.Template.ToString();
// Required
// Authors: Required
// Authors.#: Required
// Authors.#.Name: Required
// Authors.#.Name: Must not be empty
```

- The second difference between the actual [validation result](#result) and the `Template` is that the in case of the [reference loop](#reference-loop), `Template` contains only the message set by the key `Global.ReferenceLoop`.
  - The default English translation is `(reference loop)`.
  - Such error output is placed at the root of the reference loop.

``` csharp
Specification<B> specificationB = null;

Specification<A> specificationA = s => s
    .Member(m => m.B, specificationB);

specificationB = s => s
    .Member(m => m.A, specificationA);

var validator = Validator.Factory.Create(specificationA);

validator.Template.ToString();
// Required
// B: Required
// B.A: (reference loop)
```

- `Template` contains all theoretically possible errors, so it would also have the [error outputs](#error-output) that in the real world would be exclusive to each other (literally all predicates are ignored).
  - It also means, that the printing of the `Template` (generated by [ToString](#tostring) method) could be quite large.

``` csharp
Specification<AuthorModel> authorSpecification = s => s
    .Member(m => m.Email, m => m
        .NotWhiteSpace().WithMessage("Email cannot be whitespace")
        .Email()
    )
    .Member(m => m.Name, m => m
        .NotEmpty()
        .NotWhiteSpace()
        .MinLength(2)
    );

Specification<BookModel> specification = s => s
    .Member(m => m.Title, m => m.NotEmpty()).WithExtraCode("EMPTY_TITLE")
    .Member(m => m.YearOfFirstAnnouncement, m => m.BetweenOrEqualTo(1000, 3000))
    .Member(m => m.Authors, m => m
        .AsCollection(authorSpecification)
        .MaxCollectionSize(4).WithMessage("Book shouldn't have more than 4 authors").WithExtraCode("MANY_AUTHORS")
    )
    .Rule(m => m.YearOfPublication >= m.YearOfFirstAnnouncement)
    .WithCondition(m => m.YearOfPublication.HasValue)
    .WithMessage("Year of publication needs to be after the first announcement");

var validator = new Validator<BookModel>(specification);

validator.Template.ToString();
// EMPTY_TITLE, MANY_AUTHORS
//
// Required
// Year of publication needs to be after the first announcement
// Title: Required
// Title: Must not be empty
// YearOfFirstAnnouncement: Must be between 1000 and 3000 (inclusive)
// Authors: Required
// Authors: Book shouldn't have more than 4 authors
// Authors.#: Required
// Authors.#.Email: Required
// Authors.#.Email: Email cannot be whitespace
// Authors.#.Email: Must be a valid email address
// Authors.#.Name: Required
// Authors.#.Name: Must not be empty
// Authors.#.Name: Must not consist only of whitespace characters
// Authors.#.Name: Must be at least 2 characters in length
```

## Result

- Validation result is an object of type `IValidationResult` and is produced by the [Validate](#validate) method.
- The result is internally linked with the [validator](#validator) that created it.
  - This is the reason behind its ability to translate the [messages](#message) that are registered within the [validator](#validator).
  - This is also the reason you shouldn't store the `IValidationResult` object for too long or pass it around your system.
    - However, you can retrieve the data using its properties (listed below, here in this section). They are safe to operate on.

### AnyErrors

- `AnyErrors` is the flag that returns:
  - `true` - if there are errors.
  - `false` - no errors and the object is valid according to the specification.

``` csharp
Specification<string> specification = s => s
    .NotEmpty();

var validator = Validator.Factory.Create(specification);

var result1 = validator.Validate("test");

result1.AnyErrors; // false

var result2 = validator.Validate("");

result2.AnyErrors; // true
```

### Paths

- `Paths` property is the collection of all [paths](#path) that contain [error output](#error-output).
  - It doesn't matter whether it's an [error output](#error-output) with only [messages](#message), [codes](#code), or a mix.
- `Paths` can be used to check if the value under a certain [path](#path) is valid or not.
- `Paths` collection doesn't contain duplicates.
- To check what [messages](#message) and/or [codes](#code) have been saved under a [path](#path), you need to use [CodeMap](#codemap) and [MessageMap](#messagemap).
- The order of the elements in the collection is not guaranteed.
- The empty string means the root model.

``` csharp
Specification<AuthorModel> authorSpecification = s => s
    .Member(m => m.Email, m => m.Email().WithCode("EMAIL"))
    .Member(m => m.Name, m => m
        .NotEmpty()
        .MinLength(3)
        .NotContains("X").WithMessage("X character is not allowed in name")
    );

Specification<BookModel> bookSpecification = s => s
    .Member(m => m.Title, m => m.NotWhiteSpace())
    .Member(m => m.Authors, m => m
        .AsCollection(authorSpecification)
    )
    .Rule(m => m.IsSelfPublished == false).WithCode("ERROR_SELF_PUBLISHED");

var bookValidator = Validator.Factory.Create(bookSpecification);

var book = new BookModel()
{
    Title = "",
    Authors = new[]
    {
        new AuthorModel() { Email = "john.doe@gmail.com", Name = "X" },
        new AuthorModel() { Email = "jane.doe@gmail.com", Name = "Jane" },
        new AuthorModel() { Email = "inv@lidem@il", Name = "Jane" }
    },
    IsSelfPublished = true
};

var result = bookValidator.Validate(book);

result.Paths; // [ "", "Title", "Authors.#0.Name", "Authors.#2.Email" ]
```

_In the above example, all [paths](#path) with errors are listed in `Paths` collection. Including `Email` and root that contain a single [error code](#code). Also, `Authors.#0.Name` path has two errors (from `MinLength` and `NotContains` commands), but it's present only once._

### Codes

- `Codes` property is the collection of all the [codes](#code) in the [error output](#error-output).
  - The path doesn't matter. All codes from all the error outputs are listed.
- `Codes` collection can be used to check if some [code](#code) has been recorded for the validated model.
  - To check where exactly, you need to use [CodeMap](#codemap).
- `Codes` collection doesn't contain duplicates.
- The order of the elements in the collection is not guaranteed.

``` csharp
Specification<PublisherModel> specification = s => s
    .Member(m => m.Name, m => m
        .NotEmpty().WithCode("EMPTY_FIELD").WithExtraCode("NAME_ERROR")
        .MinLength(3).WithCode("SHORT_FIELD").WithExtraCode("NAME_ERROR")
    )
    .Member(m => m.CompanyId, m => m
        .NotEmpty().WithCode("EMPTY_FIELD").WithExtraCode("COMPANYID_ERROR")
        .NotContains("ID").WithCode("ID_IN_CONTENT")
    )
    .Rule(m => m.Name != m.CompanyId).WithCode("SAME_VALUES");

var validator = Validator.Factory.Create(specification);

var publisher = new PublisherModel()
{
    Name = "",
    CompanyId = ""
};

var result = validator.Validate(publisher);

result.Codes; // [ "EMPTY_FIELD", "NAME_ERROR", "SHORT_FIELD", "COMPANYID_ERROR", "SAME_VALUES" ]
```

_In the above code, `EMPTY_FIELD` and `NAME_ERROR` are not duplicated in `Codes`, despite the fact that several different rules save them in the [error output](#error-output)._

### CodeMap

- `CodeMap` is a dictionary that links [error codes](#code) with their [paths](#path).
- `CodeMap` is property of type `IReadOnlyDictionary<string, IReadOnlyList<string>>`, where:
  - the key is the [path](#path).
  - the value is the list of error [codes](#code) saved under the related path.
    - the list can contain duplicates.

``` csharp
Specification<PublisherModel> specification = s => s
    .Member(m => m.Name, m => m
        .NotEmpty().WithCode("EMPTY_FIELD").WithExtraCode("NAME_ERROR")
        .MinLength(3).WithCode("SHORT_FIELD").WithExtraCode("NAME_ERROR")
    )
    .Member(m => m.CompanyId, m => m
        .NotEmpty().WithCode("EMPTY_FIELD").WithExtraCode("COMPANYID_ERROR")
        .NotContains("company").WithCode("COPANY_IN_CONTENT")
        .NotContains("id").WithMessage("Invalid company value")
    )
    .Rule(m => m.Name is null || m.CompanyId is null).WithCode("NULL_MEMBER");

var validator = Validator.Factory.Create(specification);

var publisher = new PublisherModel()
{
    Name = "",
    CompanyId = "some_id"
};

var result = validator.Validate(publisher);

result.CodeMap["Name"]; // [ "EMPTY_FIELD", "NAME_ERROR", "SHORT_FIELD", "NAME_ERROR" ]
result.CodeMap[""]; // [ "NULL_MEMBER" ]
```

- If the [path](#path) is not present in `CodeMap.Keys` collection, it means no code has been saved for it.
  - If the path present in [Paths](#paths) collection is missing in `CodeMap.Keys`, it means that the [error output](#error-output) for it doesn't contain codes. You should check [MessageMap](#messagemap) instead.

``` csharp
result.Paths.Contains("CompanyId"); // true

result.CodeMap.Keys.Contains("CompanyId"); // false

result.MessageMap.Keys.Contains("CompanyId"); // true
```

### MessageMap

- `MessageMap` is a dictionary that links [error messages](#message) with their [paths](#path).
- `MessageMap` is property of type `IReadOnlyDictionary<string, IReadOnlyList<string>>`, where:
  - the key is the [path](#path).
  - the value is the list of [error messages](#message) saved under the related [path](#path).
    - the list can contain duplicates.
- `MessagesMap` always uses the default translation (`English`).
  - If you want to have them translated with a different dictionary, use [GetTranslatedMessageMap](#gettranslatedmessagemap) function.
  - Good to read; [Translations](#translations).

``` csharp
Specification<PublisherModel> specification = s => s
    .Member(m => m.Name, m => m
        .NotEmpty().WithMessage("The field is empty").WithExtraMessage("Error in Name field")
        .MinLength(3).WithMessage("The field is too short").WithExtraMessage("Error in Name field")
    )
    .Member(m => m.CompanyId, m => m
        .NotEmpty().WithMessage("The field is empty").WithExtraMessage("Error in CompanyId field")
        .NotContains("company").WithMessage("Company Id cannot contain 'company' word")
        .NotContains("id").WithCode("ID_IN_COMPANY")
    )
    .Rule(m => m.Name is null || m.CompanyId is null)
    .WithMessage("All members must be present");

var validator = Validator.Factory.Create(specification);

var publisher = new PublisherModel()
{
    Name = "",
    CompanyId = "some_id"
};

var result = validator.Validate(publisher);

result.MessageMap["Name"];
// [ "The field is empty", "Error in Name field", "The field is too short", "Error in Name field" ]

result.MessageMap[""];
// [ "All members must be present" ]
```

- If the [path](#path) is not present in `MessagesMap.Keys` collection, it means no [code](#code) has been saved for it.
  - If the path present in [Paths](#paths) collection is missing in `MessageMap.Keys`, it means that the [error output](#error-output) for it doesn't contain codes. You should check [CodeMap](#codemap) instead.

``` csharp
result.Paths.Contains("CompanyId"); // true

result.MessageMap.Keys.Contains("CompanyId"); // false

result.CodeMap.Keys.Contains("CompanyId"); // true
```

### GetTranslatedMessageMap

- `GetTranslatedMessageMap` returns similar result to `MessageMap`.
  - Structure and meaning are the same but the messages are translated.
- `GetTranslatedMessageMap` accepts single parameter; `translationName`:

``` csharp
Specification<AuthorModel> specification = s => s
    .Member(m => m.Name, m => m
        .NotEmpty()
        .MinLength(3).WithMessage("Name is too short")
    )
    .Member(m => m.Email, m => m
        .Email()
    );

var validator = Validator.Factory.Create(specification, settings => settings
    .WithPolishTranslation()
    .WithTranslation("Polish", "Name is too short", "Imię jest zbyt krótkie")
);

var author = new AuthorModel()
{
    Name = "",
    Email = "inv@lidem@il"
};

var result = validator.Validate(author);

var englishMessageMap = result.GetTranslatedMessageMap("English");

englishMessageMap["Name"]; // [ "Must not be empty", "Name is too short" ]
englishMessageMap["Email"]; // [ "Must be a valid email address" ]

var polishMessageMap = result.GetTranslatedMessageMap("Polish");

polishMessageMap["Name"]; // [ "Musi nie być puste", "Imię jest zbyt krótkie" ]
polishMessageMap["Email"]; // [ "Musi być poprawnym adresem email" ]
```

- If the given `translationName` is not present in [TranslationNames](#translationnames) list, exception is thrown.
- Good to read;
  - [Translations](#translations) - how translation works
  - [WithTranslation](#withtranslation) - how to set translation messages


``` csharp
var validator = Validator.Factory.Create(specification, settings => settings
    .WithPolishTranslation()
);

var result = validator.Validate(author);

result.GetTranslatedMessageMap("Russian"); // throws KeyNotFoundException
```

### TranslationNames

- `TranslationNames` is a list of all translation names that can be used to translate [messages](#message) in the [result](#result).
  - The messages can be translated with [ToString](#tostring) and [GetTranslatedMessageMap](#gettranslatedmessagemap) functions.

``` csharp
var validator = Validator.Factory.Create(specification);

var result = validator.Validate(model);

result.TranslationNames; // [ "English" ]
```

- The list is the same as in the [Validator](#validator) that produced the result.

``` csharp
var validator = Validator.Factory.Create(specification, settings => settings
    .WithPolishTranslation()
);

var result = validator.Validate(model);

result.TranslationNames; // [ "Polish", "English" ]
```

### ToString

- `ToString` is a helper method that prints the error [codes](#code) and [messages](#message) in the following format:
  - In the first line: all the [codes](#code) from [Codes](#codes) collection, comma separated.
    - If no [error codes](#code), the printing starts directly with the [messages](#message).
    - If there is a line with error codes, it's separated from the messages with the empty line.
  - Each message is printed in a separate line, each one preceded with its [path](#path).
    - In the root path, the message starts from the beginning of the line.
- Order of the codes and messages are is guaranteed.


```
CODE1, CODE2, CODE3

Root message
Path: Message in the path
Path.Nested: Nested message 1
Path.Nested: Nested message 2
Path.Nested: Nested message 3
```

- Effectively, it's like printing [Codes](#codes) in the first line and then [MessageMap](#messagemap).
- The basic version of `ToString` always uses the default translation, which is `English`.

``` csharp
Specification<PublisherModel> specification = s => s
    .Member(m => m.Name, m => m
        .NotEmpty()
        .WithMessage("The field is empty")
        .WithExtraMessage("Error in Name field")
        .WithExtraCode("NAME_EMPTY")

        .MinLength(3)
        .WithMessage("The field is too short")
        .WithExtraCode("NAME_TOO_SHORT")
    )
    .Member(m => m.CompanyId, m => m
        .NotEmpty()
        .NotContains("id")
        .WithCode("ID_IN_COMPANY")
    )
    .Rule(m => m.Name is null || m.CompanyId is null)
    .WithMessage("All members must be present");

var validator = Validator.Factory.Create(specification);

var publisher = new PublisherModel()
{
    Name = "",
    CompanyId = "some_id"
};

var result = validator.Validate(publisher);

result.ToString();
// NAME_EMPTY, NAME_TOO_SHORT, ID_IN_COMPANY

// Name: The field is empty
// Name: Error in Name field
// Name: The field is too short
// All members must be present
```

- `ToString` also has a version that accepts a single parameter; `translationName`. Use to retrieve the same content, but translated using the dictionary of the given name.
  - `translationName` needs to be listed in [TranslationNames](#translationnames). Otherwise, you can expect an exception.

``` csharp
Specification<PublisherModel> specification = s => s
    .Member(m => m.Name, m => m
        .NotEmpty()
        .MinLength(3)
    )
    .Member(m => m.CompanyId, m => m
        .NotEmpty().WithMessage("CompanyId field is required")
    );

var validator = Validator.Factory.Create(specification, settings => settings
    .WithPolishTranslation()
    .WithTranslation("Polish", "CompanyId field is required", "Pole CompanyId jest wymagane")
);

var publisher = new PublisherModel()
{
    Name = "",
    CompanyId = ""
};

var result = validator.Validate(publisher);

result.ToString();
// Name: Must not be empty
// Name: Must be at least 3 characters in length
// CompanyId: CompanyId field is required

result.ToString("Polish");
// Name: Musi nie być puste
// Name: Musi być długości minimalnie 3 znaków
// CompanyId: Pole CompanyId jest wymagane

result.ToString("Russian"); // throws exception
```
- Good to read;
  - [Translations](#translations) - how translation works.
  - [WithTranslation](#withtranslation) - how to set translation messages.
- In case of a valid result, `ToString` prints simple message: `OK`:

``` csharp
Specification<PublisherModel> specification = s => s;

var validator = Validator.Factory.Create(specification);

var model = new PublisherModel();

var result = validator.Validate(model);

result.AnyErrors; // false

result.ToString();
// OK
```

## Rules

### Global rules


| Fluent api | Message key | Args |
| - | - | - |
| `Rule` | `Global.Error` | - |
| `Required` | `Global.Required` | - |
| `Forbidden` | `Global.Forbidden` | - |
| reference loop | `Global.ReferenceLoop` | - |

- [Reference loop](#reference-loop) error is a special case, it doesn't have the dedicated fluent api command and is related to the existence of [reference loop](#reference-loop).

### Bool rules

- Rules apply to `bool`.

| Fluent api | Message key | Args |
| - | - | - |
| `True` | `BoolType.True` | - |
| `False` | `BoolType.False` | - |

### Char rules

- Rules apply to `char`.
- `char` can be validated by the below rules and all of the [number rules](#number-rules) for the unsigned types.

| Fluent api | Message key | Args |
| - | - | - |
| `EqualToIgnoreCase` | `CharType.True` | `value` : [text](#text-argument) |
| `NotEqualToIgnoreCase` | `CharType.False` | `value` : [text](#text-argument) |

### Collections rules

- Rules apply to any object that implements `IEnumerable<T>`.
- There are dedicated generic versions for: `T[]`, `IEnumerable<T>`, `IList<T>`, `IReadOnlyCollection<T>`, `IReadOnlyList<T>`, `List<T>`.
    - Dedicated means that you don't need to specify `IEnumerable<T>` and `T` explicitly as generic parameters.

| Fluent api | Message key | Args |
| - | - | - |
| `EmptyCollection` | `Collections.EmptyCollection` | - |
| `NotEmptyCollection` | `Collections.EmptyCollection` | - |
| `ExactCollectionSize` | `Collections.ExactCollectionSize` | `size` : [number](#number-argument) |
| `MaxCollectionSize` | `Collections.MaxCollectionSize` | `max` : [number](#number-argument) |
| `MinCollectionSize` | `Collections.MinCollectionSize` | `min` : [number](#number-argument) |
| `CollectionSizeBetween` | `Collections.CollectionSizeBetween` | `min` : [number](#number-argument), <br> `max` : [number](#number-argument) |

### Numbers rules

- Rules for all unsigned and signed types:

| Fluent api | Message key | Args |
| - | - | - |
| `EqualTo` | `Numbers.EqualTo` | `value` : [number](#number-argument) |
| `NotEqualTo` | `Numbers.EqualTo` | `value` : [number](#number-argument) |
| `GreaterThan` | `Numbers.Greater` | `min` : [number](#number-argument) |
| `GreaterThanOrEqualTo` | `Numbers.GreaterThanOrEqualTo` | `min` : [number](#number-argument) |
| `LessThan` | `Numbers.LessThan` | `max` : [number](#number-argument) |
| `LessThanOrEqualTo` | `Numbers.LessThanOrEqualTo` | `max` : [number](#number-argument) |
| `Between` | `Numbers.LessThan` | `min` : [number](#number-argument), <br> `max` : [number](#number-argument) |
| `BetweenOrEqualTo` | `Numbers.LessThanOrEqualTo` | `min` : [number](#number-argument), <br> `max` : [number](#number-argument) |
| `NonZero` | `Numbers.NonZero` | - |
| `Positive` | `Numbers.Positive` | - |
| `NonPositive` | `Numbers.NonPositive` | - |

- Extra rules just for signed types:

| Fluent api | Message key | Args |
| - | - | - |
| `Negative` | `Numbers.Negative` | - |
| `NonNegative` | `Numbers.NonNegative` | - |

- Floating-point types `double` and `float` have a special version of some rules that allows to set the tolerance level
  - the default value of `tolerance` is `0.0000001`.
  - this is pretty much enforced by the specifics of the binary system, so if you want to avoid the risk, please use `decimal` type.

| Fluent api | Message key | Args |
| - | - | - |
| `EqualTo` | `Numbers.EqualTo` | `value` : [number](#number-argument), `tolerance` : [number](#number-argument) |
| `NotEqualTo` | `Numbers.EqualTo` | `value` : [number](#number-argument), `tolerance` : [number](#number-argument) |
| `NonZero` | `Numbers.NonZero` | `tolerance` : [number](#number-argument) |
| `NonNan` | `Numbers.NonNan` | - |

### Texts rules

- Content rules
  - The enum that sets the comparison strategy is the standard [StringComparison](https://docs.microsoft.com/en-us/dotnet/api/system.stringcomparison?view=netstandard-2.0) enum.
  - The default value of `stringComparison` is `Ordinal`.

| Fluent api | Message key | Args |
| - | - | - |
| `EqualTo` | `Texts.EqualTo` | `value` : [text](#text-argument), <br> `stringComparison` : [enum](#enum-argument) |
| `NotEqualTo` | `Texts.NotEqualTo` | `value` : [text](#text-argument), <br> `stringComparison` : [enum](#enum-argument) |
| `Contains` | `Texts.Contains` | `value` : [text](#text-argument), <br> `stringComparison` : [enum](#enum-argument) |
| `NotContains` | `Texts.NotContains` | `value` : [text](#text-argument), <br> `stringComparison` : [enum](#enum-argument) |
| `StartsWith` | `Texts.StartsWith` | `value` : [text](#text-argument), <br> `stringComparison` : [enum](#enum-argument) |
| `EndsWith` | `Texts.EndsWith` | `value` : [text](#text-argument), <br> `stringComparison` : [enum](#enum-argument) |
| `Matches` | `Texts.Matches` | `pattern` : [text](#text-argument) |
| `NotEmpty` | `Texts.NotEmpty` | - |
| `NotWhiteSpace` | `Texts.NotWhiteSpace` | - |

- Text length rules
  - When calculating length, `Environment.NewLine` is count as 1.

| Fluent api | Message key | Args |
| - | - | - |
| `SingleLine` | `Texts.SingleLine` | - |
| `ExactLength` | `Texts.ExactLength` | `length` : [number](#number-argument) |
| `MaxLength` | `Texts.MaxLength` | `max` : [number](#number-argument) |
| `MinLength` | `Texts.MinLength` | `min` : [number](#number-argument) |
| `LengthBetween` | `Texts.LengthBetween` | `min` : [number](#number-argument), <br> `max` : [number](#number-argument) |

- Email rules
    - `Email` rule has two modes, set by the enum value of type `Validot.EmailValidationMode`
      - `.Email(mode: EmailValidationMode.ComplexRegex)` is set by default (works the same as parameterless `.Email()`) and contains the regex-based logic copy-pasted from the [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format).
      - `.Email(mode: EmailValidationMode.DataAnnotationsCompatible)` checks only if the value contains a single `@` character in the middle, which is the logic used in the dotnet's [System.ComponentModel.DataAnnotations.EmailAddressAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.emailaddressattribute).
        - It's less accurate, but benchmarks show that it's about 6x faster while consuming 32% less memory.

| Fluent api | Message key | Args |
| - | - | - |
| `Email` | `Texts.Email` | - |

### Times rules

- Rules apply to `DateTime` and `DateTimeOffset`.
- `TimeComparison` is the custom enum in Validot and describes the way time should be compared:
  - `All` - both date part and time part are compared.
  - `JustDate` - only date is compared (the time part is completely skipped)
  - `JustTime` - only time is compared (the date part is completely skipped)

| Fluent api | Message key | Args |
| - | - | - |
| `Equalto` | `Times.Equalto` | `value` : [time](#time-argument), <br> `timeComparison` : [enum](#enum-argument) |
| `NotEqualto` | `Times.Equalto` | `value` : [time](#time-argument), <br> `timeComparison` : [enum](#enum-argument) |
| `After` | `Times.After` | `min` : [time](#time-argument), <br> `timeComparison` : [enum](#enum-argument) |
| `AfterOrEqualTo` | `Times.AfterOrEqualTo` | `min` : [time](#time-argument), <br> `timeComparison` : [enum](#enum-argument) |
| `Before` | `Times.Before` | `max` : [time](#time-argument), <br> `timeComparison` : [enum](#enum-argument) |
| `BeforeOrEqualTo` | `Times.BeforeOrEqualTo` | `max` : [time](#time-argument), <br> `timeComparison` : [enum](#enum-argument) |
| `Between` | `Times.Between` | `max` : [time](#time-argument), <br> `timeComparison` : [enum](#enum-argument) |
| `BetweenOrEqualTo` | `Times.BetweenOrEqualTo` | `min` : [time](#time-argument), <br> `max` : [time](#time-argument), <br> `timeComparison` : [enum](#enum-argument) |

### Guid rules

- Rules apply to `Guid`.

| Fluent api | Message key | Args |
| - | - | - |
| `EqualTo` | `GuidType.EqualTo` | `value` : [guid](#guid-argument) |
| `NotEqualTo` | `GuidType.NotEqualTo` | `value` : [guid](#guid-argument) |
| `NotEmpty` | `GuidType.NotEmpty` | `value` : [guid](#guid-argument) |

### TimeSpan rules

- Rules apply to `TimeSpan`.
  - Most of them are same as for numbers, but with different message.

| Fluent api | Message key | Args |
| - | - | - |
| `EqualTo` | `TimeSpanType.EqualTo` | `value` : [type](#type-argument) |
| `NotEqualTo` | `TimeSpanType.EqualTo` | `value` : [type](#type-argument) |
| `GreaterThan` | `TimeSpanType.Greater` | `min` : [type](#type-argument) |
| `GreaterThanOrEqualTo` | `TimeSpanType.GreaterThanOrEqualTo` | `min` : [type](#type-argument) |
| `LessThan` | `TimeSpanType.LessThan` | `max` : [type](#type-argument) |
| `LessThanOrEqualTo` | `TimeSpanType.LessThanOrEqualTo` | `max` : [type](#type-argument) |
| `Between` | `TimeSpanType.LessThan` | `min` : [type](#type-argument), <br> `max` : [type](#type-argument) |
| `BetweenOrEqualTo` | `TimeSpanType.LessThanOrEqualTo` | `min` : [type](#type-argument), <br> `max` : [type](#type-argument) |
| `NonZero` | `TimeSpanType.NonZero` | - |
| `Positive` | `TimeSpanType.Positive` | - |
| `NonPositive` | `TimeSpanType.NonPositive` | - |
| `Negative` | `TimeSpanType.Negative` | - |
| `NonNegative` | `TimeSpanType.NonNegative` | - |


### Custom rules

- Custom rules should be based on [RuleTemplate](#ruletemplate) command, wrapped into an extension method.
  - The method needs to extend the `IRuleIn<T>` interface, where `T` is the type of the object to be validated.
  - The method needs to return `IRuleOut<T>`.
  - Both `IRuleOut<T>` and `IRuleIn<T>` ensure that the custom rule complies with the Validot's fluent api structures.
- The namespace where the extension method is doesn't matter that much.
  - However, all built-in rules live in `Validot` namespace.

``` csharp
public static class MyCustomValidotRules
{
    public static IRuleOut<string> HasCharacter(this IRuleIn<string> @this)
    {
        return @this.RuleTemplate(
            m => m.Length > 0,
            "Must have at least one character!"
        );
    }
}
```

_Above, the definition of the custom rule `HasCharacters`. Below, the example os usage._

``` csharp
Specification<string> specification = s => s
    .HasCharacter();

var validator = Validator.Factory.Create(specification);

validator.Validate("test").AnyErrors; // false

validator.Validate("").ToString();
// Must have at least one character!
```

- Custom rules can have arguments.
  - Please be extra careful with wrapping/boxing external references into the predicate. It might cause the memory leak, especially if the [validator](#validator) does exist as a singleton.
  - The pattern is: all method arguments should be available as [message arguments](#message-arguments) under the same names.

``` csharp
public static IRuleOut<string> HasCharacter(
    this IRuleIn<string> @this,
    char character,
    int count = 1)
{
    return @this.RuleTemplate(
        value => value.Count(c => c == character) == count,
        "Must have character '{character}' in the amount of {count}",
        Arg.Text(nameof(character), character),
        Arg.Number(nameof(count), count)
    );
}
```

``` csharp
Specification<string> specification = s => s
    .HasCharacter('t', 2);

var validator = Validator.Factory.Create(specification);

validator.Validate("test").AnyErrors; // false

validator.Validate("").ToString();
// Must have character 't' in the amount of 2
```

- Instead of a message, you can provide a message key. Technically there is no difference, but it's easier for the user to overwrite the content.
  - The pattern for the message key is `Category.MethodName`.
    - Example; `EqualTo` for texts is `Texts.EqualTo`
    - Example; `GreaterThan` for numbers is `Numbers.GreaterThan`
  - Good to read:
    - [Rules](#rules) - list of built-in rules, along with their message keys and available arguments.
    - [Translations](#translations) - how translations work.

``` csharp
public static IRuleOut<string> HasCharacter(
    this IRuleIn<string> @this,
    char character,
    int count = 1)
{
    return @this.RuleTemplate(
        value => value.Count(c => c == character) == count,
        "Text.HasCharacter",
        Arg.Text(nameof(character), character),
        Arg.Number(nameof(count), count)
    );
}
```

``` csharp
Specification<string> specification = s => s
    .HasCharacter('t', 2);

var validator = Validator.Factory.Create(specification, settings => settings
    .WithTranslation("English", "Text.HasCharacter", "Must have character '{character}' in the amount of {count}")
    .WithTranslation("Polish", "Text.HasCharacter", "Musi zawierać znak '{character}' w ilości {count|culture=pl-PL}")
);

validator.Validate("test").AnyErrors; // false

var result = validator.Validate("");

result.ToString();
// Must have character 't' in the amount of 2

result.ToString(translationName: "Polish");
// Musi zawierać znak 't' w ilości 2
```

## Message arguments

- [Error message](#message) might contain arguments in its content.
  - The placeholder for the argument value is using pattern: `{argumentName}`.
  - Arguments can be used in [WithMessage](#withmessage), [WithExtraMessage](#withextramessage) and [RuleTemplate](#ruletemplate).
  - The pattern followed in all [the built-in rules](#rules) is: the argument name is exactly the same as the method's argument.

``` csharp
Specification<decimal> specification = s => s
    .Between(min: 0.123M, max: 100.123M)
    .WithMessage("The number needs to fit between {min} and {max}");

var validator = Validator.Factory.Create(specification);

validator.Validate(105).ToString();
// The number needs to fit between 0.123 and 100.123
```

- Arguments can be parametrized:
  - Parameters follow format: `parameterName=parameterValue`.
  - Parameters are separated with `|` (vertical bar, pipe) character from the argument name and from each other.
  - Single parameter example: `{argumentName|parameterName=parameterValue}`.
  - Multiple parameters example: `{argumentName|param1=value1|param2=value2|param3=value3}`.

``` csharp
Specification<decimal> specification = s => s
    .Between(min: 0.123M, max: 100.123M)
    .WithMessage("The maximum value is {max|format=000.000}")
    .WithExtraMessage("The minimum value is {min|format=000.000|culture=pl-PL}");

var validator = Validator.Factory.Create(specification);

validator.Validate(105).ToString();
// The maximum value is 100.123
// The minimum value is 000,123
```

### Enum argument

- Types: all enums
- Created with `Arg.Enum("name", value)`.
- Parameters:
  - `format` - number format, the string that goes to [ToString](https://docs.microsoft.com/en-us/dotnet/api/system.enum.tostring?view=netstandard-2.0) method.
    - if not set, the default value is `G`.
  - `translation` - if set to `true`, placeholder is transformed into [translation argument](#translation-argument): `{_translation|key=messageKey}`.
    - the message key is in this format: `Enum.EnumFullTypeName.EnumValueName`.
    - ultimately, placeholder will be replace with text from the specification.
    - if `translation` is present, `format` is ignored.

| Placeholder | Argument | Final form |
| - | - | - |
| `{arg}` | `StringComparison.Ordinal` | `Oridinal` |
| `{arg\|format=G}` | `StringComparison.Ordinal` | `Oridinal` |
| `{arg\|format=D}` | `StringComparison.Ordinal` | `4` |
| `{arg\|format=X}` | `StringComparison.Ordinal` | `00000004` |
| `{arg\|translation=true}` | `StringComparison.Ordinal` | `{_translation\|key=Enum.System.StringComparison.Ordinal}` |


``` csharp
Specification<string> gmailSpecification = s => s
    .EndsWith("@gmail.com", stringComparison: StringComparison.OrdinalIgnoreCase)
    .WithMessage("Must ends with @gmail.com {stringComparison|translation=true}");

var validator = Validator.Factory.Create(gmailSpecification, settings => settings
    .WithTranslation("English", "Enum.System.StringComparison.OrdinalIgnoreCase", "(ignoring case!)")
);

validator.Validate("john.doe@outlook.com").ToString();
// Must ends with @gmail.com (ignoring case!)
```

_In the example above, [WithMessage](#withmessage) is using `{stringComparison|translation=true}` placeholder, which is - under the hood - transformed into [translation argument](#translation-argument) `{_translation|key=Enum.System.StringComparison.Ordinal}` and ultimately - replaced with the message registered under the key `Enum.System.StringComparison.Ordinal`._

- Good to read:
  - [translation argument](#translation-argument) - how to translation argument works.
  - [translations](#translations) - how translations work.

### Guid argument

- Types: `Guid`
- Created with `Arg.GuidValue("name", value)`.
- Parameters:
  - `format` - guid format, the string that goes to [ToString](https://docs.microsoft.com/en-us/dotnet/api/system.guid.tostring?view=netstandard-2.0) method.
    - if not set, the default value is `D`.
  - `case`
    - available values: `upper`, `lower`.
    - calls `ToUpper` or `ToLower` method on the stringified guid value.


| Placeholder | Argument | Final form |
| - | - | - |
| `{arg}` | `c2ce1f3b-17e5-412e-923b-6b4e268f31aa` | `c2ce1f3b-17e5-412e-923b-6b4e268f31aa` |
| `{arg\|case=upper}` | `c2ce1f3b-17e5-412e-923b-6b4e268f31aa` | `C2CE1F3B-17E5-412E-923B-6B4E268F31AA` |
| `{arg\|format=X}` | `c2ce1f3b-17e5-412e-923b-6b4e268f31aa` | `{0xc2ce1f3b,0x17e5,0x412e,{0x92,0x3b,0x6b,0x4e,0x26,0x8f,0x31,0xaa}}` |
| `{arg\|format=X\|case=upper}` | `c2ce1f3b-17e5-412e-923b-6b4e268f31aa` | `{0XC2CE1F3B,0X17E5,0X412E,{0X92,0X3B,0X6B,0X4E,0X26,0X8F,0X31,0XAA}}` |


``` csharp
Specification<Guid> specification = s => s
    .NotEqualTo(new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"))
    .WithMessage("Must not be equal to: {value|format=X|case=upper}");

var validator = Validator.Factory.Create(specification);

validator.Validate(new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa")).ToString();
// Must not be equal to: {0XC2CE1F3B,0X17E5,0X412E,{0X92,0X3B,0X6B,0X4E,0X26,0X8F,0X31,0XAA}}
```

### Number argument

- Types: `int`, `uint`, `short`, `ushort`, `long`, `ulong`, `byte`, `sbyte`, `decimal`, `double`, `float`
- Created with `Arg.Number("name", value)`.
- Parameters:
  - `format` - guid format, the string that goes to the related [ToString](https://docs.microsoft.com/en-us/dotnet/api/system.int32.tostring?view=netstandard-2.0) method.
  - `culture` - culture code, the string that goes to the [CultureInfo.GetCultureInfo](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.getcultureinfo?view=netstandard-2.0#System_Globalization_CultureInfo_GetCultureInfo_System_String_) method.
    - If not set the default culture passed to ToString method is `CultureInfo.InvariantCulture`


| Placeholder | Argument | Final form |
| - | - | - |
| `{arg}` | `123.987` | `123.987` |
| `{arg\|format=X}` | `123` | `7B` |
| `{arg\|format=0.00}` | `123.987` | `123.99` |
| `{arg\|culture=pl-PL}` | `123.987` | `123,987` |
| `{arg\|format=0.00\|culture=pl-PL}` | `123.987` | `123,99` |

``` csharp
Specification<decimal> specification = s => s
    .EqualTo(666.666M)
    .WithMessage("Needs to be equal to {value|format=0.0|culture=pl-PL}");

var validator = Validator.Factory.Create(specification);

validator.Validate(10).ToString();
// Needs to be equal to 666,7
```

### Text argument

- Types: `string`, `char`
- Created with `Arg.Text("name", value)`.
- Parameters:
  - `case`
    - available values: `upper`, `lower`.
    - calls `ToUpper` or `ToLower` method on the stringified guid value.
    - if not set, the value stays as it is


| Placeholder | Argument | Final form |
| - | - | - |
| `{arg}` | `Bart` | `Bart` |
| `{arg\|case=upper}` | `Bart` | `BART` |
| `{arg\|case=lower}` | `Bart` | `bart` |

``` csharp
Specification<string> gmailSpecification = s => s
    .EndsWith("@gmail.com")
    .WithMessage("Must ends with: {value|case=upper}");

var validator = Validator.Factory.Create(gmailSpecification);

validator.Validate("john.doe@outlook.com").ToString();
// Must ends with: @GMAIL.COM
```

### Time argument

- Types: `DateTime`, `DateTimeOffset`, `TimeSpan`
- Parameters:
  - `format` - guid format, the string that goes to the related [ToString](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tostring?view=netstandard-2.0) method.
  - The default time format: `HH:mm:ss.FFFFFFF`
  - The default date format: `yyyy-MM-dd`
  - The default date and time format: `HH:mm:ss.FFFFFFF yyyy-MM-dd`
  - `culture` - culture code, the string that goes to the [CultureInfo.GetCultureInfo](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.getcultureinfo?view=netstandard-2.0#System_Globalization_CultureInfo_GetCultureInfo_System_String_) method.
    - If not set the default culture passed to ToString method is `CultureInfo.InvariantCulture`

| Placeholder | Argument | Final form |
| - | - | - |
| `{arg}` | `new DateTime(2000, 01, 15, 16, 04, 05, 06)` | `2000-01-15 16:04:05.006` |
| `{arg\|case=upper}` | `new DateTime(2000, 01, 15, 16, 04, 05, 06)` | `2000-01-15T16:04:05` |
| `{arg\|case=lower}` | `new DateTime(2000, 01, 15, 16, 04, 05, 06)` | `20000115` |

``` csharp
Specification<DateTime> specification = s => s
    .Before(new DateTime(2000, 1, 2, 3, 4, 5, 6))
    .WithMessage("Must not be before: {max|format=yyyy MM dd + HH:mm}");

var validator = Validator.Factory.Create(specification);

validator.Validate(new DateTime(2001, 1, 1, 1, 1, 1, 1)).ToString();
// Must not be before: 2000 01 02 + 03:04
```

### Translation argument

- Translation argument allows to include a phrase from the current translation.
- It's always in this form:
  - `{_translation|key=MessageKey}`

``` csharp
Specification<int> specification = s => s
    .NotEqualTo(666)
    .WithMessage("!!! {_translation|key=TripleSix} !!!");

var validator = Validator.Factory.Create(specification, settings => settings
    .WithTranslation("English", "TripleSix", "six six six")
    .WithTranslation("Polish", "TripleSix", "sześć sześć sześć")
);

var result = validator.Validate(666);

result.ToString(translationName: "English");
// !!! six six six !!!

result.ToString(translationName: "Polish");
// !!! sześć sześć sześć !!!
```

### Type argument

- Types: `Type`
- Created with `Arg.Type("name", value)`.
- Parameters:
  - `format`
    - available values: `name`, `fullName`, `toString`.
    - `name` - gets the type name, generics are nicely resolved.
    - `fullName` - gets the full type name, generics are nicely resolved.
    - `toString` - calls [ToString()](https://docs.microsoft.com/en-us/dotnet/api/system.type.tostring?view=netstandard-2.0).
    - if not sent, the default `format` value is `name`.
  - `translation` - if set to `true`, placeholder is transformed into [translation argument](#translation-argument): `{_translation|key=messageKey}`.
    - the message key is in this format: `Type.FullName`.
    - ultimately, placeholder will be replaced with text from the specification.
    - if `translation` is present, `format` is ignored.


| Placeholder | Argument | Final form |
| - | - | - |
| `{arg}` | `typeof(int)` | `Int32` |
| `{arg\|format=name}` | `typeof(int)` | `Int32` |
| `{arg\|format=fullName}` | `typeof(int)` | `System.Int32` |
| `{arg\|format=toString}` | `typeof(int)` | `System.Int32` |
| `{arg}` | `typeof(int?)` | `Nulllable<Int32>` |
| `{arg\|format=name}` | `typeof(int?)` | `Nulllable<Int32>` |
| `{arg\|format=fullName}` | `typeof(int?)` | `System.Nulllable<System.Int32>` |
| `{arg\|format=toString}` | `typeof(int?)` | `System.Nullable'1[System.Int32]` |
| `{arg\|translation=true}` | `typeof(int?)` | `{_translation\|key=Type.System.Nullable<System.Int32>}` |

### Path argument

- Path argument allows to include the [path](#path) of the validated value.
- It's always in this form:
  - `{_path}`
- It's more difficult to cache such messages (they are less deterministic), so overusing path arguments might slightly decrease the performance.
- It doesn't contain parameters.

``` csharp
Specification<decimal> specification = s => s
    .Positive()
    .WithPath("Number.Value")
    .WithMessage("Number value under {_path} needs to be positive!");

var validator = Validator.Factory.Create(specification);

var result = validator.Validate(-1);

result.ToString();
// Number.Value: Number value under Number.Value needs to be positive!
```

- In the case of the root path, the value is just an empty string.
  - And it might look weird in the final printing.

``` csharp
Specification<decimal> specification = s => s
    .Positive()
    .WithMessage("Number value under {_path} needs to be positive!");

var validator = Validator.Factory.Create(specification);

var result = validator.Validate(-1);

result.ToString();
// Number value under  needs to be positive!
```

### Name argument

- Name argument allows to include the name of the validated value.
  - Name is the last segment of the [path](#path).
- Parameters:
  - `format`
    - available values: `titleCase`.

| Placeholder | Path | Final form |
| - | - | - |
| `{_name}` | `someWeirdName123` | `someWeirdName123` |
| `{_name|format=titleCase}` | `someWeirdName123` | `Some Weird Name 123` |
| `{_name}` | `nested.path.someWeirdName123` | `someWeirdName123` |
| `{_name|format=titleCase}` | `nested.path.someWeirdName123` | `Some Weird Name 123` |
| `{_name}` | `path.This_is_a_Test_of_Network123_in_12_days` | `path.This_is_a_Test_of_Network123_in_12_days` |
| `{_name|format=titleCase}` | `path.This_is_a_Test_of_Network123_in_12_days` | `This Is A Test Of Network 123 In 12 Days` |

- It's more difficult (and sometimes it's even impossible) to cache such messages (they are less deterministic), so overusing name arguments might slightly decrease the performance.

``` csharp
Specification<decimal> specification = s => s
    .Positive()
    .WithPath("Number.Primary.SuperValue")
    .WithMessage("The {_name} needs to be positive!");

var validator = Validator.Factory.Create(specification);

var result = validator.Validate(-1);

result.ToString();
// Number.Primary.SuperValue: The SuperValue needs to be positive!
```

- Use `{_name|format=titleCase}` to get the name title cased.

``` csharp
Specification<decimal> specification = s => s
    .Positive()
    .WithPath("Number.Primary.SuperDuperValue123")
    .WithMessage("The {_name|format=titleCase} needs to be positive!");

var validator = Validator.Factory.Create(specification);

var result = validator.Validate(-1);

result.ToString();
// Number.Primary.SuperDuperValue123: The Super Duper Value 123 needs to be positive!
```

- Similarly to [path argument](#path-argument), in case of the root path, the value is just empty string.

``` csharp
Specification<decimal> specification = s => s
    .Positive()
    .WithMessage("The {_name} needs to be positive!");

var validator = Validator.Factory.Create(specification);

var result = validator.Validate(-1);

result.ToString();
// The  needs to be positive!
```

## Translations

- From the purely technical perspective, messages used in the specification are not the error messages, but only the message keys.
  - It means that using [WithMessage](#withmessage), [WithExtraMessage](#withextramessage) and [RuleTemplate](#ruletemplate), you're setting the message key.
  - This also covers all of the default messages like the one if the required value is null.
- The [validation result](#result) uses the translation process before returning the messages through its methods (e.g. [MessageMap](#messagemap) or [ToString](#tostring)).
- The translation process step by step:
  - Get the translation dictionary using its name.
  - Look for the message key in the translation dictionary.
    - If the message key is present, return the value under the message key.
    - If the message key is not present, return the message key.

``` csharp
Specification<string> specification = s => s
    .Rule(m => m.Contains("@")).WithMessage("Must contain @ character");

var validator = Validator.Factory.Create(specification);

validator.Validate("").ToString();
// Must contain @ character
```

_In the above code, [WithMessage](#withmessage) sets `"Must contain @ character"` message key for [Rule](#rule). However, there is no such message key in the standard, default `English` translation, so [ToString](#tostring) prints the original message key._

- Translation dictionary can be populated using [WithTranslation](#withtranslation) method of the [settings](#settings) object.

``` csharp
Specification<string> specification = s => s
    .Rule(m => m.Contains("@")).WithMessage("Must contain @ character");

var validator = Validator.Factory.Create(specification, settings => settings
    .WithTranslation("Polish", "Must contain @ character", "Musi zawierać znak: @")
    .WithTranslation("English", "Must contain @ character", "Must contain character: @")
);

var result = validator.Validate(model);

result.ToString();
// Must contain character: @

result.ToString("Polish");
// Musi zawierać znak: @
```

_In the above code, [WithMessage](#withmessage) sets `"Must contain @ character"` message key for [Rule](#rule). But this time, `"Must contain @ character"` key exists in both `Polish` and `English` dictionary (thanks to the [WithTranslation](#withtranslation) method). So the final validation [result](#result) contains phrases from the dictionaries, not from the [WithMessage](#withmessage)._

- Good to read:
  - [WithMessage](#withmessage), [WithExtraMessage](#withextramessage) and [RuleTemplate](#ruletemplate) - commands that set message keys.
  - [WithTranslation](#withtranslation) - setting entries in the translation dictionary.
  - [Message arguments](#message-arguments) - about message arguments (yes, translation phrases can use them!).

### Overriding messages

- Overriding the default error messages follows the process described in the main [Translations](#translations) section.
- The only missing bit of information is; what are the message key of the default messages?
  - And the answer is; there are all listed in [Rules](#rules) section.
- If you want to override some default error message, find it in the [Rules](#rules) section and provide the new value for it using [WithTranslation](#withtranslation).

``` csharp
Specification<string> specification = s => s
    .NotEmpty();

var validator = Validator.Factory.Create(specification, settings => settings
    .WithTranslation("English", "Global.Required", "String cannot be null!")
    .WithTranslation("English", "Texts.NotEmpty", "String cannot be empty!")
);

validator.Validate(null).ToString();
// String cannot be null!

validator.Validate("").ToString();
// String cannot be empty!
```

_Above code presents how to override the default error messages of `NotEmpty` - according to the [Rules](#rules) section, it uses `Texts.NotEmpty` message key._

- Translation phrases can use [message arguments](#message-arguments).
  - Similarly to message keys, arguments along with their types are listed in the [Rules](#rules) section of this doc.


``` csharp
Specification<decimal> specification = s => s
    .BetweenOrEqualTo(16.66M, 666.666M);

var validator = Validator.Factory.Create(specification, settings => settings
    .WithTranslation(
        "English",
        "Numbers.BetweenOrEqualTo",
        "Only numbers between {min|format=000.0000} and {max|format=000.0000} are valid!")
);

validator.Validate(10).ToString();
// Only numbers between 016.6600 and 666.6660 are valid!
```

_`BetweenOrEqualTo` uses message key `Numbers.BetweenOrEqualTo` and two [number arguments](#number-argument): `min` and `max`._


### Custom translation

- Custom translation is nothing more than a translation dictionary that delivers phrases for all the default message keys.
  - `English` translation is the default one, always present in the validator, and it contains all of the phrases.
  - The code to copy-paste and adjust;
    - [EnglishTranslation.cs](../src/Validot/Translations/English/EnglishTranslation.cs) - dictionary with all the message keys.
    - [EnglishTranslationsExtensions.cs](../src/Validot/Translations/English/EnglishTranslationsExtensions.cs) - extension method.
- The pattern is to create extension method to the [settings object](#settings) that wraps [WithTranslation](#withtranslation) calls, delivering phrases for all of the [rules](#rules).

``` csharp
public static class WithYodaEnglishExtension
{
    public static ValidatorSettings WithYodaEnglish(this ValidatorSettings @this)
    {
        var dictionary = new Dictionary<string, string>()
        {
            ["Global.Required"] = "Exist, it must.",

            // more phrases ...

            ["Numbers.LessThan"] = "Greater than {max}, the number must, be not."

            // more phrases ...
        };

        return @this.WithTranslation("YodaEnglish", dictionary);
    }
}
```

_Above, the extension that applies the translation dictionary using [WithTranslation](#withtranslation). Below, the example of usage:_

``` csharp
Specification<int?> specification = s => s
    .LessThan(10);

var validator = Validator.Factory.Create(specification, settings => settings
    .WithYodaEnglish()
);

validator.Validate(null).ToString("YodaEnglish");
// Exist, it must.

validator.Validate(20).ToString("YodaEnglish");
// Greater than 10, the number must, be not.
```

- Good to read:
  - [Translations](#translations) - how translations works.
  - [Overriding messages](#overriding-messages) - how to override messages.
  - [Rules](#rules) - list of message keys and arguments.
  - [WithTranslation](#withtranslation) - how to set translation phrase.

## Development

- The build system is based on the [nuke.build](https://nuke.build/) project.
- This section contains examples that uses powershell, but bash scripts are also fully supported.
  - Just replace `pwsh build.ps1` with `bash build.sh`
- If you're keep experiencing compilation errors that your IDE doesn't show (and at the same time `dotnet build` completes OK), consider adding `--AllowWarnings`.
  - By default, the build system requires the code to follow the rules set in [editorconfig](../.editorconfig).
- If you don't provide `--Version` parameter (value needs to follow [semver](https://semver.org/) rules), the default version is `0.0.0-XHHmmss`, where `X` is the day of the current year, `HHmmss` is the timestamp.

### Build

- Compile the project with the tests:
  - `pwsh build.ps1`
    - This is the same as `pwsh build.ps1 --target Compile`
- Create nuget package:
  - `pwsh build.ps1 --target NugetPackage --Version A.B.C --Configuration Release`
  - Replace `A.B.C` with the semver-compatible version number.
  - The nuget package version will be `A.B.C`.
  - `AssemblyVersion` will be `A.0.0.0`.
  - `AssemblyFileVersion` will be `A.B.C.0`.
  - The package appears in `artifacts/nuget` directory.
- Clean the project:
  - `pwsh build.ps1 --target Clean`
  - Deletes all of the `bin` and `obj` directories in the solution.
- Reset everything.
  - `pwsh build.ps1 --target Reset`
  - Restores the original `TargetFramework` in the test projects.
  - Deletes all diretories created by the build project (`tools`, `artifacts`, etc.).
  - Also, triggers `Clean` target at the end.

### Tests

- Run tests:
  - `pwsh build.ps1 --target Tests`
  - The detailed result files (`junit` format) appear in `artifacts/tests` directory.
- Run tests on specific framework:
  - `pwsh build.ps1 --target Tests --DotNet netcoreapp2.1`
  - `pwsh build.ps1 --target Tests --DotNet net48`
  - It sets the `TargetFramework` in the test projects' csproj files.
  - You can use the framework id (`netcoreapp3.1`), as well as the sdk version (`3.1.100`)
    - the highest framework id version available in the sdk will be used.
- Get code coverage report:
  - `pwsh build.ps1 --target CodeCoverageReport`
  - HTML and JSON reports will appear in `artifacts/coverage_reports` directory.
  - During this task, the dotnet global tool `dotnet-reportgenerator-globaltool` is installed locally in `tools` directory.
  - Reports are tracking history!
    - The history data is in `artifacts/coverage_reports/_history` directory.
- Get code coverage data:
  - `pwsh build.ps1 --target CodeCoverage`
  - The opencover file will appear in `artifacts/coverage` directory.

### Benchmarks

- Run all benchmarks:
  - `pwsh build.ps1 --target Benchmarks`
  - It would take several minutes to complete the execution.
  - The results will appear in `artifacts/benchmarks` directory.
  - By default, the benchmarks are run as `short` jobs.
- Run all benchmarks, better:
  - `pwsh build.ps1 --target Benchmarks --FullBenchmark`
  - This mode doesn't set job to `short`.
  - It depends on your machine, but you can assume that it would finish in about 1-2 hours.
- Run benchmarks selectively:
  - `pwsh build.ps1 --target Benchmarks --BenchmarksFilter "X"`
  - `X` is the full name of the benchmark method: `namespace.typeName.methodName`.
    - Wildcards are accepted, so `pwsh build.ps1 --target Benchmarks --BenchmarksFilter "*NoErrors*"` would execute all methods inside [`NoErrorsBenchmark.cs`](../tests/Validot.Benchmarks/Comparisons/NoErrorsBenchmark.cs).
  - Can be combined with `--FullBenchmark`.
- Benchmarks are based on [benchmarkdotnet](https://benchmarkdotnet.org).