---
title: Crafting model specifications using Validot
date: 2022-09-26 06:00:00
---

The Validot project comes with comprehensive documentation, excellent for looking up how certain features work. On the other hand, documentations don't always serve as great tutorials or walkthrough guides, which in Validot is very much the case. This blog post aspires to answer this problem. Following the real-life use case, we'll proceed step-by-step with creating a fully-featured specification using Validot's advanced fluent-driven interface.

## Specification

Imagine having a web form that allows users to sign up. There is a text field for the one's name that's supposed to be validated. We'll start with the bare minimum form of Validot's specification:

``` csharp
Specification<string> nameSpecification = s => s;
```

`Specification<T>` describes a valid state of `T` instances. Struct or class, custom type, or something from the framework's built-in namespace - `T` has no constraints and could be virtually anything you need it to be. Above, `Specification<string>` describes a valid string object and the assigned value (`s => s`) is a fluent API expression builder, a method chain that, in this particular case, stands empty.

Methods that are part of the chain are called _commands_. By default, Validot requires the value to be non-null, so even having no explicitly defined commands, the specification is correct and says the following: "there are no rules for a string value, but the value itself needs to be non-null".

An empty specification can hardly play a role in a real-life scenario, so how about adding some logic:

``` csharp
Specification<string> nameSpecification = s => s
    .Rule(name => name.Length > 3);
```

`Rule` is an essential fluent API command because it allows defining a custom validation code. It takes the regular dotnet's `Predicate<T>` that receives the analyzed value and is supposed to return `true` if it's valid and `false` if otherwise. Having said that, the above snippet could easily be replaced by:

``` csharp
Predicate<string> mustHaveMoreThanThreeCharacters = text => text.Length > 3;

Specification<string> nameSpecification = s => s
    .Rule(mustHaveMoreThanThreeCharacters);
```

There is no sanctioned way of doing this. Sometimes the code might benefit from defining predicates separately (e.g., when you want to utilize one in multiple places), but in general - it's totally up to your personal preference.

## Validator

So, at this point, we have a specification that says "a string value must be present (non-null) and must have more than three characters". However, specification on its own is merely a definition of the valid state. Not much less than a bunch of instructions on distinguishing a valid entity from an invalid. Executing these instructions against objects is the validator's job. You can create one by calling the static factory `Validator.Factory.Create<T>`, where the generic argument `T` comes from the delivered `Specification<T>`, and then lands in the produced `Validator<T>`:

``` csharp
var validator = Validator.Factory.Create(nameSpecification);

validator.IsValid("Alice"); // true
validator.IsValid("Bob"); // false

var result = validator.Validate("Bob");

result.ToString();
// Invalid
```

That's right: `Validator<T>` can process only objects of type `T`, according to the single `Specitication<T>` it was created with (nothing stops you from creating multiple validators, though). In exchange, it offers immutability, thread-safety and exceptionally high performance of work, which it does using its two methods `IsValid<T>` and `Validate<T>`.

`IsValid` is the ultra-optimized way to make quick correctness checks. It's super-fast, allocates virtually nothing on the heap, but it has its price: `IsValid` delivers no information other than a simple boolean flag. So, if you don't care about the details and need to drive your app's logic according to only a binary validation result, using `IsValid` makes the most sense.

`Validate`, on the other hand, delivers a more comprehensive report. For now, we're interested in its overloaded `ToString()` method that prints all error messages (prefixed with their path) in separate lines. In the above snippet, we see only a single `"Invalid"`, the default error message for the `Rule` command. You can alter its content by placing another command - `WithMessage` - directly after the related `Rule`. Like this:

``` csharp
Specification<string> nameSpecification = s => s
    .Rule(name => name.Length > 3).WithMessage("Min length is 4 characters");
```

``` csharp
var validator = Validator.Factory.Create(nameSpecification);

validator.IsValid("Alice"); // true
validator.IsValid("Bob"); // false

validator.Validate("Bob").ToString();
// Min length is 4 characters
```

## Command types

In theory, a specification can consist of three types of command:

