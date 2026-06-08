# AIKernel.Control リリースノート

[English](RELEASE_NOTES.md)

## 0.1.0

> [EN] Control 0.1.0 defines the public governance surface: execution requests, snapshots, and results become contract-stable.
>
> [JA] Control 0.1.0 は公開ガバナンス面を定義──Execution Request・Snapshot・Result が契約的に安定化される。

AIKernel.Control 0.1.0 は、AIKernel Semantic Runtime の物理実行レイヤーを導入します。

- AIKernel.NET の public Control contract を利用します。対象は execution graph、node、request、result、policy、scheduler、state observer です。
- CPU-only graph execution、breakpoint、watch、trace、replay-oriented test のための deterministic ControlEmulator を提供します。
- allocation-free Q1_0 kernel と VFS/ROM model asset ownership を備えた、CPU 側 Bonsai 1.7B built-in provider boundary を追加します。
- GPU execution は Control.GPU boundary に閉じ込め、device-specific execution が Core や Demo に漏れないようにします。
- public governance surface を単一 API として公開する `aikernel-governance` Python package を追加します。managed assemblies の同梱と pythonnet loading を含みます。
- Core は semantic graph、Control は physical execution mapping、Demo は runtime consumer である、という責務分離を文書化します。

Control 0.1.0 は、統治された semantic graph を決定論的な物理実行へ接続する橋です。
