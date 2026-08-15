# RE4 PS2 TPL Manager v1.2.0

Stable interlace-preservation release. PNG/BMP/TGA Replace, Batch Replace, Apply Changes, Increase/Decrease Color Depth and mipmap replacement now preserve the destination texture interlace layout (BGRA/BGRA Inverted/PS2/PS2 Inverted) whenever the validated swizzle supports that texture.

The previously experimental **Tools > Convert Interlace...** command is now an official tool. It still refuses unsafe 4-bit layouts and textures with mipmaps instead of risking corrupt output.

See `Docs/V1.2.0.md` and `Docs/TESTES_V1.2.0.md`.
