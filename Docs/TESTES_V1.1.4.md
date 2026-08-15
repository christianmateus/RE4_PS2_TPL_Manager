# Tests for v1.1.4

1. Open a 4-bit texture with transparent/semi-transparent areas and use Increase Color Depth. Compare preview before/after.
2. Open an 8-bit texture with transparency and use Decrease Color Depth. Compare preview before/after.
3. Close and reopen the TPL after each conversion to verify alpha was really written to the CLUT.
4. Test Increase All / Decrease All on a copy of a TPL containing transparent textures.
5. Import a transparent PNG and verify its alpha survives Replace and reopening the TPL.
6. Test Apply Changes on a transparent texture.
7. If possible, validate representative textures in-game on RE4 PS2.

Note: reducing 256 colors to 16 necessarily quantizes colors and alpha levels; exact visual identity is only possible when the source fits in 16 palette entries. Transparency should nevertheless remain present instead of becoming fully opaque.
