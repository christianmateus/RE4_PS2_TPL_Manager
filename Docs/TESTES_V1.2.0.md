# Testes v1.2.0

1. Abra uma textura 8-bit PS2, faça Replace por PNG e confirme que continua marcada como PS2 após reabrir.
2. Repita com 4-bit PS2 em tamanho suportado.
3. Teste BGRA e BGRA Inverted: o valor de Interlace deve permanecer igual após Replace.
4. Faça Increase Color Depth em uma textura 4-bit PS2: deve virar 8-bit mantendo PS2 e alpha.
5. Faça Decrease Color Depth em uma textura 8-bit PS2 suportada para 4-bit: deve manter PS2.
6. Rode Batch Replace nos três modos (Preserve/Force 4-bit/Force 8-bit) e confira que o modo muda somente bit depth; Interlace deve permanecer o do destino.
7. Faça Apply Changes em uma textura PS2 e reabra o arquivo.
8. Em uma textura com mipmaps, aceite atualizar mipmaps e verifique no jogo.
9. Teste Tools > Convert Interlace... novamente nos dois sentidos.
10. Em 4-bit PS2 com dimensão não suportada, confirme que a ferramenta recusa a operação sem alterar o arquivo.
