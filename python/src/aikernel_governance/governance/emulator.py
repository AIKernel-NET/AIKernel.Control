from __future__ import annotations

from dataclasses import dataclass, field
from typing import Mapping, Sequence

from aikernel_governance.native import load_governance_runtime

from .managed import ManagedObject, to_dictionary
from .request import ExecutionRequest
from .result import ExecutionResult


@dataclass(frozen=True)
class EmulatedExecutionNode:
    """[EN]
    Python wrapper for the public EmulatedExecutionNode contract.

    [JA]
    公開 EmulatedExecutionNode 契約の Python wrapper です。
    """
    node_id: str
    operator_id: str
    metadata: Mapping[str, str] = field(default_factory=dict)
    fault_message: str | None = None

    def to_managed(self):
        """[EN]
        Convert this wrapper to AIKernel.Control.Emulator.EmulatedExecutionNode.

        [JA]
        この wrapper を AIKernel.Control.Emulator.EmulatedExecutionNode に変換します。
        """
        load_governance_runtime()
        from AIKernel.Control.Emulator import EmulatedExecutionNode as ManagedNode  # type: ignore[import-not-found]

        if self.fault_message is None:
            return ManagedNode(self.node_id, self.operator_id, to_dictionary(self.metadata))
        return ManagedNode(self.node_id, self.operator_id, to_dictionary(self.metadata), self.fault_message)


@dataclass(frozen=True)
class EmulatedExecutionGraph:
    """[EN]
    Python wrapper for the public EmulatedExecutionGraph contract.

    [JA]
    公開 EmulatedExecutionGraph 契約の Python wrapper です。
    """
    graph_id: str
    nodes: Sequence[EmulatedExecutionNode]

    def to_managed(self):
        """[EN]
        Convert this wrapper to AIKernel.Control.Emulator.EmulatedExecutionGraph.

        [JA]
        この wrapper を AIKernel.Control.Emulator.EmulatedExecutionGraph に変換します。
        """
        load_governance_runtime()
        from AIKernel.Control.Emulator import EmulatedExecutionGraph as ManagedGraph  # type: ignore[import-not-found]
        from System.Collections.Generic import List  # type: ignore[import-not-found]
        from AIKernel.Abstractions.Control import IExecutionNode  # type: ignore[import-not-found]

        nodes = List[IExecutionNode]()
        for node in self.nodes:
            nodes.Add(node.to_managed())
        return ManagedGraph(self.graph_id, nodes)


class AllowAllControlPolicy(ManagedObject):
    """[EN]
    Wrapper for the public AllowAllControlPolicy implementation.

    [JA]
    公開 AllowAllControlPolicy 実装の wrapper です。
    """

    @classmethod
    def create(cls) -> "AllowAllControlPolicy":
        """[EN]
        Create the managed allow-all policy.

        [JA]
        managed allow-all policy を生成します。
        """
        load_governance_runtime()
        from AIKernel.Control.Emulator import AllowAllControlPolicy as ManagedPolicy  # type: ignore[import-not-found]

        return cls(ManagedPolicy())


class DeterministicNodeScheduler(ManagedObject):
    """[EN]
    Wrapper for the public deterministic node scheduler.

    [JA]
    公開 deterministic node scheduler の wrapper です。
    """

    @classmethod
    def create(cls) -> "DeterministicNodeScheduler":
        """[EN]
        Create the managed deterministic scheduler.

        [JA]
        managed deterministic scheduler を生成します。
        """
        load_governance_runtime()
        from AIKernel.Control.Emulator import DeterministicNodeScheduler as ManagedScheduler  # type: ignore[import-not-found]

        return cls(ManagedScheduler())


class ControlEmulatorEngine(ManagedObject):
    """[EN]
    Wrapper for the public ControlEmulatorEngine.

    [JA]
    公開 ControlEmulatorEngine の wrapper です。
    """

    @classmethod
    def create(cls, scheduler=None, policy=None, observer=None) -> "ControlEmulatorEngine":
        """[EN]
        Create the managed emulator engine from public dependencies.

        [JA]
        公開 dependency から managed emulator engine を生成します。
        """
        load_governance_runtime()
        from AIKernel.Control.Emulator import ControlEmulatorEngine as ManagedEngine  # type: ignore[import-not-found]

        managed_scheduler = scheduler.to_managed() if hasattr(scheduler, "to_managed") else scheduler
        managed_policy = policy.to_managed() if hasattr(policy, "to_managed") else policy
        managed_observer = observer.to_managed() if hasattr(observer, "to_managed") else observer
        return cls(ManagedEngine(managed_scheduler, managed_policy, managed_observer))

    @property
    def engine_id(self) -> str:
        """[EN]
        Return the engine identifier exposed by the C# engine.

        [JA]
        C# engine が公開する engine identifier を返します。
        """
        return str(self.managed.EngineId)

    def execute(self, graph: EmulatedExecutionGraph, request: ExecutionRequest):
        """[EN]
        Execute a graph through the C# emulator engine.

        [JA]
        C# emulator engine を通じて graph を実行します。
        """
        managed_result = self.managed.ExecuteAsync(
            graph.to_managed(),
            request.to_managed(),
        ).AsTask().GetAwaiter().GetResult()
        return ExecutionResult.from_managed(managed_result)
