using System;
using System.Collections.Generic;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Catálogo central de códigos C# de referência e formulações matemáticas para todos os algoritmos das abas interativas (PDI, 2D, 3D, Ray Tracing).
    /// </summary>
    public static class AlgorithmCodeSnippets
    {
        public static readonly string GrayscaleCode =
@"// Conversão para Escala de Cinza (Luminância Ponderada NTSC / Rec. 601):
public static unsafe DirectBitmap ConvertToGrayscale(DirectBitmap src)
{
    DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
    src.Lock(); dst.Lock();

    Parallel.For(0, src.Height, y =>
    {
        byte* sRow = src.BackBuffer + (y * src.Stride);
        byte* dRow = dst.BackBuffer + (y * dst.Stride);
        for (int x = 0; x < src.Width; x++)
        {
            byte b = sRow[x * 4 + 0];
            byte g = sRow[x * 4 + 1];
            byte r = sRow[x * 4 + 2];

            // Coeficientes perceptivos do olho humano: 29.9% R, 58.7% G, 11.4% B
            byte lum = (byte)((r * 299 + g * 587 + b * 114) / 1000);

            dRow[x * 4 + 0] = lum; // Blue
            dRow[x * 4 + 1] = lum; // Green
            dRow[x * 4 + 2] = lum; // Red
            dRow[x * 4 + 3] = 255; // Alpha
        }
    });

    src.Unlock(false); dst.Unlock(true);
    return dst;
}";

        public static readonly string InvertCode =
@"// Inversão Negativa de Cores:
public static unsafe DirectBitmap InvertColors(DirectBitmap src)
{
    DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
    src.Lock(); dst.Lock();

    Parallel.For(0, src.Height, y =>
    {
        byte* sRow = src.BackBuffer + (y * src.Stride);
        byte* dRow = dst.BackBuffer + (y * dst.Stride);
        for (int x = 0; x < src.Width; x++)
        {
            dRow[x * 4 + 0] = (byte)(255 - sRow[x * 4 + 0]); // 255 - B
            dRow[x * 4 + 1] = (byte)(255 - sRow[x * 4 + 1]); // 255 - G
            dRow[x * 4 + 2] = (byte)(255 - sRow[x * 4 + 2]); // 255 - R
            dRow[x * 4 + 3] = 255;
        }
    });

    src.Unlock(false); dst.Unlock(true);
    return dst;
}";

        public static readonly string SobelCode =
@"// Detecção de Bordas por Gradiente de Sobel:
public static unsafe DirectBitmap ApplySobel(DirectBitmap src)
{
    int[,] Gx = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
    int[,] Gy = { { -1, -2, -1 }, {  0,  0,  0 }, {  1,  2,  1 } };

    DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
    src.Lock(); dst.Lock();

    Parallel.For(1, src.Height - 1, y =>
    {
        for (int x = 1; x < src.Width - 1; x++)
        {
            double sumX = 0, sumY = 0;
            for (int ky = -1; ky <= 1; ky++)
            {
                byte* row = src.BackBuffer + ((y + ky) * src.Stride);
                for (int kx = -1; kx <= 1; kx++)
                {
                    byte b = row[(x + kx) * 4 + 0];
                    byte g = row[(x + kx) * 4 + 1];
                    byte r = row[(x + kx) * 4 + 2];
                    byte lum = (byte)((r * 299 + g * 587 + b * 114) / 1000);

                    sumX += lum * Gx[ky + 1, kx + 1];
                    sumY += lum * Gy[ky + 1, kx + 1];
                }
            }

            // Magnitude euclidiana do gradiente: G = sqrt(Gx^2 + Gy^2)
            double mag = Math.Sqrt(sumX * sumX + sumY * sumY);
            byte val = (byte)Math.Clamp(mag, 0, 255);

            byte* dstPixel = dst.BackBuffer + (y * dst.Stride) + (x * 4);
            dstPixel[0] = val; dstPixel[1] = val; dstPixel[2] = val; dstPixel[3] = 255;
        }
    });

    src.Unlock(false); dst.Unlock(true);
    return dst;
}";

        public static readonly string GaussianCode =
@"// Filtro Gaussiano 2D com Distribuição Normal:
public static unsafe DirectBitmap GaussianBlur(DirectBitmap src, double sigma = 1.4, int size = 5)
{
    double[,] kernel = new double[size, size];
    int radius = size / 2;
    double sum = 0;

    for (int y = -radius; y <= radius; y++)
    {
        for (int x = -radius; x <= radius; x++)
        {
            double w = Math.Exp(-(x * x + y * y) / (2 * sigma * sigma)) / (2 * Math.PI * sigma * sigma);
            kernel[y + radius, x + radius] = w;
            sum += w;
        }
    }

    return SpatialFilters.Convolve2D(src, kernel, sum, 0.0);
}";

        public static readonly string OtsuCode =
@"// Limiarização Automática de Otsu:
public static DirectBitmap OtsuThreshold(DirectBitmap src, out byte calculatedThreshold)
{
    int[] hist = new int[256];
    // 1. Calcula histograma de luminância
    CalculateHistogram(src, hist);

    double sumTotal = 0;
    int totalPixels = src.Width * src.Height;
    for (int t = 0; t < 256; t++) sumTotal += t * hist[t];

    double sumB = 0, maxVariance = 0;
    int weightB = 0;
    byte bestT = 128;

    for (int t = 0; t < 256; t++)
    {
        weightB += hist[t];
        if (weightB == 0) continue;
        int weightF = totalPixels - weightB;
        if (weightF == 0) break;

        sumB += t * hist[t];
        double meanB = sumB / weightB;
        double meanF = (sumTotal - sumB) / weightF;

        // Variância entre classes: sigma_B^2 = w0 * w1 * (mu0 - mu1)^2
        double varBetween = (double)weightB * weightF * (meanB - meanF) * (meanB - meanF);
        if (varBetween > maxVariance)
        {
            maxVariance = varBetween;
            bestT = (byte)t;
        }
    }

    calculatedThreshold = bestT;
    return ApplyThreshold(src, bestT);
}";

        public static readonly string BresenhamLineCode =
@"// Algoritmo de Rasterização de Reta de Bresenham (Apenas inteiros):
public static void DrawLineBresenham(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color color)
{
    int dx = Math.Abs(x1 - x0);
    int dy = Math.Abs(y1 - y0);
    int sx = x0 < x1 ? 1 : -1;
    int sy = y0 < y1 ? 1 : -1;
    int err = dx - dy;

    while (true)
    {
        bmp.SetPixel(x0, y0, color);
        if (x0 == x1 && y0 == y1) break;

        int e2 = 2 * err;
        if (e2 > -dy)
        {
            err -= dy;
            x0 += sx;
        }
        if (e2 < dx)
        {
            err += dx;
            y0 += sy;
        }
    }
}";

        public static readonly string MidpointCircleCode =
@"// Algoritmo do Ponto Médio para Círculos (Simetria de 8 octantes):
public static void DrawCircleMidpoint(DirectBitmap bmp, int xc, int yc, int r, Color color)
{
    int x = 0;
    int y = r;
    int d = 1 - r; // Parâmetro inicial de decisão

    void Plot8(int px, int py)
    {
        bmp.SetPixel(xc + px, yc + py, color);
        bmp.SetPixel(xc - px, yc + py, color);
        bmp.SetPixel(xc + px, yc - py, color);
        bmp.SetPixel(xc - px, yc - py, color);
        bmp.SetPixel(xc + py, yc + px, color);
        bmp.SetPixel(xc - py, yc + px, color);
        bmp.SetPixel(xc + py, yc - px, color);
        bmp.SetPixel(xc - py, yc - px, color);
    }

    Plot8(x, y);
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
        Plot8(x, y);
    }
}";

        public static readonly string Matrix3x3Code =
