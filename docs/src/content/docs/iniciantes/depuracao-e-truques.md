---
title: Depuração e Truques no Visual Studio (Breakpoints e Inspeção)
description: Aprenda como pausar o código, inspecionar valores de pixels em tempo real e entender mensagens de erro como um programador profissional.
---

Aprender a depurar (*debugar*) é a habilidade mais importante para entender como os algoritmos de Computação Gráfica e Processamento de Imagens funcionam por dentro. Em vez de tentar adivinhar o que o código está fazendo, você pode **pausar a execução no tempo** e ver o valor exato de cada pixel!

---

## 🛑 1. O que é um Breakpoint (Ponto de Interrupção)?

Um **Breakpoint** é uma marcação que você coloca em uma linha de código. Quando o programa chega nessa linha, ele **congela imediatamente** e mostra o estado da memória.

### Como colocar um Breakpoint:
1. Abra qualquer arquivo de código no Visual Studio (por exemplo, `CGPDI.StudyLab/ImageProcessing/PointAndHistograms.cs`).
2. Clique na **margem cinza à esquerda** da linha de código (ao lado do número da linha) ou coloque o cursor na linha e aperte a tecla **`F9`**.
3. Uma bolinha vermelha 🔴 aparecerá na margem.
4. Execute o programa apertando **`F5`**.
5. Quando você clicar no botão na tela correspondente àquele filtro, a tela do Visual Studio piscará em amarelo destacando a linha onde o programa parou!

---

## 🔍 2. Inspecionando Valores de Variáveis

Quando o código está pausado em um Breakpoint:
- **Passe o mouse por cima** de qualquer variável (como `x`, `y`, `r`, `g`, `b` ou `contrastFactor`) para ver o valor numérico que ela contém naquele exato instante.
- Abra a janela inferior **Locais** (*Locals*) ou **Automáticos** (*Autos*) para ver todas as variáveis da função organizadas em uma tabela.
- Use a janela de **Inspeção** (*Watch*) digitando expressões matemáticas (como `r * 0.299 + g * 0.587`) para ver o resultado do cálculo em tempo real.

---

## 🕹️ 3. Teclas de Controle de Execução

Ao pausar em uma linha amarela, você pode avançar passo a passo usando as teclas de atalho:

| Tecla | Comando | O que faz? |
| :--- | :--- | :--- |
| **`F10`** | **Depurar Passo a Passo (Step Over)** | Executa a linha atual e vai para a próxima linha do mesmo arquivo. |
| **`F11`** | **Intervir (Step Into)** | Se a linha for uma chamada de função (ex: `ColorSpaces.RgbToHsv`), "entra" dentro dessa função para você ver seu código interno. |
| **`Shift + F11`** | **Sair da Função (Step Out)** | Termina de executar a função atual e volta para onde ela foi chamada. |
| **`F5`** | **Continuar (Continue)** | Descongela a execução até encontrar o próximo Breakpoint ou terminar. |
| **`Shift + F5`** | **Parar Depuração (Stop)** | Encerra o programa imediatamente. |

---

## ⚡ 4. Hot Reload (Editar Código Sem Reiniciar)

No .NET 10 e Visual Studio moderno, você pode alterar fórmulas matemáticas ou cores enquanto o programa está rodando!

1. Execute o programa normalmente (`F5`).
2. Vá no código C# e altere, por exemplo, o valor de um peso de cor de `0.2126` para `0.5`.
3. Clique no botão de **Foguinho / Raio (Hot Reload)** no topo da barra de ferramentas ou aperte `Alt + F10`.
4. O programa atualizará a lógica instantaneamente sem precisar fechar e reabrir a janela!

---

## ⚠️ 5. Erros Comuns e Como Resolver

### Erro: `IndexOutOfRangeException` (Índice Fora dos Limites)
- **O que significa:** O código tentou acessar um pixel fora das dimensões da imagem (por exemplo, pixel $x = 512$ em uma imagem de largura $512$, onde os índices válidos vão de $0$ a $511$).
- **Como corrigir:** Verifique se os laços de repetição estão usando `< Width` e `< Height` e use a função `Math.Clamp(x, 0, width - 1)`.

### Erro: `AccessViolationException` ou Ponteiro Nulo
- **O que significa:** O método tentou ler ou escrever em um ponteiro de memória antes de chamar `DirectBitmap.Lock()` ou depois de `DirectBitmap.Unlock()`.
- **Como corrigir:** Certifique-se de que todo o processamento de pixels esteja envelopado entre um par `bmp.Lock();` no início e `bmp.Unlock();` no final.

---

👉 **Próximo Passo:** Entenda a [Visão Geral da Arquitetura do Software](/CGPDI.StudyLab/arquitetura/visao-geral/).
