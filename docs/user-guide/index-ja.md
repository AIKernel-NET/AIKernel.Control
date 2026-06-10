# AIKernel.Control User Guide

このガイドは、AIKernel.Control を AIKernel semantic graph の物理実行レイヤー
として利用するための手順をまとめたものです。

Control は AIOS SDK の governance / security / physical execution layer です。
distribution は semantic graph を policy、scheduler、diagnostics、CPU / emulator
execution、optional GPU execution へ接続できます。

公式 AIOS ディストリビューション **AIKernel.Monolith** の開発も開始されています。
Monolith は 0.1.x 系の安定化後に semantic runtime、capability graph、governance を
体現する標準 reference distribution として位置づけられます。

## インストール

host の役割に合わせて Control package を導入します。

```bash
dotnet add package AIKernel.Control.Core --version 0.1.1
dotnet add package AIKernel.Control.CPU --version 0.1.1
dotnet add package AIKernel.Control.Emulator --version 0.1.1
dotnet add package AIKernel.Control.Diagnostics --version 0.1.1
```

concrete GPU backend を bind する host では `AIKernel.Control.GPU` も導入します。

```bash
dotnet add package AIKernel.Control.GPU --version 0.1.1
```

Python host では単一の governance wrapper package を使用します。

```bash
pip install aikernel-governance
```

## Runtime の役割

| Package | 役割 |
| --- | --- |
| `AIKernel.Control.Core` | Control-plane entry point と Bonsai provider contract。 |
| `AIKernel.Control.CPU` | 検証と CPU host 向けの決定論的 CPU execution kernel。 |
| `AIKernel.Control.Emulator` | step-by-step graph execution、replay、watch、breakpoint。 |
| `AIKernel.Control.Diagnostics` | timing、graph、replay inspection surface。 |
| `AIKernel.Control.GPU` | concrete GPU execution backend のための delegate boundary。 |

## Asset Mounting

Control は model weights や tokenizer asset を同梱しません。再現性を保つため、
asset は AIKernel VFS / ROM 境界を通じて mount します。

```text
/sys/roms/bonsai-1.7b/config.json
/sys/roms/bonsai-1.7b/tokenizer.json
/sys/roms/bonsai-1.7b/model.q1_0.bin
```

GPU execution delegate を有効化する前に、Emulator または CPU package で asset
visibility を検証してください。

## Python Wrapper

Python package は public governance surface を公開します。

```python
from aikernel_governance import ExecutionRequest, GovernanceClient
```

bundled managed assembly を読み込み、意味論は C# package へ委譲します。Python
wrapper を独立実装として扱わないでください。

## 検証

package 更新前に repository test を実行します。

```powershell
dotnet build AIKernel.Control.slnx -c Release -p:WarningsAsErrors=1591
dotnet test AIKernel.Control.slnx -c Release --no-build
py -m pytest python/tests
```

## Failure Behavior

asset、graph contract、execution delegate が一致しない場合、Control は
fail-closed で停止するべきです。不完全な成功よりも、明示的な diagnostics と
replay 可能な state を優先します。
