# Conventions

How this repo writes code. The review pass reads this; a rule here overrides
any general best practice.

## Layout

Everything lives under `src/`, one folder per project, `src/packages/` for
NuGet (packages.config, not PackageReference). Target framework is .NET
Framework 4.7.2 across every project.

- **ScriptScripter.Processor** — all logic. Class library, no UI. Split into
  `Data/Contracts`, `Data/Models`, `Data/Repositories`, `Services`,
  `Services/Contracts`, `Dto`.
- **ScriptScripter.DesktopApp** — WPF UI (MahApps.Metro + NinjaMvvm).
  `ViewModels/`, `Views/`, `Controls/`, `Converters/`, `Themes/`. References
  Processor.
- **ScriptScripter.Command** / **ScriptScripter.Container.Command** — console
  entry points. Each is `Program.cs` + `IncomingOptions.cs` + `Ninjector.cs` +
  `NLog.config`.
- **ScriptScripter.ProcessorTests** (unit), **ScriptScripter.Processor.IntegrationTests**
  (hits a real local SQL Server), **ScriptScripter.DesktopAppTests** (ViewModel
  unit tests).

Interfaces live in a `Contracts` sub-namespace next to their implementations,
never in a separate project.

DI is Ninject. Each executable has its own `Ninjector : NinjectModule`, and the
Processor has one too; the host loads both modules and assigns the static
`Ninjector.Container` on each (`ScriptScripter.Command/Program.cs:151`).

There is no `.editorconfig`, ruleset, or analyzer configuration. Style is
carried by convention only, which is why this file matters.

## Naming

- Types, methods, and properties PascalCase. Private fields `_camelCase`.
- Interfaces `IThing`. Factories are `IThingFactory` + `ThingFactory`,
  sometimes bound with Ninject's `.ToFactory()`.
- **Types are referenced by fully-qualified name inline rather than pulled in
  with `using`.** `System.IO.Abstractions.IFileSystem _fileSystem;`,
  `Processor.Data.Contracts.IScriptRepositoryFactory`,
  `Dto.ActionResult.SuccessResult()`. `using` is reserved for a short list
  (System, System.Linq, NLog, Moq, FluentAssertions, FluentValidation,
  FaultlessExecution.Extensions, Dapper). This is a real convention — new code
  follows it.
- Named arguments are used liberally at call sites for readability, even for a
  single parameter: `TruncateString(value, length: 255)`,
  `vm.Init(scriptContainer: lineItem.ScriptContainer)`.
- Async methods end in `Async`.
- Constructor bodies assign fields bare: `_navigator = navigator;` — no
  `this.` prefix on the assignment.
- Injected fields are `private readonly`.
- Models are plain get/set POCOs in `Data/Models`. `Dto/` holds result and
  progress shapes, not persisted state.
- ViewModel bindable properties use NinjaMvvm's `GetField<T>()` /
  `SetField(value)`, never hand-written backing fields.
- WPF commands are a `#region <Name> Command` holding a lazily-constructed
  `RelayCommand` property, a `CanX()` method, and an `X()` method.

## Data access

There is no ORM and no migration framework. Four distinct stores:

1. **Applied-revision table in the target database, reads** — **Dapper**
   (`Data/Repositories/RevisionRepository.cs`). Raw SQL in verbatim strings,
   table name held in a `private const string _tableNameWithSchema`.
   Connections are created per call inside a `using`, never cached or shared.
2. **The same table, writes** — via **SMO**
   (`Services/DatabaseUpdater.cs`, `Microsoft.SqlServer.Management.Smo`).
   Deliberately on the SMO connection so the transaction does not escalate to
   MSDTC. SMO DLLs are vendored under `Processor/smo_libs/v12.0` and copied by
   the pipeline. Transactions are explicit: `BeginTransaction` /
   `CommitTransaction` / `RollbackTransaction` on the SMO connection context.
3. **Settings file** — JSON via Newtonsoft. `Data/Repositories/ConfigFileBase.cs`
   reads and writes the entire `Settings` object each time; derived repositories
   (`ConfigurationRepository`, `ScriptContainerRepository`) always
   read-modify-write the whole file.
4. **Script containers** — either one XML file (`ScriptFileRepository`) or a
   folder of per-script files (`ScriptFolderRepository`). `ScriptsRepository`
   chooses between them at runtime by inspecting the path.

All filesystem access goes through an injected
`System.IO.Abstractions.IFileSystem` — never `System.IO.File` directly. The one
exception is `ScriptContainerWatcherService`, which uses `System.IO` directly
and carries a TODO about it.

Repositories that need a target are configured by settable property
(`ScriptContainerPath`, `DatabaseConnectionParams`), not constructor argument,
and are handed out by a factory.

## Error handling

Three layers, three different rules:

- **Processor** returns `Dto.ActionResult` / `Dto.ActionResult<T>` for expected
  failures — `SuccessResult()` / `FailedResult(message)`. It throws
  `ApplicationException` only for caller programming errors (e.g. adding a
  script that already exists), and `InvalidScriptContainterContentsException`
  for an unreadable container file.
- **DesktopApp ViewModels do not try/catch.** Everything goes through
  `Contracts.IViewModelFaultlessService` (FaultlessExecution), whose
  `OnException` opens `ErrorViewModel` as a dialog. Chain results with
  `.OnSuccessAsync()` / `.OnExceptionAsync()`.
- **Console apps** log via NLog and call `Environment.Exit(<code>)` — distinct
  codes 5000–5007. Nothing is allowed to throw out of `Main`.