@"// Matriz de Transformação Homogênea 2D 3x3:
public struct Matrix3x3
{
    public double M11, M12, M13;
    public double M21, M22, M23;
    public double M31, M32, M33;

    public static Matrix3x3 CreateTranslation(double tx, double ty) =>
        new Matrix3x3(1, 0, tx, 0, 1, ty, 0, 0, 1);

    public static Matrix3x3 CreateRotation(double degrees)
    {
        double rad = degrees * Math.PI / 180.0;
        double cos = Math.Cos(rad), sin = Math.Sin(rad);
        return new Matrix3x3(cos, -sin, 0, sin, cos, 0, 0, 0, 1);
    }

    public static Matrix3x3 CreateScale(double sx, double sy) =>
        new Matrix3x3(sx, 0, 0, 0, sy, 0, 0, 0, 1);

    public Point TransformPoint(Point p)
    {
        double x = M11 * p.X + M12 * p.Y + M13;
        double y = M21 * p.X + M22 * p.Y + M23;
        double w = M31 * p.X + M32 * p.Y + M33;
        return new Point(x / w, y / w);
    }
}";

        public static readonly string Pipeline3DMVPCode =
@"// Pipeline de Transformação 3D MVP e Projeção Perspectiva:
public static Vector4 TransformVertex(Vector3 vLocal, Matrix4x4 model, Matrix4x4 view, Matrix4x4 proj)
{
    // 1. Espaço de Objeto -> Espaço de Mundo
    Vector4 vWorld = Vector4.Transform(new Vector4(vLocal, 1.0f), model);

    // 2. Espaço de Mundo -> Espaço de Câmera (View)
    Vector4 vView = Vector4.Transform(vWorld, view);

    // 3. Espaço de Câmera -> Espaço de Projeção (Clip Space)
    Vector4 vClip = Vector4.Transform(vView, proj);

    // 4. Divisão Perspectiva (NDC - Normalized Device Coordinates: [-1, 1])
    Vector3 vNDC = new Vector3(vClip.X / vClip.W, vClip.Y / vClip.W, vClip.Z / vClip.W);

    return vClip;
}";

        public static readonly string BlinnPhongCode =
