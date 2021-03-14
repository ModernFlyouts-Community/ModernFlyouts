# Contribution Workflow

You can contribute to ModernFlyouts with issues and PRs. Simply filing issues for problems you encounter is a great way to contribute. Contributing implementations is greatly appreciated.

Please make sure you have some basic knowledge in git and GitHub workflow.

## Suggested Workflow

1. Create an issue for your work
    - Reuse an existing issue on the topic, if there is one.
    - Notify our team about your wills and intentions. You can start your work after getting our team's approval.
2. Create a personal fork of the repository on GitHub (if you don't already have one).
3. Create a branch off of `main` (also make sure to `checkout` it)
    - Name the branch so that it clearly communicates your intentions.
    - Branches are useful since they isolate your changes from incoming changes from upstream. They also enable you to create multiple PRs from the same fork.
4. Make, stage, review and then commit your changes. (Please follow the [Commit Messages](#commit-messages) guidelines)
5. Make sure your local cloned project build successfully and works properly the way you intended it to.
6. Create a pull request (PR) against the upstream repository's master branch.
    - Push your changes to your fork on GitHub (if you haven't already).
    - Note: It is okay for your PR to include a large number of commits. Once your change is accepted, we will squash your commits into one while your PR is being merged.
    - Note: It is okay to create your PR as "[WIP]" or **draft** on the upstream repo before the implementation is done. This can be useful if you'd like to start the feedback process while you're still working on the implementation. State that this is the case in the initial PR comment.

## DOs and DON'Ts

Please do:
* **DO** follow our coding style.
* **DO** give priority to the current style of the project or file you're changing even if it diverges from the general guidelines.
* **DO** keep the discussions focused. When a new or related topic comes up it's often better to create new issue than to side track the discussion.
* **DO** stage necessary changes, review them and then commit them with a proper commit message.

Please do not:
* **DON'T** surprise us with big pull requests. Instead, file an issue and start a discussion so we can agree on a direction before you invest a large amount of time.
* **DON'T** commit code that you didn't write (stealing code = bad. Only we can do that LOL).
* **DON'T** submit PRs that alter licensing related files or headers.
* **DON'T** add or change APIs or UI without filing an issue and discussing it first.
* **DON'T** commit un-necessary changes such as changes to the **ModernFlyouts.Package.wapproj** and the **Package.appxmanifest** files and changes that you did accidentally. Please stage, review and then commit changes.

## Commit Messages

Please format commit messages as follows (based on [A Note About Git Commit Messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)):

```
Summarize change in 50 characters or less
	
Provide more detail after the first line. Leave one blank line below the 
summary and wrap all lines at 72 characters or less.
	
If the change fixes an issue, leave another blank line after the final 
paragraph and indicate which issue is fixed in the specific format below.
	
Fix #42
```

Also do your best to factor commits appropriately, not too large with unrelated 
things in the same commit, and not too small with the same small change applied 
N times in N different commits.
