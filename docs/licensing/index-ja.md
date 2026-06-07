# ライセンス

[English](index.md)

AIKernel.Control は Apache License 2.0 で提供されます。

## なぜ Apache 2.0 か

Control は execution-engine logic、physical scheduling boundary、CPU/GPU mapping、
model-runtime integration point を含みます。これらは実装 surface であるため、
商用・研究用途で採用しやすいように、特許条項を持つ Apache 2.0 を採用します。

## AIKernel.NET との関係

AIKernel.NET の contract package は、interface、DTO、enum、schema contract のみを
含むため MIT licensed です。AIKernel.Control はそれらの contract を利用しますが、
その license を変更しません。

要約:

- AIKernel.NET contracts: MIT
- AIKernel.Control implementation: Apache-2.0
- GPU / native Capability repositories: Apache-2.0

## Third-party Assets

AIKernel.Control は third-party model / tokenizer asset を再ライセンスしません。
Bonsai model weights、tokenizer files、llama.cpp / ggml references、その他の
external ROM assets は、それぞれの元ライセンス条件に従って使用する必要があります。

Downstream package が third-party binary や source file を vendor する場合は、
対象 asset に必要な license file と notice file を含めてください。

## Local Environment Files

`ref/env.txt` のような local environment file は package contract の一部ではなく、
commit してはいけません。Documentation では用途に触れてもかまいませんが、
repository content が local absolute path に依存してはいけません。
