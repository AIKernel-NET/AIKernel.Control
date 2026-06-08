from __future__ import annotations

from dataclasses import dataclass, field
from typing import Mapping

from aikernel_governance.native import load_governance_runtime


@dataclass(frozen=True)
class ExecutionResult:
    """[EN]
    Public governance result envelope.

    [JA]
    公開 governance result envelope を表します。
    """

    id: str
    output: str
    status: str
    metadata: Mapping[str, str] = field(default_factory=dict)

    @classmethod
    def from_managed(cls, value) -> "ExecutionResult":
        """[EN]
        Create a Python result from ControlExecutionResult.

        [JA]
        ControlExecutionResult から Python result を生成します。
        """
        metadata = {str(item.Key): str(item.Value) for item in value.Metadata}
        return cls(
            id=str(value.ExecutionId),
            output=metadata.get("output", ""),
            status=str(value.Status),
            metadata=metadata,
        )

    def to_managed(self):
        """[EN]
        Convert this wrapper to AIKernel.Dtos.Control.ControlExecutionResult.

        [JA]
        この wrapper を AIKernel.Dtos.Control.ControlExecutionResult に変換します。
        """
        load_governance_runtime()
        from AIKernel.Dtos.Control import ControlExecutionResult  # type: ignore[import-not-found]

        metadata = dict(self.metadata)
        if self.output:
            metadata["output"] = self.output
        return ControlExecutionResult(self.id, self.status, _to_dictionary(metadata))


def _to_dictionary(values: Mapping[str, str]):
    from System.Collections.Generic import Dictionary  # type: ignore[import-not-found]

    dictionary = Dictionary[str, str]()
    for key, value in values.items():
        dictionary[str(key)] = str(value)
    return dictionary
