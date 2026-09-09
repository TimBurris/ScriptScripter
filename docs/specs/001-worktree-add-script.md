# 001 — Add New Script from a git worktree

**Status:** Draft
**Date:** 2026-09-09
**Repo:** ScriptScripter

## Problem

Tim runs multiple git worktrees at once (via Claude Code and DiffNinja), each
with its own copy of a repo's SQL scripts folder — e.g.
`E:\Code\NaviPay\.claude\worktrees\payment-error-statuses-163b7d\Database\Scripts`
alongside the primary `E:\Code\NaviPay\Database\Scripts`. DiffNinja launches
`ScriptScripter.DesktopApp.exe -a "<worktree path>" -clipboard` to open Add
New Script against whichever checkout is open in Visual Studio. Today
`MainViewModel.AddNewScriptForContainer` only does an exact path match
against the configured `ScriptContainer` list (`ScriptContainerRepository`),
so any worktree path — which is never configured and changes every time a
worktree is created — fails with "The specified script container was not
found," and Tim can't add a script while reviewing a worktree.

## Behavior

- When `-a <path>` matches a configured `ScriptContainer.ScriptContainerPath`
  exactly (case-insensitive), behavior is unchanged, **except**: if more than
  one configured container shares that exact path (e.g. `SyndeoBroker` and
  `SyndeoWaste` both pointing at `e:\code\quoteninja\database\scripts`), the
  system prompts the user to pick which one before opening Add New Script,
  instead of silently using the first one found.
- When `-a <path>` does not match any configured container exactly, the
  system checks whether `<path>` sits inside a git worktree. It walks up from
  `<path>` looking for a directory containing a `.git` **file** (not a `.git`
  folder — that marks a worktree checkout). If found, it reads that file's
  `gitdir:` line, then reads `commondir` from that gitdir directory, and
  resolves the main checkout's root from there. It then takes `<path>`
  relative to the worktree root and re-roots that same relative path under
  the main checkout root, producing a candidate main-checkout path.
- If that candidate path exactly matches one configured container, the
  system opens Add New Script using a **transient, in-memory clone** of that
  container: same `DatabaseName`, same `CustomServerConnectionParameters`,
  but `ScriptContainerPath` set to the original worktree `<path>`. Nothing is
  written to the configuration file — no new list entry, no watcher, no
  `ContainerUid` persisted. The dialog title, save location, and Commit /
  Commit-and-Apply behavior all work exactly as they do for a configured
  container, just pointed at the worktree folder, so the new script XML is
  written there and travels with the branch.
- If that candidate path matches more than one configured container, the
  system prompts the user to pick which one (same picker as the exact-match
  ambiguity case above), then proceeds with the transient clone using the
  chosen container's `DatabaseName` / connection params.
- When Add New Script is opened against a transient worktree container, the
  dialog says so: a warning-styled banner under the database name reads
  "git worktree copy: `<worktree folder name>`" (the full container path is
  its tooltip), and the window title is suffixed
  "(worktree: `<worktree folder name>`)". A normal configured container is
  unchanged - no banner, original title.
- Commit and Apply is allowed unchanged for a transient worktree container.
  Applying writes `AppliedRevision` rows to whatever database the inherited
  connection params point at; if the worktree/branch is later abandoned,
  those rows have no home container and will show as `Newer` on the primary
  container's status — this is accepted, not fixed by this spec.
- When `<path>` is not inside a worktree (no `.git` file found while walking
  up) and there's no exact match, the current "not found" dialog is shown
  unchanged — title "Error", message "The specified script container was not
  found," details listing the searched path and all configured paths.
- When `<path>` **is** inside a worktree, but the re-rooted candidate path
  also doesn't match any configured container, the "not found" dialog's
  details are extended to explain what was tried: the detected worktree
  root, the resolved main checkout root, and the candidate path that was
  searched for — followed by the list of configured paths, so the failure is
  diagnosable at a glance instead of just listing paths that were never the
  thing being searched for.
- Path comparisons (exact match and re-rooted match) are case-insensitive
  with trailing directory separators trimmed, matching the existing
  `OrdinalIgnoreCase` comparison in `MainViewModel`.
- If the user cancels the picker dialog, no further dialog is shown and Add
  New Script does not open — same as any other cancelled dialog in this app.

## Out of scope

- No new CLI flag or option is added to `ScriptScripter.DesktopApp`
  (`IncomingOptions`) — resolution is an automatic fallback behind the
  existing `-a`.
- `ScriptScripter.Command` and `ScriptScripter.Container.Command` are
  untouched — both take `-s`/`-d` explicitly and never consult the
  configured container list.
- The resolved worktree container is never persisted to the config file or
  exposed anywhere in the UI outside of the one Add New Script dialog it was
  invoked for (no entry in `DatabaseListViewModel`, no `FileSystemWatcher`
  via `ScriptContainerWatcherService`).
