# AIKernel.Control Release Notes

[日本語](RELEASE_NOTES-ja.md)

## 0.1.1.1

**June 14th, 2026 - CTG control-plane integration and NuGet-only development line.**

AIKernel.Control 0.1.1.1 prepares the repository for the next implementation
line using the same package policy as AIKernel.Core 0.1.1.1 and adds the
opt-in CTG control-plane integration surface.

- Align package versioning with `0.1.1.1` stable packages and
  `0.1.1.1-dev{build-number}` local development packages.
- Add a repository NuGet configuration that resolves local AIKernel packages
  from `../artifacts/local-packages` before NuGet.org.
- Align AIKernel.NET contract package references with `0.1.1.1`.
- Prepare an `AIKernelCorePackageVersion` property for local
  `AIKernel.Core 0.1.1.1-dev1` references when implementation work requires
  direct Core package consumption.
- Disable PyPI publishing for this update line. AIKernel.Control 0.1.1.1 is
  NuGet-only.
- Add opt-in CTG Apply Policy integration through `AddCtgControl()`,
  `CtgControlCoordinator`, `CtgControlPolicyAdapter`, and
  `CtgExecutionGatePolicy`.
- Normalize provider vote material through a discrete-only flow:
  `ProviderVoteOutput` -> `CouncilVote` -> `CouncilDecision` -> vote-only
  `GateInput` -> Core `IDecisionGate`.
- Keep Gate logic in AIKernel.Core only. Control, Diagnostics, and Emulator do
  not compute approve counts, veto conditions, `GateDecisionKind`, or trajectory
  halt rules.
- Add deterministic provider registry behavior: missing providers produce
  `Unknown` votes, multiple matches produce deterministic errors, and fallback
  routing remains opt-in only.
- Add CTG Emulator dry-run scenarios for truth-table parity, Ethos reject, all
  abstain, empty trajectory halt, denied-step trajectory halt, and replay
  metadata stability.
- Add CTG Diagnostics helpers for replay metadata, trace emission, formatter
  output, CanonReference / RejectReason formatting, and gate telemetry
  continuous-carrier validation.
- Document CTG Control integration in English and Japanese under
  `docs/development/control-ctg*.md`.

## 0.1.1

**June 10th, 2026 - Governing the control plane.**
**2026年6月10日--制御プレーンを統治する。**

Governing the control plane: policies, schedulers, and emulators align into a
deterministic governance layer. 制御プレーンの統治--ポリシー・スケジューラ・
エミュレータが決定論的ガバナンス層へ整列する。

AIKernel.Control 0.1.1 synchronizes the physical execution and governance layer
for the AIKernel Semantic OS package family.

- Consume public Control contracts from AIKernel.NET: execution graphs, nodes,
  requests, results, policies, schedulers, and state observers.
- Provide a deterministic ControlEmulator for CPU-only graph execution,
  breakpoints, watch, trace, and replay-oriented tests.
- Add the CPU-side Bonsai 1.7B built-in provider boundary with an allocation-free
  Q1_0 kernel and VFS/ROM model asset ownership.
- Keep GPU execution behind the Control.GPU boundary so device-specific
  execution can evolve without leaking into Core or Demo.
- Add the `aikernel-governance` Python package as a single wrapper over the
  public governance surface, including bundled managed assemblies and pythonnet
  loading.
- Document the responsibility split: Core owns semantic graphs, Control maps
  them to physical execution, and Demo only consumes the runtime.

Control 0.1.1 is the governed bridge from semantic graphs to deterministic
physical execution.
