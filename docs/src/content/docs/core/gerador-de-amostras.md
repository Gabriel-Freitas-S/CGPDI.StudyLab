---
title: Gerador de Amostras Sintéticas & Testes Óticos
description: Como a classe ImageSampleGenerator.cs cria padrões geométricos procedurais para validação de algoritmos de PDI.
---

Para validar e avaliar o comportamento de filtros espaciais, detecção de contornos e transformadas de Fourier, o sistema gera **padrões geométricos precisos** diretamente na memória.

A classe [`ImageSampleGenerator.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Core/ImageSampleGenerator.cs) é responsável por essa geração procedural.

---

## 1. Padrões de Calibração Implementados

### 1. Barras de Cores SMPTE / NTSC
- **Objetivo:** Avaliar a fidelidade na conversão entre espaços RGB, HSV e YCbCr.
- **Estrutura:** 8 faixas verticais: Branco, Amarelo, Ciano, Verde, Magenta, Vermelho, Azul e Preto.

### 2. Círculos Concêntricos de Frequência Espacial (Siemens Star)
- **Objetivo:** Testar resolução espacial e observar efeitos de *Aliasing* (Moiré) em transformações geométricas e filtros passa-baixa.
- **Equação:**
$$
f(x, y) = 127.5 \times \left(1 + \cos\left( \frac{\sqrt{(x - x_c)^2 + (y - y_c)^2}}{k} \right)\right)
$$

### 3. Tabuleiro de Xadrez (Checkerboard)
- **Objetivo:** Testar detectores de cantos e derivadas espaciais de segunda ordem (Laplaciano).
- **Lógica:**
```csharp
int squareSize = 32;
bool isWhite = ((x / squareSize) + (y / squareSize)) % 2 == 0;
byte val = isWhite ? (byte)255 : (byte)0;
```

### 4. Rampa de Gradiente Linear
- **Objetivo:** Testar linearidade de contraste, correção Gamma e efeitos de posterização.
- **Equação:**
$$
I(x, y) = \text{round}\left( \frac{x}{\text{Width}} \times 255 \right)
$$

---

## 2. Como Utilizar no Aplicativo

Na interface do aplicativo:
1. Acesse a aba **Processamento Digital de Imagens (PDI)**.
2. No painel de controle, selecione o menu **"Carregar Imagem de Teste"**.
3. Escolha o padrão desejado para visualização imediata.

---

**Próximo Passo:** Entre no módulo de [Operações Pontuais e Histogramas de PDI](/pdi/operacoes-pontuais-e-histogramas/).
