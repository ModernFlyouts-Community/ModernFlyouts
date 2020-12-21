# Contributing to ModernFlyouts

Thank you for showing your interest in contributing to **ModernFlyouts**!

You can contribute to **ModernFlyouts** by filing issues (which includes bug reports and feature requests) making pull requests (including code, docs and translations). Simply filing issues for the problems you encounter is a great way to contribute. Contributing code via PRs is greatly appreciated!

Below is our guidance for how to file bug reports, propose new features, and submit contributions via Pull Requests (PRs).

For contributors who are willing to translate the app to their language of interest see [this section on how to do it](#translation-contribution-guidelines).

Note that all community interactions must abide by the [Code of Conduct](CODE_OF_CONDUCT.md).

## Before you start, file an issue

We use GitHub issues to track bugs and features. It helps us to prioritize tasks and make our plan.

Please follow this simple rule to help us eliminate any unnecessary wasted effort & frustration, and ensure an efficient and effective use of everyone's time - yours, ours, and other community members':

> If you have a question, think you've discovered an issue, would like to propose a new feature, etc., then find/file an issue **BEFORE** starting work to fix/implement it.

### Search existing issues first

Before filing a new issue, search existing open and closed issues first: It is likely someone else has found the problem you're seeing, and someone may be working on or have already contributed a fix!

If no existing item describes your issue/feature, great - please file a new issue:

### File a new Issue

* Experienced a bug or crash? [File a bug report](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/new?labels=bug&template=bug_report.md&title=Bug%3A)
* Got a great idea for a new feature or have a suggestion? [File a feature request](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/new?labels=enhancement&template=feature_request.md&title=Feature+Request%3A)
* Want ModernFlyouts to support your language of interest? (or) Update/Correct existing translations? [File a Language request/translation issue](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/new?assignees=Samuel12321&labels=translation&template=language-or-translation-issue-or-request-.md&title=)
* Don't know whether you're reporting a bug or requesting a feature? [File an issue](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/new?assignees=&labels=&template=blank-issue.md&title=)
* Found an existing issue that describes yours? Great - upvote and add additional commentary / info / repro-steps / etc.
* Want to know if we're planning on building a particular feature? (or) Don't understand how to do something? (or) Have a question that you don't see answered in this repo? [Connect with us](https://github.com/ModernFlyouts-Community/ModernFlyouts#connect-with-us) and ask our team and the community to get your doubts cleared. (Please don't open issues just for questions or doubts)

We provide some basic issue templates. If none of them suit your need, you can start from a blank issue.

## Code Contribution guidelines

Before contributing any code via PRs to **ModernFlyouts**, please **file a new issue** regarding it and ask for the teams approval.
If you want to implement or fix any existing issues, please leave a comment on the issue notifying us about your will to contribute code.
We'll then give you some guidelines regarding the project and source code structure and stuffs. Now, let's get back on how to contribute code to **ModernFlyouts**.

### Code contribution process

- [Setup and build environment](docs/developer_guide.md)
- [Contribution Workflow](docs/contribution_workflow.md)

### Copying files from other projects

Don't steal code from others! Only we can do that. Just kidding.
The following rules must be followed for PRs that include files from another project:

- The license of the file is [permissive](https://en.wikipedia.org/wiki/Permissive_software_license).
- The license of the file is left intact.
- The contribution is correctly attributed in the [3rd party notices](NOTICE.md) file in the repository, as needed.

## Translation Contribution guidelines

First of all, we must thank you for stepping forward to contribute support a language!
We know how time consuming and hard it is to translate the text resources used by this app. Unfortunately in the past we had to ask some contributors to redo all their work from scratch due to some technical difficulties on our side. That's why we want you to know these pre-cautions to be taken before contributing translations.

**Read these docs first:** [Setup and build environment](docs/developer_guide.md) and [Contribution Workflow](docs/contribution_workflow.md).

Please remember these points! We have suffered enough pain telling contributors to redo their work.

- **(!!!Important!!!)** The files you need to translate are present in the [ModernFlyouts/MultilingualResources](ModernFlyouts/MultilingualResources) directory.
- The **RESX files** (i.e. **Strings.(lang-code).resx** ) present inside the [ModernFlyouts/Properties](ModernFlyouts/Properties) directory are intended for internal use and are modified every time the project is built.
- Only the **ModernFlyouts.(lang-code).xlf** files present inside the [ModernFlyouts/MultilingualResources](ModernFlyouts/MultilingualResources) directory are stable and intended to be translated by contributors.
- Please don't modify any files other than file related to your target language! Which is **ModernFlyouts.(lang-code).xlf** for e.g. **ModernFlyouts.en-GB.xlf**.
