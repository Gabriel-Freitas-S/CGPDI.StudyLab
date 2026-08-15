# 🖼️ Capítulo 2: Processamento Digital de Imagens (PDI)

O módulo de PDI está organizado em classes dedicadas no diretório [`ImageProcessing/`](file:///D:/source/repos/teste/teste/ImageProcessing/) e [`Core/ColorSpaces.cs`](file:///D:/source/repos/teste/teste/Core/ColorSpaces.cs).

---

## 1. Modelos de Cor & Percepção Humana

### 1.1 Conversão para Escala de Cinza Perceptiva
A visão humana possui sensibilidade espectral assimétrica devido à densidade dos cones na fóvea ($M$ e $L$ superam amplamente os cones $S$):
* **Padrão ITU-R BT.709 (HDTV / sRGB):**
  $$Y = 0.2126 \cdot R + 0.7152 \cdot G + 0.0722 \cdot B$$
* **Padrão ITU-R BT.601 (NTSC / PAL / SDTV):**
  $$Y = 0.299 \cdot R + 0.587 \cdot G + 0.114 \cdot B$$

### 1.2 Espaço Cilíndrico HSV (Hue, Saturation, Value)
Permite separar o matiz cromático da iluminação:
$$V = \max(R, G, B), \quad \Delta = V - \min(R, G, B)$$
$$S = \begin{cases} 0, & \text{se } V = 0 \\ \frac{\Delta}{V}, & \text{se } V > 0 \end{cases}$$
$$H = \begin{cases} 0^\circ, & \text{se } \Delta = 0 \\ 60^\circ \times \left( \frac{G - B}{\Delta} \bmod 6 \right), & \text{se } V = R \\ 60^\circ \times \left( \frac{B - R}{\Delta} + 2 \right), & \text{se } V = G \\ 60^\circ \times \left( \frac{R - G}{\Delta} + 4 \right), & \text{se } V = B \end{cases}$$

### 1.3 Espaço YCbCr (Padrão JPEG / MPEG)
Isola a luminância $Y$ das componentes de diferença de azul ($C_b$) e vermelho ($C_r$):
$$Y = 0.299 \cdot R + 0.587 \cdot G + 0.114 \cdot B$$
$$C_b = 128 - 0.168736 \cdot R - 0.331264 \cdot G + 0.5 \cdot B$$
$$C_r = 128 + 0.5 \cdot R - 0.418688 \cdot G - 0.081312 \cdot B$$

---

## 2. Operações Pontuais & Histogramas

### 2.1 Correção de Gamma (Lei de Potência)
Compensa a resposta não-linear de displays e fotorreceptores:
$$s = c \cdot r^\gamma \implies g(x, y) = 255 \times \left( \frac{f(x, y)}{255} \right)^\gamma$$

### 2.2 Equalização de Histograma Global via CDF
Maximiza o contraste global distribuindo a probabilidade acumulada dos tons:
$$P(k) = \frac{n_k}{W \times H}, \quad \text{CDF}(k) = \sum_{j=0}^{k} P(j)$$
$$h_{\text{eq}}(v) = \text{round}\left( \frac{\text{CDF}(v) - \text{CDF}_{\min}}{(W \times H) - \text{CDF}_{\min}} \times 255 \right)$$

---

## 3. Filtros Espaciais & Convolução 2D

### 3.1 Equação da Convolução Discreta
$$g(x, y) = \sum_{u=-k}^{k} \sum_{v=-k}^{k} f(x - u, y - v) \cdot K(u, v)$$

* **Filtro Gaussiano 2D:**
  $$G(x, y) = \frac{1}{2\pi\sigma^2} e^{-\frac{x^2 + y^2}{2\sigma^2}}$$
* **Unsharp Masking (Realce de Detalhes):**
  $$g(x, y) = f(x, y) + \alpha \cdot \left[ f(x, y) - f_{\text{gauss}}(x, y) \right]$$
* **Filtro da Mediana (Não-linear):**
  $$g(x, y) = \text{mediana}\left(\{ f(x+i, y+j) \mid -r \le i, j \le r \}\right)$$
  Remove ruído impulsivo (*Salt and Pepper*) sem borrar bordas nítidas.

---

## 4. Detecção de Bordas & O Algoritmo Canny

### 4.1 Operadores de Gradiente
* **Sobel:**
  $$G_x = \begin{bmatrix} -1 & 0 & 1 \\ -2 & 0 & 2 \\ -1 & 0 & 1 \end{bmatrix} * f, \quad G_y = \begin{bmatrix} -1 & -2 & -1 \\ 0 & 0 & 0 \\ 1 & 2 & 1 \end{bmatrix} * f$$
  $$G = \sqrt{G_x^2 + G_y^2}, \quad \theta = \text{atan2}(G_y, G_x)$$

* **Laplaciano ($\nabla^2 f$ - Segunda Derivada):**
  $$\nabla^2 f = \frac{\partial^2 f}{\partial x^2} + \frac{\partial^2 f}{\partial y^2} = \begin{bmatrix} -1 & -1 & -1 \\ -1 & 8 & -1 \\ -1 & -1 & -1 \end{bmatrix} * f$$

### 4.2 Pipeline do Detector Canny em 5 Etapas
1. **Suavização Gaussiana:** Eliminação de ruídos espúrios de alta frequência.
2. **Cálculo do Gradiente:** Determinação de magnitude $G$ e ângulo $\theta$.
3. **Supressão de Não-Máximos (NMS):** O pixel só é mantido se seu gradiente for o pico estritamente local na direção do vetor normal à borda (setores $0^\circ, 45^\circ, 90^\circ, 135^\circ$), afinando a borda para 1 pixel.
4. **Limiarização Dupla (*Double Threshold*):** Separação em bordas fortes ($G \ge T_{\text{high}}$) e fracas ($T_{\text{low}} \le G < T_{\text{high}}$).
5. **Rastreamento por Histerese:** Algoritmo BFS que preserva bordas fracas conectadas a fortes e descarta ilhas isoladas.

---

## 5. Morfologia Matemática & Otsu

### 5.1 Binarização Ótima de Otsu
Encontra o limiar $t^*$ que maximiza a variância inter-classes:
$$\sigma_B^2(t) = \omega_0(t) \cdot \omega_1(t) \cdot \left[ \mu_0(t) - \mu_1(t) \right]^2$$

### 5.2 Operadores Morfológicos com Elemento Estruturante $B$
* **Erosão:** $(A \ominus B)(x, y) = \min_{(i,j) \in B} f(x+i, y+j)$
* **Dilatação:** $(A \oplus B)(x, y) = \max_{(i,j) \in B} f(x-i, y-j)$
* **Abertura:** $(A \ominus B) \oplus B$ (Elimina pequenas saliências claras)
* **Fechamento:** $(A \oplus B) \ominus B$ (Preenche lacunas e orifícios escuros)
* **Gradiente Morfológico:** $(A \oplus B) - (A \ominus B)$ (Extrai o contorno morfológico do objeto)

---

## 6. Transformações Geométricas & Frequência

### 6.1 Mapeamento Inverso (*Backward Mapping*) & Interpolações
Para evitar buracos na imagem destino, calcula-se $(x_{\text{src}}, y_{\text{src}}) = T^{-1}(x_{\text{dst}}, y_{\text{dst}})$.
* **Interpolação Bilinear:**
  $$f(x, y) = (1-u)(1-v)f_{00} + u(1-v)f_{10} + (1-u)v f_{01} + uv f_{11}$$
* **Interpolação Bicúbica:** Interpolação via polinômios cúbicos de Catmull-Rom sobre grade $4\times4$.

### 6.2 Transformada Discreta de Fourier 2D (DFT)
Converte do domínio espacial para o domínio das frequências espaciais $(u, v)$:
$$F(u, v) = \sum_{x=0}^{W-1} \sum_{y=0}^{H-1} f(x, y) e^{-j 2\pi \left( \frac{ux}{W} + \frac{vy}{H} \right)}$$
O espectro de magnitude é exibido em escala logarítmica com centralização de frequência zero via **FFTShift**:
$$\text{Display}(u, v) = \log\left( 1 + |F(u, v)| \right)$$
