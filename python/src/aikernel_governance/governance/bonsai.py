from __future__ import annotations

from dataclasses import dataclass, field
from typing import Mapping, Sequence

from aikernel_governance.native import load_governance_runtime

from .managed import ManagedObject, to_dictionary


@dataclass(frozen=True)
class BonsaiModelConfig:
    """[EN]
    Python wrapper for the public BonsaiModelConfig contract.

    [JA]
    公開 BonsaiModelConfig 契約の Python wrapper です。
    """
    layer_count: int
    hidden_size: int
    head_count: int
    vocabulary_size: int
    context_length: int

    @classmethod
    def parse(cls, json: str) -> "BonsaiModelConfig":
        """[EN]
        Parse model configuration JSON through the C# contract.

        [JA]
        C# 契約を通じて model configuration JSON を解析します。
        """
        load_governance_runtime()
        from AIKernel.Control.Core.Bonsai import BonsaiModelConfig as ManagedConfig  # type: ignore[import-not-found]

        return cls.from_managed(ManagedConfig.Parse(json))

    @classmethod
    def from_managed(cls, value) -> "BonsaiModelConfig":
        """[EN]
        Create a Python wrapper from a managed BonsaiModelConfig.

        [JA]
        managed BonsaiModelConfig から Python wrapper を生成します。
        """
        return cls(
            layer_count=int(value.LayerCount),
            hidden_size=int(value.HiddenSize),
            head_count=int(value.HeadCount),
            vocabulary_size=int(value.VocabularySize),
            context_length=int(value.ContextLength),
        )

    def estimate_activation_float_count(self) -> int:
        """[EN]
        Return the C# estimate for activation buffer size.

        [JA]
        activation buffer size の C# 推定値を返します。
        """
        return int(self.to_managed().EstimateActivationFloatCount())

    def to_managed(self):
        """[EN]
        Convert this wrapper to AIKernel.Control.Core.Bonsai.BonsaiModelConfig.

        [JA]
        この wrapper を AIKernel.Control.Core.Bonsai.BonsaiModelConfig に変換します。
        """
        load_governance_runtime()
        from AIKernel.Control.Core.Bonsai import BonsaiModelConfig as ManagedConfig  # type: ignore[import-not-found]

        return ManagedConfig(
            self.layer_count,
            self.hidden_size,
            self.head_count,
            self.vocabulary_size,
            self.context_length,
        )


class BonsaiTokenizer(ManagedObject):
    """[EN]
    Python wrapper for the public BonsaiTokenizer contract.

    [JA]
    公開 BonsaiTokenizer 契約の Python wrapper です。
    """
    @classmethod
    def parse(cls, json: str) -> "BonsaiTokenizer":
        """[EN]
        Parse tokenizer JSON through the C# tokenizer.

        [JA]
        C# tokenizer を通じて tokenizer JSON を解析します。
        """
        load_governance_runtime()
        from AIKernel.Control.Core.Bonsai import BonsaiTokenizer as ManagedTokenizer  # type: ignore[import-not-found]

        return cls(ManagedTokenizer.Parse(json))

    @property
    def vocabulary_size(self) -> int:
        """[EN]
        Return the vocabulary size exposed by the C# tokenizer.

        [JA]
        C# tokenizer が公開する vocabulary size を返します。
        """
        return int(self.managed.VocabularySize)

    def tokenize_first(self, text: str) -> int:
        """[EN]
        Tokenize text by delegating to the C# tokenizer.

        [JA]
        C# tokenizer に委譲して text を token 化します。
        """
        return int(self.managed.TokenizeFirst(text))

    def decode(self, token_id: int) -> str:
        """[EN]
        Decode a token id by delegating to the C# tokenizer.

        [JA]
        C# tokenizer に委譲して token id を decode します。
        """
        return str(self.managed.Decode(token_id))


class BonsaiModelState(ManagedObject):
    """[EN]
    Python wrapper for the public BonsaiModelState contract.

    [JA]
    公開 BonsaiModelState 契約の Python wrapper です。
    """
    @classmethod
    def create(
        cls,
        config: BonsaiModelConfig,
        tokenizer: BonsaiTokenizer,
        q1_weights: bytes,
        activation_buffer: Sequence[float],
        logits_buffer: Sequence[float],
    ) -> "BonsaiModelState":
        """[EN]
        Create a managed BonsaiModelState from public contract inputs.

        [JA]
        公開契約入力から managed BonsaiModelState を生成します。
        """
        load_governance_runtime()
        from AIKernel.Control.Core.Bonsai import BonsaiModelState as ManagedState  # type: ignore[import-not-found]
        from System import Array, Byte, Single  # type: ignore[import-not-found]

        return cls(
            ManagedState(
                config.to_managed(),
                tokenizer.to_managed(),
                Array[Byte](q1_weights),
                Array[Single]([float(value) for value in activation_buffer]),
                Array[Single]([float(value) for value in logits_buffer]),
            )
        )

    @property
    def is_initialized(self) -> bool:
        """[EN]
        Return whether the C# state is initialized.

        [JA]
        C# state が initialized かどうかを返します。
        """
        return bool(self.managed.IsInitialized)

    def mark_initialized(self) -> None:
        """[EN]
        Mark the managed state as initialized.

        [JA]
        managed state を initialized として記録します。
        """
        self.managed.MarkInitialized()


