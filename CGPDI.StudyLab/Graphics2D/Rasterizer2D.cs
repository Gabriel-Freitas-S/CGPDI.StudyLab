using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.Graphics2D
{
    /// <summary>
    /// Algoritmos Clássicos de Rasterização 2D construídos a partir dos primeiros princípios da Computação Gráfica.
    /// Contém implementação completa de:
    /// - Algoritmo DDA vs Bresenham para Traçado de Retas
    /// - Algoritmo de Linhas Suavizadas de Xiaolin Wu (Anti-Aliasing)
    /// - Algoritmo do Ponto Médio para Círculos (Bresenham Circle) com Simetria em 8 Octantes
    /// - Algoritmo do Ponto Médio para Elipses
    /// - Curvas Paramétricas de Bézier Quadráticas e Cúbicas (De Casteljau)
    /// - Preenchimento de Polígonos por Varredura (Scanline Polygon Fill)
    /// - Algoritmo de Recorte de Linhas Cohen-Sutherland (Outcodes)
    /// - Flood Fill (Preenchimento por Inundação baseado em Fila)
    /// </summary>
    public static class Rasterizer2D
    {
        #region Traçado de Retas (DDA, Bresenham & Wu)

        /// <summary>
        /// Algoritmo DDA (Digital Differential Analyzer):
        /// Calcula os passos incrementais ao longo do eixo de maior variação usando ponto flutuante:
        /// dx = (x1 - x0) / steps, dy = (y1 - y0) / steps
        /// </summary>
        public static void DrawLineDDA(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color color)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            if (steps == 0)
            {
                bmp.SetPixel(x0, y0, color);
                return;
            }

            double xInc = (double)dx / steps;
            double yInc = (double)dy / steps;

            double x = x0;
            double y = y0;

            for (int i = 0; i <= steps; i++)
            {
                bmp.SetPixel((int)Math.Round(x), (int)Math.Round(y), color);
                x += xInc;
                y += yInc;
            }
        }

        /// <summary>
        /// Algoritmo de Reta de Bresenham (Jack Bresenham, 1965):
        /// Considerado um dos marcos fundamentais da Computação Gráfica.
        /// Utiliza EXCLUSIVAMENTE aritmética inteira (soma, subtração e multiplicação por 2 via bitshift),
        /// eliminando operações caras de divisão ou ponto flutuante.
        /// 
        /// FÓRMULA DO ERRO:
        /// e = 2 \Delta y - \Delta x
        /// Se e >= 0, incrementa y e atualiza e = e + 2(\Delta y - \Delta x);
        /// Caso contrário, apenas incrementa x e atualiza e = e + 2\Delta y.
        /// </summary>
        public static void DrawLineBresenham(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;

            while (true)
            {
                bmp.SetPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;

                int e2 = err;
                if (e2 > -dx)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dy)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        /// <summary>
        /// Algoritmo de Xiaolin Wu (Anti-Aliased Line):
        /// Desenha linhas com suavização de serrilhado em tempo real calculando a intensidade proporcional
        /// dos dois pixels mais próximos da linha em cada etapa através de interpolação linear.
        /// </summary>
        public static void DrawLineWu(DirectBitmap bmp, double x0, double y0, double x1, double y1, Color color)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            double dx = x1 - x0;
            double dy = y1 - y0;
            double gradient = dx == 0 ? 1.0 : dy / dx;

            // Primeiro endpoint
            double xEnd = Math.Round(x0);
            double yEnd = y0 + gradient * (xEnd - x0);
            double xGap = 1.0 - (x0 + 0.5 - Math.Floor(x0 + 0.5));
            double xpxl1 = xEnd;
            double ypxl1 = Math.Floor(yEnd);

            void Plot(double x, double y, double c)
            {
                byte a = (byte)(color.A * Math.Clamp(c, 0, 1));
                Color blended = Color.FromArgb(a, color.R, color.G, color.B);
                if (steep)
                    bmp.SetPixel((int)y, (int)x, blended);
                else
                    bmp.SetPixel((int)x, (int)y, blended);
            }

            Plot(xpxl1, ypxl1, (1.0 - (yEnd - Math.Floor(yEnd))) * xGap);
            Plot(xpxl1, ypxl1 + 1, (yEnd - Math.Floor(yEnd)) * xGap);
            double intery = yEnd + gradient;

            // Segundo endpoint
            xEnd = Math.Round(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = x1 + 0.5 - Math.Floor(x1 + 0.5);
            double xpxl2 = xEnd;
            double ypxl2 = Math.Floor(yEnd);
            Plot(xpxl2, ypxl2, (1.0 - (yEnd - Math.Floor(yEnd))) * xGap);
            Plot(xpxl2, ypxl2 + 1, (yEnd - Math.Floor(yEnd)) * xGap);

            // Loop principal
            for (double x = xpxl1 + 1; x <= xpxl2 - 1; x++)
            {
                Plot(x, Math.Floor(intery), 1.0 - (intery - Math.Floor(intery)));
                Plot(x, Math.Floor(intery) + 1, intery - Math.Floor(intery));
                intery += gradient;
            }
        }

        #endregion

        #region Traçado de Círculos e Elipses (Ponto Médio)

        /// <summary>
        /// Algoritmo do Ponto Médio para Círculos (Bresenham Circle):
        /// Explora a SIMETRIA EM 8 OCTANTES do círculo: calculando apenas 1/8 do perímetro (45 graus),
        /// obtém-se os outros 7 octantes por simples espelhamento de coordenadas (+-x, +-y).
        /// 
        /// VARIÁVEL DE DECISÃO INICIAL:
        /// d = 1 - r
        /// Se d &lt; 0: d = d + 2x + 3
        /// Se d &gt;= 0: d = d + 2(x - y) + 5; decrementa y.
        /// </summary>
        public static void DrawCircleMidpoint(DirectBitmap bmp, int xc, int yc, int radius, Color color, bool fill = false)
        {
            int x = 0;
            int y = radius;
            int d = 1 - radius;

            void PlotCirclePoints(int cx, int cy, int px, int py)
            {
                if (fill)
                {
                    // Preenchimento de linhas horizontais entre os octantes simétricos
                    DrawHorizontalLine(bmp, cx - px, cx + px, cy + py, color);
                    DrawHorizontalLine(bmp, cx - px, cx + px, cy - py, color);
                    DrawHorizontalLine(bmp, cx - py, cx + py, cy + px, color);
                    DrawHorizontalLine(bmp, cx - py, cx + py, cy - px, color);
                }
                else
                {
                    bmp.SetPixel(cx + px, cy + py, color);
                    bmp.SetPixel(cx - px, cy + py, color);
                    bmp.SetPixel(cx + px, cy - py, color);
                    bmp.SetPixel(cx - px, cy - py, color);
                    bmp.SetPixel(cx + py, cy + px, color);
                    bmp.SetPixel(cx - py, cy + px, color);
                    bmp.SetPixel(cx + py, cy - px, color);
                    bmp.SetPixel(cx - py, cy - px, color);
                }
            }

            PlotCirclePoints(xc, yc, x, y);

            while (x < y)
            {
                x++;
                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    y--;
                    d += 2 * (x - y) + 1;
                }
                PlotCirclePoints(xc, yc, x, y);
            }
        }

        private static void DrawHorizontalLine(DirectBitmap bmp, int x0, int x1, int y, Color color)
        {
            if (y < 0 || y >= bmp.Height) return;
            int startX = Math.Clamp(Math.Min(x0, x1), 0, bmp.Width - 1);
            int endX = Math.Clamp(Math.Max(x0, x1), 0, bmp.Width - 1);

            for (int x = startX; x <= endX; x++)
            {
                bmp.SetPixel(x, y, color);
            }
        }

        /// <summary>
        /// Algoritmo do Ponto Médio para Elipses:
        /// Divide o quadrante em duas regiões baseadas na inclinação da reta tangente (|dy/dx| &lt; 1 na Região 1 e |dy/dx| &gt; 1 na Região 2).
        /// Explora a simetria em 4 quadrantes (+-x, +-y).
        /// </summary>
        public static void DrawEllipseMidpoint(DirectBitmap bmp, int xc, int yc, int rx, int ry, Color color, bool fill = false)
        {
            double rxSq = rx * rx;
            double rySq = ry * ry;
            double x = 0;
            double y = ry;

            void Plot4(int cx, int cy, int px, int py)
            {
                if (fill)
                {
                    DrawHorizontalLine(bmp, cx - px, cx + px, cy + py, color);
                    DrawHorizontalLine(bmp, cx - px, cx + px, cy - py, color);
                }
                else
                {
                    bmp.SetPixel(cx + px, cy + py, color);
                    bmp.SetPixel(cx - px, cy + py, color);
                    bmp.SetPixel(cx + px, cy - py, color);
                    bmp.SetPixel(cx - px, cy - py, color);
                }
            }

            // Região 1 (dx > dy)
            double p1 = rySq - (rxSq * ry) + (0.25 * rxSq);
            double dx = 2 * rySq * x;
            double dy = 2 * rxSq * y;

            while (dx < dy)
            {
                Plot4(xc, yc, (int)x, (int)y);
                x++;
                if (p1 < 0)
                {
                    dx += 2 * rySq;
                    p1 += dx + rySq;
                }
                else
                {
                    y--;
                    dx += 2 * rySq;
                    dy -= 2 * rxSq;
                    p1 += dx - dy + rySq;
                }
            }

            // Região 2 (dx >= dy)
            double p2 = (rySq * (x + 0.5) * (x + 0.5)) + (rxSq * (y - 1) * (y - 1)) - (rxSq * rySq);
            while (y >= 0)
            {
                Plot4(xc, yc, (int)x, (int)y);
                y--;
                if (p2 > 0)
                {
                    dy -= 2 * rxSq;
                    p2 += rxSq - dy;
                }
                else
                {
                    x++;
                    dx += 2 * rySq;
                    dy -= 2 * rxSq;
                    p2 += dx - dy + rxSq;
                }
            }
        }

        #endregion

        #region Curvas Paramétricas de Bézier (Pierre Bézier / Paul de Casteljau)

        /// <summary>
        /// Curva de Bézier Quadrática (3 Pontos de Controle P0, P1, P2):
        /// B(t) = (1-t)^2 P0 + 2(1-t)t P1 + t^2 P2  com t \in [0, 1]
        /// </summary>
        public static void DrawBezierQuadratic(DirectBitmap bmp, Point p0, Point p1, Point p2, Color color, int segments = 60)
        {
            Point prev = p0;
            for (int i = 1; i <= segments; i++)
            {
                double t = (double)i / segments;
                double u = 1.0 - t;

                double x = u * u * p0.X + 2.0 * u * t * p1.X + t * t * p2.X;
                double y = u * u * p0.Y + 2.0 * u * t * p1.Y + t * t * p2.Y;
                Point curr = new Point(x, y);

                DrawLineBresenham(bmp, (int)Math.Round(prev.X), (int)Math.Round(prev.Y), (int)Math.Round(curr.X), (int)Math.Round(curr.Y), color);
                prev = curr;
            }
        }

        /// <summary>
        /// Curva de Bézier Cúbica (4 Pontos de Controle P0, P1, P2, P3):
        /// B(t) = (1-t)^3 P0 + 3(1-t)^2 t P1 + 3(1-t)t^2 P2 + t^3 P3
        /// Padrão da indústria em fontes tipográficas TrueType/PostScript, SVG e gráficos vetoriais.
        /// </summary>
        public static void DrawBezierCubic(DirectBitmap bmp, Point p0, Point p1, Point p2, Point p3, Color color, int segments = 100)
        {
            Point prev = p0;
            for (int i = 1; i <= segments; i++)
            {
                double t = (double)i / segments;
                double u = 1.0 - t;

                double x = u * u * u * p0.X + 3.0 * u * u * t * p1.X + 3.0 * u * t * t * p2.X + t * t * t * p3.X;
                double y = u * u * u * p0.Y + 3.0 * u * u * t * p1.Y + 3.0 * u * t * t * p2.Y + t * t * t * p3.Y;
                Point curr = new Point(x, y);

                DrawLineBresenham(bmp, (int)Math.Round(prev.X), (int)Math.Round(prev.Y), (int)Math.Round(curr.X), (int)Math.Round(curr.Y), color);
                prev = curr;
            }
        }

        #endregion

        #region Preenchimento de Polígonos por Varredura (Scanline Fill)

        private class Edge
        {
            public int YMax;
            public double XCurrent;
            public double InvSlope; // 1 / m = dx / dy
        }

        /// <summary>
        /// Preenchimento de Polígono por Linha de Varredura (Scanline Polygon Fill Algorithm):
        /// Constrói a Tabela de Arestas (Edge Table - ET) e gerencia a Tabela de Arestas Ativas (Active Edge Table - AET).
        /// Em cada linha de varredura y:
        /// 1. Insere novas arestas com YMin = y na AET.
        /// 2. Remove arestas cujo YMax = y da AET.
        /// 3. Ordena os pontos de interseção x da esquerda para a direita.
        /// 4. Preenche os pares de pixels entre as interseções (Paridade / Regra Par-Ímpar).
        /// 5. Atualiza x = x + (1/m) para o próximo scanline.
        /// </summary>
        public static void DrawPolygonScanline(DirectBitmap bmp, Point[] vertices, Color color)
        {
            if (vertices.Length < 3) return;

            int minY = int.MaxValue;
            int maxY = int.MinValue;

            // Encontra limites verticais
            foreach (Point v in vertices)
            {
                int y = (int)Math.Round(v.Y);
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            minY = Math.Clamp(minY, 0, bmp.Height - 1);
            maxY = Math.Clamp(maxY, 0, bmp.Height - 1);

            // Constrói a Edge Table (ET) indexada por YMin
            Dictionary<int, List<Edge>> edgeTable = new Dictionary<int, List<Edge>>();

            for (int i = 0; i < vertices.Length; i++)
            {
                Point p1 = vertices[i];
                Point p2 = vertices[(i + 1) % vertices.Length];

                if ((int)Math.Round(p1.Y) == (int)Math.Round(p2.Y))
                    continue; // Descarta arestas horizontais

                Point lower = p1.Y < p2.Y ? p1 : p2;
                Point upper = p1.Y < p2.Y ? p2 : p1;

                int yMin = (int)Math.Round(lower.Y);
                int yMax = (int)Math.Round(upper.Y);
                double invSlope = (upper.X - lower.X) / (upper.Y - lower.Y);

                Edge edge = new Edge
                {
                    YMax = yMax,
                    XCurrent = lower.X,
                    InvSlope = invSlope
                };

                if (!edgeTable.ContainsKey(yMin))
                    edgeTable[yMin] = new List<Edge>();

                edgeTable[yMin].Add(edge);
            }

            List<Edge> activeEdgeTable = new List<Edge>();

            for (int y = minY; y <= maxY; y++)
            {
                // 1. Adiciona arestas da ET para a AET
                if (edgeTable.TryGetValue(y, out List<Edge>? newEdges))
                {
                    activeEdgeTable.AddRange(newEdges);
                }

                // 2. Remove arestas cujo YMax <= y
                activeEdgeTable.RemoveAll(e => e.YMax <= y);

                // 3. Ordena arestas por XCurrent
                activeEdgeTable.Sort((a, b) => a.XCurrent.CompareTo(b.XCurrent));

                // 4. Desenha spans de pixels aos pares (Parity rule)
                for (int i = 0; i < activeEdgeTable.Count - 1; i += 2)
                {
                    int xStart = (int)Math.Ceiling(activeEdgeTable[i].XCurrent);
                    int xEnd = (int)Math.Floor(activeEdgeTable[i + 1].XCurrent);

                    xStart = Math.Clamp(xStart, 0, bmp.Width - 1);
                    xEnd = Math.Clamp(xEnd, 0, bmp.Width - 1);

                    for (int x = xStart; x <= xEnd; x++)
                    {
                        bmp.SetPixel(x, y, color);
                    }
                }

                // 5. Atualiza XCurrent para a próxima linha
                foreach (Edge edge in activeEdgeTable)
                {
                    edge.XCurrent += edge.InvSlope;
                }
            }
        }

        #endregion

        #region Recorte de Linhas Cohen-Sutherland (Line Clipping)

        [Flags]
        public enum OutCode
        {
            Inside = 0,
            Left = 1,
            Right = 2,
            Bottom = 4,
            Top = 8
        }

        private static OutCode ComputeOutCode(double x, double y, Rect clip)
        {
            OutCode code = OutCode.Inside;
            if (x < clip.Left) code |= OutCode.Left;
            else if (x > clip.Right) code |= OutCode.Right;
            if (y < clip.Top) code |= OutCode.Top;
            else if (y > clip.Bottom) code |= OutCode.Bottom;
            return code;
        }

        /// <summary>
        /// Algoritmo de Recorte de Linhas de Cohen-Sutherland:
        /// Divide o plano 2D em 9 regiões através de códigos de 4 bits (Outcodes).
        /// - Trivial Accept: (code0 | code1) == 0 (ambos pontos dentro da janela).
        /// - Trivial Reject: (code0 & code1) != 0 (ambos pontos compartilham o mesmo lado externo).
        /// - Caso contrário: Divide a reta no ponto de interseção com a borda e repete o teste.
        /// </summary>
        public static bool ClipLineCohenSutherland(Rect clip, ref Point p0, ref Point p1)
        {
            double x0 = p0.X, y0 = p0.Y;
            double x1 = p1.X, y1 = p1.Y;

            OutCode code0 = ComputeOutCode(x0, y0, clip);
            OutCode code1 = ComputeOutCode(x1, y1, clip);

            bool accept = false;

            while (true)
            {
                if ((code0 | code1) == OutCode.Inside)
                {
                    accept = true;
                    break;
                }
                else if ((code0 & code1) != 0)
                {
                    // Trivial Reject
                    break;
                }
                else
                {
                    // Pelo menos um ponto está fora: calcula interseção
                    OutCode outcodeOut = code0 != OutCode.Inside ? code0 : code1;
                    double x = 0, y = 0;

                    if (outcodeOut.HasFlag(OutCode.Top))
                    {
                        x = x0 + (x1 - x0) * (clip.Top - y0) / (y1 - y0);
                        y = clip.Top;
                    }
                    else if (outcodeOut.HasFlag(OutCode.Bottom))
                    {
                        x = x0 + (x1 - x0) * (clip.Bottom - y0) / (y1 - y0);
                        y = clip.Bottom;
                    }
                    else if (outcodeOut.HasFlag(OutCode.Right))
                    {
                        y = y0 + (y1 - y0) * (clip.Right - x0) / (x1 - x0);
                        x = clip.Right;
                    }
                    else if (outcodeOut.HasFlag(OutCode.Left))
                    {
                        y = y0 + (y1 - y0) * (clip.Left - x0) / (x1 - x0);
                        x = clip.Left;
                    }

                    if (outcodeOut == code0)
                    {
                        x0 = x;
                        y0 = y;
                        code0 = ComputeOutCode(x0, y0, clip);
                    }
                    else
                    {
                        x1 = x;
                        y1 = y;
                        code1 = ComputeOutCode(x1, y1, clip);
                    }
                }
            }

            if (accept)
            {
                p0 = new Point(x0, y0);
                p1 = new Point(x1, y1);
            }

            return accept;
        }

        #endregion

        #region Flood Fill (Preenchimento por Inundação)

        /// <summary>
        /// Flood Fill iterativo baseado em Fila (Queue-based 4-connected Flood Fill):
        /// Substitui uma região contígua com a cor do alvo pela cor de substituição.
        /// Evita estouro de pilha (StackOverflowException) de implementações recursivas simples.
        /// </summary>
        public static void FloodFill(DirectBitmap bmp, int startX, int startY, Color fillColor)
        {
            if (startX < 0 || startX >= bmp.Width || startY < 0 || startY >= bmp.Height)
                return;

            Color targetColor = bmp.GetPixel(startX, startY);
            if (targetColor == fillColor) return;

            uint targetBgra = (uint)((targetColor.A << 24) | (targetColor.R << 16) | (targetColor.G << 8) | targetColor.B);
            uint fillBgra = (uint)((fillColor.A << 24) | (fillColor.R << 16) | (fillColor.G << 8) | fillColor.B);

            int width = bmp.Width;
            int height = bmp.Height;

            Queue<(int x, int y)> queue = new Queue<(int, int)>();
            queue.Enqueue((startX, startY));

            unsafe
            {
                byte* buf = bmp.BackBuffer;
                int stride = bmp.Stride;

                while (queue.Count > 0)
                {
                    var (x, y) = queue.Dequeue();
                    if (x < 0 || x >= width || y < 0 || y >= height) continue;

                    uint* p = (uint*)(buf + (y * stride) + (x * 4));
                    if (*p == targetBgra)
                    {
                        *p = fillBgra;
                        queue.Enqueue((x + 1, y));
                        queue.Enqueue((x - 1, y));
                        queue.Enqueue((x, y + 1));
                        queue.Enqueue((x, y - 1));
                    }
                }
            }
        }

        #endregion
    }
}
