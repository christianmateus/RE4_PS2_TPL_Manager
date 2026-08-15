# Etapa 2 — UI escura e organização do projeto

## Interface

- Criado `UI/Theming/DarkTheme.cs` como ponto único para cores e estilos.
- Tema geral em preto/cinza escuro.
- Fonte padrão alterada em runtime para Segoe UI.
- Botões usam estilo flat, borda discreta e estados hover/click escuros.
- DataGridView ganhou cabeçalho escuro, linhas alternadas, seleção discreta e espaçamento melhor.
- Menu principal, status bar e menus de contexto usam renderer escuro.
- NumericUpDown, TextBox, ComboBox, GroupBox, painéis e dialogs seguem a mesma linguagem visual.
- Preview continua usando `Zoom` e recebeu fundo mais escuro.
- O tema é aplicado fora dos arquivos Designer para facilitar manutenção futura.

## Estrutura de pastas

- `App/` — ponto de entrada da aplicação.
- `Core/Conversion/` — conversores/importadores BMP/TGA e auxiliares.
- `Core/Formats/` — definição/estrutura do formato TPL.
- `Imaging/` — quantização, dithering, buffers, pixels, helpers e path providers.
- `ThirdParty/` — código de bibliotecas incorporadas ao projeto.
- `UI/Forms/` — formulário principal e tela de créditos.
- `UI/Dialogs/` — diálogos auxiliares.
- `UI/Theming/` — tema visual centralizado.
- `Docs/` — notas das etapas de desenvolvimento.
- `Properties/` — arquivos padrão do projeto WinForms.

Os namespaces existentes foram preservados nesta etapa para evitar uma refatoração funcional desnecessária. O `.csproj` foi atualizado para refletir todos os novos caminhos físicos.

## O que testar

1. Abrir o projeto no Visual Studio e executar `Clean Solution` + `Rebuild Solution`.
2. Abrir a aplicação sem arquivo e conferir a tela inicial.
3. Abrir um TPL e confirmar que tabela, preview, menus e editor continuam funcionando.
4. Abrir o menu de contexto da tabela e do preview e verificar o tema escuro.
5. Testar os diálogos de seleção de cor/índice.
6. Testar redimensionamento da janela em 100%, 125% e 150% de escala do Windows, se possível.
7. Confirmar que abrir/preview continua com a performance da Etapa 1.
8. Fazer uma operação de exportação e uma de replace apenas como teste de regressão.