Logging is NLog with `ILogger` injected; the DesktopApp Ninjector binds it
per-declaring-type. ViewModels receive it through `ScriptScripterViewModelBase`,
which logs load/reload failures. The Processor mostly does not log — it returns
a result and lets the caller decide.

## Async

- Service methods that touch SQL are async. Where the underlying API is
  synchronous, the work is wrapped in `Task.Run`
  (`ScriptingService.ApplyScriptsToDatabaseAsync`).
- Repositories are synchronous. ViewModels bridge with
  `TryExecuteSyncAsAsync(...)`.
- `async void` is used on purpose, for WPF command handlers only
  (`ApplyScriptsViewModel.ApplyScriptsAsync`,
  `DatabaseConnectionViewModel.TestConnectionAsync`).
- `ConfigureAwait` is never used anywhere in this repo. Do not introduce it.
- Cancellation: `CancellationToken` overloads exist for the connection tests and
  are checked manually after the await rather than allowed to throw.

## Configuration

- Compile-time settings live in `Properties/Settings.settings` (e.g.
  `ConfigurationSettingsFile`) plus per-app `App.config`; NLog config in a
  per-executable `NLog.config`.
- User state — developer name, theme, server connection, script container list —
  lives in a JSON file whose path comes from settings and is run through
  `Environment.ExpandEnvironmentVariables`.
- The SQL password in that file is encrypted with
  `Services.Contracts.ICryptoService` before write and decrypted on read. That
  happens in the repository, never in a ViewModel.
- Console apps take everything on the command line via CommandLineParser
  (`IncomingOptions.cs`). Passwords are scrubbed before being logged
  (`Program.LogParamState`).

## Testing

MSTest + FluentAssertions + Moq, with `System.IO.Abstractions.TestingHelpers`
(`MockFileSystem`) for filesystem work.

- Unit tests mirror the source folder structure and sit in a `.Tests` namespace
  (`ScriptScripter.Processor.Services.Tests`).
- Test classes are named `XxxTests`. Test methods are snake_case and describe
  the behavior: `warns_of_use_db_at_beginning_of_script()`,
  `GetLastRevision_returns_last_by_scriptdate()`.
- Newer tests use an arrange/act/assert shape: fields for the inputs, a shared
  `Act()` method, and `//arrange`, `//act`, `//assert` comments
  (`ProcessorTests/Services/ScriptWarningServiceTests.cs`).
- ViewModel tests derive from `VMTestBase`, which builds `RepoMockery` and
  `ServiceMockery` with `MockBehavior.Strict` and ends every test with
  `VerifyAllMocks()`.
- Integration tests derive from `DatabaseTestingBase`, which creates and drops a
  real `SSTest` database on `(local)` using a trusted connection. They live in a
  separate project on purpose and need a local SQL Server to run.
- Not tested, deliberately: Views, XAML, converters, and the SMO internals of
  `DatabaseUpdater`. `ScriptingService.ApplyScriptsToDatabase` is explicitly
  marked TODO-untested.

## Known inconsistencies

These appear in more than one form. The listed direction is what new and
changed code should do; existing code in the other form is not a finding.

- **`private readonly` vs plain `private`** on injected fields. DesktopApp
  ViewModels and the newer repositories use `readonly`; most Processor services
  do not. → **`private readonly` going forward.**
- **`this._x = x;` vs `_x = x;`** in constructors — both appear, sometimes in
  the same constructor (`DesktopApp/ViewModels/ScriptViewModel.cs:34`).
  → **bare `_x = x;` going forward.**
- **Test class naming** — `ScriptWarningService_Tests`, `ScriptingServiceTests`,
  and BDD-style `given_container_exists_when_Begin`. **Test method naming** —
  old VS-generated `CommitTransactionTest()` vs newer descriptive snake_case.
  → **`XxxTests` classes with snake_case methods going forward.**
- **`this.` prefix on instance member calls** (`this.ReadFile()`) is used
  through most of the Processor and inconsistently in ViewModels. No direction
  set; do not flag either form.

## Deliberate deviations

Things that look wrong but are intentional. Do not report these.

- **`DatabaseUpdater.LogScript` builds SQL by string interpolation** instead of
  parameters. Documented in-line: the entire tool exists to execute arbitrary
  user-supplied SQL against the target database, so injection is not in the
  threat model. Quotes are still escaped via `SqlSafeString`.
- **`Script.XScriptDateForXml`** is a string property that exists only because
  `XmlSerializer` cannot serialize `DateTimeOffset`.
- **`#if DEBUG` parameterless ViewModel constructors** exist for the XAML
  designer and are excluded from Release because IoC otherwise selected the
  wrong constructor.
- **`ConfigFileContractResolver`** reflects over Json.NET internals to keep
  deserializing the legacy `ScriptFilePath` property name alongside the current
  `ScriptContainerPath`.
- **`catch { }`** in `MainWindow.xaml.cs` (window placement) and
  `DatabaseUpdater.Dispose` — both annotated, both intentional.
- **`ScriptFileRepository.ReadScripts` reads via a `FileStream` with
  `FileShare.ReadWrite`** rather than `File.ReadAllText`, because the container
  file is often open in an editor.
- **Hardcoded key material in `CryptoService`** — obfuscation of a locally
  stored SQL password, not a secret-management boundary. The switch to
  `AesCryptoServiceProvider` was a deliberate FIPS-compliance change.
