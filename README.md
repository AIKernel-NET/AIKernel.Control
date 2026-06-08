# AIKernel.Control

[日本語 README](README-ja.md)

AIKernel.Core が生成する意味論的 Execution を、CPU/GPU/Emulator などの
物理実行へマッピングする Control Plane リポジトリです。

AIKernel.Control は AIKernel の物理実行レイヤーです。

- Semantic Graph（AIKernel.Core）を Physical Execution にマッピング
- CPU/GPU/Emulator の実行エンジンを提供
- Bonsai-1.7B の標準 Provider を内蔵

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

AIKernel.Control participates in the 0.1.0 prototype validation phase scheduled
for 2026-06-09. It validates the path from AIKernel semantic graphs to physical
execution engines without moving that execution-engine responsibility into
AIKernel.Demo.

Release notes:

- [English](RELEASE_NOTES.md)
- [日本語](RELEASE_NOTES-ja.md)

## Projects

- `AIKernel.Control.Core` - control-plane runtime entry package. The shared
  contracts live in `AIKernel.Abstractions.Control` and `AIKernel.Dtos.Control`;
  this project references those contracts so CPU/GPU/Emulator implementations
  do not duplicate interface or DTO definitions.
  Capability manifests use `AIKernel.Dtos.Capabilities.CapabilityModuleDescriptor`
  rather than Control-local descriptor DTOs.
- `AIKernel.Control.Emulator` - ControlEmulator, the Bonsai-style emulator that
  converts Bonsai Graphs into AIKernel Graphs and supports CPU/GPU execution,
  step-by-step execution, breakpoints, watches, traces, and deterministic replay.
  ControlEmulator does not emulate Bonsai itself; it deterministically executes
  AIKernel `ExecutionGraph` instances in a CPU-only runtime.
- `AIKernel.Control.CPU` - CPU execution engine for Bonsai Node to CPU Operator
  mapping, SIMD/AVX optimization, and ThreadPool/TaskGraph execution.
- `AIKernel.Control.GPU` - GPU execution engine for Bonsai Node to GPU Kernel
  mapping, tensor Capability binding, GPU memory management, stream/event
  orchestration, and graph execution.
- `AIKernel.Control.Diagnostics` - observability, graph visualization, node
  timing, CPU/GPU load inspection, and ReplayLog integration.

## Python Package

`aikernel-governance` is the Python wrapper for the public governance surface of
AIKernel.Control.

It exposes the C# package boundary as a single Python API:

- execution request, result, and snapshot envelopes
- provider contract metadata
- Bonsai provider, tokenizer, model config, and model-state wrappers
- emulator graph, node, scheduler, policy, and engine wrappers
- CPU kernel and diagnostics wrappers
- managed assembly discovery and pythonnet loading

The Python package does not reimplement Control internals, scheduler logic,
provider execution, or CPU/GPU kernels. It bundles the C# assemblies and uses a
thin wrapper layer to call the public managed contracts.

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

- [Bonsai mapping](docs/bonsai-mapping/index.md)
- [Bonsai-1.7B built-in provider](docs/bonsai-mapping/bonsai-1.7b-provider.md)
- [Q1_0 CPU execution kernel](docs/execution-engine/q1-0-cpu-kernel.md)
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
dotnet add package AIKernel.Control.Core --version 0.1.0
dotnet add package AIKernel.Control.CPU --version 0.1.0
dotnet add package AIKernel.Control.Emulator --version 0.1.0
dotnet add package AIKernel.Control.Diagnostics --version 0.1.0
dotnet add package AIKernel.Control.GPU --version 0.1.0
```

For Python hosts:

```bash
pip install aikernel-governance
```

Import the Python module as `aikernel_governance`:

```python
from aikernel_governance import ExecutionRequest, GovernanceClient
```

The wheel bundles managed AIKernel.Control assemblies under
`aikernel_governance/native`. It is a wrapper over the public C# contract
surface, not a separate Python implementation of governance semantics.

See [Python governance wrapper](docs/python/index.md) for package scope,
assembly loading, and publication guidance.
