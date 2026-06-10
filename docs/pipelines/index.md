# Control Pipelines

[日本語](index-ja.md)

Control pipelines in AIKernel are finite, deterministic, and fully observable.
A control pipeline maps a semantic `ExecutionGraph` (from AIKernel.Core) onto a
physical execution engine (CPU/GPU/Emulator) with policy enforcement and
replayable state transitions.

The initial 0.1.1 pipeline consists of three deterministic phases:

1. **Receive Envelope**  
   The engine receives a `ControlExecutionRequest` and the associated
   `ExecutionGraph`. No physical execution occurs before the request is fully
   validated and the graph is frozen.

2. **Apply Policy**  
   An `IControlPolicy` implementation evaluates the request and each node's
   metadata. The policy may allow, deny, or abort execution. All policy
   decisions are recorded in the replay/audit stream.

3. **Execute & Emit Replay/Audit Metadata**  
   The deterministic scheduler orders nodes by contract identity. Each node
   transition emits a `ControlStateSnapshot` (`Scheduled`, `Running`,
   `Completed`, `Faulted`) so the entire execution is replayable without
   embedding execution-engine logic in demos or Core.

   Contract identity means lexicographical ordering of `NodeId` and `OperatorId`.

   Snapshot phases:

   - `Scheduled`
   - `Running`
   - `Completed`
   - `Faulted`