class BonsaiProviderCapabilities(ManagedObject):
    """[EN]
    Python wrapper for public Bonsai provider capabilities.

    [JA]
    公開 Bonsai provider capabilities の Python wrapper です。
    """
    @classmethod
    def create(cls) -> "BonsaiProviderCapabilities":
        """[EN]
        Create the managed BonsaiProviderCapabilities instance.

        [JA]
        managed BonsaiProviderCapabilities instance を生成します。
        """
        load_governance_runtime()
        from AIKernel.Control.Core.Bonsai import BonsaiProviderCapabilities as ManagedCapabilities  # type: ignore[import-not-found]

        return cls(ManagedCapabilities())

    @property
    def supported_operations(self) -> tuple[str, ...]:
        """[EN]
        Return operations supported by the C# provider capabilities.

        [JA]
        C# provider capabilities が対応する operation を返します。
        """
        return tuple(str(value) for value in self.managed.SupportedOperations)

    @property
    def supported_data_types(self) -> tuple[str, ...]:
        """[EN]
        Return data types supported by the C# provider capabilities.

        [JA]
        C# provider capabilities が対応する data type を返します。
        """
        return tuple(str(value) for value in self.managed.SupportedDataTypes)

    def supports_operation(self, operation: str) -> bool:
        """[EN]
        Check operation support through the C# capability object.

        [JA]
        C# capability object を通じて operation support を確認します。
        """
        return bool(self.managed.SupportsOperation(operation))

    def supports_data_type(self, data_type: str) -> bool:
        """[EN]
        Check data-type support through the C# capability object.

        [JA]
        C# capability object を通じて data-type support を確認します。
        """
        return bool(self.managed.SupportsDataType(data_type))

    def supports_quantization(self, quantization_level: str) -> bool:
        """[EN]
        Check quantization support through the C# capability object.

        [JA]
        C# capability object を通じて quantization support を確認します。
        """
        return bool(self.managed.SupportsQuantization(quantization_level))


class BonsaiBuiltInProvider(ManagedObject):
    """[EN]
    Python wrapper for the public BonsaiBuiltInProvider.

    [JA]
    公開 BonsaiBuiltInProvider の Python wrapper です。
    """
    default_model_root = "/sys/roms/bonsai-1.7b"
    operation_chat_local = "chat.local"
    operation_tokenize = "text.tokenize"

    @classmethod
    def create(cls, kernel, observer=None, model_root: str = default_model_root) -> "BonsaiBuiltInProvider":
        """[EN]
        Create a managed BonsaiBuiltInProvider without reimplementing provider logic.

        [JA]
        provider logic を再実装せず managed BonsaiBuiltInProvider を生成します。
        """
        load_governance_runtime()
        from AIKernel.Control.Core.Bonsai import BonsaiBuiltInProvider as ManagedProvider  # type: ignore[import-not-found]

        managed_kernel = kernel.to_managed() if hasattr(kernel, "to_managed") else kernel
        managed_observer = observer.to_managed() if hasattr(observer, "to_managed") else observer
        return cls(ManagedProvider(managed_kernel, managed_observer, model_root))

    @property
    def provider_id(self) -> str:
        """[EN]
        Return the provider identifier exposed by the C# provider.

        [JA]
        C# provider が公開する provider identifier を返します。
        """
        return str(self.managed.ProviderId)

    @property
    def name(self) -> str:
        """[EN]
        Return the provider display name.

        [JA]
        provider display name を返します。
        """
        return str(self.managed.Name)

    @property
    def version(self) -> str:
        """[EN]
        Return the provider version.

        [JA]
        provider version を返します。
        """
        return str(self.managed.Version)

    @property
    def model_root(self) -> str:
        """[EN]
        Return the configured model root.

        [JA]
        設定された model root を返します。
        """
        return str(self.managed.ModelRoot)

    @property
    def is_initialized(self) -> bool:
        """[EN]
        Return whether the provider is initialized.

        [JA]
        provider が initialized かどうかを返します。
        """
        return bool(self.managed.IsInitialized)

    def get_capabilities(self) -> BonsaiProviderCapabilities:
        """[EN]
        Return public provider capabilities from the C# provider.

        [JA]
        C# provider から公開 provider capabilities を返します。
        """
        return BonsaiProviderCapabilities(self.managed.GetCapabilities())


@dataclass(frozen=True)
class ProviderContractSurface:
    """[EN]
    Normalized provider contract metadata surface.

    [JA]
    正規化された provider contract metadata surface です。
    """
    provider_id: str
    name: str
    version: str
    metadata: Mapping[str, str] = field(default_factory=dict)

    def to_provider_contract(self):
        """[EN]
        Convert this surface to the top-level ProviderContract wrapper.

        [JA]
        この surface を top-level ProviderContract wrapper に変換します。
        """
        from .provider import ProviderContract

        return ProviderContract(
            provider_id=self.provider_id,
            name=self.name,
            version=self.version,
            metadata=self.metadata,
        )
