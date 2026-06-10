# AIKernel.Control ドキュメント

AIKernel.Control は、AIKernel の意味論的グラフを物理実行へ接続する
Execution Layer です。.NET / Python host で Control engine を実行、検査、
パッケージ化する場合は、このドキュメントを起点にしてください。

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

## 運用チェックリスト

- 対応する `AIKernel.Control.*` package を同じ version family で導入します。
- model asset はローカルの場当たり的な path ではなく VFS / ROM 経由で mount
  します。
- GPU execution を bind する前に、CPU / Emulator package で決定論的な検証を
  行います。
- Python wrapper は managed assembly への薄い bridge として扱います。
