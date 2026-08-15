---
title: Preenchimento de Polígonos & Recorte (Rasterizer2D.cs)
description: Algoritmo de Preenchimento por Varredura (Scanline com AET), Flood Fill por fila e Recorte de Linhas de Cohen-Sutherland com Outcodes.
---

O arquivo [`Rasterizer2D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics2D/Rasterizer2D.cs) implementa o preenchimento de polígonos côncavos e complexos e o recorte contra a janela de visualização.

---

## 1. Preenchimento por Varredura (Scanline Polygon Fill)

Para preencher o interior de qualquer polígono geométrico linha por linha:
1. **Tabela de Arestas Ativas (AET):** Guarda as arestas que cruzam a linha horizontal atual $y$.
2. **Regra de Paridade (Par-Ímpar):** Ordena os pontos de corte da linha em $x$. Pinta os pixels entre o 1º e o 2º corte, pula do 2º ao 3º, e pinta do 3º ao 4º.

---

## 2. Algoritmo de Inundação (Flood Fill baseado em Fila)

O **Flood Fill** (ferramenta de balde de tinta) substitui uma cor conectada por uma nova cor.

:::danger[Por que não usar recursão ingênua?]
Funções recursivas para imagens de $512 \times 512$ geram centenas de milhares de chamadas empilhadas, causando o erro de estouro de pilha (*StackOverflowException*).
:::

No nosso projeto, utilizamos uma **Fila Explícita (`Queue<Point>`)**, garantindo estabilidade e memória suficiente para qualquer preenchimento.

---

## 3. Recorte de Linhas de Cohen-Sutherland (Outcodes de 4 bits)

O algoritmo divide o espaço em 9 regiões usando códigos binários de 4 bits `[Cima, Baixo, Direita, Esquerda]`:
- **Totalmente Visível:** $\text{code}_0 \mid \text{code}_1 == 0$
- **Totalmente Invisível:** $\text{code}_0 \ \& \ \text{code}_1 \neq 0$
- **Caso Contrário:** Corta a linha na borda e recalcula o segmento restante.

---

👉 **Próximo Passo:** Entre no módulo de [Computação Gráfica 3D & Matrizes MVP](/CGPDI.StudyLab/cg3d/matematica-vetorial-e-mvp/).
