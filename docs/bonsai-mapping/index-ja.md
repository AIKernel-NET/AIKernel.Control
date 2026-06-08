# Bonsai Mapping

[English](index.md)

AIKernel.Control は、Bonsai スタイルの Behavior Tree / Graph Execution を
AIKernel の Semantic Graph に対する物理実行モデルとして扱います。

## マッピング

- Bonsai Graph -> AIKernel Execution Graph
- Bonsai Node -> CPU Operator または GPU Kernel
- Bonsai edge / state transition -> 決定論的な AIKernel execution step
- Bonsai trace / watch / breakpoint -> ReplayLog 互換 diagnostics

ControlEmulator は execution engine です。Demo surface ではないため、この
mapping を Control 側で所有します。

## Built-in Provider

Bonsai-1.7B built-in provider は、`chat.local` と `text.tokenize` node を
標準 Bonsai inference boundary へマッピングします。

Model metadata は `/sys/roms/bonsai-1.7b/` に ROM として mount されます。
Provider は local file を直接読みません。

詳細は [Bonsai-1.7B Built-in Provider](bonsai-1.7b-provider-ja.md) を参照してください。
