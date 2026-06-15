# Perception to CTG Control

`AIKernel.Control.Core.Perception` adds an opt-in adapter surface between perception output and the existing CTG Control coordinator.

Control does not execute perception and does not implement Gate rules. It normalizes perception-derived discrete vote candidates into `ProviderVoteOutput`, delegates evaluation to the existing CTG coordinator, and reads the Core decision result to choose a pipeline carrier.

## Boundary Rules

- No approve-count, ethos-veto, or trajectory halt logic is implemented in Control.
- Confidence, risk, scores, diagnostics, and explanations remain metadata outside `GateInput`.
- Dynamic pipeline selection reads Core CTG output; it does not decide Gate outcomes.
- WASM runtime control, input injection, pause, and halt remain execution-side operations.

## v0.1.2 Alignment

`PerceptionControlSignal` and `PerceptionControlRequest` are adapter-side carriers. They can be replaced by canonical perception/control contracts when AIKernel.NET v0.1.2 adds or consolidates interfaces.
