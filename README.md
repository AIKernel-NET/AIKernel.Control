# AIKernel.Control

[日本語 README](README-ja.md)

AIKernel.Core が生成する意味論的 Execution を、CPU/GPU/Emulator などの
物理実行へマッピングする Control Plane リポジトリです。

AIKernel.Control は AIKernel の物理実行レイヤーです。

- Semantic Graph（AIKernel.Core）を Physical Execution にマッピング
- CPU/GPU/Emulator の実行エンジンを提供
- Bonsai-1.7B の標準 Provider を内蔵

In the AIOS SDK, AIKernel.Control is the governance, security, and physical
execution layer. It lets an AIOS distribution map semantic graphs onto explicit
policies, deterministic schedulers, diagnostics, and execution engines.

AIKernel also provides an official AIOS distribution, codenamed
**AIKernel.Monolith**. Monolith has begun development as the standard AIOS that
will integrate the governance and control-plane layer with the rest of the SDK
after the 0.1.x line stabilizes.

## Repository Role

AIKernel.Control is the execution-engine workspace for AIKernel. AIKernel.Core
owns semantic graphs and deterministic runtime contracts; AIKernel.Control maps
those graphs onto physical execution engines, scheduler boundaries, diagnostics,
and Bonsai-style graph execution.

Bonsai-style means a node-based execution graph with explicit operators and
deterministic scheduling.

Demo repositories are consumers. Control owns execution engines. Keeping the
emulator and CPU/GPU schedulers here prevents demo code from becoming a runtime
dependency.

Control does not depend on AIKernel.Demo. Demo projects consume Control as an
independent runtime surface.

AIKernel.Control 0.1.3 follows the same development policy as
AIKernel.Core 0.1.3. The line publishes NuGet packages and a synchronized
Python wrapper. Local development packages use `0.1.3-dev{build-number}`.

## Concept Elevation

AIKernel.Control follows the common Concept Elevation naming policy maintained
in AIKernel.NET. Control concept names stay in orchestration-level facades and
do not duplicate Core CTG gate logic.

Repository notes: [docs/development/concept-elevation.md](docs/development/concept-elevation.md)

Release notes:

- [English](RELEASE_NOTES.md)
- [日本語](RELEASE_NOTES-ja.md)

## Quick Start

Start with the deterministic emulator and CPU packages before binding GPU or
model assets. Control does not vendor model weights; model assets should be
mounted through VFS/ROM.

```bash
dotnet add package AIKernel.Control --version 0.1.3
```

Install the split packages directly only when a host intentionally wants a
smaller dependency surface:

```bash
dotnet add package AIKernel.Control.Core --version 0.1.3
dotnet add package AIKernel.Control.CPU --version 0.1.3
dotnet add package AIKernel.Control.Emulator --version 0.1.3
```

Validate the repository surface:

```powershell
dotnet build AIKernel.Control.slnx -c Release
dotnet test AIKernel.Control.slnx -c Release --no-build
```

Add `AIKernel.Control.GPU` only after CPU/Emulator validation passes and a GPU
execution backend is intentionally being integrated.

First emulator object to create:

```csharp
using AIKernel.Control.Emulator;

var engine = new ControlEmulatorEngine(
    new DeterministicNodeScheduler(),
    new AllowAllControlPolicy());
```

This is the smallest Control entry point: deterministic scheduling plus an
explicit policy boundary. Move to CPU, diagnostics, and GPU packages after this
surface is understood.

## CTG Control Governance

AIKernel.Control can opt in to Canonical Triadic Governance during the
`Apply Policy` pipeline stage. Control does not own CTG gate rules. It
normalizes provider vote material into `CouncilVote` / `CouncilDecision`,
extracts a vote-only `GateInput`, and delegates decision and trajectory
evaluation to AIKernel.Core through `IDecisionGate` and `ITrajectoryGate`.

The CTG control surface is intentionally narrow:

- `GateInput` contains only `Logos`, `Ethos`, and `Pathos` votes.
- Control, Diagnostics, and Emulator do not calculate approve counts, veto
  conditions, or trajectory halt aggregation.
- Continuous provider carriers such as confidence, risk, and score are not
  accepted by the Control CTG vote-output surface.
- Provider registry fallback is explicit: missing providers become `Unknown`
  votes, multiple matches produce deterministic errors, and fallback routing
  must be opt-in.

See [CTG Control integration](docs/development/control-ctg.md).

## Projects

- `AIKernel.Control` - dependency-only entry package that installs the Control
  Core, CPU, Emulator, Diagnostics, and GPU boundary packages.
- `AIKernel.Control.Core` - control-plane runtime entry package. The shared
  contracts live in `AIKernel.Abstractions.Control` and `AIKernel.Dtos.Control`;
  this project references those contracts so CPU/GPU/Emulator implementations
  do not duplicate interface or DTO definitions.
  Capability manifests use `AIKernel.Dtos.Capabilities.CapabilityModuleDescriptor`
  rather than Control-local descriptor DTOs. The package also provides the CTG
  provider vote adapters, coordinator, opt-in policy adapter, and DI extension.
- `AIKernel.Control.Emulator` - ControlEmulator, the Bonsai-style emulator that
  converts Bonsai Graphs into AIKernel Graphs and supports CPU/GPU execution,
  step-by-step execution, breakpoints, watches, traces, and deterministic replay.
  ControlEmulator does not emulate Bonsai itself; it deterministically executes
  AIKernel `ExecutionGraph` instances in a CPU-only runtime. It also includes
  CTG dry-run scenarios that compare Core evaluator results without duplicating
  gate logic.
- `AIKernel.Control.CPU` - CPU execution engine for Bonsai Node to CPU Operator
  mapping, SIMD/AVX optimization, and ThreadPool/TaskGraph execution.