* scope commands - to wrap the validation logic (e.g., `Rule`, where we passed our predicate)
* parameter commands - to parametrize other commands (e.g., `WithMessage`, which we used to overwrite the `Rule`'s error message)
* presence commands - to set if the value is required or not (by default, it's always required, but of course, you can alter this behavior).

In practice, it's much simpler than it might look at first glance. In Validot, there are only two so-called presence commands (`Optional` and `Required`), and they can be placed only at the beginning of the chain. Therefore, if the specification doesn't start with `Optional()`, then it by default behaves like it would with `Required()`.

Secondly, all parameter commands start with `With…`, and they affect the closest preceding command that holds validation logic.

```
.RuleA().WithX(...).WithY(...)
.RuleB().WithZ(...)
```

In the above hypothetical example, `RuleA` is affected by `WithX` and `WithY`, and respectively - `RuleB` is affected by `WithZ`. There are quite a few parameter commands and the [documentation describes them](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md){:target="_blank"} very intensively. `WithMessage`, for instance, overwrites the entire output with a single message. Ultimately, that's what happened - `Rule'`s default error content got replaced with `"Min length is 4 characters"`.

## Chaining commands

We already know that specification can contain multiple scope commands, each with its set of aligned parameter commands. This is how it could be arranged in our example:

``` csharp
Specification<string> nameSpecification = s => s
    .Rule(name => name.Length > 3).WithMessage("Min length is 4 characters")
    .Rule(name => name.Length < 16).WithMessage("Max length is 15 characters")
    .Rule(name => name.Any() && char.IsUpperCase(name.First())).WithMessage("Must start with a capital letter");
```

When you look at it, it's a reasonably well-structured method chain: each line wrapping a single rule and all of its modifiers. But most code editors will try to break it according to the one-method-one-line style. To maintain readability, you might consider using `And()` to visually separate the subsequent scope commands. `And` command contains no logic and does nothing - its sole purpose is to break method chains into groups:

``` csharp
Specification<string> nameSpecification = s => s
    .Rule(name => name.Length > 3)
    .WithMessage("Min length is 4 characters")
    .And()
    .Rule(name => name.Length < 16)
    .WithMessage("Max length is 15 characters")
    .And()
    .Rule(name => name.Any() && char.IsUpperCase(name.First()))
    .WithMessage("Must start with a capital letter");
```

Both the above specifications are logically the same:

``` csharp
var validator = Validator.Factory.Create(nameSpecification);

validator.IsValid("Alice"); // true
validator.IsValid("alice"); // false

validator.Validate("bob").ToString();
// Min length is 4 characters
// Must start with a capital letter

validator.Validate("Elizabeth Alexandra Mary").ToString();
// Max length is 15 characters
```

Notice that the string value gets tested against all of the `Rule` commands, no matter how many declared failures. This is because an error doesn't stop the validation. Instead, all error outputs are collected and appear in the final report.

## Predefined rules

Validot is shipped with dozens of predefined rules, ready to be included within specifications. You can safely assume that they are single-commands combining `Rule` with a dedicated predicate, followed by `WithMessage` with an appropriate error description. Naturally, you'd still need to write custom predicates for more complex scenarios; however the most common cases are covered pretty well. The complete list is available [in the documentation](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md){:target="_blank"} and constantly gets extended with new releases.

We'll replace the first four commands in our example with the predefined `MinLength` and `MaxLength`:

``` csharp
Specification<string> nameSpecification = s => s
    .MinLength(4)
    .MaxLength(15)
    .And()
    .Rule(name => name.Any() && char.IsUpperCase(name.First()))
    .WithMessage("Must start with a capital letter");

var validator = Validator.Factory.Create(nameSpecification);

validator.Validate("bob").ToString();
// Must be at least 15 characters in length
// Must start with a capital letter

validator.Validate("Elizabeth Alexandra Mary").ToString();
// Must be at most 15 characters in length
```

If for whatever reason, you don't like the error message that comes with a predefined rule, you can always overwrite it by placing `WithMessage` directly after it in the fluent API method chain:

``` csharp
Specification<string> nameSpecification = s => s
    .MinLength(min: 4)
    .WithMessage("Min length is {min} characters")
    .And()
    .MaxLength(max: 15)
    .WithMessage("Max length is {max} characters")
    .And()
    .Rule(name => name.Any() && char.IsUpperCase(name.First()))
    .WithMessage("Must start with a capital letter");

var validator = Validator.Factory.Create(nameSpecification);

validator.Validate("bob").ToString();
// Min length is 4 characters
// Must start with a capital letter

validator.Validate("Elizabeth Alexandra Mary").ToString();
// Max length is 15 characters
```

In Validot, you can peek at the rule's argument name and use it inside the message in a placeholder wrapped with curly brackets. Or more figuratively speaking: the `min` parameter from `.MinLength(min: 4)` goes to `{min}` in the message, so effectively `.WithMessage("Min length is {min} characters")` prints out `"Min length is 4 characters"`.

What's even more interesting, placeholders support parameters, so if `min` is of numeric type , `.WithMessage("Min length is {min|format=000.000|culture=pl-PL} characters")` would output `"Min length is 004,000 characters"` in the above code snippet.

Message arguments are extensively developed, and the [documentation explains all aspects of the formatting](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#message-arguments){:target="_blank"}, plus contains [the complete list of built-in rules along with their parameters](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#rules){:target="_blank"}.

## Validating members

So far, we've been taking care of a simple, single string. In practice, though, you'll be facing multi-level objects with all types of properties: collections, nullables and enums, and - of course - nested models, which means combination of all of these options. Validot allows to easily nest specifications and gracefully handle scenarios containing such a real world's variety.

At first, we'll try to model the case in the first paragraph (about the user trying to sign up):

``` csharp
class Contact
{
    public string EmailAddress { get; set; }
    public bool SubscribedToNewsfeed { get; set; }
    public bool? TermsAndConditionsAccepted  { get; set; }
}

class User
{
    public string FirstName { get; set; }
    public IEnumerable<string> MiddleNames { get; set; }
    public string LastName { get; set; }
    public int? Age { get; set; }
    public Contact Contact { get; set; }
}
```

To validate a model's property, use the `Member` command with two arguments: the first is the expression selecting this property, and the second is the specification for it. Since we already have `nameSpecification` done, we can apply it to both `FirstName` and `LastName` members:

``` csharp
Specification<User> userSpecification = user => user
    .Member(m => m.FirstName, nameSpecification)
    .Member(m => m.LastName, nameSpecification);
```

`Member` opens a new nested scope (and that's why it's called a scope command) that works independently from its parent. All spotted errors will be saved with an additional information about the exact path where it occurred. For example, the result's `ToString()` printing will include it directly before each message. Like this:

``` csharp
var user = new User
{
    FirstName = "Elizabeth Alexandra Mary",
    LastName = "bob"
};

var result = validator.Validate(user);

result.ToString();
// FirstName: Min length is 4 characters
// FirstName: Must start with a capital letter
// LastName: Max length is 15 characters
```

The result contains also `MessageMap` dictionary that for each path holds the collection of errors, so you can examine them independently, one by one:

``` csharp
var lastNameErrors = result.MessageMap["FirstName"];

// lastNameErrors.Length == 2
// lastNameErrors[0] == "Min length is 4 characters"
// lastNameErrors[1] == "Must start with a capital letter"
```

You can also pass an inline specification to the `Member` command, as well as introduce more and more nested levels according to your needs and the model's structure.

``` csharp
Specification<User> userSpecification = user => user
    .Member(u => u.FirstName, nameSpecification)
    .Member(u => u.LastName, nameSpecification)
    .And()
    .Member(u => u.Contact, u => u
        .Member(c => c.EmailAddress, c => c
            .Email()
            .WithMessage("Email is very invalid")
            .And()
            .MaxLength(30)
        )
    );
```

``` csharp
var user = new User
{
    FirstName = "Elizabeth",
    LastName = "Smith",
    Contact = new Contact()
    {
        EmailAddress = "invalid_email"
    }
};

var result = validator.Validate(user);

result.ToString();
// Contact.EmailAddress: Email is very invalid
```

Paths use dot as a separator between the property names:

``` csharp
result.MessageMap["Contact.EmailAddress"].Length
// 1

result.MessageMap["Contact.EmailAddress"].Single() == "Email is very invalid"
// true
```

Because `MessageMap` is a regular dotnet's dictionary, you can verify a specific property easily with the built-in `ContainsKey` method:

``` csharp
result.MessageMap.ContainsKey("Contact.EmailAddress")
// true
```

## Validating collections

Although `Member` can handle all types of nested structures, collections are a bit tricky. For instance, `Member` requires a fixed path to a specific object, but you can't possibly know how many items the collection has when constructing specification. Of course, you could receive the entire collection in the `Rule`'s predicate, but how to apply another specification for each item? And how would the path of their eventual error messages look like? Validot comes to the rescue with `AsCollection` command, available for all objects that derive from `IEnumerable<T>`.

``` csharp
Specification<IEnumerable<string>> middleNamesCollectionSpec = m => m
    .AsCollection(nameSpecification);
```

Similarly to `Member`, `AsCollection` is a scope command, which means that an independent context (scope) is opened for the nested objects. Only this time, there are multiple scopes, because `AsCollection` applies the delivered specification to the all items acquired - one by one - from the enumerator. The path for their error output is `#n`, where `n` is the index under which they have been yielded to validation.

``` csharp
var validator = Validator.Factory.Create(middleNamesCollectionSpec);

var middleNames = new[] { "bob", "Elizabeth Alexandra Mary" };

validator.Validate(middleNames).ToString();
// #0: Min length is 4 characters
// #0: Must start with a capital letter
// #1: Max length is 15 characters
```

Naturally, next to `AsCollection` you can place other custom and [built-in rules](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#collections-rules) to validate the collection as a whole. For example, `MaxCollectionSize` to specify the maximum number of objects it could have.

``` csharp
Specification<User> userMiddleNamesSpecification = user => user
    .Member(u => u.MiddleNames, names => names
    	.AsCollection(nameSpecification)
	.And()
	.MaxCollectionSize(3)
	.WithMessage("Maximum three middle names are allowed")
    );

var user = new User
{
    MiddleNames = new [] { "bob", "Alexander", "patrick", "Al" };
}

var validator = Validator.Factory.Create(userMiddleNamesSpecification);

var result = validator.Validate(user);

result.ToString();
// MiddleNames.#0: Min length is 4 characters
// MiddleNames.#0: Must start with a capital letter
// MiddleNames.#2: Must start with a capital letter
// MiddleNames.#3: Min length is 4 characters
// MiddleNames: Maximum three middle names are allowed
```


## Validating nullables

Nullables may not be as tricky as collections; still, it would be handy to have the underlying value unwrapped and validated effortlessly. And that's the purpose of yet another scope command - `AsNullable`.

In our example, `User` contains the `Age` property of type `Nullable<T>` (`int?`). Using `AsNullable` we can write the specification for just `int` and apply it directly to `Age`.

``` csharp
Specification<int> ageSpecification = age => age
    .GreaterThanOrEqualTo(0).WithMessage("Age must be 0 or more")
    .LessThan(129).WithMessage("People don't live that long");

Specification<User> userAgeSpecification = user => user
    .Member(u => u.Age, age => age.AsNullable(ageSpecification));
```

Technically speaking, `AsNullable` unwraps the `T` value out from `Nullable<T>` and validates it according to the given `Specification<T>`.

In contrast to `AsCollection` and `Member`, `AsNullable` saves the error output under the same path. Therefore, you shouldn't expect `Age.Value` path in the results, but just `Age`.


``` csharp
var result = validator.Validate(new User { Age = 140; });

result.ToString();
// Age: People don't live that long
```

The above code presents usage of `AsNullable`, however in this particular case you can achieve the same results without it. In our example, `ageSpecification` uses only built-in rules, without any extra custom logic. And all built-in rules for the value types Validot delivers in both standard and nullable version. Therefore, `GreaterThanOrEqualTo` and `LessThan` could be applied directly, like this:

``` csharp
Specification<User> userAgeSpecification = user => user
    .Member(u => u.Age, age => age
        .GreaterThanOrEqualTo(0).WithMessage("Age must be 0 or more")
        .LessThan(129).WithMessage("People don't live that long")
);
```

Of course, in other cases (e.g., custom logic and reusing already prepared specifications), `AsNullable` makes your life easier.

## Making a value optional

The last command type is called a presence command and includes commands like `Required()` and `Optional()`. Their names are pretty self-explanatory, but just to have it on paper: their sole purpose is to define whether null is an acceptable case or an error.

As previously mentioned, in Validot everything is required to be non-null by default, so it doesn't matter whether you start with `Required()` or not. These two specifications below are equal to each other:

``` csharp
Specification<User> userAgeSpecification1 = user => user
    .Member(u => u.Age, age => age
        .AsNullable(ageSpecification)
    );

Specification<User> userAgeSpecification2 = user => user
    .Member(u => u.Age, age => age
        .Required()
        .AsNullable(ageSpecification)
    );

var validator1 = Validator.Factory.Create(userAgeSpecification1);
var validator2 = Validator.Factory.Create(userAgeSpecification1);

validator1.Validate(new User { Age = null }).ToString();
// Age: Required

validator2.Validate(new User { Age = null }).ToString();
// Age: Required
```

However, explicitly using the `Required` command in the specification lets you define a custom error output in case of null.

``` csharp
Specification<User> userAgeSpecification = user => user
    .Member(u => u.Age, age => age
        .Required()
        .WithMessage("Information about age is mandatory for all user profiles")
        .And()
        .AsNullable(ageSpecification)
);

var validator = Validator.Factory.Create(userAgeSpecification);

validator.Validate(new User { Age = null }).ToString();
// Age: Information about age is mandatory for all user profiles
```

The opposition to `Required` is - of course - `Optional` and it makes a no-value acceptable. In case of null nothing happens (no error is recorded) and validation proceeds with further commands.

``` csharp
Specification<User> optionalUserAgeSpecification = user => user
    .Member(u => u.Age, a => a
        .Optional()
        .AsNullable(ageSpecification)
    );

var validator = Validator.Factory.Create(optionalUserAgeSpecification);

validator.Validate(new User { Age = null }).AnyErrors; // false

validator.Validate(new User { Age = 18 }).AnyErrors; // false

validator.Validate(new User { Age = 188 }).ToString();
// Age: People don't live that long
```

## Merging and extending specifications

So far in this post, we've created quite a few specifications for the `User` class while describing different types of Validot commands. The first one, `userSpecification`, shows how to validate members, but there have also been `userMiddleNamesSpecification` for collections and `optionalUserAgeSpecification` for the optional nullable values.

Is there a way to glue them together so we don't need to duplicate the code? There is. In theory, `AsModel` is the command that applies the delivered specification directly to the current value. Effectively, it allows the user to merge multiple specifications into one:

``` csharp
Specification<User> finalUserSpecification = s => s
    .AsModel(userSpecification)
    .AsModel(userMiddleNamesSpecification)
    .AsModel(optionalUserAgeSpecification);
```

The presented above `finalUserSpecification` works the same as it had all commands from `userSpecification`, `userMiddleNamesSpecification`, and `optionalUserAgeSpecification` copy-pasted one after another. Let's present it:

``` csharp
var user = new User
{
    MiddleNames = new [] { "bob", "Alexander", "patrick", "Al" };
    LastName = "smith",
    Contact = new Contact()
    {
        EmailAddress = "invalid_email"
    },
    Age = 200,
};

var validator = Validator.Factory.Create(finalUserSpecification);

validator.Validate(user).ToString();
// FirstName: Required
// MiddleNames.#0: Min length is 4 characters
// MiddleNames.#0: Must start with a capital letter
// MiddleNames.#2: Must start with a capital letter
// MiddleNames.#3: Min length is 4 characters
// MiddleNames: Maximum three middle names are allowed
// LastName: Must start with a capital letter
// Contact.EmailAddress: Email is very invalid
// Age: People don't live that long
```

Naturally, `AsModel` is a regular Validot command, so you can follow it with another, extending the specification with more and more logic.

``` csharp
Specification<User> finalUserSpecification = s => s
    .AsModel(userSpecification)
    .AsModel(userMiddleNamesSpecification)
    .AsModel(optionalUserAgeSpecification)
    .Rule(m => m.FirstName != m.LastName).WithMessage("First and last name must be different!");
```

``` csharp
var user = new User
{
    FirstName = "Michael",
    LastName = "Michael",
};

var validator = Validator.Factory.Create(finalUserSpecification);

validator.Validate(user).ToString();
// Contact: Required
// MiddleNames: Required
// First and last name must be different!
```

## Overwriting error output

Imagine a specification that produces very extensive and comprehensive report with lot of potential errors. We want to reuse it to validate collection of objects, but we don't care that much about the details, as with large set of items they are nothing but a noise.

Validot allows error output overwriting and in fact, we've been using it across all of this post's code snippets. It's `WithMessage`!

Let's look back at the first example it appeared:

``` csharp
Specification<string> nameSpecification = s => s
    .Rule(name => name.Length > 3).WithMessage("Min length is 4 characters");
```

`Rule` has its default behavior and returns `"Invalid"` message, but we overwrote it with a custom one. `WithMessage`, as all parameter commands, affect the closest preceding scope command. In the above example, it's `Rule`, but it could be anything.

What happens if `AsCollection` is followed by `WithMessage` in the specification handling the user middle names?

``` csharp
Specification<User> userMiddleNamesSpecification = user => user
    .Member(u => u.MiddleNames, names => names
    	.AsCollection(nameSpecification)
	.WithMessage("Contains invalid name")
	.And()
	.MaxCollectionSize(3)
	.WithMessage("Maximum three middle names are allowed")
    );
```

``` csharp
var user = new User
{
    MiddleNames = new [] { "bob", "Alexander", "patrick", "Al" };
};

var validator = Validator.Factory.Create(finalUserSpecification);

validator.Validate(user).ToString();
// MiddleNames: Contains invalid name
// MiddleNames: Maximum three middle names are allowed
```

No matter how many invalid values are detected in `MiddleNames` collection, the output from `AsCollection` contains only a single message. This trick works on all scope commands, including `AsModel`:

``` csharp
Specification<User> finalUserSpecification = s => s
    .AsModel(userSpecification)
    .AsModel(userMiddleNamesSpecification).WithMessage("Invalid collection of middle names")
    .AsModel(optionalUserAgeSpecification)
    .Rule(m => m.FirstName != m.LastName).WithMessage("First and last name must be different!");
```

``` csharp
var user = new User
{
    FirstName = "John",
    MiddleNames = new [] { "bob", "Alexander", "patrick", "Al" };
    LastName = "Smith",
    Contact = new Contact()
    {
        EmailAddress = "valid@address"
    },
};

var validator = Validator.Factory.Create(finalUserSpecification);

validator.Validate(user).ToString();
// Invalid collection of middle names
```

## Validot is so much more

The list of possibilities doesn't end here. After reading this post and getting familiar with the philosophy of crafting specifications, you can go on and explore the full set of features detaily described on the [official documentation](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md){:target="_blank"}. Just to give a quick overview of what more to expect; you can

- [append messages](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#withextramessage){:target="_blank"} to the existing error output instead of overwriting everything
- return [error codes](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#code){:target="_blank"} instead of just messages
- prepare [translations](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#translations){:target="_blank"} for the error messages (also there are [few translations](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#built-in-translations){:target="_blank"} available out of the box)
- [overwrite paths](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#withpath){:target="_blank"} of the error output
- [forbid](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#forbidden){:target="_blank"} the value to be present (set up null as the only valid case)
- [convert the value](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#asconverted){:target="_blank"} to something different (even of different type) before validating it further
- prepare your own [custom, reusable rules](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#custom-rules){:target="_blank"} with the same mechanism that the Validot's internal are using

And much, much more.

Validot is an open-source, MIT-licenced, fully tested, and documented project, hosted entirely on [github](https://github.com/bartoszlenar/Validot){:target="_blank"}.

Type

```
dotnet add package Validot
```

and give it a try in your next dotnet-based microservice.