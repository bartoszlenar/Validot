# Contributing

If you're reading this file, it means that you are - more or less - interested in contributing to the project. Before anything else, I'd like to say - thank you! It really means a lot to me that you consider this project to be worth your time.


## Flow

1. Let's have a discussion first!
   - It's really important that you visit the project's [Issues page](https://github.com/bartoszlenar/Validot/issues) and check if there is already an ongoing discussion around the same (or similar) idea.
   - Let's have a short chat about a feature that you have in mind (or a bug that you found), _before_ any actual code work.
2. Fork and code!
   - How to build, run unit and functional tests? How to analyse code coverage a execute benchmarks? It's all covered in the [documentation](./DOCUMENTATION.md#Development).
   - Please get familiar with Validot's [project principles](#project-principles), [git](#git) and [code standards](#code-standards)
   - Provide changes in the documentation if needed.
3. Raise the PR!

## Project principles

- Validot is not the ultimate solution to all validation scenarios and cases in the world. Let's keep it compact and simple, focused on a single problem.
- Validot should not have any other dependencies than .NET Standard 2.0.
- Validot - unless absolutely necessary and detaily discussed - should not sacrifice performance for extra features.
- Validot follows [semantic versioning](https://semver.org/) very strictly, no matter how annoying it could be.

## Code standards


- Be aware that the code needs to compile and pass the tests on all of the LTS versions of .NET, under all supported OSes.
  - The CI system will let you know if your PR fails.
- Please ensure that your code is covered with unit and functional tests.
  - Don't hesitate to ask - I'm more than happy to help you with everything!
- CI system verifies the code style as well.
- If your change is related with some core validation mechanism, please run the benchmarks to ensure it isn't affecting the performance.

## Git
- The commits should follow the pattern of short notes in the past tense:
  - `Added Hungarian translation`
  - `Fixed IsValid bug #XXX`
- Ideally, PR has a single commit with all changes, but that's not a requirement. As long as the each commit has logical sense and complies with all the rules - it compiles, passes tests, contains the related documentation changes - then it's fine.
