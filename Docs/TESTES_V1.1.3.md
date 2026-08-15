# Tests for v1.1.3

1. Clean Solution and Rebuild Solution.
2. Delete any old `.temp` folder beside the executable/project, launch the manager, and confirm it is not recreated.
3. Open 4-bit and 8-bit TPL files and verify preview/loading still works.
4. Use Convert all to BMP and confirm `Converted/<tpl name>/0.bmp`, `1.bmp`, etc. are produced without a temporary cache.
5. Use Convert all to PNG and compare the exported images with the preview.
6. Replace a texture that has mipmaps and answer Yes to mipmap update; reopen the TPL and inspect base texture plus mipmaps in game/tooling.
7. Test mipmap replacement on both 4-bit and 8-bit textures when samples are available.
8. Extract TPLs from at least one known-good EFF and verify every extracted TPL opens normally.
9. Run PNG/BMP/TGA replacement and color-depth operations again to check that v1.1.2 behavior did not regress.
10. Close the application and confirm there is no temporary-folder cleanup error.
