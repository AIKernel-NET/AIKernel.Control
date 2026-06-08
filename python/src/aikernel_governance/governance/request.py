from __future__ import annotations

from dataclasses import dataclass, field
from typing import Mapping

from aikernel_governance.native import load_governance_runtime


@dataclass(frozen=True)
class ExecutionRequest:
    """[EN]
    Public governance request envelope.

    [JA]
    公開 governance request envelope を表します。
    """

    model: str
    input: str
    parameters: Mapping[str, str] = field(default_factory=dict)

    @property
    def id(self) -> str:
        """[EN]
        Return the deterministic execution identifier used by the C# contract.

        [JA]
        C# 契約で使用する決定論的 execution identifier を返します。
        """
        return self.parameters.get("execution_id", self.model)

    def to_metadata(self) -> dict[str, str]:
        """[EN]
        Convert the Python request into contract metadata without side effects.

        [JA]
        Python request を副作用なく契約 metadata に変換します。
        """
        metadata = {
            "model": self.model,
            "input": self.input,
        }
        metadata.update({f"parameter.{key}": value for key, value in self.parameters.items()})
        return metadata

    def to_managed(self):
        """[EN]
        Convert this wrapper to AIKernel.Dtos.Control.ControlExecutionRequest.

        [JA]
        この wrapper を AIKernel.Dtos.Control.ControlExecutionRequest に変換します。
        """
        load_governance_runtime()
        from AIKernel.Dtos.Control import ControlExecutionRequest  # type: ignore[import-not-found]

        return ControlExecutionRequest(self.id, _to_dictionary(self.to_metadata()))


def _to_dictionary(values: Mapping[str, str]):
    from System.Collections.Generic import Dictionary  # type: ignore[import-not-found]

    dictionary = Dictionary[str, str]()
    for key, value in values.items():
        dictionary[str(key)] = str(value)
    return dictionary
