---
title: Gerador de Amostras Sintéticas & Testes Óticos
description: Como a classe ImageSampleGenerator.cs cria padrões geométricos procedurais para validação de algoritmos de PDI.
---

Para validar e estudar algoritmos de Processamento Digital de Imagens (como detecção de bordas, filtros de nitidez e transformadas de Fourier), é fundamental dispor de **padrões de calibração ótica geometricamente perfeitos** e isentos de ruído de compressão JPG.

A classe [`ImageSampleGenerator.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Core/ImageSampleGenerator.cs) permite gerar estes padrões sob demanda na memória.

---

## 🎨 1. Padrões Disponíveis no Sistema

### 1. Barras de Cores Padrão SMPTE / NTSC
- **Objetivo:** Calibração de fidelidade cromática e resposta dos canais RGB, HSV e YCbCr.
- **Estrutura:** 8 barras verticais perfeitas: Branco, Amarelo, Ciano, Verde, Magenta, Vermelho, Azul e Preto.

### 2. Círculos Concêntricos de Frequência Espacial (Siemens Star / Alvo Ótico)
- **Objetivo:** Testar resolução espacial e observar o efeito de *Aliasing* (Moiré) em transformações geométricas e filtros passa-baixa.
- **Fórmula Matemática:**
$$
f(x, y) = 127.5 \times \left(1 + \cos\left( \frac{\sqrt{(x - x_c)^2 + (y - y_c)^2}}{k} \right)\right)
$$

### 3. Tabuleiro de Xadrez (Checkerboard)
- **Objetivo:** Testar algoritmos de detecção de cantos (Corner Detectors) e derivadas de segunda ordem (Laplaciano).
- **Lógica:**
```csharp
int squareSize = 32;
bool isWhite = ((x / squareSize) + (y / squareSize)) % 2 == 0;
byte val = isWhite ? (byte)255 : (byte)0;
```

### 4. Gradiente de Rampa Suave de Luminância
- **Objetivo:** Testar linearidade de contraste, correção Gamma e efeitos de quantização (posterização).
- **Fórmula:**
$$
I(x, y) = \text{round}\left( \frac{x}{\text{Width}} \times 255 \right)
$$

---

## 💻 2. Como Utilizar no Aplicativo

Na interface gráfica do aplicativo:
1. Navegue até a aba **🖼️ Processamento Digital de Imagens (PDI)**.
2. No painel superior esquerdo, clique no menu suspenso **"Carregar Imagem de Teste"**.
3. Selecione qualquer um dos padrões procedurais gerados pelo `ImageSampleGenerator`.

---

👉 **Próximo Passo:** Entre no módulo de [Operações Pontuais e Histogramas de PDI](/CGPDI.StudyLab/pdi/operacoes-pontuais-e-histogramas/).