@"// Modelo de Iluminação Reflexiva de Blinn-Phong:
public static Vec3 CalculateBlinnPhong(Vec3 point, Vec3 normal, Vec3 viewDir, Vec3 lightDir, MaterialRay mat)
{
    // 1. Componente Ambiente
    Vec3 ambient = mat.Color * mat.Ambient;

    // 2. Componente Difusa (Lambertiana): max(0, N · L)
    double nDotL = Math.Max(0.0, Vec3.Dot(normal, lightDir));
    Vec3 diffuse = mat.Color * (mat.Diffuse * nDotL);

    // 3. Componente Especular de Blinn-Phong: H = normalize(L + V), (N · H)^alpha
    Vec3 halfway = Vec3.Normalize(lightDir + viewDir);
    double nDotH = Math.Max(0.0, Vec3.Dot(normal, halfway));
    double specFactor = Math.Pow(nDotH, mat.Shininess);
    Vec3 specular = new Vec3(1, 1, 1) * (mat.Specular * specFactor);

    return ambient + diffuse + specular;
}";

        public static readonly string RayTracingSphereCode =
@"// Interseção Raio-Esfera Analítica no Ray Tracer:
public override bool Intersect(Ray3D ray, out double t, out Vec3 normal)
{
    t = 0;
    normal = Vec3.Zero;

    Vec3 oc = ray.Origin - Center;
    double b = Vec3.Dot(oc, ray.Direction);
    double c = Vec3.Dot(oc, oc) - (Radius * Radius);
    double discriminant = b * b - c;

    if (discriminant < 0) return false; // Raio não atinge a esfera

    double sqrtD = Math.Sqrt(discriminant);
    double t0 = -b - sqrtD;
    double t1 = -b + sqrtD;

    if (t0 > 0.001) t = t0;
    else if (t1 > 0.001) t = t1;
    else return false;

    Vec3 hitPoint = ray.Origin + ray.Direction * t;
    normal = Vec3.Normalize(hitPoint - Center);
    return true;
}";
    }
}
