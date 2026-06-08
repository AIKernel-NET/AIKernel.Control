# Bonsai-1.7B Built-in Provider

[English](bonsai-1.7b-provider.md)

`BonsaiBuiltInProvider` は Bonsai-1.7B の標準 Control Plane Provider です。
AIKernel.NET の provider / VFS / capability / control-state contract を使いながら、
model lifecycle は AIKernel.Control 内に閉じます。

Bonsai-1.7B は、CPU / WASM execution を想定した compact な 1-bit quantized model です。

## ROM Layout

Provider は `IVfsProvider` 経由で Bonsai asset を読み込みます。直接の file I/O は
runtime contract に含まれません。

必須 ROM file:

- `/sys/roms/bonsai-1.7b/config.json`
- `/sys/roms/bonsai-1.7b/tokenizer.json`

任意 ROM file:

- `/sys/roms/bonsai-1.7b/model.q1_0.bin`

`model.q1_0.bin` が存在しない場合、Provider は初期化時に決定論的な bootstrap
weights を生成します。この fallback は contract validation、emulator smoke test、
初期統合のためのものです。Production inference では、実際の ROM-bound Q1_0
weight payload を VFS 経由で提供してください。

## Lifecycle

初期化は決定論的な phase sequence に従います。

- `ModelDownload`: ROM asset を VFS 経由で要求します。
- `Initializing`: config、tokenizer、tensor buffer を parse / bind します。
- `Ready`: model state が完全に bind され immutable になり、inference kernel が
  node を実行可能になります。
- `Generating`: execution node を評価しています。

各 phase は `IControlStateObserver` を通じて emit されます。これにより、Demo や
Core に execution-engine logic を埋め込まず、kernel timeline が物理実行を
観測できます。

## Capabilities

Provider は次の capability を宣言します。

- `chat.local`
- `text.tokenize`

`ExecuteNodeAsync` は要求された node operation を評価し、決定論的 metadata を持つ
`ControlExecutionResult` を返します。物理推論は `IBonsaiInferenceKernel` へ
委譲されるため、CPU backend と GPU backend は同じ provider contract を共有できます。

`ExecuteNodeAsync` は `provider_id`、`operation`、`token_count` などの決定論的
metadata を返します。

## Memory and Allocation Rules

Provider は初期化時に一度だけ model state を bind します。Inference kernel は、
すでに bind された token / activation / weight / output buffer に対する span を
受け取ります。

hot path では次を避ける必要があります。

- boxing
- LINQ
- closure capture
- per-token heap allocation

CPU kernel は WebAssembly-compatible であり、GC pressure を完全に避けます。

## Licensing

AIKernel.Control のコードは Apache-2.0 です。

Bonsai model weights、tokenizer files、llama.cpp / ggml references、その他の
third-party assets は元ライセンスを維持します。AIKernel.Control はそれらを
同梱せず、再配布権も付与しません。

`/sys/roms/bonsai-1.7b/` を mount する operator は、対象 asset を利用・再配布
できることを自分の target environment で確認する責任があります。

`ref/env.txt` のような local operator path file は環境依存であり、commit しては
いけません。
