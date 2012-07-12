csharp-bk-tree
==============

Generic BK-Tree (Burkhard-Keller) for fuzzy matching in discrete metric spaces.

This implementation is largely a port of Josh Clemm's Java example.
http://code.google.com/p/java-bk-tree

Modifications to original Java code:
 1. Promoted private node class that handled only some data primitives to generic public abstract class able to handle any data.
 2. Refactored to use native C# features and removed all calls back to tree from node.
 3. Added three example distance metrics: Hamming, Lee, and Levenshtein.
    - Hamming implementation is the Wegner algorithm posted in the Hamming distance Wikipedia page
    - Levenshtein implementation by Josh Clemm ported from Java to C#
    - Lee implementation of my own devising
