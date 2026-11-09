# AIKernel.Control Release Notes

[日本語](RELEASE_NOTES-ja.md)

## 0.1.3 (Unreleased)

- Align the Control, Core, CPU, Emulator, GPU, and Diagnostics packages with the 0.1.3 AIKernel.NET and Core package line. The GPU package provides a control boundary; it is not a native accelerator runtime.
- For `aikernel-governance`, rebuild managed DLLs and dependency manifests from the matching package graph and verify loading from the bundled files. A successful NuGet validation does not certify a PyPI wheel.

## 0.1.2

**June 16th, 2026 - Unified control package line.**

AIKernel.Control 0.1.2 aligns the Control execution layer with AIKernel.NET and AIKernel.Core 0.1.2.

- Publish `AIKernel.Control` as the dependency-only entry package for the standard Control surface.
- Keep split packages available for targeted hosts: `AIKernel.Control.Core`, `AIKernel.Control.CPU`, `AIKernel.Control.Emulator`, `AIKernel.Control.Diagnostics`, and `AIKernel.Control.GPU`.
- Resolve release validation through NuGet.org package references only.
- Publish the synchronized `aikernel-governance` Python wrapper through the 0.1.2 release flow.
- Preserve CTG Apply Policy integration while keeping Gate logic in AIKernel.Core.
