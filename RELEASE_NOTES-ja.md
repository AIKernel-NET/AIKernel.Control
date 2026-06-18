# AIKernel.Control リリースノート

[English](RELEASE_NOTES.md)

## 0.1.2

**2026年6月16日 - 統一 Control package line。**

AIKernel.Control 0.1.2 は、Control execution layer を AIKernel.NET / AIKernel.Core 0.1.2 に揃えます。

- 標準 Control surface の dependency-only entry package として `AIKernel.Control` を公開します。
- targeted host 向けに `AIKernel.Control.Core`、`AIKernel.Control.CPU`、`AIKernel.Control.Emulator`、`AIKernel.Control.Diagnostics`、`AIKernel.Control.GPU` の split package を維持します。
- release validation は NuGet.org package reference のみで解決します。
- 同期 Python wrapper `aikernel-governance` を 0.1.2 release flow で公開します。
- Gate logic を AIKernel.Core に保持しながら CTG Apply Policy integration を維持します。