- `AIKernel.Control.GPU` - GPU execution engine for Bonsai Node to GPU Kernel
  mapping, tensor Capability binding, GPU memory management, stream/event
  orchestration, and graph execution.
- `AIKernel.Control.Diagnostics` - observability, graph visualization, node
  timing, CPU/GPU load inspection, ReplayLog integration, and CTG trace /
  replay metadata formatting.

## Python Wrapper Reference

`aikernel-governance` is the reserved Python wrapper name for the public
governance surface of AIKernel.Control.

The 0.1.3 development line publishes a synchronized PyPI wrapper. Existing
Python materials remain in the repository for reference and future scheduled
Python releases only.

It exposes the C# package boundary as a single Python API:

- execution request, result, and snapshot envelopes
- provider contract metadata
- Bonsai provider, tokenizer, model config, and model-state wrappers
- emulator graph, node, scheduler, policy, and engine wrappers
- CPU kernel and diagnostics wrappers
- managed assembly discovery and pythonnet loading

The Python wrapper design does not reimplement Control internals, scheduler
logic, provider execution, or CPU/GPU kernels. It stays a thin wrapper layer
over the public managed contracts.

## Built-in Bonsai Model

`AIKernel.Control.Core` now includes `BonsaiBuiltInProvider`, the standard
Control-plane provider surface for Bonsai-1.7B. The provider loads
`config.json` and `tokenizer.json` through `IVfsProvider` from
`/sys/roms/bonsai-1.7b/`, optionally binds `model.q1_0.bin` from the same ROM
namespace, emits deterministic phase snapshots
(`ModelDownload`, `Initializing`, `Generating`) through
`IControlStateObserver`, and delegates physical inference to
`IBonsaiInferenceKernel`.

`AIKernel.Control.CPU` provides `Bonsai1BitCpuKernel`, a Q1_0 1-bit execution
kernel that evaluates packed signs as conditional add/subtract operations over
`Span<T>` inputs. The implementation keeps allocations outside the inference
loop and exposes explicit `DequantizeRowQ1_0` and `DotRowQ1_0` methods for
validation against ggml/llama.cpp quantization assets.

The CPU kernel is allocation-free and WebAssembly-compatible, enabling
browser-side execution.

`AIKernel.Control.GPU` exposes `IBonsaiGpuExecutionDelegate` so a CUDA, WebGPU,
ROCm, or Vulkan execution backend can implement the same Bonsai inference
contract without making `AIKernel.Control.Core` depend on a concrete GPU
runtime.

CPU/GPU projects own only the physical execution part of
`BonsaiBuiltInProvider`. Model structure, tokenizer parsing, config parsing, and
provider lifecycle remain in `AIKernel.Control.Core`.

Control does not own model weights or local model files. All model assets live
in VFS/ROM; Control reads those assets through contracts and executes the bound
model state.

See:

- [Documentation index](docs/README.md)
- [User Guide](docs/user-guide/index.md)
- [Architecture](docs/architecture/index.md)
- [Bonsai mapping](docs/bonsai-mapping/index.md)
- [Bonsai-1.7B built-in provider](docs/bonsai-mapping/bonsai-1.7b-provider.md)
- [Execution engine](docs/execution-engine/index.md)
- [Q1_0 CPU execution kernel](docs/execution-engine/q1-0-cpu-kernel.md)
- [Control pipelines](docs/pipelines/index.md)
- [CTG Control integration](docs/development/control-ctg.md)
- [Licensing](docs/licensing/index.md)

## Design Direction

ControlEmulator is the AIKernel analogue of ONNX Runtime: AIKernel.Core produces
the semantic graph, while Control supplies the physical execution provider. This
also makes AIKernel.Control the OSS-oriented Bonsai execution layer for the
AIKernel Semantic Runtime.

Control is the physical execution layer of the AIKernel Semantic Runtime.

## Licensing

AIKernel.Control is licensed under the Apache License 2.0. Control contains
execution-engine code and physical runtime mappings, so it uses Apache 2.0 for
the same patent-protection reason as AIKernel.Core and native Capability
repositories.

The shared interface and DTO packages consumed by this repository, such as
`AIKernel.Abstractions.*`, `AIKernel.Dtos.*`, and `AIKernel.Enums.*`, are part
of AIKernel.NET and are MIT licensed because they contain contracts only.

Bonsai model files, tokenizer assets, llama.cpp/ggml-derived quantization
references, and any other third-party ROM assets are not relicensed by
AIKernel.Control. Operators must use those assets only under their original
licenses and redistribution terms. This repository documents the VFS paths and
execution contracts; it does not vendor model weights or local environment
paths.

## Build

```powershell
dotnet build AIKernel.Control.slnx
```

Common project properties are centralized in `Directory.Build.props`.

## Package Installation

For .NET hosts:

```bash
dotnet add package AIKernel.Control --version 0.1.3
```

Python materials are published as the synchronized `aikernel-governance`
wrapper for the 0.1.3 public package line.

See [Python governance wrapper](docs/python/index.md) for the 0.1.3 package
scope and validation flow.

## Contributor Guidelines

Control changes must follow the shared AIKernel development discipline:

- [AIKernel Development Guidelines](../AIKernel.NET/docs/guidelines/AIKERNEL_DEVELOPMENT_GUIDELINES.md)
- [AIKernel 開発ガイドライン](../AIKernel.NET/docs/guidelines/AIKERNEL_DEVELOPMENT_GUIDELINES-jp.md)

Execution and governance code should expose fail-closed contracts, keep Bonsai
and emulator behavior deterministic, avoid leaking implementation exceptions
across public boundaries, and keep Python wrappers aligned with the public C#
surface.
