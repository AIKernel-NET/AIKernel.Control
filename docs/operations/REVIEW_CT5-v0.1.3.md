# CT5: Bonsai CPUカーネルの命令セット判定と数値結果の整合

検証日: 2026-11-13。Controlのdev `2229477`から作成した`review/CT5`の変更です。
CT4等の他のレビュー待ちブランチは取り込んでいません。

## 再現

`KernelId`は`Avx2.IsSupported`で識別子を選ぶ一方、`DotBlockSigns`は
`Avx.IsSupported`でSIMD処理に入っていました。AVX2だけを無効にすると、
`bonsai.q1_0.cpu.scalar`を返しながら256ビットのSIMD演算を実行します。

加算順序も異なります。SIMD側は正負別・レーン別に累積して最後に足し合わせますが、
スカラー側は入力順に累積します。例えば全符号を正、入力の0/8/16/24番を
`16777216, 1, -16777216, 1`、残りを0、scaleを1にすると、
スカラーの結果は1、旧SIMDの結果は0になります。旧SIMDのMax/MinはNaNも失わせます。

改修前の追加23ケースは、AVX2有効・AVXのみの両方で6件失敗、17件成功、
AVX無効では23件成功でした。命令セットの有効状態とKernelIdをテスト出力に記録し、
JITの出力でもAVXのみの条件に256ビット命令があることを確認しました。

## 変更

- KernelIdと演算の分岐を同じ非公開`UseAvx2`判定に統一しました。
  公開済みの`bonsai.q1_0.cpu.avx2` / `bonsai.q1_0.cpu.scalar`という文字列を維持します。
  AVXのみの環境ではスカラー経路を選びます。
- SIMD側は符号ビットのXORで符号を適用し、入力順に加算します。
  ゼロやNaNの符号・ペイロードを含めてスカラーの符号処理に合わせ、並列累積を除去しました。
  両経路で符号付き値を1ブロック分のスタック領域（1 KiB）へ書き、同じループで累積します。
  これによりJITの段階ごとに別の加算式が異なるNaNオペランドを返す差も避けます。
- スカラーの演算順序、ブロックごとのscale適用と合算は維持しました。
  公開API、DTO、enum、Canon、ハッシュ計算、既存テストは変更していません。

旧SIMDで丸め結果が異なっていた入力は、今回から既存スカラーの結果になります。
この数値修正はCT5のビット一致条件の一部です。識別子の変更による対応は行いません。
入力順の累積による性能への影響は未測定です。

## 追加テスト

`BonsaiCpuParityTests`に23ケースを追加しました。許容誤差でなくfloatのビット列を比較します。

- 実行時のAVX/AVX2状態とKernelId、および検証プロセスへのISA設定の反映
- 丸め誤差が出る固定例、全256種類の符号パターン
- 固定シードの512例（1～8ブロック、混在する符号・scale・指数）
- 正負ゼロ、非正規化数、最大有限値、無限大、正負のquiet/signaling NaN
- 複数ブロックのゼロ・負・最小・最大・無限大・NaNのscale、空入力

## 検証結果

WindowsでControlと下流4リポジトリのrestore/buildが成功しました。
テスト367件成功、Doomの既知の2件のみ失敗。Controlのbuildは警告・エラーともに0件です。

| リポジトリ | 成功 | 失敗 |
| --- | ---: | ---: |
| Control | 65 | 0 |
| Tools | 34 | 0 |
| Wasm | 64 | 0 |
| Demo | 71 | 0 |
| Doom | 133 | 2（既知） |

追加23ケースはAVX2有効・AVXのみ・AVX無効の3条件で成功しました。
各条件で段階的JIT有効/無効を試し、計138件の実行がすべて成功しました。
有効ISAの検査も通過し、最適化済みJIT命令列ではAVX2有効時だけ符号処理に256ビット命令を使います。

Doomの既知の失敗は以下の2件で、前回PF3の記録とテスト名・成否・メッセージが一致しました。

- `DoomWebGpuProvider_SeparatesRawFramebufferAndHudCompositeTargets`
- `DoomPrompt_UsesCanonicalAisthesisNoesisKrisisKinesisPanelMapping`

どちらも`const HUD_UNIFORM_FLOAT_COUNT = 32`を探す既存Assert.Containsの失敗です。
対象5リポジトリの既存テストの成否も照合し、新しい失敗や削除がないことを確認しました。

AIKernel.Control.CPU.dllの生成元・候補nupkg・隔離キャッシュ・出力4か所がSHA-256で一致しました。
CPUパッケージを直接参照するDemoでも候補DLLが参照されていることを確認しました。
SHA-256: `47473f5f19b329682d4bfdaff01b72c0912b05d95681109290624c937c7ed0f1`。

集計は`verified-results.json`、全体試験は`results/*.trx`です。
途中のNaN不一致の記録は`initial-attempt`へ保存し、最終集計と分離しています。

次のレビュー項目の候補はWS4（WebGPUのバッファ・ディスパッチ上限）です。

## 再実行

ControlのReleaseビルド後、以下の3条件で同じテストを実行します。
AVX2を利用できるx64ホストでの検証コマンドです。
`--environment`はtesthostだけに適用し、ホストのグローバル設定は変更しません。
最適化済みコードでの比較には、それぞれに`--environment DOTNET_TieredCompilation=0`を追加します。

```powershell
dotnet test tests/AIKernel.Control.Tests/AIKernel.Control.Tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~BonsaiCpuParityTests --environment AIKERNEL_CT5_EXPECT_ISA=avx2
dotnet test tests/AIKernel.Control.Tests/AIKernel.Control.Tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~BonsaiCpuParityTests --environment DOTNET_EnableAVX2=0 --environment AIKERNEL_CT5_EXPECT_ISA=avx-only
dotnet test tests/AIKernel.Control.Tests/AIKernel.Control.Tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~BonsaiCpuParityTests --environment DOTNET_EnableAVX=0 --environment AIKERNEL_CT5_EXPECT_ISA=scalar
```

証跡: `D:\AIKernel\.codex_tmp\review-ct5-20261113`。
`before-isa`が改修前、`final-isa`と`tiered-isa`が改修後のTRX、`*-jit.log`が実際のJIT命令列です。
Controlと下流4リポジトリは専用フィード・キャッシュで検証し、他のレビュー候補を混ぜません。
上流のNET/Core/Providers/Cuda13.0は変更していません。
検証用パッケージは隔離フィード内だけの0.1.3で、公開用ではありません。
Linux、ARM、性能、実モデル推論は未検証です。

## 別途確認が必要な点

`Forward`の行数計算は`Q1Weights.Length / Q1BlockBytes`でブロック数を使うため、
隠れ層幅512、重み68バイト（1行）、logits長2の状態では2行と判断し、
2行目の重みを切り出す際に`ArgumentOutOfRangeException`になります。
実際の行幅`rowBytes`で算定するか、不完全なモデル状態として先に拒否するかの整理が必要です。
この計算は今回の差分に含まれない既存箇所で、一時プローブで再現しました。
証跡は`boundary-probe.log`です。新しく見つけた問題を直さず報告する作業条件に従い、未改修です。

push、PR、dev/mainへの統合、タグ作成、パッケージ公開は行いません。
