# Testes sugeridos - v1.1.7

1. Clean Solution e Rebuild Solution.
2. Abra um TPL contendo texturas 4-bit e 8-bit.
3. Prepare uma pasta Batch Replace com PNGs true-color para ambos os tipos.
4. Execute Batch Replace e confirme que nenhum diálogo 16/256 aparece por textura.
5. Reabra o TPL e confira que alvos 4-bit continuam 4-bit e alvos 8-bit continuam 8-bit.
6. Confira transparência das texturas substituídas.
7. Use Create Empty TPL, escolha nome e pasta próprios e confirme que o arquivo é criado ali.
8. Cancele Create Empty TPL e confirme que nenhum arquivo é criado.
9. Confirme que não aparece `temp.tpl` na pasta do programa.
10. Inicie o executável diretamente e confirme que nenhuma janela de console é aberta.
11. Confira se o novo TPL aparece em Recent Files.
