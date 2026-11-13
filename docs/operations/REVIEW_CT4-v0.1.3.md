# CT4: 未知のGPUバックエンドをCPUへフォールバック

検証日: 2026-11-13。対象はControlのCT4のみです。
dev `2229477` 起点の `review/CT4` で改修します。
B4、PF3、CUDA等のレビュー待ちブランチは取り込んでいません。

## 再現と変更

`GpuExecutionArbitrator.Resolve`は、条件を満たす要求のBackendがUnknownの場合に
WebGpuへ置き換えてUseGpu=trueを返していました。未定義のenum値もそのままGPU承認に進みます。
改修前の追加テストでは、この問題の11ケースが失敗し、既知GPUと既存フォールバックの20ケースは成功しました。

Unknown、未定義値、Backendの未指定時は既存のCpuヘルパーで次の結果を返します。

- UseGpu=false、Backend=CpuFallback、UsingCpuFallback=true
- Reason=`unknown-backend`、Tags=`cpu,unknown-backend`
- 診断メタデータのbackend/fallback/reason/execution modeもCPUフォールバックを示す

CPU強制、GPU喪失、能力不足、zero-copy、raw target、native pass bridge、readbackの
既存の判定と理由を優先し、GPU承認の直前で未知値を検査します。
WebGpu、Dawn、Cuda、Vulkan、Direct3D12、Metalの既存の承認は維持します。
公開DTO・enum・メソッド・理由定数の変更や追加はありません。
Canon、StepId、ReplayLogHash等の計算にも触れていません。

## 追加テスト

`GpuUnknownBackendTests`に31ケースを追加しました。

- Unknown、-1、8、int.MinValue、int.MaxValue × 必要能力なし/Computeの10ケース
- Backendを省略した要求の1ケース
- 既知GPUバックエンド6種類の承認と診断メタデータの維持
- 既存フォールバック理由7種類 × Unknown/未定義値の14ケース

フォールバック結果では、要求のID・pass・能力・フレーム種別・readback/bridge設定の保持と、
診断メタデータの既存バリデーターへの適合も検証します。既存テストは変更していません。

## 検証結果

WindowsでControlと下流4リポジトリのrestore/buildが成功しました。
テストは375件成功、Doomの既知の2件のみ失敗しました。Controlのbuildは警告・エラーともに0件です。

| リポジトリ | 成功 | 失敗 |
| --- | ---: | ---: |
| Control | 73 | 0 |
| Tools | 34 | 0 |
| Wasm | 64 | 0 |
| Demo | 71 | 0 |
| Doom | 133 | 2（既知） |

既知の失敗は以下の2件で、前回PF3の記録とテスト名・成否・エラーメッセージが一致しました。

- `DoomWebGpuProvider_SeparatesRawFramebufferAndHudCompositeTargets`
- `DoomPrompt_UsesCanonicalAisthesisNoesisKrisisKinesisPanelMapping`

どちらも`const HUD_UNIFORM_FLOAT_COUNT = 32`を探す既存Assert.Containsの失敗です。
対象5リポジトリの既存テストの成否も前回の記録と照合し、新しい失敗や削除がないことを確認しました。

生成元DLL・候補nupkg・隔離NuGetキャッシュ・ソリューション出力6か所の
AIKernel.Control.GPU.dllがSHA-256で一致しました。ToolsとDemoで新しいDLLを使っていることも確認しました。
SHA-256: `0b597c28c9780bfad4e7bf044490e06d4470ff6903174aa4d8ccba7cdb5d1bb2`。

機械可読の集計は`verified-results.json`、全体試験は`results/*.trx`、
改修前の再現は`reproduction/reproduction.trx`です。
LinuxとGitHub CI、GPU実機での実行は検証していません。今回は判定ロジックと.NETの互換性検証です。
検証用パッケージは現行の0.1.3設定による隔離フィード用で、公開・本番導入用ではありません。

証跡: `D:\AIKernel\.codex_tmp\review-ct4-20261113`。
Controlはこのレビュー系列、Tools/Wasm/Demo/Doomはdevの隔離コピーを使います。
候補のAIKernel.Control.GPUパッケージは専用フィード・キャッシュに置き、下流のDLLと照合します。
上流のNET/Core/Providers/Cuda13.0は変更せず、参照するパッケージも既存の0.1.3です。

## 別途確認が必要な点

明示的な`GpuBackend.CpuFallback`は定義済み値であり、他の条件が揃うと現行コードは
`Backend=CpuFallback`かつ`UseGpu=true`に到達します。これはUnknown/未定義値とは別の問題です。
「作業中に見つけた新しい問題は直さず報告する」という作業条件に従い、今回変更していません。

次の既存レビュー項目の候補はCT5（Bonsai1BitCpuKernelのAVX分岐とKernelIdの整合）です。
dev/main、既存reviewブランチ、ユーザーの未コミット変更は保持します。
push、PR、devへの統合、タグ、パッケージ公開は行いません。
