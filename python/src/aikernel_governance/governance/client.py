from __future__ import annotations

from dataclasses import dataclass
from typing import Any

from .request import ExecutionRequest
from .result import ExecutionResult
from .snapshot import Snapshot


@dataclass(frozen=True)
class GovernanceClient:
    """[EN]
    Thin client facade over a public governance backend.

    [JA]
    公開 governance backend に委譲する薄い client facade です。
    """

    backend: Any

    def submit(self, request: ExecutionRequest) -> ExecutionResult:
        """[EN]
        Submit an execution request through the configured backend.

        [JA]
        設定された backend 経由で execution request を送信します。
        """
        result = _call(self.backend, "submit", "Submit", request)
        return _coerce_result(result)

    def snapshot(self, id: str) -> Snapshot:
        """[EN]
        Retrieve a governance snapshot by execution identifier.

        [JA]
        execution identifier に基づいて governance snapshot を取得します。
        """
        result = _call(self.backend, "snapshot", "Snapshot", id)
        return _coerce_snapshot(result)

    def result(self, id: str) -> ExecutionResult:
        """[EN]
        Retrieve an execution result by execution identifier.

        [JA]
        execution identifier に基づいて execution result を取得します。
        """
        result = _call(self.backend, "result", "Result", id)
        return _coerce_result(result)


def _call(target: Any, python_name: str, managed_name: str, argument: Any) -> Any:
    method = getattr(target, python_name, None) or getattr(target, managed_name, None)
    if method is None:
        raise AttributeError(
            f"governance backend must expose {python_name}({argument.__class__.__name__})"
        )
    return method(argument)


def _coerce_result(value: Any) -> ExecutionResult:
    if isinstance(value, ExecutionResult):
        return value
    return ExecutionResult.from_managed(value)


def _coerce_snapshot(value: Any) -> Snapshot:
    if isinstance(value, Snapshot):
        return value
    return Snapshot.from_managed(value)
