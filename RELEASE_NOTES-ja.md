# AIKernel.Control リリースノート

[English](RELEASE_NOTES.md)

## 0.1.1.1

**2026年6月14日 - CTG control-plane integration and NuGet-only development line.**

AIKernel.Control 0.1.1.1 は、AIKernel.Core 0.1.1.1 と同じ package policy で
次の実装 line に入るための repository 準備を行い、opt-in の CTG control-plane
integration surface を追加します。

- stable package version を `0.1.1.1`、local development package を
  `0.1.1.1-dev{build-number}` に揃えます。
- repository の NuGet configuration を追加し、NuGet.org より先に
  `../artifacts/local-packages` の local AIKernel packages を解決します。
- AIKernel.NET contract package 参照を `0.1.1.1` に揃えます。
- 実装作業で直接 Core package が必要になった場合に備え、
  local `AIKernel.Core 0.1.1.1-dev1` 用の `AIKernelCorePackageVersion`
  property を準備します。
- この update line では PyPI publishing を無効化します。AIKernel.Control
  0.1.1.1 は NuGet-only です。
- `AddCtgControl()`、`CtgControlCoordinator`、`CtgControlPolicyAdapter`、
  `CtgExecutionGatePolicy` による opt-in の CTG Apply Policy integration を追加します。
- provider vote material を discrete-only flow で正規化します:
  `ProviderVoteOutput` -> `CouncilVote` -> `CouncilDecision` -> vote-only
  `GateInput` -> Core `IDecisionGate`。
- Gate logic は AIKernel.Core のみに保持します。Control、Diagnostics、Emulator は
  approve count、veto 条件、`GateDecisionKind`、trajectory halt rule を計算しません。
- deterministic provider registry behavior を追加します。provider が見つからない場合は
  `Unknown` vote、複数一致は deterministic error、fallback routing は opt-in のみに限定します。
- truth-table parity、Ethos reject、all abstain、empty trajectory halt、
  denied-step trajectory halt、replay metadata stability 用の CTG Emulator dry-run
  scenario を追加します。
- replay metadata、trace emission、formatter output、CanonReference / RejectReason
  formatting、gate telemetry continuous-carrier validation 用の CTG Diagnostics helper を追加します。
- CTG Control integration を `docs/development/control-ctg*.md` に英日で文書化します。

## 0.1.1

**June 10th, 2026 - Governing the control plane.**
**2026年6月10日--制御プレーンを統治する。**

Governing the control plane: policies, schedulers, and emulators align into a
deterministic governance layer. 制御プレーンの統治--ポリシー・スケジューラ・
エミュレータが決定論的ガバナンス層へ整列する。

AIKernel.Control 0.1.1 は、AIKernel Semantic OS package family の物理実行層と
governance layer を同期します。

- AIKernel.NET の public Control contract を利用します。対象は execution graph、node、request、result、policy、scheduler、state observer です。
- CPU-only graph execution、breakpoint、watch、trace、replay-oriented test のための deterministic ControlEmulator を提供します。
- allocation-free Q1_0 kernel と VFS/ROM model asset ownership を備えた、CPU 側 Bonsai 1.7B built-in provider boundary を追加します。
- GPU execution は Control.GPU boundary に閉じ込め、device-specific execution が Core や Demo に漏れないようにします。
- public governance surface を単一 API として公開する `aikernel-governance` Python package を追加します。managed assemblies の同梱と pythonnet loading を含みます。
- Core は semantic graph、Control は physical execution mapping、Demo は runtime consumer である、という責務分離を文書化します。

Control 0.1.1 は、統治された semantic graph を決定論的な物理実行へ接続する橋です。
