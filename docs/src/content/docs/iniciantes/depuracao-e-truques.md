---
title: Depuração e Técnicas no Visual Studio (Breakpoints e Inspeção)
description: Como pausar a execução, inspecionar valores de pixels em tempo real e compreender o fluxo dos algoritmos.
---

Aprender a depurar (*debugar*) permite enxergar o que o computador está calculando passo a passo. Em vez de adivinhar o comportamento de um algoritmo, você pode **congelar o tempo** e examinar a cor e a posição exata de cada pixel.

---

## 1. O que é um Ponto de Interrupção (Breakpoint)?

### A Analogia do Botão de Pausa do Videogame:
Um **Breakpoint** funciona como o botão de *Pause* de um jogo: você escolhe uma linha de código e, no instante em que o computador chega nela, ele congela e espera você inspecionar o que está acontecendo.

### Como Inserir um Breakpoint:
1. Abra qualquer arquivo de código C# (por exemplo, `CGPDI.StudyLab/ImageProcessing/PointAndHistograms.cs`).
2. Clique na **margem esquerda** ao lado do número da linha onde deseja pausar (ou posicione o cursor e pressione **`F9`**).
3. Um indicador circular vermelho será exibido na margem.
4. Execute a aplicação pressionando **`F5`**.
5. Ao interagir com a interface e acionar a função correspondente, o Visual Studio pausará a execução e destacará a linha em amarelo.

---

## 2. Inspecionando Variáveis e Cálculos

Com a execução pausada:
- **Passe o cursor do mouse** sobre qualquer variável (como `x`, `y`, `r`, `g`, `b`) para visualizar seu valor numérico atual.
- Utilize a janela inferior **Locais** (*Locals*) para ver todas as variáveis da função organizadas em tabela.
- Utilize a janela de **Inspeção** (*Watch*) para testar expressões matemáticas (como `(r * 299 + g * 587 + b * 114) / 1000`) em tempo real.

---

## 3. Teclas de Controle de Execução Passo a Passo

| Tecla | Comando | Descrição |
| :--- | :--- | :--- |
| **`F10`** | **Avançar (Step Over)** | Executa a linha atual e avança para a próxima linha da mesma função. |
| **`F11`** | **Intervir (Step Into)** | Se a linha chamar outro método (ex: `ColorSpaces.RgbToHsv`), entra no corpo do método chamado. |
| **`Shift + F11`** | **Sair (Step Out)** | Conclui a execução do método atual e retorna ao chamador. |
| **`F5`** | **Continuar (Continue)** | Retoma a execução normal até o próximo ponto de interrupção. |
| **`Shift + F5`** | **Parar Depuração (Stop)** | Encerra o aplicativo. |

---

## 4. Hot Reload (Alteração de Código em Tempo de Execução)

O .NET 10 permite ajustar valores numéricos ou fórmulas e visualizar o resultado sem reiniciar a aplicação:

1. Execute o projeto (`F5`).
2. Altere um parâmetro matemático no código C#.
3. Clique no botão de **Hot Reload** (ou pressione `Alt + F10`).
4. A lógica será atualizada instantaneamente.

---

## 5. Diagnóstico de Erros Comuns

### Erro: `IndexOutOfRangeException` (Índice Fora dos Limites)
- **Causa:** O código tentou acessar um pixel fora das dimensões da imagem (por exemplo, coluna $x = 512$ em uma imagem de largura $512$, onde os índices válidos vão de $0$ a $511$).
- **Solução:** Certifique-se de que os laços utilizem `< Width` e aplique a função `Math.Clamp(x, 0, width - 1)`.

### Erro: `AccessViolationException` ou Ponteiro Nulo
- **Causa:** O método tentou acessar o ponteiro de memória antes de chamar `DirectBitmap.Lock()` ou após `DirectBitmap.Unlock()`.
- **Solução:** Envolva o processamento sempre no padrão `bmp.Lock()` no início e `bmp.Unlock()` no término.

---

<div class="ms-ref-card">
  <h4>📚 Referências Oficiais Microsoft Learn</h4>
  <p>Recursos para depuração e diagnóstico de performance no Visual Studio:</p>
  <ul>
    <li><a href="https://learn.microsoft.com/pt-br/visualstudio/debugger/debugger-feature-tour" target="_blank" rel="noopener">Tour de Recursos do Depurador do Visual Studio</a> — Como utilizar Breakpoints, DataTips, Watch e Janela de Memória.</li>
    <li><a href="https://learn.microsoft.com/pt-br/visualstudio/profiling/profiling-feature-tour" target="_blank" rel="noopener">Ferramentas de Criação de Perfil (Profiling) no Visual Studio</a> — Diagnóstico de CPU, alocação de memória e uso de GPU.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.diagnostics.stopwatch" target="_blank" rel="noopener">Classe Stopwatch (System.Diagnostics)</a> — Medição precisa de intervalos de tempo em milissegundos e tiques de CPU.</li>
  </ul>
</div>

---

👉 **Próximo Passo:** Conheça a [Visão Geral da Arquitetura do Software](/arquitetura/visao-geral/).
