"""[EN]
GPU governance wrappers.

AIKernel.Control.GPU currently exposes the public IBonsaiGpuExecutionDelegate
contract. Python receives the loaded C# interface through pythonnet; concrete
GPU implementations remain external and are not reimplemented here.

[JA]
GPU governance wrapper です。

AIKernel.Control.GPU は現在、公開 IBonsaiGpuExecutionDelegate 契約を公開します。
Python は pythonnet 経由で読み込まれた C# interface を受け取り、具体的な GPU
実装は外部に残します。ここでは再実装しません。
"""

from __future__ import annotations

from aikernel_governance.native import load_governance_runtime


def bonsai_gpu_execution_delegate_contract():
    """[EN]
    Return the public managed IBonsaiGpuExecutionDelegate contract.

    [JA]
    公開 managed IBonsaiGpuExecutionDelegate 契約を返します。
    """
    load_governance_runtime()
    from AIKernel.Control.GPU import IBonsaiGpuExecutionDelegate  # type: ignore[import-not-found]

    return IBonsaiGpuExecutionDelegate
