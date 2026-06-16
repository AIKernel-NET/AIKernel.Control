# AIKernel.Control ドキュメント

AIKernel.Control は、AIKernel の意味論的グラフを物理実行へ接続する
Execution Layer です。.NET host で Control engine を実行、検査、
パッケージ化する場合は、このドキュメントを起点にしてください。

この docs は、AIOS SDK の governance / security / physical execution layer として
Control を説明します。Core の semantic graph を明示的な policy、決定論的
scheduler、diagnostics、execution engine へ接続します。

公式 AIOS ディストリビューション **AIKernel.Monolith** の開発も開始されています。
Monolith は 0.1.x 系の安定化後に control plane と Semantic OS layer を統合する
標準 reference distribution として位置づけられます。

## リポジトリ横断整合

共有の repository boundary、v0.1.2 development versioning、依存関係順、
PyPI Trusted Publishing、Python wrapper scope は
[Package Release Alignment v0.1.2](https://github.com/AIKernel-NET/AIKernel.NET/blob/main/docs/development/package-release-alignment-v0.1.2-ja.md)
で定義します。履歴としての v0.1.1.1 validation rule は
[AIKernel Repository Alignment v0.1.1.1](https://github.com/AIKernel-NET/AIKernel.NET/blob/main/docs/development/repository-alignment-v0.1.1.1-ja.md)
に残します。
複数 repository をまたぐ変更を行う場合は、まず
[リポジトリ横断開発者ガイド v0.1.1.1](https://github.com/AIKernel-NET/AIKernel.NET/blob/main/docs/development/cross-repository-developer-guide-v0.1.1.1-ja.md)
を読んでください。

Control は orchestration、policy application、Core gate invocation、runtime control、
execution coordination を所有します。Decision Gate / Trajectory Gate truth table、
Provider semantic evaluation、browser runtime execution を再実装しません。

## 最初に読むもの

- [User Guide](user-guide/index-ja.md)
- [Architecture](architecture/index-ja.md)
- [Bonsai mapping](bonsai-mapping/index-ja.md)
- [Bonsai-1.7B built-in provider](bonsai-mapping/bonsai-1.7b-provider-ja.md)
- [Execution engine](execution-engine/index-ja.md)
- [Q1_0 CPU execution kernel](execution-engine/q1-0-cpu-kernel-ja.md)
- [Control pipelines](pipelines/index-ja.md)
- [CTG Control integration](development/control-ctg-ja.md)
- [Concept Elevation Notes / 概念昇格ノート](development/concept-elevation.md)
- [Python governance wrapper](python/index-ja.md)
- [Licensing](licensing/index-ja.md)

## どのページを読むべきか

- install command と最小の deterministic emulator / CPU validation path を確認する場合は
  User Guide を読んでください。
- semantic graph は Core が所有し、physical execution engine は Control が所有する理由を
  確認する場合は Architecture を読んでください。
- Bonsai-style graph を emulator、CPU、GPU execution へ接続する前に Bonsai mapping を
  読んでください。
- Apply Policy stage で Core CTG gate evaluation を opt-in 接続する場合は
  CTG Control integration を読んでください。
- Python から Control を利用する場合は、`aikernel-governance` が managed assembly
  への薄い bridge であることを確認してください。Python 側で CTG Gate logic を
  再実装してはいけません。

## 最初の検証

GPU execution を bind する前に、CPU と Emulator の validation から始めます。

```powershell
dotnet build AIKernel.Control.slnx -c Release
dotnet test AIKernel.Control.slnx -c Release --no-build
```

## 運用チェックリスト

- 対応する `AIKernel.Control.*` package を同じ version family で導入します。
- v0.1.2 development では、local NuGet package version は
  `0.1.2-dev{buildNumber}`、local Python wheel version は
  `0.1.2.dev{buildNumber}` のような形式を使います。
- model asset はローカルの場当たり的な path ではなく VFS / ROM 経由で mount
  します。
- GPU execution を bind する前に、CPU / Emulator package で決定論的な検証を
  行います。
- `aikernel-governance` は managed assemblies の薄い wrapper に保ち、stable publication
  が明示的に開始された場合だけ v0.1.2 Trusted Publishing flow で公開します。
