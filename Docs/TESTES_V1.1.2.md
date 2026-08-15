# Tests for v1.1.2

1. Clean + Rebuild Solution.
2. Open a TPL and use Increase Color Depth on a 4-bit texture. Confirm it becomes 8-bit and preview remains correct.
3. Use Decrease Color Depth on an 8-bit texture. Confirm it becomes 4-bit and preview remains correct.
4. Test Increase All / Decrease All on a copy of a TPL.
5. Replace a texture with PNG, BMP and TGA. Confirm the table refreshes automatically after each replacement.
6. Edit the preview and click Apply Changes. Confirm the table/preview refresh automatically.
7. Tools > PNG to TPL: cancel the file dialog. Nothing should happen and no error should be shown.
8. Repeat cancellation for Open TPL, BMP to TPL, Fix Broken TPL, SMD/EFF file actions, image swap/mask/overlay and folder-selection actions.
9. Use PNG to TPL normally and verify a new file appears in Converted/ and opens correctly in the manager.
10. Batch replace: verify it refreshes once after the batch finishes.
11. Close the application with and without a `.temp` directory present; it should close without errors.

Important: mipmap replacement still uses a legacy path and should be tested separately. Color-depth changes on textures with mipmaps deserve extra validation because mipmap payload sizes differ between 4-bit and 8-bit formats.
