# Perception to CTG Control

`AIKernel.Control.Core.Perception` adds an opt-in adapter surface between perception output and the existing CTG Control coordinator.

Control does not execute perception and does not implement Gate rules. It normalizes perception-derived discrete vote candidates into `ProviderVoteOutput`, delegates evaluation to the existing CTG coordinator, and reads the Core decision result to choose a pipeline carrier.

## Boundary Rules

- No approve-count, ethos-veto, or trajectory halt logic is implemented in Control.
- Confidence, risk, scores, diagnostics, and explanations remain metadata outside `GateInput`.
- Dynamic pipeline selection reads Core CTG output; it does not decide Gate outcomes.
- WASM runtime control, input injection, pause, and halt remain execution-side operations.
- High-priority sensor carriers such as `health-death` retry intent stay outside
  `GateInput`. Control may attach them to `CtgControlExecutionContext` and
  `CtgControlDecisionEnvelope` metadata after the Core Gate result is available.
- `PerceptionPipelineSelector` may select `retry` from an attached retry intent
  carrier. This is pipeline routing, not Gate evaluation; the Core decision
  result remains unchanged and observable in the envelope.

## Sensor OS Inputs

Control receives Sensor OS output as adapter-side carriers. It does not execute
visual, auditory, health, compass, or spatial sensors.

| Sensor | Concept | Control behavior |
| --- | --- | --- |
| `visual`, `audio`, `health` | `Aisthesis` | Remains metadata or retry intent outside `GateInput`. |
| `motor` | `Kinesis` | May inform runtime dispatch after policy, not Gate evaluation. |
| `compass`, `spatial` | `Phantasia` | May inform pipeline selection, not Gate decisions. |

For repository ownership and promotion rules, see the
[Cross-Repository Developer Guide v0.1.1.1](https://github.com/AIKernel-NET/AIKernel.NET/blob/main/docs/development/cross-repository-developer-guide-v0.1.1.1.md).

## v0.1.2 Alignment

`PerceptionControlSignal`, `PerceptionControlRequest`, `PerceptionPipelineSelection`, and retry intent carriers are adapter-side carriers. They can be replaced by canonical perception/control contracts when AIKernel.NET v0.1.2 adds or consolidates interfaces.
