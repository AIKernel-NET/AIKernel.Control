from aikernel_governance import (
    AllowAllControlPolicy,
    Bonsai1BitCpuKernel,
    BonsaiBuiltInProvider,
    BonsaiModelConfig,
    BonsaiModelState,
    BonsaiProviderCapabilities,
    BonsaiTokenizer,
    ControlEmulatorEngine,
    DeterministicNodeScheduler,
    EmulatedExecutionGraph,
    EmulatedExecutionNode,
    ExecutionRequest,
    ExecutionResult,
    GovernanceClient,
    managed_api_summary,
    managed_type_names,
    ProviderContract,
    ReplayApprovalRecord,
    Snapshot,
    SnapshotMetadata,
    bonsai_gpu_execution_delegate_contract,
    governance_assemblies,
)


def test_public_import_surface():
    assert ExecutionRequest
    assert ExecutionResult
    assert Snapshot
    assert ProviderContract
    assert GovernanceClient
    assert BonsaiModelConfig
    assert BonsaiTokenizer
    assert BonsaiModelState
    assert BonsaiProviderCapabilities
    assert BonsaiBuiltInProvider
    assert Bonsai1BitCpuKernel
    assert ReplayApprovalRecord
    assert EmulatedExecutionNode
    assert EmulatedExecutionGraph
    assert DeterministicNodeScheduler
    assert AllowAllControlPolicy
    assert ControlEmulatorEngine
    assert bonsai_gpu_execution_delegate_contract


def test_bonsai_model_config_keeps_contract_fields():
    config = BonsaiModelConfig(
        layer_count=2,
        hidden_size=256,
        head_count=4,
        vocabulary_size=32000,
        context_length=128,
    )

    assert config.layer_count == 2
    assert config.hidden_size == 256
    assert config.head_count == 4
    assert config.vocabulary_size == 32000
    assert config.context_length == 128


def test_execution_request_metadata_is_deterministic():
    request = ExecutionRequest(
        model="bonsai-1.7b",
        input="hello",
        parameters={"execution_id": "exec-1", "temperature": "0"},
    )

    assert request.id == "exec-1"
    assert request.to_metadata() == {
        "model": "bonsai-1.7b",
        "input": "hello",
        "parameter.execution_id": "exec-1",
        "parameter.temperature": "0",
    }


def test_governance_client_delegates_to_backend():
    backend = RecordingBackend()
    client = GovernanceClient(backend)
    request = ExecutionRequest("m", "input", {"execution_id": "exec-2"})

    assert client.submit(request) == ExecutionResult("exec-2", "ok", "Completed")
    assert client.snapshot("exec-2") == Snapshot(
        "exec-2",
        "Completed",
        SnapshotMetadata({"source": "test"}),
    )
    assert client.result("exec-2") == ExecutionResult("exec-2", "ok", "Completed")


def test_emulator_graph_keeps_public_node_contract():
    node = EmulatedExecutionNode(
        node_id="node-1",
        operator_id="op.mock",
        metadata={"phase": "Generating"},
    )
    graph = EmulatedExecutionGraph("graph-1", [node])

    assert graph.graph_id == "graph-1"
    assert graph.nodes[0].node_id == "node-1"
    assert graph.nodes[0].operator_id == "op.mock"


def test_replay_approval_record_keeps_diagnostic_contract():
    record = ReplayApprovalRecord(
        replay_log_hash="sha256:replay",
        approved_by="operator",
        decision="Approved",
    )

    assert record.replay_log_hash == "sha256:replay"
    assert record.approved_by == "operator"
    assert record.decision == "Approved"


def test_governance_assembly_manifest_names():
    names = {path.name for path in governance_assemblies().assemblies}

    assert "AIKernel.Control.Core.dll" in names
    assert "AIKernel.Abstractions.dll" in names
    assert "AIKernel.Dtos.dll" in names
    assert "AIKernel.Enums.dll" in names


def test_managed_api_catalog_covers_control_surface():
    names = set(managed_type_names())
    summary = managed_api_summary()

    assert "AIKernel.Control.Core.Ctg.CtgControlCoordinator" in names
    assert "AIKernel.Control.Core.Ctg.CtgControlDecisionEnvelope" in names
    assert "AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider" in names
    assert summary["AIKernel.Control.Core"] > 0


class RecordingBackend:
    def submit(self, request):
        return ExecutionResult(request.id, "ok", "Completed")

    def snapshot(self, id):
        return Snapshot(id, "Completed", SnapshotMetadata({"source": "test"}))

    def result(self, id):
        return ExecutionResult(id, "ok", "Completed")
