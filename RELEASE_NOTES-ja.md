# AIKernel.Control リリースノート

[English](RELEASE_NOTES.md)

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
