# AIKernel.Control
AIKernel.Core が生成する意味論的 Execution を、CPU/GPU/Emulator などの
物理実行へマッピングする Control Plane リポジトリです。

## Repository Role

AIKernel.Control is the execution-engine workspace for AIKernel. AIKernel.Core
owns semantic graphs and deterministic runtime contracts; AIKernel.Control maps
those graphs onto physical execution engines, scheduler boundaries, diagnostics,
and Bonsai-style graph execution.

Demo repositories are consumers. Control owns execution engines. Keeping the
emulator and CPU/GPU schedulers here prevents demo code from becoming a runtime
dependency.

AIKernel.Control participates in the 0.1.0 prototype validation phase scheduled
for 2026-06-09. It validates the path from AIKernel semantic graphs to physical
execution engines without moving that execution-engine responsibility into
AIKernel.Demo.

## Projects

- `AIKernel.Control.Core` - control-plane abstractions and contracts:
  `IControlEngine`, `IExecutionGraph`, `INodeScheduler`, `IControlPolicy`, and
  `IControlStateObserver` will live here as the contract surface stabilizes.
- `AIKernel.Control.Emulator` - ControlEmulator, the Bonsai-style emulator that
  converts Bonsai Graphs into AIKernel Graphs and supports CPU/GPU execution,
  step-by-step execution, breakpoints, watches, traces, and deterministic replay.
- `AIKernel.Control.CPU` - CPU execution engine for Bonsai Node to CPU Operator
  mapping, SIMD/AVX optimization, and ThreadPool/TaskGraph execution.
- `AIKernel.Control.GPU` - GPU execution engine for Bonsai Node to GPU Kernel
  mapping, tensor Capability binding, GPU memory management, stream/event
  orchestration, and graph execution.
- `AIKernel.Control.Diagnostics` - observability, graph visualization, node
  timing, CPU/GPU load inspection, and ReplayLog integration.

## Design Direction

ControlEmulator is the AIKernel analogue of ONNX Runtime: AIKernel.Core produces
the semantic graph, while Control supplies the physical execution provider. This
also makes AIKernel.Control the OSS-oriented Bonsai execution layer for the
AIKernel Semantic Runtime.

## Build

```powershell
dotnet build AIKernel.Control.slnx
```

Common project properties are centralized in `Directory.Build.props`.
