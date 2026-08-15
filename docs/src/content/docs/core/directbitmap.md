---
title: DirectBitmap & Buffer Bgra32 (Análise do Código)
description: Uma explicação linha a linha da classe mais importante do projeto, que gerencia memória e paralelismo.
---

A classe [`DirectBitmap.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Core/DirectBitmap.cs) é a fundação sobre a qual todo o laboratório foi construído. Abaixo você confere sua estrutura detalhada.

---

## 💻 1. Estrutura da Classe

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
        
        // ...
    }
}
```

### O que significa cada elemento?
- **`sealed`:** Impede que a classe seja herdada, permitindo que o compilador faça otimizações de *devirtualização* de chamadas.
- **`unsafe`:** Indica ao compilador que a classe manipula ponteiros de memória direta (`byte*`).
- **`IDisposable`:** Garante a liberação correta de recursos quando a imagem não for mais necessária.

---

## 🔒 2. Os Métodos `Lock()` e `Unlock()`

Antes de alterar pixels na memória, precisamos "trancar" o buffer para que o *Garbage Collector* do .NET não mova o array de lugar durante uma compactação de memória:

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

Quando o processamento termina, liberamos o buffer e notificamos o DirectX do WPF que a tela inteira precisa ser redesenhada:

```csharp
public void Unlock(bool markDirty = true)
{
    if (_isLocked)
    {
        if (markDirty)
        {
            // Notifica o subsistema DirectX que toda a área da imagem foi alterada
            Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
        }
        Bitmap.Unlock();
        _backBuffer = null;
        _isLocked = false;
    }
}
```

---

## ⚡ 3. Leitura e Escrita Otimizada de Pixels

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
A anotação `[MethodImpl(MethodImplOptions.AggressiveInlining)]` diz ao compilador JIT: *"Não crie uma chamada de função tradicional para este método. Em vez disso, copie e cole o código das 4 linhas de bytes diretamente no local onde ele for chamado."* Isso elimina o custo de salto de pilha da CPU!
:::

---

## 🚀 4. Paralelismo Multinúcleo com `Parallel.For`

Para aplicar um algoritmo de imagem em todos os núcleos da CPU simultaneamente:

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

        // Processa o pixel
        byte cinza = (byte)((r * 2126 + g * 7152 + b * 722) / 10000);

        dstRow[idx + 0] = cinza; // B
        dstRow[idx + 1] = cinza; // G
        dstRow[idx + 2] = cinza; // R
        dstRow[idx + 3] = 255;   // A
    }
});
```

Se o seu computador tem um processador com 8 núcleos (16 threads), o `Parallel.For` divide a altura da imagem em 16 pedaços horizontais e os processa todos ao mesmo tempo!

---

👉 **Próximo Passo:** Explore os [Modelos de Cores & Percepção Humana](/CGPDI.StudyLab/core/modelos-de-cor/).
