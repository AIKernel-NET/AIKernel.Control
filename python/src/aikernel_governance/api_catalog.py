"""[EN]
Static public managed API catalog generated from the C# source tree.

[JA]
C# source tree から生成した public managed API の静的 catalog です。
"""

from __future__ import annotations

from dataclasses import dataclass


@dataclass(frozen=True)
class ManagedMemberDescriptor:
    """[EN] Describes one public managed member discovered from C# source.

    [JA] C# source から検出した public managed member を表します。
    """

    kind: str
    name: str
    signature: str


@dataclass(frozen=True)
class ManagedTypeDescriptor:
    """[EN] Describes one public managed type exposed by the package.

    [JA] package が公開する public managed type を表します。
    """

    namespace: str
    name: str
    kind: str
    assembly: str
    source: str
    members: tuple[ManagedMemberDescriptor, ...] = ()

    @property
    def full_name(self) -> str:
        """[EN] Return the namespace-qualified managed type name.

        [JA] namespace で修飾された managed type 名を返します。
        """
        return f"{self.namespace}.{self.name}" if self.namespace else self.name


_CATALOG: tuple[ManagedTypeDescriptor, ...] = (
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.CPU',
        name='Bonsai1BitCpuKernel',
        kind='class',
        assembly='AIKernel.Control.CPU',
        source='AIKernel.Control/src/AIKernel.Control.CPU/Bonsai1BitCpuKernel.cs',
        members=(
            ManagedMemberDescriptor('method', 'Forward', 'public void Forward('),
            ManagedMemberDescriptor('method', 'PrepareActivation', 'PrepareActivation(inputTokenIds, state.ActivationBuffer.AsSpan(0, hidden));'),
            ManagedMemberDescriptor('method', 'DotRowQ1_0', 'public static float DotRowQ1_0('),
            ManagedMemberDescriptor('method', 'ArgumentException', 'throw new ArgumentException("Input length must be a multiple of the Q1_0 block width.", nameof(input));'),
            ManagedMemberDescriptor('method', 'ArgumentException', 'throw new ArgumentException("Q1_0 row is shorter than the input vector.", nameof(q1Blocks));'),
            ManagedMemberDescriptor('method', 'DequantizeRowQ1_0', 'public static void DequantizeRowQ1_0('),
            ManagedMemberDescriptor('method', 'ArgumentException', 'throw new ArgumentException("Destination length must be a multiple of the Q1_0 block width.", nameof(destination));'),
            ManagedMemberDescriptor('method', 'ArgumentException', 'throw new ArgumentException("Q1_0 source is shorter than the destination row.", nameof(q1Blocks));'),
            ManagedMemberDescriptor('method', 'HorizontalAdd', 'return HorizontalAdd(Avx.Add(positive, negative));'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Bonsai',
        name='BonsaiBuiltInProvider',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Bonsai/BonsaiBuiltInProvider.cs',
        members=(
            ManagedMemberDescriptor('property', 'ModelRoot', 'public string ModelRoot { get; }'),
            ManagedMemberDescriptor('method', 'InitializeAsync', 'public async Task InitializeAsync(IVfsProvider vfs)'),
            ManagedMemberDescriptor('method', 'ObserveAsync', 'await ObserveAsync("bonsai.init", "ModelDownload", "Loading Bonsai ROM assets from VFS.").ConfigureAwait(false);'),
            ManagedMemberDescriptor('method', 'ObserveAsync', 'await ObserveAsync("bonsai.init", "Initializing", "Parsing topology and binding deterministic buffers.").ConfigureAwait(false);'),
            ManagedMemberDescriptor('method', 'CreateDeterministicWeights', '?? CreateDeterministicWeights(config, _kernel.Q1BlockElementCount, _kernel.Q1BlockByteCount);'),
            ManagedMemberDescriptor('method', 'ObserveAsync', 'await ObserveAsync("bonsai.init", "Ready", "Bonsai model buffers are bound.").ConfigureAwait(false);'),
            ManagedMemberDescriptor('method', 'InitializeAsync', 'public Task InitializeAsync()'),
            ManagedMemberDescriptor('method', 'InvalidOperationException', 'throw new InvalidOperationException('),
            ManagedMemberDescriptor('method', 'ShutdownAsync', 'public Task ShutdownAsync()'),
            ManagedMemberDescriptor('method', 'IsAvailableAsync', 'public Task<bool> IsAvailableAsync()'),
            ManagedMemberDescriptor('method', 'GetHealthAsync', 'public Task<ProviderHealthStatus> GetHealthAsync()'),
            ManagedMemberDescriptor('method', 'GetCapabilities', 'public IProviderCapabilities GetCapabilities()'),
            ManagedMemberDescriptor('method', 'ExecuteNodeAsync', 'public async ValueTask<ControlExecutionResult> ExecuteNodeAsync('),
            ManagedMemberDescriptor('method', 'InvalidOperationException', '?? throw new InvalidOperationException("Bonsai model is not initialized.");'),
            ManagedMemberDescriptor('method', 'ObserveAsync', 'await ObserveAsync(node.NodeId, "Generating", "Executing Bonsai node.").ConfigureAwait(false);'),
            ManagedMemberDescriptor('method', 'ControlExecutionResult', 'return new ControlExecutionResult('),
            ManagedMemberDescriptor('method', 'ControlStateSnapshot', 'new ControlStateSnapshot('),
            ManagedMemberDescriptor('property', 'Parameters', 'public IReadOnlyDictionary<string, object>? Parameters { get; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Bonsai',
        name='BonsaiModelConfig',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Bonsai/BonsaiModelConfig.cs',
        members=(
            ManagedMemberDescriptor('method', 'Parse', 'public static BonsaiModelConfig Parse(string json)'),
            ManagedMemberDescriptor('method', 'ReadRequiredInt32', 'ReadRequiredInt32(root, "num_hidden_layers", "n_layer", "n_layers", "layer_count"),'),
            ManagedMemberDescriptor('method', 'ReadRequiredInt32', 'ReadRequiredInt32(root, "hidden_size", "n_embd", "embedding_length"),'),
            ManagedMemberDescriptor('method', 'ReadRequiredInt32', 'ReadRequiredInt32(root, "num_attention_heads", "n_head", "head_count"),'),
            ManagedMemberDescriptor('method', 'ReadRequiredInt32', 'ReadRequiredInt32(root, "vocab_size", "vocabulary_size", "n_vocab"),'),
            ManagedMemberDescriptor('method', 'ReadRequiredInt32', 'ReadRequiredInt32(root, "max_position_embeddings", "context_length", "n_ctx"));'),
            ManagedMemberDescriptor('method', 'EstimateActivationFloatCount', 'public long EstimateActivationFloatCount()'),
            ManagedMemberDescriptor('method', 'return', 'return (long)HiddenSize * Math.Max(1, ContextLength);'),
            ManagedMemberDescriptor('method', 'InvalidDataException', 'throw new InvalidDataException('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Bonsai',
        name='BonsaiModelState',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Bonsai/BonsaiModelState.cs',
        members=(
            ManagedMemberDescriptor('property', 'Config', 'public BonsaiModelConfig Config { get; }'),
            ManagedMemberDescriptor('property', 'Tokenizer', 'public BonsaiTokenizer Tokenizer { get; }'),
            ManagedMemberDescriptor('property', 'Q1Weights', 'public byte[] Q1Weights { get; }'),
            ManagedMemberDescriptor('property', 'ActivationBuffer', 'public float[] ActivationBuffer { get; }'),
            ManagedMemberDescriptor('property', 'LogitsBuffer', 'public float[] LogitsBuffer { get; }'),
            ManagedMemberDescriptor('property', 'IsInitialized', 'public bool IsInitialized { get; private set; }'),
            ManagedMemberDescriptor('method', 'MarkInitialized', 'public void MarkInitialized()'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Bonsai',
        name='BonsaiProviderCapabilities',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Bonsai/BonsaiProviderCapabilities.cs',
        members=(
            ManagedMemberDescriptor('property', 'Vector', 'public ModelCapacityVector Vector { get; } = new('),
            ManagedMemberDescriptor('method', 'SupportsOperation', 'public bool SupportsOperation(string operation)'),
            ManagedMemberDescriptor('method', 'SupportsDataType', 'public bool SupportsDataType(string dataType)'),
            ManagedMemberDescriptor('method', 'GetDynamicCapacities', 'public IDictionary<string, float>? GetDynamicCapacities(IExecutionConstraints constraints)'),
            ManagedMemberDescriptor('method', 'GetCapabilityProfile', 'public ICapabilityProfile? GetCapabilityProfile()'),
            ManagedMemberDescriptor('method', 'SupportsQuantization', 'public bool SupportsQuantization(string quantizationLevel)'),
            ManagedMemberDescriptor('method', 'SupportsQueryProcessingOperation', 'public bool SupportsQueryProcessingOperation(string operation)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Bonsai',
        name='BonsaiTokenizer',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Bonsai/BonsaiTokenizer.cs',
        members=(
            ManagedMemberDescriptor('method', 'Parse', 'public static BonsaiTokenizer Parse(string json)'),
            ManagedMemberDescriptor('method', 'TokenizeFirst', 'public int TokenizeFirst(ReadOnlySpan<char> text)'),
            ManagedMemberDescriptor('method', 'return', 'return (int)(hash % (uint)Math.Max(1, _idToToken.Length));'),
            ManagedMemberDescriptor('method', 'Decode', 'public string Decode(int tokenId)'),
            ManagedMemberDescriptor('method', 'InvalidDataException', 'throw new InvalidDataException("Bonsai tokenizer JSON does not contain a vocab object.");'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Bonsai',
        name='IBonsaiInferenceKernel',
        kind='interface',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Bonsai/IBonsaiInferenceKernel.cs',
        members=(
            ManagedMemberDescriptor('property', 'KernelId', 'string KernelId { get; }'),
            ManagedMemberDescriptor('property', 'Q1BlockElementCount', 'int Q1BlockElementCount { get; }'),
            ManagedMemberDescriptor('property', 'Q1BlockByteCount', 'int Q1BlockByteCount { get; }'),
            ManagedMemberDescriptor('method', 'Forward', 'void Forward('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Concepts',
        name='KairosScheduler',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Concepts/ConceptElevationFacades.cs',
        members=(
            ManagedMemberDescriptor('method', 'OrderCandidates', 'public IReadOnlyList<DateTimeOffset> OrderCandidates(IEnumerable<DateTimeOffset> candidates)'),
            ManagedMemberDescriptor('method', 'Label', 'public string Label(string scope)'),
            ManagedMemberDescriptor('method', 'SelectDeterministic', 'public string SelectDeterministic(IEnumerable<string> strategyNames)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Concepts',
        name='KairosTrigger',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Concepts/ConceptElevationFacades.cs',
        members=(
            ManagedMemberDescriptor('method', 'CanFire', 'public bool CanFire(DateTimeOffset now, DateTimeOffset earliestAllowedTime)'),
            ManagedMemberDescriptor('method', 'OrderCandidates', 'public IReadOnlyList<DateTimeOffset> OrderCandidates(IEnumerable<DateTimeOffset> candidates)'),
            ManagedMemberDescriptor('method', 'Label', 'public string Label(string scope)'),
            ManagedMemberDescriptor('method', 'SelectDeterministic', 'public string SelectDeterministic(IEnumerable<string> strategyNames)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Concepts',
        name='NousStrategy',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Concepts/ConceptElevationFacades.cs',
        members=(
            ManagedMemberDescriptor('method', 'SelectDeterministic', 'public string SelectDeterministic(IEnumerable<string> strategyNames)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Concepts',
        name='NousSupervisor',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Concepts/ConceptElevationFacades.cs',
        members=(
            ManagedMemberDescriptor('method', 'Label', 'public string Label(string scope)'),
            ManagedMemberDescriptor('method', 'SelectDeterministic', 'public string SelectDeterministic(IEnumerable<string> strategyNames)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CouncilDecisionBuilder',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CouncilDecisionBuilder.cs',
        members=(
            ManagedMemberDescriptor('method', 'Build', 'public Result<CouncilEvaluationResult> Build('),
            ManagedMemberDescriptor('method', 'ValidateContext', 'from validContext in ValidateContext(context)'),
            ManagedMemberDescriptor('method', 'ValidateVotes', 'from validVotes in ValidateVotes(votes)'),
            ManagedMemberDescriptor('method', 'CreateEvaluation', 'select CreateEvaluation(validContext, validVotes);'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CouncilDecisionToGateInputAdapter',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CouncilDecisionToGateInputAdapter.cs',
        members=(
            ManagedMemberDescriptor('method', 'Adapt', 'public Result<GateInput> Adapt(CouncilDecision? decision)'),
            ManagedMemberDescriptor('method', 'ValidateDecision', 'from validDecision in ValidateDecision(decision)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgControlCoordinator',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgControlCoordinator.cs',
        members=(
            ManagedMemberDescriptor('method', 'EvaluateAsync', 'public async ValueTask<Result<CtgControlDecisionEnvelope>> EvaluateAsync('),
            ManagedMemberDescriptor('method', 'CtgControlPipelineState', 'select new CtgControlPipelineState(councilEvaluation, gateInput);'),
            ManagedMemberDescriptor('method', 'BuildEnvelope', 'select BuildEnvelope(context, state.CouncilEvaluation, state.GateInput, gate);'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgControlCoordinatorOptions',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgControlCoordinatorOptions.cs',
        members=(
            ManagedMemberDescriptor('property', 'OperationId', 'public string OperationId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'StepId', 'public string StepId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'ProviderOutputs', 'public IReadOnlyList<ProviderVoteOutput> ProviderOutputs { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'CanonReferences', 'public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'CorrelationId', 'public string? CorrelationId { get; init; }'),
            ManagedMemberDescriptor('property', 'TraceId', 'public string? TraceId { get; init; }'),
            ManagedMemberDescriptor('property', 'ObservedAt', 'public DateTimeOffset? ObservedAt { get; init; }'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgControlDecisionEnvelope',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgControlDecisionEnvelope.cs',
        members=(
            ManagedMemberDescriptor('property', 'Context', 'public CtgControlExecutionContext Context { get; init; } = new();'),
            ManagedMemberDescriptor('property', 'CouncilEvaluation', 'public CouncilEvaluationResult CouncilEvaluation { get; init; } = new();'),
            ManagedMemberDescriptor('property', 'GateInput', 'public GateInput GateInput { get; init; } = new();'),
            ManagedMemberDescriptor('property', 'DecisionGate', 'public DecisionGateResult DecisionGate { get; init; } = new();'),
            ManagedMemberDescriptor('property', 'StepTrace', 'public StepGovernanceTrace StepTrace { get; init; } = new();'),
            ManagedMemberDescriptor('property', 'RetryIntent', 'public CtgRetryIntentCarrier? RetryIntent { get; init; }'),
            ManagedMemberDescriptor('property', 'TrajectoryGate', 'public TrajectoryGateResult? TrajectoryGate { get; init; }'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgControlExecutionContext',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgControlExecutionContext.cs',
        members=(
            ManagedMemberDescriptor('property', 'OperationId', 'public string OperationId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'StepId', 'public string StepId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'Graph', 'public IExecutionGraph? Graph { get; init; }'),
            ManagedMemberDescriptor('property', 'Request', 'public ControlExecutionRequest? Request { get; init; }'),
            ManagedMemberDescriptor('property', 'ProviderOutputs', 'public IReadOnlyList<ProviderVoteOutput> ProviderOutputs { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'CanonReferences', 'public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'RetryIntent', 'public CtgRetryIntentCarrier? RetryIntent { get; init; }'),
            ManagedMemberDescriptor('property', 'CorrelationId', 'public string? CorrelationId { get; init; }'),
            ManagedMemberDescriptor('property', 'TraceId', 'public string? TraceId { get; init; }'),
            ManagedMemberDescriptor('property', 'ObservedAt', 'public DateTimeOffset? ObservedAt { get; init; }'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgControlPolicyAdapter',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgControlPolicyAdapter.cs',
        members=(
            ManagedMemberDescriptor('method', 'EvaluateAsync', 'public async ValueTask<ControlPolicyEvaluation> EvaluateAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgControlServiceCollectionExtensions',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgControlServiceCollectionExtensions.cs',
        members=(
            ManagedMemberDescriptor('method', 'AddCtgControl', 'public static IServiceCollection AddCtgControl(this IServiceCollection services)'),
            ManagedMemberDescriptor('method', 'AddCtgControl', 'public static IServiceCollection AddCtgControl('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgCouncilEvaluationOrchestrator',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgCouncilEvaluationOrchestrator.cs',
        members=(
            ManagedMemberDescriptor('method', 'ResolveProviderVotesAsync', 'public ValueTask<Result<IReadOnlyList<ProviderVoteOutput>>> ResolveProviderVotesAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgCouncilEvaluationProviderRegistry',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgCouncilEvaluationProviderRegistry.cs',
        members=(
            ManagedMemberDescriptor('method', 'Register', 'public void Register(ProviderVoteOutput output)'),
            ManagedMemberDescriptor('method', 'ResolveResultAsync', 'public ValueTask<Result<IReadOnlyList<ProviderVoteOutput>>> ResolveResultAsync('),
            ManagedMemberDescriptor('method', 'ErrorContext', 'new ErrorContext('),
            ManagedMemberDescriptor('method', 'ResolveAsync', 'public async ValueTask<IReadOnlyList<ProviderVoteOutput>> ResolveAsync('),
            ManagedMemberDescriptor('method', 'Clear', 'public void Clear()'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgExecutionGatePolicy',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgExecutionGatePolicy.cs',
        members=(
            ManagedMemberDescriptor('method', 'EvaluateAsync', 'public ValueTask<ControlPolicyEvaluation> EvaluateAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgPolicyDecisionMapper',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgPolicyDecisionMapper.cs',
        members=(
            ManagedMemberDescriptor('method', 'Map', 'public ControlPolicyEvaluation Map(CtgControlDecisionEnvelope envelope)'),
            ManagedMemberDescriptor('property', 'is', 'return envelope.TrajectoryGate is { Accepted: false }'),
            ManagedMemberDescriptor('method', 'ControlPolicyEvaluation', '? new ControlPolicyEvaluation(false, "ABORT", CreateReason(envelope.TrajectoryGate.RejectReasons))'),
            ManagedMemberDescriptor('method', 'Map', 'public ControlPolicyEvaluation Map(TrajectoryGateResult trajectoryGate)'),
            ManagedMemberDescriptor('method', 'ControlPolicyEvaluation', '? new ControlPolicyEvaluation(true, "ALLOW", "CTG trajectory gate allowed execution.")'),
            ManagedMemberDescriptor('method', 'ControlPolicyEvaluation', '? new ControlPolicyEvaluation(true, "ALLOW", "CTG decision gate allowed execution.")'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgRetryIntentCarrier',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgRetryIntentCarrier.cs',
        members=(
            ManagedMemberDescriptor('property', 'Requested', 'public bool Requested { get; init; }'),
            ManagedMemberDescriptor('property', 'ReasonCode', 'public string ReasonCode { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'Priority', 'public int Priority { get; init; }'),
            ManagedMemberDescriptor('property', 'Confidence', 'public double Confidence { get; init; }'),
            ManagedMemberDescriptor('property', 'SourceSensor', 'public string SourceSensor { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='CtgStepTraceAssembler',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/CtgStepTraceAssembler.cs',
        members=(
            ManagedMemberDescriptor('method', 'Assemble', 'public StepGovernanceTrace Assemble('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='ICtgControlCoordinator',
        kind='interface',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/ICtgControlCoordinator.cs',
        members=(
            ManagedMemberDescriptor('method', 'EvaluateAsync', 'ValueTask<Result<CtgControlDecisionEnvelope>> EvaluateAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='ProviderVoteAdapter',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/ProviderVoteAdapter.cs',
        members=(
            ManagedMemberDescriptor('method', 'AdaptAsync', 'public ValueTask<Result<CouncilVote>> AdaptAsync('),
            ManagedMemberDescriptor('method', 'ValidateOutput', 'from validOutput in ValidateOutput(output)'),
            ManagedMemberDescriptor('method', 'ValidateContext', 'from validContext in ValidateContext(context)'),
            ManagedMemberDescriptor('method', 'CreateVote', 'select CreateVote(validOutput, validContext);'),
            ManagedMemberDescriptor('method', 'SanitizeProviderMetadata', 'SanitizeProviderMetadata(output.Metadata),'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Ctg',
        name='ProviderVoteOutput',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Ctg/ProviderVoteOutput.cs',
        members=(
            ManagedMemberDescriptor('property', 'ProviderId', 'public string ProviderId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'CouncilKind', 'public CouncilKind CouncilKind { get; init; } = CouncilKind.Unknown;'),
            ManagedMemberDescriptor('property', 'VoteValue', 'public CouncilVoteValue VoteValue { get; init; } = CouncilVoteValue.Unknown;'),
            ManagedMemberDescriptor('property', 'CanonReferences', 'public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Perception',
        name='PerceptionControlAdapter',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Perception/PerceptionControlAdapter.cs',
        members=(
            ManagedMemberDescriptor('method', 'Adapt', 'public Result<IReadOnlyList<ProviderVoteOutput>> Adapt(PerceptionControlRequest? request)'),
            ManagedMemberDescriptor('method', 'ValidateRequest', 'from validRequest in ValidateRequest(request)'),
            ManagedMemberDescriptor('method', 'select', 'select (IReadOnlyList<ProviderVoteOutput>)validRequest.Signals'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Perception',
        name='PerceptionControlRequest',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Perception/PerceptionControlContracts.cs',
        members=(
            ManagedMemberDescriptor('property', 'OperationId', 'public string OperationId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'StepId', 'public string StepId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'Graph', 'public IExecutionGraph? Graph { get; init; }'),
            ManagedMemberDescriptor('property', 'ExecutionRequest', 'public ControlExecutionRequest? ExecutionRequest { get; init; }'),
            ManagedMemberDescriptor('property', 'Signals', 'public IReadOnlyList<PerceptionControlSignal> Signals { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'RetryIntent', 'public CtgRetryIntentCarrier? RetryIntent { get; init; }'),
            ManagedMemberDescriptor('property', 'CanonReferences', 'public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'CorrelationId', 'public string? CorrelationId { get; init; }'),
            ManagedMemberDescriptor('property', 'TraceId', 'public string? TraceId { get; init; }'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
            ManagedMemberDescriptor('property', 'Mode', 'public string Mode { get; init; } = "observe";'),
            ManagedMemberDescriptor('property', 'DecisionEnvelope', 'public CtgControlDecisionEnvelope DecisionEnvelope { get; init; } = new();'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Perception',
        name='PerceptionControlServiceCollectionExtensions',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Perception/PerceptionControlServiceCollectionExtensions.cs',
        members=(
            ManagedMemberDescriptor('method', 'AddPerceptionCtgControl', 'public static IServiceCollection AddPerceptionCtgControl(this IServiceCollection services)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Perception',
        name='PerceptionControlSignal',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Perception/PerceptionControlContracts.cs',
        members=(
            ManagedMemberDescriptor('property', 'SignalId', 'public string SignalId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'ProviderId', 'public string ProviderId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'CouncilKind', 'public CouncilKind CouncilKind { get; init; } = CouncilKind.Unknown;'),
            ManagedMemberDescriptor('property', 'VoteValue', 'public CouncilVoteValue VoteValue { get; init; } = CouncilVoteValue.Unknown;'),
            ManagedMemberDescriptor('property', 'CanonReferences', 'public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
            ManagedMemberDescriptor('property', 'OperationId', 'public string OperationId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'StepId', 'public string StepId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'Graph', 'public IExecutionGraph? Graph { get; init; }'),
            ManagedMemberDescriptor('property', 'ExecutionRequest', 'public ControlExecutionRequest? ExecutionRequest { get; init; }'),
            ManagedMemberDescriptor('property', 'Signals', 'public IReadOnlyList<PerceptionControlSignal> Signals { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'RetryIntent', 'public CtgRetryIntentCarrier? RetryIntent { get; init; }'),
            ManagedMemberDescriptor('property', 'CorrelationId', 'public string? CorrelationId { get; init; }'),
            ManagedMemberDescriptor('property', 'TraceId', 'public string? TraceId { get; init; }'),
            ManagedMemberDescriptor('property', 'Mode', 'public string Mode { get; init; } = "observe";'),
            ManagedMemberDescriptor('property', 'DecisionEnvelope', 'public CtgControlDecisionEnvelope DecisionEnvelope { get; init; } = new();'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Perception',
        name='PerceptionCtgControlCoordinator',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Perception/PerceptionCtgControlCoordinator.cs',
        members=(
            ManagedMemberDescriptor('method', 'EvaluateAsync', 'public async ValueTask<Result<CtgControlDecisionEnvelope>> EvaluateAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Perception',
        name='PerceptionPipelineSelection',
        kind='record',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Perception/PerceptionControlContracts.cs',
        members=(
            ManagedMemberDescriptor('property', 'Mode', 'public string Mode { get; init; } = "observe";'),
            ManagedMemberDescriptor('property', 'DecisionEnvelope', 'public CtgControlDecisionEnvelope DecisionEnvelope { get; init; } = new();'),
            ManagedMemberDescriptor('property', 'RetryIntent', 'public CtgRetryIntentCarrier? RetryIntent { get; init; }'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Core.Perception',
        name='PerceptionPipelineSelector',
        kind='class',
        assembly='AIKernel.Control.Core',
        source='AIKernel.Control/src/AIKernel.Control.Core/Perception/PerceptionPipelineSelector.cs',
        members=(
            ManagedMemberDescriptor('method', 'Select', 'public PerceptionPipelineSelection Select(CtgControlDecisionEnvelope envelope)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgCanonReferenceFormatter',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgCanonReferenceFormatter.cs',
        members=(
            ManagedMemberDescriptor('method', 'Format', 'public string Format(CanonReference reference)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgControlStateObserver',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgControlStateObserver.cs',
        members=(
            ManagedMemberDescriptor('method', 'ObserveAsync', 'public async ValueTask ObserveAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgControlTimelineFormatter',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgControlTimelineFormatter.cs',
        members=(
            ManagedMemberDescriptor('method', 'Format', 'public string Format(IReadOnlyList<ControlStateSnapshot> snapshots)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgDiagnosticsFormatter',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgDiagnosticsFormatter.cs',
        members=(
            ManagedMemberDescriptor('method', 'FormatDecisionGate', 'public string FormatDecisionGate(DecisionGateResult result)'),
            ManagedMemberDescriptor('method', 'FormatTrajectoryGate', 'public string FormatTrajectoryGate(TrajectoryGateResult result)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgGateTelemetryValidator',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgGateTelemetryValidator.cs',
        members=(
            ManagedMemberDescriptor('method', 'IsGateTelemetryDiscreteOnly', 'public bool IsGateTelemetryDiscreteOnly(DecisionGateResult result)'),
            ManagedMemberDescriptor('method', 'FindContinuousCarrierKeys', 'public IReadOnlyList<string> FindContinuousCarrierKeys(DecisionGateResult result)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgGovernanceTraceEmitter',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgGovernanceTraceEmitter.cs',
        members=(
            ManagedMemberDescriptor('method', 'EmitStepAsync', 'public ValueTask EmitStepAsync('),
            ManagedMemberDescriptor('method', 'ControlStateSnapshot', 'new ControlStateSnapshot('),
            ManagedMemberDescriptor('method', 'EmitTrajectoryAsync', 'public ValueTask EmitTrajectoryAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgRejectReasonFormatter',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgRejectReasonFormatter.cs',
        members=(
            ManagedMemberDescriptor('method', 'Format', 'public string Format(RejectReasonInfo reason)'),
            ManagedMemberDescriptor('method', 'FormatKind', '? FormatKind(reason.Kind)'),
            ManagedMemberDescriptor('method', 'FormatKind', 'public string FormatKind(RejectReasonKind kind)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgReplayMetadataWriter',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgReplayMetadataWriter.cs',
        members=(
            ManagedMemberDescriptor('method', 'WriteStep', 'public IReadOnlyDictionary<string, string> WriteStep(StepGovernanceTrace trace)'),
            ManagedMemberDescriptor('method', 'WriteRejectReasons', 'WriteRejectReasons(metadata, trace.RejectReasons);'),
            ManagedMemberDescriptor('method', 'WriteCanonReferences', 'WriteCanonReferences(metadata, trace.CanonReferences);'),
            ManagedMemberDescriptor('method', 'WriteTrajectory', 'public IReadOnlyDictionary<string, string> WriteTrajectory(TrajectoryGateResult result)'),
            ManagedMemberDescriptor('method', 'WriteRejectReasons', 'WriteRejectReasons(metadata, result.RejectReasons);'),
            ManagedMemberDescriptor('method', 'WriteCanonReferences', 'WriteCanonReferences(metadata, result.Trace.CanonReferences);'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='CtgTraceRenderer',
        kind='class',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/CtgTraceRenderer.cs',
        members=(
            ManagedMemberDescriptor('method', 'RenderStep', 'public string RenderStep(StepGovernanceTrace trace)'),
            ManagedMemberDescriptor('method', 'RenderGovernanceTrace', 'public string RenderGovernanceTrace(GovernanceTrace trace)'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Diagnostics',
        name='ReplayApprovalRecord',
        kind='record',
        assembly='AIKernel.Control.Diagnostics',
        source='AIKernel.Control/src/AIKernel.Control.Diagnostics/ReplayApprovalRecord.cs',
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='AllAbstainDenyScenario',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/AllAbstainDenyScenario.cs',
        members=(
            ManagedMemberDescriptor('method', 'Create', 'public CtgControlEmulatorScenario Create()'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='AllowAllControlPolicy',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/AllowAllControlPolicy.cs',
        members=(
            ManagedMemberDescriptor('method', 'EvaluateAsync', 'public ValueTask<ControlPolicyEvaluation> EvaluateAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='AnyDenyTrajectoryHaltScenario',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/AnyDenyTrajectoryHaltScenario.cs',
        members=(
            ManagedMemberDescriptor('method', 'Create', 'public CtgControlEmulatorScenario Create()'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='ControlEmulatorEngine',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/ControlEmulatorEngine.cs',
        members=(
            ManagedMemberDescriptor('method', 'ExecuteAsync', 'public async ValueTask<ControlExecutionResult> ExecuteAsync('),
            ManagedMemberDescriptor('method', 'ControlExecutionResult', 'return new ControlExecutionResult('),
            ManagedMemberDescriptor('method', 'PolicyStatus', 'PolicyStatus(evaluation),'),
            ManagedMemberDescriptor('method', 'MergeMetadata', 'MergeMetadata('),
            ManagedMemberDescriptor('method', 'ControlStateSnapshot', 'new ControlStateSnapshot('),
            ManagedMemberDescriptor('method', 'RecordNodeMetadata', 'RecordNodeMetadata(executionMetadata, node);'),
            ManagedMemberDescriptor('method', 'ExecuteEmulatedNode', 'ExecuteEmulatedNode(node);'),
            ManagedMemberDescriptor('method', 'MergeMetadata', 'MergeMetadata(request.Metadata, executionMetadata));'),
            ManagedMemberDescriptor('method', 'InvalidOperationException', 'throw new InvalidOperationException(emulated.FaultMessage);'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='CtgControlEmulator',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/CtgControlEmulator.cs',
        members=(
            ManagedMemberDescriptor('method', 'RunScenarioAsync', 'public async ValueTask<CtgEmulationResult> RunScenarioAsync('),
            ManagedMemberDescriptor('method', 'CreateFaultedResult', 'return CreateFaultedResult(scenario, envelopeResult.Error!.Message);'),
            ManagedMemberDescriptor('method', 'CtgControlCoordinator', 'return new CtgControlCoordinator('),
            ManagedMemberDescriptor('method', 'ProviderVoteAdapter', 'new ProviderVoteAdapter(),'),
            ManagedMemberDescriptor('method', 'CouncilDecisionBuilder', 'new CouncilDecisionBuilder(),'),
            ManagedMemberDescriptor('method', 'CouncilDecisionToGateInputAdapter', 'new CouncilDecisionToGateInputAdapter(),'),
            ManagedMemberDescriptor('method', 'CtgDecisionGateEvaluator', 'new CtgDecisionGateEvaluator());'),
            ManagedMemberDescriptor('method', 'CtgControlPolicyAdapter', 'return new CtgControlPolicyAdapter('),
            ManagedMemberDescriptor('method', 'ControlExecutionResult', 'return new ControlExecutionResult('),
            ManagedMemberDescriptor('method', 'ObserveAsync', 'public ValueTask ObserveAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='CtgControlEmulatorScenario',
        kind='record',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/CtgControlEmulatorScenario.cs',
        members=(
            ManagedMemberDescriptor('property', 'ScenarioId', 'public string ScenarioId { get; init; } = "ctg.scenario";'),
            ManagedMemberDescriptor('property', 'Graph', 'public IExecutionGraph Graph { get; init; } = new EmulatedExecutionGraph('),
            ManagedMemberDescriptor('property', 'Request', 'public ControlExecutionRequest Request { get; init; } = new('),
            ManagedMemberDescriptor('property', 'ProviderOutputs', 'public IReadOnlyList<ProviderVoteOutput> ProviderOutputs { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'TrajectorySteps', 'public IReadOnlyList<StepGovernanceTrace>? TrajectorySteps { get; init; }'),
            ManagedMemberDescriptor('property', 'Metadata', 'public IReadOnlyDictionary<string, string> Metadata { get; init; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='CtgEmulationResult',
        kind='record',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/CtgEmulationResult.cs',
        members=(
            ManagedMemberDescriptor('property', 'ScenarioId', 'public string ScenarioId { get; init; } = string.Empty;'),
            ManagedMemberDescriptor('property', 'ExecutionResult', 'public ControlExecutionResult ExecutionResult { get; init; } = new('),
            ManagedMemberDescriptor('property', 'DecisionEnvelope', 'public CtgControlDecisionEnvelope? DecisionEnvelope { get; init; }'),
            ManagedMemberDescriptor('property', 'TrajectoryGate', 'public TrajectoryGateResult? TrajectoryGate { get; init; }'),
            ManagedMemberDescriptor('property', 'Snapshots', 'public IReadOnlyList<ControlStateSnapshot> Snapshots { get; init; } = [];'),
            ManagedMemberDescriptor('property', 'ReplayMetadata', 'public IReadOnlyDictionary<string, string> ReplayMetadata { get; init; } ='),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='CtgMockCouncilEvaluator',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/CtgMockCouncilEvaluator.cs',
        members=(
            ManagedMemberDescriptor('method', 'CreateVotes', 'public IReadOnlyList<ProviderVoteOutput> CreateVotes('),
            ManagedMemberDescriptor('method', 'CreateVote', 'CreateVote(CouncilKind.Logos, logos),'),
            ManagedMemberDescriptor('method', 'CreateVote', 'CreateVote(CouncilKind.Ethos, ethos),'),
            ManagedMemberDescriptor('method', 'CreateVote', 'CreateVote(CouncilKind.Pathos, pathos)'),
            ManagedMemberDescriptor('method', 'CreateVote', 'public ProviderVoteOutput CreateVote('),
            ManagedMemberDescriptor('method', 'CreateScenario', 'public CtgControlEmulatorScenario CreateScenario('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='CtgReplayScenario',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/CtgReplayScenario.cs',
        members=(
            ManagedMemberDescriptor('method', 'Create', 'public CtgControlEmulatorScenario Create()'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='CtgTruthTableScenario',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/CtgTruthTableScenario.cs',
        members=(
            ManagedMemberDescriptor('method', 'CreateScenarios', 'public IReadOnlyList<CtgControlEmulatorScenario> CreateScenarios()'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='DeterministicNodeScheduler',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/DeterministicNodeScheduler.cs',
        members=(
            ManagedMemberDescriptor('method', 'ScheduleAsync', 'public ValueTask<IReadOnlyList<IExecutionNode>> ScheduleAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='EmptyTrajectoryHaltScenario',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/EmptyTrajectoryHaltScenario.cs',
        members=(
            ManagedMemberDescriptor('method', 'Create', 'public CtgControlEmulatorScenario Create()'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='EmulatedExecutionGraph',
        kind='record',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/EmulatedExecutionGraph.cs',
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='EmulatedExecutionNode',
        kind='record',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/EmulatedExecutionNode.cs',
        members=(
            ManagedMemberDescriptor('property', 'FaultMessage', 'public string? FaultMessage { get; init; }'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='EthosRejectScenario',
        kind='class',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/EthosRejectScenario.cs',
        members=(
            ManagedMemberDescriptor('method', 'Create', 'public CtgControlEmulatorScenario Create()'),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.Emulator',
        name='ICtgControlEmulator',
        kind='interface',
        assembly='AIKernel.Control.Emulator',
        source='AIKernel.Control/src/AIKernel.Control.Emulator/ICtgControlEmulator.cs',
        members=(
            ManagedMemberDescriptor('method', 'RunScenarioAsync', 'ValueTask<CtgEmulationResult> RunScenarioAsync('),
        ),
    ),
    ManagedTypeDescriptor(
        namespace='AIKernel.Control.GPU',
        name='IBonsaiGpuExecutionDelegate',
        kind='interface',
        assembly='AIKernel.Control.GPU',
        source='AIKernel.Control/src/AIKernel.Control.GPU/IBonsaiGpuExecutionDelegate.cs',
        members=(
            ManagedMemberDescriptor('property', 'DeviceId', 'string DeviceId { get; }'),
            ManagedMemberDescriptor('property', 'IsAvailable', 'bool IsAvailable { get; }'),
        ),
    ),
)


def managed_api_catalog() -> tuple[ManagedTypeDescriptor, ...]:
    """[EN] Return the generated public managed API catalog.

    [JA] 生成済み public managed API catalog を返します。
    """
    return _CATALOG


def managed_type_names() -> tuple[str, ...]:
    """[EN] Return all namespace-qualified managed type names.

    [JA] namespace 修飾済み managed type 名をすべて返します。
    """
    return tuple(item.full_name for item in _CATALOG)


def find_managed_type(full_name: str) -> ManagedTypeDescriptor | None:
    """[EN] Find a managed type descriptor by namespace-qualified name.

    [JA] namespace 修飾名から managed type descriptor を検索します。
    """
    for item in _CATALOG:
        if item.full_name == full_name:
            return item
    return None


def managed_api_summary() -> dict[str, int]:
    """[EN] Return public managed API counts by assembly.

    [JA] assembly ごとの public managed API 件数を返します。
    """
    summary: dict[str, int] = {}
    for item in _CATALOG:
        summary[item.assembly] = summary.get(item.assembly, 0) + 1
    return dict(sorted(summary.items()))


__all__ = [
    "ManagedMemberDescriptor",
    "ManagedTypeDescriptor",
    "find_managed_type",
    "managed_api_catalog",
    "managed_api_summary",
    "managed_type_names",
]
