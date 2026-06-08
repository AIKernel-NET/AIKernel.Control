from __future__ import annotations

from dataclasses import dataclass

from aikernel_governance.native import load_governance_runtime


@dataclass(frozen=True)
class ReplayApprovalRecord:
    """[EN]
    Python wrapper for the public ReplayApprovalRecord diagnostics contract.

    [JA]
    公開 ReplayApprovalRecord diagnostics 契約の Python wrapper です。
    """
    replay_log_hash: str
    approved_by: str
    decision: str

    @classmethod
    def from_managed(cls, value) -> "ReplayApprovalRecord":
        """[EN]
        Create a Python record from the managed diagnostics contract.

        [JA]
        managed diagnostics contract から Python record を生成します。
        """
        return cls(
            replay_log_hash=str(value.ReplayLogHash),
            approved_by=str(value.ApprovedBy),
            decision=str(value.Decision),
        )

    def to_managed(self):
        """[EN]
        Convert this wrapper to AIKernel.Control.Diagnostics.ReplayApprovalRecord.

        [JA]
        この wrapper を AIKernel.Control.Diagnostics.ReplayApprovalRecord に変換します。
        """
        load_governance_runtime()
        from AIKernel.Control.Diagnostics import ReplayApprovalRecord as ManagedRecord  # type: ignore[import-not-found]

        return ManagedRecord(self.replay_log_hash, self.approved_by, self.decision)
