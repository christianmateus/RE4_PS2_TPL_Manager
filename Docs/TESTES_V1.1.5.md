# Test checklist - v1.1.5

1. Open a TPL and right-click a row that is not currently selected. The clicked row must become visibly selected before the context menu appears.
2. Confirm context menu operations affect that visibly selected row.
3. Try to edit the `Title` column. It must not enter edit mode.
4. Preview very small textures such as 8x8, 16x16, 32x32 and 64x64. Pixels should remain crisp instead of blurred.
5. Preview large textures. They should fit inside the preview without distortion.
6. Check transparent textures: the preview background should expose transparency through the checkerboard.
7. Resize the application window and verify the preview re-renders cleanly.
8. Perform open, refresh, save, extract, add, replace, color-depth, export and remove actions and verify the status bar reports useful feedback.
9. Run the v1.1.4 regression tests for transparency and color depth to confirm the image pipeline was not changed.
