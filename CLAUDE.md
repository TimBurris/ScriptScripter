# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Personal Practices

My standing cross-project coding preferences (code style, DI, how I want you to work, etc.) live in a
shared file — follow them here. Note this is a .NET Framework desktop/CLI tool, so the web/UI/ORM
sections of that file largely don't apply:
@~/.claude/personal-practices.md

## What this is

ScriptScripter is a SQL Server database migration tool using a "database first" approach: developers hand-write T-SQL scripts, record them in a *script container*, and apply them (in `ScriptDate` order) to databases. Each target database tracks which scripts it has already run in a `[ScriptScripter].[AppliedRevision]` table, so only outstanding scripts get applied. See `README.md` for the full product rationale.

## Build / Test / Run

This is a **.NET Framework 4.7.2** solution using the **old-style (packages.config) MSBuild project format** — not the .NET SDK. There is no `dotnet build`. Use Visual Studio or MSBuild + NuGet directly.

```powershell
# Restore (packages land in src\packages\)
nuget restore src\ScriptScripter.sln

# Build
msbuild src\ScriptScripter.sln /p:Configuration=Debug /p:Platform="Any CPU"

# Run all tests (after build) — point vstest at the built test DLLs:
vstest.console.exe src\ScriptScripter.ProcessorTests\bin\Debug\ScriptScripter.ProcessorTests.dll `
                   src\ScriptScripter.DesktopAppTests\bin\Debug\ScriptScripter.DesktopAppTests.dll

# Run a single test by name
vstest.console.exe <test.dll> /Tests:GetScriptsThatNeedRun_WhenNoRevisions_ReturnsAll
```

Tests use **MSTest** (`[TestClass]`/`[TestMethod]`), **Moq** (mocks are created with `MockBehavior.Strict`), and **FluentAssertions**.

### Test project layout (important distinction)
- **ScriptScripter.ProcessorTests** / **ScriptScripter.DesktopAppTests** — pure unit tests. The filesystem is faked via `System.IO.Abstractions.TestingHelpers` (`MockFileSystem`), so they touch no real disk. Run anywhere.
- **ScriptScripter.Processor.IntegrationTests** — require a **real SQL Server reachable at `(local)` via Windows Auth** (see `DatabaseTestingBase.cs`). They create/drop real databases. Do not run in CI/dev without that instance.

## Projects & architecture

The solution is split into a UI-agnostic core (`Processor`) and three front-ends that all bootstrap the same core via Ninject.

- **ScriptScripter.Processor** (Library) — all business logic, data access, and SQL Server interaction. Has no UI dependency.
- **ScriptScripter.DesktopApp** (WinExe) — WPF MVVM desktop app (MahApps.Metro theming, NinjaMvvm).
- **ScriptScripter.Command** (Exe) — CLI that applies a script container to a database. Used in Azure DevOps deploy pipelines.
- **ScriptScripter.Container.Command** (Exe) — CLI that copies all scripts from one container to another.

### Dependency injection (Ninject) — the spine of the app
Every entry point constructs a kernel from **two modules**: its own `Ninjector` + `ScriptScripter.Processor.Ninjector`, then assigns the kernel to the **static `Ninjector.Container` property on both modules** (see `App.xaml.cs`, `Command/Program.cs`, `Container.Command/Program.cs`). This static container is not just for top-level resolution — `ScriptRepositoryFactory` and `DatabaseUpdaterFactory` resolve from it at runtime. When adding a service, register it in the appropriate `Ninjector.Load()`.

### Script containers: file vs. folder (key abstraction)
A "script container" is a path that is **either a single XML file or a folder of per-script XML files**. `ScriptsRepository` (registered for `IScriptsRepository`) inspects the path at runtime and delegates to `ScriptFileRepository` or `ScriptFolderRepository` accordingly. Folder mode exists specifically so multiple developers' scripts merge cleanly in source control. Always go through `IScriptRepositoryFactory.GetScriptsRepository(path)` — it sets `ScriptContainerPath` on the resolved repo for you.

### Data layer
- **Script containers** are XML (deliberately not JSON — multi-line SQL must stay human-readable in a text editor; see README).
- **App configuration** (`ConfigurationRepository` via `ConfigFileBase`) is JSON, stored at `Properties.Settings.Default.ConfigurationSettingsFile`. Holds developer name, theme, server connection, and the list of known script containers. Server passwords are encrypted via `CryptoService`.
- **Backwards-compat shim**: the old property name `ScriptFilePath` is still deserialized into the current `ScriptContainerPath` (custom `JsonContractResolver` in `ConfigFileBase`; CLI also still accepts the `-z/--scriptfilepath` option). Preserve this when touching config/options code. `ConfigurationFileUpgradeService` runs on app start to migrate old config files.

### SQL execution
`DatabaseUpdater` uses **SQL Server Management Objects (SMO) v12**, referenced as loose DLLs in `Processor\smo_libs\v12.0\` (HintPath references, **not** NuGet — the build pipeline copies this folder alongside each output). SMO lets scripts use full SSMS syntax including `GO` batch separators. `ScriptingService.ApplyScriptsToDatabase` wraps the whole run in one SMO transaction and rolls back on any failure. `CreateScriptingSupportObjects()` lazily creates the `ScriptScripter` schema + `AppliedRevision` table. Note: SQL string-building here is intentionally not parameterized — by design the tool runs arbitrary user SQL, so injection is moot (see the in-code comment).

### "Scripts that need to run" logic
`ScriptingService.GetScriptsThatNeedRun` = all scripts in the container minus those whose `ScriptId` already exists in the DB's revisions, ordered by `ScriptDate`. `GetDatabaseScriptState` returns a `[Flags]` state (`UpToDate` / `OutOfdate` / `Newer`) — `Newer` means the DB has revisions absent from the container, which is the highest-priority warning.

### Desktop app MVVM
- ViewModels derive from `ScriptScripterViewModelBase` → `NinjaMvvm.Wpf.WpfViewModelBase`. Async data loading goes in `OnReloadDataAsync`; design-time data in `OnLoadDesignData`. Bindable properties use `GetField<T>()`/`SetField(value)`.
- Navigation is via injected `NinjaMvvm.Wpf.Abstractions.INavigator` (`NavigateTo<TVm>()` for content, `ShowDialog<TVm>()` for dialogs). The concrete navigator (`ScriptScripterNavigator`) is wired through `NinjaMvvm.Wpf.Ninject.Component.Init`.
- Service/repo calls from VMs are wrapped in `IViewModelFaultlessService` (FaultlessExecution) which returns a result rather than throwing.
- `MainViewModel.AddNewScriptForContainer` is invoked when the desktop app is launched with `-a <containerPath>` (used to trigger "Add New Script" from a Stream Deck button); container path matching is case-insensitive.

## CI

`azure-pipelines.yml` (manual trigger only) runs on `windows-latest`: NuGet restore → `BuildUtilities/ApplyVersionToAssemblies.ps1` (stamps assembly versions from the build number) → VSBuild Release → publishes three artifacts (DesktopApp, Command, ContainerCommand), each bundled with the `smo_libs\v12.0` folder.
