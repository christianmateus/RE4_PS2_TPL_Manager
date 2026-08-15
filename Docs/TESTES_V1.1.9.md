# Tests - v1.1.9 Experimental

- Clean + Rebuild Solution.
- Open a TPL and verify existing preview/replace/export still work.
- 8-bit BGRA -> PS2 -> reopen -> preview -> in-game.
- 8-bit PS2 -> BGRA -> reopen -> preview -> in-game.
- Repeat with Inverted source; the low-bit flag must remain Inverted.
- Test a supported 4-bit 32/64/128/256 square texture in both directions.
- Confirm non-square 4-bit and mipmapped textures are rejected with a clear message and are not modified.
- Confirm a `.bak` is created before the first successful conversion.
- Round-trip a test texture BGRA -> PS2 -> BGRA and compare visually with the starting texture.
