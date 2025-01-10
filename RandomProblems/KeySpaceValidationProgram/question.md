Question
In the mysterious Grand Library of Alexandria (which secretly still exists!), historians discovered an ancient library containing some sacred knowledge in the form of books. The books were so holy that each one was kept in a vault and each unique vault was locked by a unique pattern, an n-dimensional vector.

Young apprentice librarian Maya found a set of keys lying on the floor along with a scroll. The scroll contained a spell that could work on a set of keys and generate all possible keys to open all possible vaults of the library. The spell combines the keys in a linear way.

Help Maya find if the set of keys she found is sufficient enough and also fewest possible to generate all possible keys and unlock all the vaults of the library. Help Maya write a program that checks this.

Note: Since the ancient mechanism works with real numbers, your solution should handle floating-point arithmetic carefully.

from typing import List

def can_unlock_library(keys: List[List[float]], tolerance: float = 1e-10) -> bool:
    """
    Args:
        keys: List of n vectors, each being a list of n floating-point numbers
        precision: Threshold for numerical calculations (default: 1e-10)

    Returns:
        bool: True if keys can unlock the library, False otherwise
    """
    pass
Example:

keys = [
    [1.0, 0.0, 0.0],
    [0.0, 1.0, 0.0],
    [0.0, 0.0, 1.0]
]
result = can_unlock_library(keys)  # Returns True
Explanation:

The set of provided keys are fewest possible keys that can generate all possible keys in 3-dimensional space when combined linearly and hence can unlock all the vaults of the library.

keys = [
    [2, 0, 0],
    [0, 2, 0],
    [4, 4, 0]
]
result = can_unlock_library(keys)  # Returns False
Explanation:

The set of provided keys cannot represent the key [0, 0, 1]. Hence all possible keys required to unlock the library.



Solution : https://gist.github.com/aarshdeepsinghchadha/61c9235c56a2c1cae316f331f8b4b1a3