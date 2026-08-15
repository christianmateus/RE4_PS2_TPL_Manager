# Testes recomendados - v1.1.0

1. Clean Solution e Rebuild Solution.
2. Abrir TPL pequeno e grande; conferir quantidade, dimensões, bit depth, interlace e preview.
3. Abrir texturas 4-bit, 8-bit e 32-bit, incluindo não quadradas.
4. Exportar algumas texturas e comparar visualmente com a v1.0/Etapa 2.1.
5. Replace de uma textura por BMP/PNG 16 cores.
6. Replace de uma textura por BMP/PNG 256 cores.
7. Replace por outro arquivo TPL, inclusive escolhendo um índice quando o arquivo tiver várias texturas.
8. Fazer Replace por imagem de dimensões diferentes e reabrir o arquivo para validar offsets.
9. Em textura com mipmaps, testar Replace escolhendo Sim e depois, em uma cópia, escolhendo Não para atualização dos mipmaps.
10. Testar Duplicate, Remove, Remove Mipmaps e Rearrange para garantir compatibilidade com as rotinas legadas que ainda usam o novo rebuild de offsets.
11. Se possuir TPL 32-bit, fazer pelo menos abertura + Replace e reabrir o arquivo; esta versão corrigiu o cálculo de 4 bytes por pixel no writer.

Se ocorrer erro, anotar a operação, índice da textura, bit depth, dimensões e enviar o TPL usado no teste.
