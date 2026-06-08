# Bonsai-1.7B Built-in Provider

[日本語](bonsai-1.7b-provider-ja.md)

`BonsaiBuiltInProvider` is the standard Control-plane provider surface for
Bonsai-1.7B. It keeps the model lifecycle inside AIKernel.Control while using
AIKernel.NET contracts for provider, VFS, capability, and control-state
boundaries.

Bonsai-1.7B is a compact 1-bit quantized model designed for CPU/WASM execution.

## ROM Layout

The provider reads Bonsai assets through `IVfsProvider`; direct file I/O is not
part of the runtime contract.

Required ROM files:

- `/sys/roms/bonsai-1.7b/config.json`
- `/sys/roms/bonsai-1.7b/tokenizer.json`

Optional ROM file:

- `/sys/roms/bonsai-1.7b/model.q1_0.bin`

If `model.q1_0.bin` is absent, the provider creates deterministic bootstrap
weights during initialization. This fallback is for contract validation,
emulator smoke tests, and early integration only. Production inference should
provide a real ROM-bound Q1_0 weight payload through VFS.

## Lifecycle

Initialization follows a deterministic phase sequence:

- `ModelDownload`: ROM assets are requested through VFS.
- `Initializing`: config, tokenizer, and tensor buffers are parsed and bound.
- `Ready`: model state is fully bound and immutable; inference kernels may now
  execute nodes.
- `Generating`: an execution node is being evaluated.

Each phase is emitted through `IControlStateObserver` so the kernel timeline can
observe physical execution without embedding execution-engine logic in demos or
Core.

## Capabilities

The provider declares:

- `chat.local`
- `text.tokenize`

`ExecuteNodeAsync` evaluates the requested node operation and returns a
`ControlExecutionResult` with deterministic metadata. The physical inference
work is delegated to `IBonsaiInferenceKernel`, allowing CPU and GPU backends to
share the same provider contract.

`ExecuteNodeAsync` returns deterministic metadata such as `provider_id`,
`operation`, and `token_count`.

## Memory and Allocation Rules

The provider binds model state once during initialization. The inference kernel
accepts spans over already-bound token, activation, weight, and output buffers.
Hot-path work must avoid boxing, LINQ, closure capture, and per-token heap
allocation.

The CPU kernel is WebAssembly-compatible and avoids GC pressure entirely.

## Licensing

AIKernel.Control code is Apache-2.0 licensed.

Bonsai model weights, tokenizer files, llama.cpp/ggml references, and any other
third-party assets remain under their original licenses. AIKernel.Control does
not vendor those assets and does not grant redistribution rights for them. When
an operator mounts `/sys/roms/bonsai-1.7b/`, that operator is responsible for
ensuring the mounted assets can be used and redistributed in the target
environment.

Local operator path files such as `ref/env.txt` are environment-specific and
must not be committed.
