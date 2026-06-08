from __future__ import annotations

from typing import Mapping

from aikernel_governance.native import load_governance_runtime


class ManagedObject:
    """[EN]
    Base wrapper for public C# governance objects.

    [JA]
    公開 C# governance object の基底 wrapper です。
    """

    def __init__(self, managed):
        self._managed = managed

    @property
    def managed(self):
        """[EN]
        Return the underlying C# object.

        [JA]
        背後の C# object を返します。
        """
        return self._managed

    def to_managed(self):
        """[EN]
        Return the underlying C# object for contract calls.

        [JA]
        契約呼び出し用に背後の C# object を返します。
        """
        return self._managed


def to_dictionary(values: Mapping[str, str]):
    """[EN]
    Convert a Python mapping to System.Collections.Generic.Dictionary.

    [JA]
    Python mapping を System.Collections.Generic.Dictionary に変換します。
    """
    load_governance_runtime()
    from System.Collections.Generic import Dictionary  # type: ignore[import-not-found]

    dictionary = Dictionary[str, str]()
    for key, value in values.items():
        dictionary[str(key)] = str(value)
    return dictionary


def to_string_list(values):
    """[EN]
    Convert Python values to System.Collections.Generic.List[str].

    [JA]
    Python values を System.Collections.Generic.List[str] に変換します。
    """
    load_governance_runtime()
    from System.Collections.Generic import List  # type: ignore[import-not-found]

    items = List[str]()
    for value in values:
        items.Add(str(value))
    return items
