---
title: Validot's performance explained
date: 2020-10-07 06:00:00
---

## A three-layer cake

The road to achieving the performance goals kicked off elegantly - with a whiteboard. To visually grasp the problem, I wrote down all possible actions from the moment user defines the object's acceptable state until they can interpret the results. Consequently, it's easier to understand the validation flow, recognize usage patterns, categorize the activities, and notice relationships between them.

The first noteworthy distinction among the listed steps divides those that will be executed during each validation, and those triggered only once. Some kinds of information will inevitably depend on the validated model (_is the value correct?_), while the other can be precalculated and reused in the subsequent calls (_what is the error message?_). Furthermore, some actions are optional because why would the library care about preparing a human-readable report if the user only wants to review the `IsValid` bool flag ? Using [FluentValidation](https://fluentvalidation.net/){:target="_blank"} (a great lib, by the way), how often did you see the code similar to this one below?

``` csharp
// trigger fully-featured validation process
var validationResult = validator.Validate(input);

// and use just a fraction of the acquired information...
if (!validationResult.IsValid)
{
  // ...only to immediately terminate the execution (in one way or another)
  throw Exception();
}
```

Having the best performance level as the primary objective, Validot's API needed to be designed to avoid such situations easily. Under the hood, it came down to introducing lazy loading wherever possible, splitting the process into reasonably separate stages, and caching information more aggressively, even at the cost of fewer features. Soon enough, Validot became an excellent looking, three-layer cake.

## Building scheme

The first tier starts right in the validator's constructor and is all about the specification analysis. Validot has the fluent interface exposed to the end-user, but each validator internally stores the rules in its own format, called the scheme. Consider it as a compiled specification; created because it's faster to check a bool flag `IsOptional` than to verify whether the `Optional()` method has been executed within the chain.  It's the same set of rules and logic, but in the form of a cachable object that can be accessed and processed more efficiently.

An even more critical operation is the message construction. The scheme contains all error contents with the inline arguments resolved, for all given translations. What does it imply?

``` csharp
Specification<string> nameSpecification = s => s
  .LengthBetween(3, 10).WithMessage("Allowed length: {min}-{max}");

var nameValidator = Validator.Factory.Create(nameSpecification);
```

Initialized with the `nameSpecification` above, `nameValidator` contains the final form of the error message (`"Allowed length: 3 to 10"`) already generated and cached. Performance-wise this is a great approach; still, it also implicates a severe drawback (which, by the way, is the greatest sacrifice I made while designing Validot's internals) - the messages are fully deterministic. So, e.g., you can't include the analyzed value in the error message's content, simply because it's unknown at the time of message generation. Eventually, this could be implemented in the succeeding steps, but is certainly not coming in an initial couple of releases. The messages being pre-generated also make scenarios that include logic branches virtually impossible. Let's take a look at this code using FluentValidation:

``` csharp
// part of the FluentValidation's custom validator:
RuleFor(x => x.Name).Custom((name, context) => {
  if (name.Length > 10)
  {
    context.AddFailure("Max length is 10");
  }
  else if (name.Length < 4)
  {
    context.AddFailure("Min length is 4");
  }
});
```

In Validot, the logic is wrapped by the predicates that answer the simple question: is the value valid or not. You don't even get a context where you can check state, assign errors, or decide during validation, e.g., which message will end up in the final report. If asked to replicate the same flow using Validot, I would do this:

``` csharp
// part of the Validot's specification:
.Member(m => m.Name, m => m
  .Rule(name => name.Length > 10).WithMessage("Max length is 10")
  .Rule(name => name.Length < 4).WithMessage("Min length is 4")
)
```

Yet it is rather an exception to the rule, as it might be impracticable to handle even slightly more complicated cases. It's safe to consider Validot as the library of a somewhat different philosophy. Therefore - although it helps achieve similar goals - it isn't a full, one-to-one replacement to FluentValidation.

Now, after the validator is initialized, the phase of specification analysis is over. It's a one-time operation with an outcome that can be reused for all upcoming validations because the validator itself is immutable - once created, its internal state can't change. This simple fact also implicates a positive side-effect: more safety in a multi-thread environment.

## Validating objects

The second layer of the Validot cake covers error detection. Each `Validate` call creates a validation context that has access to the scheme. With this information, the context traverses through the object (using member selectors from the specification), executes the validation logic (using predicates from the specification), and assembles errors (already pre-generated, also from the specification), linked to their location on the model's map. Obviously, it's an individual process for each validated object, and as an action potentially executed zillion times during the validator's lifetime - it's governed by a completely different set of rules.

Principle number one: reduce the amount of work to the bare minimum. The validation context mustn't do anything redundant, so every operation that doesn't change the outcome is entirely skipped. It's easier to present the idea with an example:

``` csharp
Specification<string> nameSpecification = s => s
  .Rule(predicate1).WithMessage("Error1")
  .Rule(predicate2).WithMessage("Error2")
  .Rule(predicate3).WithMessage("Error3");

Specification<AuthorModel> authorSpecification = s => s
  .Member(m => m.FirstName, nameSpecification)
  .Member(m => m.LastName, nameSpecification).WithMessage("Invalid last name")

var authorValidator = Validator.Factory.Create(authorSpecification);
```

In the snippet above, `nameSpecification` wraps three rules, and it's used within `authorSpecification` to describe the valid state of `FirstName` and `LastName` members.  During the validation process, the context enters `FirstName` scope and verifies its value against all rules in `authorSpecification`.  However, the situation is different in the `LastName` scope. According to the scheme, its entire error output gets overridden (`WithMessage` command does it). The context knows this fact and avoids unnecessary work by executing the validation logic only until the first discrepancy. Ultimately, it doesn't matter how many and what kind of errors `nameSpecification` indicates. For `LastName` member, the outcome is always the same - `"Invalid last name"`.

Principle number two: avoid allocations. Sounds bold, so let's break it down. The fruit of the context's labor is a collection of detected errors assigned to the paths. According to the rule, the collection itself is not even initialized until the first error is found. At first sight, it might look like a micro-optimization, but this operation's scale could be massive. Imagine a validator registered as a singleton within the internal microservice that receives a million calls per day. A collection would be created for each request, even though 99.9% of the requests are correct. Sure, it comes at the price of nulls flying around plus null-checks everywhere, which obviously don't look good. But hey, how often do you debug a nuget package's source code? I consider it a tolerable problem for Validot's maintainers, while its users won't even notice it. What they will notice, though, is the significant performance boost comparing to the process that allocates objects never to be used.

Additionally, the validation context's final collection doesn't contain the errors' full content, but merely their integer identifiers. It does make more sense when you consider this example:

``` csharp
Specification<int> ageSpecification = s => s
  .Rule(predicate1).WithMessage("Error 1").WithExtraCode("ERR_1")
  .Rule(predicate2).WithMessage("Error 2").WithExtraCode("ERR_2")
  .Rule(predicate3).WithMessage("Error 3").WithExtraCode("ERR_3");

Specification<AuthorModel> authorSpecification = s => s
  .Member(m => m.CurrentAge, ageSpecification)
  .Member(m => m.DeclaredAge, ageSpecification)
  .Member(m => m.BecomingAuthorAge, ageSpecification)
```

Above, `ageSpecification` holds three rules with a potentially rich output containing codes and messages. Also, `authorSpecification` uses it for its three members. If the context were creating a full set of string messages at this stage, it would unquestionably end up duplicating a lot of data. Using identifiers, it can operate on a simple list of integers, and the final errors' content can easily be resolved later, on-demand.

Principle number three: to satisfy the previous two, the code can freely discard the SOLID principles. The most prominent example of such a weird postulate is at the end of the second phase when the validation context finishes its job. The information about detected errors is in the form of `Dictionary<string, List<int>>` (error identifiers, mapped the path where they occurred). Indeed more SOLIDish would be passing `IReadOnlyDictionary<string, IReadOnlyList<int>>` to the next Validot cake layer. Unfortunately, in the real world, that could mean casting and eventually - allocation. Hence, as long as the unSOLID code lives in Validot internals and doesn't leak to the public space, such style is justified.

## Serving results

The cake's final layer is about wrapping the validation context's output into an object that allows a quick look-up of the errors found. This tier shares the principles with the preceding one (long story short: don't do unnecessary work, don't allocate prematurely). Yet, this entire process can be considered a separate flow that starts when the validator creates the result of `Validate` method.

`IValidationResult` implementation has access to the scheme and the detected error collection (so the products of, respectively, the first and second phases). Unhappily, that makes the result object strongly coupled with the validator, and as such, it's not recommended to pass it outside of the scope of its creation. Alternatively, you should acquire the information you want and let the GC act immediately. Moving the `IValidationResult` object between your app's domains - or worse, caching it in any form - could make your code vulnerable to memory leaks. It is another sacrifice made on the altar of the performance gods. Why? The example scenario is in the [first section](#a-three-layer-cake), so let's analyze something more advanced:

``` csharp
// validate input
var validationResult = validator.Validate(input);

// if no errors, return success immediately
if (!validationResult.AnyErrors)
{
  return new SuccessResult();
}

// on EMAIL_ERROR code recorded, return appropriate result
if (validationResult.Codes.Contains("EMAIL_ERROR"))
{
  return new EmailErrorResult(input.Email);
}

// log the validation errors (the default language is English)
Logger.LogError(validationResult.ToString());

string userLanguage = GetCurrentUserLanguageName();

// create message for the frontend
return new ValidationFailureResult(validationResult.ToString(userLanguage));
```

As stated earlier, returning `validationResult` from the method could be risky, although using its members completely safe. Certainly, this strategy is not that intuitive (and perhaps inconvenient for some), but the reason is straightforward - the performance Validot can reach.

Let me explain: the result object delivers the final values on demand, using a lazy manner. The members are initialized and populated only during the first call (or access). It does mean that all potentially expensive operations, e.g., extracting all the error codes or acquiring messages in a particular language, are not performed until the user explicitly wants it. Another thing that sounds great in theory, but the trade-off here is that it works only if the `validationResult` has access to the validator's internals all the time.

## Little optimizations that make a big difference

In addition to the principles described in the above sections, Validot is interwoven with little micro-optimizations - both local and spreading across all the three cake layers. Many of them are tailored for the specific use case, like `IsValid` method, but there will be a separate article about it. Another one undoubtedly worth noticing is the success result being cached and shared for all the validations that report correctness.

What's the reason behind it? Well, once again, imagine having a microservice that uses Validot for the payloads incoming through its REST API. It's also a part of the internal network, so you can expect that most of the incoming calls will be correct. It happens that the result objects are immutable-ish (the core state doesn't change, but the values are lazy-loaded), so the one without errors is common and could quickly be returned in all such situations. What's more - the validation context doesn't even create the error collection if none detected. Consequently, for a valid model, Validot allocates only a single object - the validation context itself. Naturally, it also depends on the rules' logic, but overall, as a library, Validot is extremely careful when it comes to the managed heap utilization.

The [benchmarks below](#validot-vs-fluentvalidation) shows that this approach paid off. In the case when your microservice is public and - let's say - as much as 60% of the incoming traffic is faulty, even then Validot could be 2.5x faster (while consuming 8x less memory) than [FluentValidation](https://fluentvalidation.net/){:target="_blank"}. Results are even better in a not-that-impossible case of all traffic being correct.

## Validot vs FluentValidation

[FluentValidation](https://fluentvalidation.net/){:target="_blank"} by [Jeremy Skinner](https://twitter.com/JeremySkinner){:target="_blank"} is the gold standard in the dotnet world and a great, reliable, opensource, battle-tested library that is around for years. I don't believe the one could find a better reference point for the validation benchmarks.

For these test runs, I created a model containing all kinds of members. They are grouped into three sets, each containing 10k objects - the first one has errors in all items, the second one - in circa 60% of them, and the third one is error-free. Validot's specification and FluentValidation's custom validator reflect each other as much as technically possible.

And this is how `Validate` performs in both libs:

| Set | Library | Mean [ms] | Allocated [MB] |
| - | - | -: | -: |
| 1st (100% errors) | FluentValidation | `774.69` | `747.66` |
| 1st (100% errors) | Validot | `331.70` | `183.19` |
| 2nd (~60% errors) | FluentValidation | `711.15` | `675.69` |
| 2nd (~60% errors) | Validot | `271.77` | `85.10` |
| 3rd (0% errors) | FluentValidation | `659.07` | `660.00` |
| 3rd (0% errors) | Validot | `242.92` | `78.82 ` |

<small>_Benchmarks environment: Validot 1.1.0, FluentValidation 9.2.0, .NET Core 3.1.7, i7-9750H (2.60GHz, 1 CPU, 12 logical and 6 physical cores), X64 RyuJIT, macOS Catalina, BenchmarkDotNet 0.12.1. You are very welcome to [run the benchmarks yourself](https://github.com/bartoszlenar/Validot/blob/main/docs/DOCUMENTATION.md#benchmarks){:target="_blank"} and [review their code](https://github.com/bartoszlenar/Validot/tree/main/tests/Validot.Benchmarks){:target="_blank"}_</small>

Please bear in mind that although Validot could be more performant, the trade-offs are limitations, different API, philosophy of work, and eventually - a smaller range of possibilities. Some scenarios are not possible yet - others never will be. True, Validot can't be considered a 100% replacement to FluentValidation (it's more like 90%), but I firmly believe it can handle most validation cases, including highly complex ones.

## Afterword

Validot is an open-source, MIT-licenced, fully tested, and documented project, hosted entirely on [github](https://github.com/bartoszlenar/Validot){:target="_blank"}.

Type

```
dotnet add package Validot
```

and give it a try in your next dotnet-based microservice.