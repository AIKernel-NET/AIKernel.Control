from __future__ import annotations

from dataclasses import dataclass, field
from typing import Mapping


@dataclass(frozen=True)
class ProviderContract:
    """[EN]
    Public provider contract metadata visible to governance callers.

    [JA]
    governance caller に公開される provider contract metadata を表します。
    """

    provider_id: str
    name: str
    version: str
    capabilities: tuple[str, ...] = ()
    metadata: Mapping[str, str] = field(default_factory=dict)
