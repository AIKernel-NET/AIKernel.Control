from __future__ import annotations

from dataclasses import dataclass, field
from typing import Mapping

from aikernel_governance.native import load_governance_runtime


@dataclass(frozen=True)
class SnapshotMetadata:
    """[EN]
    Public metadata wrapper for governance snapshots.

    [JA]
    governance snapshot の公開 metadata wrapper です。
    """

    values: Mapping[str, str] = field(default_factory=dict)


@dataclass(frozen=True)
class Snapshot:
    """[EN]
    Public governance state snapshot.

    [JA]
    公開 governance state snapshot を表します。
    """

    id: str
    state: str
    metadata: SnapshotMetadata = field(default_factory=SnapshotMetadata)

    @classmethod
    def from_managed(cls, value) -> "Snapshot":
        """[EN]
        Create a Python snapshot from ControlStateSnapshot.

        [JA]
        ControlStateSnapshot から Python snapshot を生成します。
        """
        metadata = {str(item.Key): str(item.Value) for item in value.Metadata}
        metadata.setdefault("graph_id", str(value.GraphId))
        metadata.setdefault("node_id", str(value.NodeId))
        return cls(
            id=str(value.ExecutionId),
            state=str(value.NodeId),
            metadata=SnapshotMetadata(metadata),
        )

    def to_managed(self):
        """[EN]
        Convert this wrapper to AIKernel.Dtos.Control.ControlStateSnapshot.

        [JA]
        この wrapper を AIKernel.Dtos.Control.ControlStateSnapshot に変換します。
        """
        load_governance_runtime()
        from AIKernel.Dtos.Control import ControlStateSnapshot  # type: ignore[import-not-found]

        metadata = dict(self.metadata.values)
        graph_id = metadata.pop("graph_id", "")
        node_id = metadata.pop("node_id", self.state)
        return ControlStateSnapshot(self.id, graph_id, node_id, _to_dictionary(metadata))


def _to_dictionary(values: Mapping[str, str]):
    from System.Collections.Generic import Dictionary  # type: ignore[import-not-found]

    dictionary = Dictionary[str, str]()
    for key, value in values.items():
        dictionary[str(key)] = str(value)
    return dictionary
