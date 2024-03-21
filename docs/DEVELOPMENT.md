
## HowTo Async cmdlet

The standard base class for C# cmdlet does not support `async` libraries. According the
[post on SO](https://github.com/PowerShell/PowerShell/issues/7690) there are some exising implementations:

* [KubCtl AsyncCmdlet](https://github.com/felixfbecker/PSKubectl/blob/master/src/AsyncCmdlet.cs)  (from [SO](https://github.com/PowerShell/PowerShell/issues/7690#issuecomment-1126772320)
* [Nito.AsyncEx.Context](https://github.com/StephenCleary/AsyncEx) (from [SO](https://github.com/PowerShell/PowerShell/issues/7690#issuecomment-1798766988))

As the `AsyncEx` library is somewhat outdated and has a lot more functionality than neede, therefore the
implementation here uses the KubCtl code to provide an `AsyncCmdlet` base class. Two files
have been copied `AsyncCmdlet.cd` and `ThreadAffinitiveSynchronizationContext.cs`, only the
namespace as been adjusted.

TODO to decide: either carve out the KubCtl implenetation into its own nuget package or use
the `AsyncEx` package.

## Other References

* Microsoft experimenal Visual Studio Extension [NuGetResolve](https://devblogs.microsoft.com/nuget/introducing-nugetsolver-a-powerful-tool-for-resolving-nuget-dependency-conflicts-in-visual-studio/)
* Gist with [NugetManger.cs](https://gist.github.com/cpyfferoen/74092a74b165e85aed5ca1d51973b9d2)
  > "NuGet API wrapper to search/install packages WITH AUTHENTICATION the same way Nuget.Exe/Client does it -
  > looks up CredentialProviders and plugins (also for Credentials)."
