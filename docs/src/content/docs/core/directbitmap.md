---
title: DirectBitmap & Buffer Bgra32 (Análise do Código)
description: Uma explicação linha a linha da classe mais importante do projeto, que gerencia memória e paralelismo.
---

A classe [`DirectBitmap.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Core/DirectBitmap.cs) é a base de manipulação de memória de todo o laboratório. Abaixo está a análise detalhada de seu funcionamento.

---

## 1. Estrutura da Classe

```csharp
namespace CGPDI.StudyLab.Core
{
    public sealed unsafe class DirectBitmap : IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public int Stride { get; }
        public WriteableBitmap Bitmap { get; }

        private byte* _backBuffer;
        private bool _isLocked;
        private bool _disposed;
    }
}
```

- **`sealed`:** Impede herança, permitindo otimizações de compilação JIT.
- **`unsafe`:** Permite o uso de ponteiros de memória direta (`byte*`).
- **`IDisposable`:** Assegura a liberação adequada dos recursos nativos.

---

## 2. Métodos de Bloqueio: `Lock()` e `Unlock()`

Antes de alterar os pixels, precisamos fixar o buffer na memória RAM para que o *Garbage Collector* não altere seu endereço durante a execução:

```csharp
public void Lock()
{
    if (!_isLocked)
    {
        Bitmap.Lock();
        _backBuffer = (byte*)Bitmap.BackBuffer.ToPointer();
        _isLocked = true;
    }
}
```

Ao término, o buffer é liberado e o subsistema gráfico é notificado para atualizar a janela:

```csharp
public void Unlock(bool markDirty = true)
{
    if (_isLocked)
    {
        if (markDirty)
        {
            // Notifica o DirectX que a area da imagem foi alterada
            Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
        }
        Bitmap.Unlock();
        _backBuffer = null;
        _isLocked = false;
    }
}
```

---

## 3. Escrita de Pixels Otimizada

```csharp
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetPixel(int x, int y, Color color)
{
    if (x < 0 || x >= Width || y < 0 || y >= Height)
        return;

    byte* pixelPtr = _backBuffer + (y * Stride) + (x * 4);
    pixelPtr[0] = color.B; // Blue (Azul)
    pixelPtr[1] = color.G; // Green (Verde)
    pixelPtr[2] = color.R; // Red (Vermelho)
    pixelPtr[3] = color.A; // Alpha (Opacidade)
}
```

:::tip[O que é `AggressiveInlining`?]
A anotação `[MethodImpl(MethodImplOptions.AggressiveInlining)]` orienta o compilador a substituir a chamada de função pelo próprio corpo do método no local onde é chamado, eliminando o custo de saltos de pilha.
:::

---

## 4. Paralelismo Multinúcleo com `Parallel.For`

### A Analogia da Equipe de Pintores:
Se uma única pessoa pintar uma parede de 512 linhas, ela levará muito tempo. Se contratarmos uma equipe de 8 pintores, cada um pinta uma faixa horizontal da parede ao mesmo tempo:

```csharp
Parallel.For(0, Height, y =>
{
    byte* srcRow = src.BackBuffer + (y * src.Stride);
    byte* dstRow = dst.BackBuffer + (y * dst.Stride);

    for (int x = 0; x < Width; x++)
    {
        int idx = x * 4;
        byte b = srcRow[idx + 0];
        byte g = srcRow[idx + 1];
        byte r = srcRow[idx + 2];

        // Calculo de escala de cinza perceptiva
        byte cinza = (byte)((r * 2126 + g * 7152 + b * 722) / 10000);

        dstRow[idx + 0] = cinza; // B
        dstRow[idx + 1] = cinza; // G
        dstRow[idx + 2] = cinza; // R
        dstRow[idx + 3] = 255;   // A
    }
});
```

---

👉 **Próximo Passo:** Explore os [Modelos de Cores & Percepção Humana](/CGPDI.StudyLab/core/modelos-de-cor/).
