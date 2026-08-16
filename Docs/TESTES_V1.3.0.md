# Tests - v1.3.0

Recommended tests using a copy of the original TPL:

1. Double-click a texture with `Mipmaps = 2` and verify Main/Mip1/Mip2 previews.
2. Replace only Mip 1 with a PNG and confirm Main/Mip2 remain unchanged.
3. Replace only Mip 2 and test the TPL in RE4 PS2.
4. Use Regenerate Mipmaps, reopen the TPL and verify both mip levels.
5. Double-click a texture with `Mipmaps = 0`, use Add Mipmaps and verify the table changes to `2`.
6. Reopen the file, then remove the mipmaps and verify the table returns to `0`.
7. On a 256x256 4-bit PS2 texture, verify generated mip interlace follows the common RE4 layout (128x128 PS2, 64x64 BGRA).
8. Test alpha/transparency on foliage, masks or HUD textures after regeneration.
9. Test normal Replace on a texture with mipmaps and choose Yes when asked to update them.
10. Test Increase/Decrease Color Depth on a texture with mipmaps and verify the mip levels remain valid.