- No cleanup or detection of stale `AppliedRevision` rows left behind by an
  abandoned worktree branch.
- Fixing DiffNinja's `-clipboard` (single-dash long option, which
  `CommandLineParser` 2.7.82 parses as short-option syntax and may silently
  fail to bind) is being done separately in the DiffNinja repo, not here.
- Deduplicating configured containers that share a path (`SyndeoBroker` /
  `SyndeoWaste`) is not attempted — the picker handles the ambiguity, the
  duplication itself is legitimate (same scripts, two databases).

## Data and schema

None. No changes to the `[ScriptScripter].[AppliedRevision]` table or the
JSON config file schema (`ConfigFileBase.Settings`). No migration, no
backfill.

## Seams

- **`ScriptScripter.Processor/Services`** — new
  `IWorktreeResolverService` / `WorktreeResolverService` (or similar; final
  naming at implementation time), constructed with
  `System.IO.Abstractions.IFileSystem` per this repo's convention for
  filesystem code (see `ConfigFileBase`, `ScriptFileRepository`), so it's
  testable against `MockFileSystem`. Given a candidate path, walks up the
  directory tree looking for a `.git` file, parses `gitdir:` and `commondir`,
  and returns the resolved main-checkout-relative candidate path (or a "not
  a worktree" result). Registered in `Processor.Ninjector.Load()`.
- **`ScriptScripter.DesktopApp/ViewModels/MainViewModel.cs`,
  `AddNewScriptForContainer`** — currently a single `FirstOrDefault` exact
  match against `_scriptsContainerRepository.GetAll()`. Needs to: (1) find
  *all* exact matches, not just the first, and route to a picker when there's
  more than one; (2) on zero exact matches, call the new worktree resolver
  and repeat the same "all matches → 0/1/many" handling against the
  re-rooted candidate path; (3) build the transient `ScriptContainer` clone
  (same type, `ScriptContainerPath` swapped) rather than requiring one to
  exist in the repository; (4) extend the not-found `MessageBoxViewModel`
  details message for the worktree-detected-but-unmatched case.
- **New picker dialog** — modeled directly on
  `ViewModels/SelectThemeViewModel.cs` + `Views/SelectThemeView.xaml`
  (`ObservableCollection` of options bound to a list, a select command, a
  close/cancel command, shown via
  `_navigator.ShowDialog<TPickerViewModel>()`). Options are the ambiguous
  `ScriptContainer`s; each row shows enough to disambiguate — at minimum
  `DatabaseName`, and the server (`ServerConnectionParameters.ToString()`
  already formats as `"server (Integrated Security)"` /
  `"server (user ********)"`).
- **`ScriptScripter.DesktopApp/ViewModels/ScriptViewModel.cs`** and
  **`ApplyScriptsViewModel.cs`** — no changes. Both already operate purely
  on the `ScriptContainer` instance handed to `Init(...)` (`ScriptContainerPath`,
  `DatabaseName`, `CustomServerConnectionParameters`) with no dependency on
  `ContainerUid` or on the container existing in the repository, confirmed by
  reading both files.
- **`ScriptScripter.Processor.Ninjector`** — register the new resolver
  service.

## Decisions and assumptions

- New script XML is written into the worktree's own scripts folder so it
  travels with the branch, not into the primary checkout — Tim's explicit
  answer; this is the whole point of the feature.
- The transient worktree container is in-memory only, never written to
  config — worktrees are disposable and Tim works multiple at once; a
  persisted entry would accumulate dead paths.
- Worktree resolution reads `.git`/`gitdir`/`commondir` files directly rather
  than shelling out to `git.exe` — keeps the resolver pure and testable with
  `MockFileSystem`, consistent with how every other filesystem-touching class
  in this repo is built and tested.
- Resolution is an automatic fallback on exact-match miss, with no new CLI
  flag — zero change needed in DiffNinja, and it works identically whether
  launched from the primary checkout or any worktree.
- Commit and Apply stays enabled, unchanged, for a transient worktree
  container — Tim's explicit answer, accepting that an abandoned branch can
  leave orphaned `AppliedRevision` rows that surface as `Newer` on the
  primary container later.
- The ambiguous-match picker applies uniformly to both the exact-match path
  and the worktree-resolved path — Tim confirmed the `SyndeoBroker` /
  `SyndeoWaste` collision is real and wants a picker rather than the current
  silent `FirstOrDefault`, and having two different behaviors for the same
  ambiguity depending on which code path hit it would itself be confusing.
- **Assumption:** cancelling the picker dialog leaves the app on whatever
  view it was already showing (the home `DatabaseListViewModel`, since
  `-a` fires shortly after startup) — no further dialog, no error. Consistent
  with how `Cancel()` behaves on every other dialog VM in this app.
- **Assumption:** path comparisons (both exact-match and re-rooted-match) are
  case-insensitive with trailing separators trimmed — matches the existing
  `OrdinalIgnoreCase` comparison already in `MainViewModel`, extended
  slightly since worktree paths won't have been hand-typed to match casing.
- Scope is `ScriptScripter.DesktopApp` only — the two console CLIs take
  `-s`/`-d` explicitly and never look at the configured container list, so
  worktree resolution doesn't apply to them.

## Slices

1. **Worktree resolver service, unit tested.** Add
   `IWorktreeResolverService`/`WorktreeResolverService` to
   `ScriptScripter.Processor/Services` (+ `Contracts`), registered in
   `Processor.Ninjector`. Given a path and an `IFileSystem`, it returns
   either "not a worktree" or the resolved main-checkout candidate path
   (worktree root, main checkout root, candidate path all available for the
   caller to use in messaging). Done when: unit tests pass against
   `MockFileSystem` covering the cases below, with no UI changes yet.
2. **Wire into `MainViewModel.AddNewScriptForContainer` + ambiguity picker.**
   Replace the `FirstOrDefault` exact match with an all-matches lookup;
   route >1 match to a new picker dialog (`SelectScriptContainerViewModel` /
   view, modeled on `SelectThemeViewModel`); on zero exact matches, call the
   resolver, repeat the 0/1/many handling against the candidate path, and
   build the transient in-memory `ScriptContainer` clone before opening
   `ScriptViewModel`; extend the not-found dialog's details for the
   worktree-detected case. Done when: the manual verification walkthrough
   below passes end-to-end from a real worktree, including the
   SyndeoBroker/SyndeoWaste picker case from the primary checkout.

This is small enough that a third slice isn't warranted — slice 2 is one
coherent gate (VM change + new dialog + messaging), and there's nothing to
verify about the picker or the transient container in isolation from the
resolver actually driving them.

## Verification plan

**Automated:** Unit test the resolver service against
`System.IO.Abstractions.TestingHelpers.MockFileSystem`
(`ScriptScripter.ProcessorTests/Services/`), following this repo's MSTest +
Moq (`MockBehavior.Strict`) + FluentAssertions conventions. Cases:
- Path is inside a worktree with a well-formed `.git` file, `gitdir`, and
  `commondir` → returns the correct re-rooted candidate path.
- Path is a normal (non-worktree) folder with no `.git` file anywhere up the
  tree → returns "not a worktree".
- Path is inside a *primary* checkout (`.git` is a folder, not a file) →
  returns "not a worktree" (must not misfire on the common case).
- `.git` file exists but `gitdir` line is missing/malformed, or the
  `commondir` file is missing → returns "not a worktree" / a clean failure
  rather than throwing.
- Worktree several directories deep (`<path>` is a few levels below the
  worktree root) → relative path is preserved correctly when re-rooted.

No integration tests — no database or process-launch involved in this logic.
Not proposing tests for `MainViewModel` changes: the branching there
(0/1/many matches, twice) is plumbing over the already-tested resolver and
repository, and any test would either hit `MockFileSystem`-backed real
repositories (borderline integration) or be mock-heavy assertions about what
got called — better covered by the manual walkthrough below.

**Manual:**
1. In a NaviPay git worktree (VS solution open on the worktree), trigger
   DiffNinja's "Add to ScriptScripter" with clipboard content. Expect the Add
   New Script dialog to open, titled with the NaviPay database name.
2. Enter a comment, commit. Confirm the new script XML appears under
   `...\worktrees\<name>\Database\Scripts` and shows up in `git status` on
   that worktree's branch — and does **not** appear under
   `E:\Code\NaviPay\Database\Scripts`.
3. In a quoteninja worktree (or the primary `e:\code\quoteninja\database\scripts`
   checkout), trigger Add New Script. Confirm the picker appears offering
   SyndeoBroker and SyndeoWaste, and that picking one opens the dialog titled
   with the chosen database.
4. Unhappy path: launch `-a` with a path that is not inside any worktree and
   doesn't match any configured container. Confirm the original "not found"
   dialog appears, unchanged.
5. Unhappy path: launch `-a` with a path inside a worktree whose re-rooted
   main-checkout path is *not* in the configured list. Confirm the "not
   found" dialog's details show the detected worktree root, resolved main
   checkout root, and the candidate path that was searched for, followed by
   the configured list.

**Regression watch:**
- Launching `-a` with a path that exactly matches exactly one configured
  container (the common case today) still opens Add New Script directly,
  with no picker prompt.
- Existing non-worktree "not found" messaging is unchanged in the case where
  no `.git` file is found at all.
- `DatabaseListViewModel`'s container list is unaffected by any transient
  worktree container created during a `-a` launch — no phantom entries.
