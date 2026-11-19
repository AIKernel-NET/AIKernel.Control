# B4: Control境界のキャンセル判定

検証日: 2026-11-12〜13 (JST)。dev `2229477` から作成した `review/B4`。

CtgControlCoordinatorとControlEmulatorEngineの判定を、Core側で追加した
`AIKernel.Common.Results.CancellationExceptionClassifier` に集約します。
OperationCanceledException / TaskCanceledException / 独自派生型 / リンク済みトークンを同じ取消とします。
coordinatorは元の取消例外を伝播し、emulatorは従来どおりCanceled / CONTROL_CANCELEDを返します。
通常例外・TimeoutExceptionは従来の失敗のままです。

回帰テストを12件追加。既存テストの期待値は変更していません。
ControlはRelease build成功・54件成功。全.NETはbuild成功、1,185件成功、Doom既知失敗2件です。
全体の手順・結果はAIKernel.Coreの `docs/operations/REVIEW_B4-v0.1.3.md` にまとめています。
証跡は `D:\AIKernel\.codex_tmp\review-b4-20261112` です。

Core / Providers / Control / Wasmのreview/B4は一緒にレビューし、0.1.4候補ではCommonと消費側の
版・依存下限を揃えてください。新しい消費側DLLを公開済み0.1.3 Commonだけで動かすことはできません。
検証用のローカル0.1.3再packは配布物ではありません。

レビュー待ちのreview/CT1は保持し、このdevからの変更に混ぜていません。
dev/mainへの取り込み、push、PR、タグ、公開は行いません。
