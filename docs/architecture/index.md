# AIKernel.Control Architecture

[日本語](index-ja.md)

AIKernel.Control maps semantic execution graphs from AIKernel.Core onto physical
execution engines. Core remains the semantic runtime; Control owns scheduling,
emulation, CPU/GPU execution, and diagnostics.

## Layer Boundary

- `AIKernel.Control.Core` hosts the control-plane runtime entry points and
  consumes the shared control contracts from `AIKernel.Abstractions.Control`
  and `AIKernel.Dtos.Control`. It also hosts CTG orchestration and adapters,
  but not CTG gate rules.
- `AIKernel.Control.Emulator` converts Bonsai Graphs into AIKernel Graphs and
  executes them deterministically. Its CTG scenarios compare Core evaluator
  results instead of implementing decisions locally.
- `AIKernel.Control.CPU` maps Bonsai Nodes to CPU Operators.
- `AIKernel.Control.GPU` maps Bonsai Nodes to GPU kernels and tensor
  Capabilities.
- `AIKernel.Control.Diagnostics` observes graph execution, replay, timing, and
  load. CTG diagnostics format and emit traces; they do not decide outcomes.

AIKernel.Demo must use Control; it must not own execution-engine code.

## CTG Governance Boundary

Canonical Triadic Governance is connected at the Control `Apply Policy` stage.
Control is responsible for orchestration:

- resolve provider vote material deterministically;
- normalize material into `CouncilVote` and `CouncilDecision`;
- extract vote-only `GateInput`;
- call Core `IDecisionGate` / `ITrajectoryGate`;
- map Core results to existing Control policy results and replay metadata.

Control is not responsible for Gate semantics. It must not calculate approve
counts, Ethos veto behavior, trajectory halt aggregation, or create gate
decisions by itself.

Provider routing follows fail-closed deterministic behavior: missing providers
become `Unknown` votes, multiple matches become deterministic errors, and
fallback routing must be explicitly opted in.

## Python Boundary

`aikernel-governance` is the Python wrapper name for the same public Control
boundary in the 0.1.3 canonical series. Python packaging wraps the managed C#
assemblies and the public governance surface through pythonnet; it does not
reimplement CTG, policy, emulator, or scheduling logic.

Stable Python packages are created only when the 0.1.3 publication task opens.
Local validation uses `0.1.3.dev<build-number>` wheels that match the managed
`0.1.3-dev<build-number>` package family.

Python sees the contract boundary: execution requests, results, snapshots,
provider metadata, Bonsai public wrappers, emulator wrappers, CPU kernel
wrappers, diagnostics wrappers, and the GPU delegate contract. It does not see
internal engine helpers, transport-specific implementation, or private runtime
state.

See [Python governance wrapper](../python/index.md).

## Standard Model Boundary

Bonsai-1.7B is exposed as a built-in Control provider, not as a demo fixture.
The provider uses VFS ROM paths for model assets and delegates physical
inference to CPU/GPU kernels through `IBonsaiInferenceKernel`.

This keeps the dependency direction intact:

- AIKernel.NET defines contracts.
- AIKernel.Control implements execution.
- AIKernel.Demo consumes execution.

## License Boundary

AIKernel.Control implementation code is Apache-2.0 licensed. The AIKernel.NET
contracts it consumes are MIT licensed. Third-party Bonsai, tokenizer, and
ggml/llama.cpp-derived assets keep their original licenses and are not
relicensed by this repository.
