# Control Pipelines

[English](index.md)

AIKernel の Control Pipeline は有限で、決定論的で、完全に観測可能です。
Control Pipeline は、AIKernel.Core から渡される semantic `ExecutionGraph` を、
policy enforcement と replayable state transition を伴って、物理実行エンジン
（CPU / GPU / Emulator）へマッピングします。

初期 0.1.1 pipeline は、次の 3 つの決定論的 phase で構成されます。

1. **Receive Envelope**  
   Engine は `ControlExecutionRequest` と対応する `ExecutionGraph` を受け取ります。
   Request が完全に validation され、graph が freeze されるまで、物理実行は開始されません。

2. **Apply Policy**  
   `IControlPolicy` 実装が request と各 node metadata を評価します。Policy は execution を
   allow、deny、abort できます。すべての policy decision は replay / audit stream に記録されます。

3. **Execute & Emit Replay/Audit Metadata**  
   Deterministic scheduler は contract identity に基づいて node を並べます。各 node transition は
   `ControlStateSnapshot`（`Scheduled`, `Running`, `Completed`, `Faulted`）を emit します。
   これにより、Demo や Core に execution-engine logic を埋め込まずに、execution 全体を
   replay 可能にします。

   Contract identity とは、`NodeId` と `OperatorId` の lexicographical ordering を
   意味します。

   Snapshot phase:

   - `Scheduled`
   - `Running`
   - `Completed`
   - `Faulted`
