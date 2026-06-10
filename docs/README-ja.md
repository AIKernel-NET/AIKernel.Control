# AIKernel.Control ドキュメント

AIKernel.Control は、AIKernel の意味論的グラフを物理実行へ接続する
Execution Layer です。.NET / Python host で Control engine を実行、検査、
パッケージ化する場合は、このドキュメントを起点にしてください。

この docs は、AIOS SDK の governance / security / physical execution layer として
Control を説明します。Core の semantic graph を明示的な policy、決定論的
scheduler、diagnostics、execution engine へ接続します。

公式 AIOS ディストリビューション **AIKernel.Monolith** の開発も開始されています。
Monolith は 0.1.x 系の安定化後に control plane と Semantic OS layer を統合する
標準 reference distribution として位置づけられます。

## 最初に読むもの

- [User Guide](user-guide/index-ja.md)
- [Architecture](architecture/index-ja.md)
- [Bonsai mapping](bonsai-mapping/index-ja.md)
- [Bonsai-1.7B built-in provider](bonsai-mapping/bonsai-1.7b-provider-ja.md)
- [Execution engine](execution-engine/index-ja.md)
- [Q1_0 CPU execution kernel](execution-engine/q1-0-cpu-kernel-ja.md)
- [Control pipelines](pipelines/index-ja.md)
- [Python governance wrapper](python/index-ja.md)
- [Licensing](licensing/index-ja.md)

## どのページを読むべきか

- install command と最小の deterministic emulator / CPU validation path を確認する場合は
  User Guide を読んでください。
- semantic graph は Core が所有し、physical execution engine は Control が所有する理由を
  確認する場合は Architecture を読んでください。
- Bonsai-style graph を emulator、CPU、GPU execution へ接続する前に Bonsai mapping を
  読んでください。
- Python から Control を利用する場合は、Python governance wrapper が managed assembly
  への薄い bridge であることを確認してください。

## 最初の検証

GPU execution を bind する前に、CPU と Emulator の validation から始めます。

```powershell
dotnet build AIKernel.Control.slnx -c Release
dotnet test AIKernel.Control.slnx -c Release --no-build
```

## 運用チェックリスト

- 対応する `AIKernel.Control.*` package を同じ version family で導入します。
- model asset はローカルの場当たり的な path ではなく VFS / ROM 経由で mount
  します。
- GPU execution を bind する前に、CPU / Emulator package で決定論的な検証を
  行います。
- Python wrapper は managed assembly への薄い bridge として扱います。
