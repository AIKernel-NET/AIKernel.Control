from __future__ import annotations

from aikernel_governance.native import load_governance_runtime

from .managed import ManagedObject


class Bonsai1BitCpuKernel(ManagedObject):
    """[EN]
    Wrapper for the public Bonsai1BitCpuKernel.

    [JA]
    公開 Bonsai1BitCpuKernel の wrapper です。
    """

    @classmethod
    def create(cls) -> "Bonsai1BitCpuKernel":
        """[EN]
        Create the managed CPU kernel.

        [JA]
        managed CPU kernel を生成します。
        """
        load_governance_runtime()
        from AIKernel.Control.CPU import Bonsai1BitCpuKernel as ManagedKernel  # type: ignore[import-not-found]

        return cls(ManagedKernel())

    @property
    def kernel_id(self) -> str:
        """[EN]
        Return the C# kernel identifier.

        [JA]
        C# kernel identifier を返します。
        """
        return str(self.managed.KernelId)

    @property
    def q1_block_element_count(self) -> int:
        """[EN]
        Return the Q1 block element count from the C# kernel.

        [JA]
        C# kernel の Q1 block element count を返します。
        """
        return int(self.managed.Q1BlockElementCount)

    @property
    def q1_block_byte_count(self) -> int:
        """[EN]
        Return the Q1 block byte count from the C# kernel.

        [JA]
        C# kernel の Q1 block byte count を返します。
        """
        return int(self.managed.Q1BlockByteCount)
