# Licensing

[日本語](index-ja.md)

AIKernel.Control is licensed under the Apache License 2.0.

## Why Apache 2.0

Control contains execution-engine logic, physical scheduling boundaries,
CPU/GPU mappings, and model-runtime integration points. These are implementation
surfaces, so Apache 2.0 provides the patent language needed for adoption by
commercial and research users.

## Relationship to AIKernel.NET

AIKernel.NET contract packages are MIT licensed because they contain interfaces,
DTOs, enums, and schema contracts only. AIKernel.Control consumes those
contracts but does not change their license.

In short:

- AIKernel.NET contracts: MIT
- AIKernel.Control implementation: Apache-2.0
- GPU/native Capability repositories: Apache-2.0

## Third-party Assets

AIKernel.Control does not relicense third-party model or tokenizer assets.
Bonsai model weights, tokenizer files, llama.cpp/ggml references, and any other
external ROM assets must be used under their original license terms.

If a downstream package vendors third-party binaries or source files, it must
include the required license and notice files for those assets.

## Local Environment Files

Local environment files, including path notes such as `ref/env.txt`, are not
part of the package contract and must not be committed. Documentation may refer
to their purpose, but repository content must not depend on local absolute
paths.
