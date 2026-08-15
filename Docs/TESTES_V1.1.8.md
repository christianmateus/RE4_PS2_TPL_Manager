# Testes sugeridos - v1.1.8

1. Open a TPL containing a mix of 4-bit and 8-bit textures.
2. Run Batch Replace and cancel the color-depth dialog. No file/folder operation should occur.
3. Run Batch Replace with **Preserve TPL color depth** and confirm 4-bit targets remain 4-bit and 8-bit targets remain 8-bit.
4. Run on a copy of the TPL with **Force 4-bit**. All image replacements found by the batch should become 4-bit / 16 colors.
5. Run on another copy with **Force 8-bit**. All image replacements should become 8-bit / 256 colors.
6. Verify alpha/transparency remains correct after each mode.
7. Verify Batch Replace does not ask for color depth for each texture.
8. Verify PNG, BMP, TGA, JPG and JPEG image sources work.
9. If a .tpl file is used as a batch source, verify it still replaces using texture index 0 and ignores the image color-depth mode.
10. Verify the status bar reports the selected mode and completion counts.
