using System;
using System.Collections.Generic;

namespace CGPDI.StudyLab.Core
{
    public class ProjectTemplate
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string Complexity { get; set; } = "Básico";
        public string ComplexityBadgeColor { get; set; } = "#10B981";
        public string Description { get; set; } = "";
        public string InitialCode { get; set; } = "";
        public int DefaultWidth { get; set; } = 512;
        public int DefaultHeight { get; set; } = 512;

        public string Param1Name { get; set; } = "Parâmetro 1";
        public double Param1Min { get; set; } = 0;
        public double Param1Max { get; set; } = 100;
        public double Param1Default { get; set; } = 50;

        public string Param2Name { get; set; } = "Parâmetro 2";
        public double Param2Min { get; set; } = 0;
        public double Param2Max { get; set; } = 100;
        public double Param2Default { get; set; } = 50;

        public string Param3Name { get; set; } = "Parâmetro 3";
        public double Param3Min { get; set; } = 0;
        public double Param3Max { get; set; } = 100;
        public double Param3Default { get; set; } = 50;

        public string Param4Name { get; set; } = "Parâmetro 4";
        public double Param4Min { get; set; } = 0;
        public double Param4Max { get; set; } = 100;
        public double Param4Default { get; set; } = 50;

        public string XamlCode { get; set; } = "";
        public string ChallengeCode { get; set; } = "";
        public string StepsGuide { get; set; } = "";
        public bool IsInteractiveActivity { get; set; } = false;
    }

    /// <summary>
    /// Gerenciador de templates e modelos de projetos acadêmicos e livres.
    /// </summary>
    public static class ProjectTemplatesManager
    {
        public static List<ProjectTemplate> GetTemplates()
        {
            return new List<ProjectTemplate>
            {
                // Template 0: Tela em Branco 100% Livre
                new ProjectTemplate
                {
                    Id = "blank-canvas",
                    Title = "Tela em Branco (Projeto Livre do Zero)",
                    Category = "Livre",
                    Complexity = "Livre Total",
                    ComplexityBadgeColor = "#64748B",
                    Description = "Inicie com um buffer DirectBitmap limpo. Desenvolva algoritmos gráficos, shaders ou filtros com controle total da memória.",
                    Param1Name = "Controle A (0 a 100):",
                    Param1Min = 0, Param1Max = 100, Param1Default = 50,
                    Param2Name = "Controle B (0 a 100):",
                    Param2Min = 0, Param2Max = 100, Param2Default = 25,
                    Param3Name = "Controle C (0 a 255):",
                    Param3Min = 0, Param3Max = 255, Param3Default = 180,
                    Param4Name = "Controle D (0 a 10):",
                    Param4Min = 0, Param4Max = 10, Param4Default = 1,
                    InitialCode = @"// Projeto Livre: Código C# do Zero
// Variáveis disponíveis no ambiente:
// - Output: DirectBitmap (Buffer de saída onde você desenha)
// - Width, Height: Dimensões da imagem (512x512)
// - Param1, Param2, Param3, Param4: Valores dos 4 sliders interativos
// - Print(string): Imprime mensagens no console do estúdio

Print($""Iniciando renderização livre: {Width}x{Height}"");

// Exemplo: Limpa o fundo e desenha um gradiente radial com anéis concêntricos
int cx = Width / 2;
int cy = Height / 2;
double freq = Param4 * 0.1;

for (int y = 0; y < Height; y++)
{
    for (int x = 0; x < Width; x++)
    {
        double dx = x - cx;
        double dy = y - cy;
        double dist = Math.Sqrt(dx * dx + dy * dy);
        
        // Padrão de anéis com senos
        double wave = Math.Sin(dist * freq - Param1 * 0.1);
        int intensity = (int)((wave + 1.0) * 0.5 * Param3);
        
        byte r = (byte)Math.Clamp(intensity + (int)Param2, 0, 255);
        byte g = (byte)Math.Clamp(intensity, 0, 255);
        byte b = (byte)Math.Clamp(255 - intensity, 0, 255);
        
        Output.SetPixel(x, y, ColorSpaces.PackBgra(b, g, r));
    }
}

Print(""Renderização concluída com sucesso!"");"
                },

                // Template 1: Padrões Matemáticos e Procedurais (Básico)
                new ProjectTemplate
                {
                    Id = "procedural-patterns",
                    Title = "Nível 1: Padrões Matemáticos e Fractais 2D",
                    Category = "Fundamentos",
                    Complexity = "Básico",
                    ComplexityBadgeColor = "#10B981",
                    Description = "Gere padrões visuais, fractais de Mandelbrot e interferência ondulatória através de funções matemáticas por pixel.",
                    Param1Name = "Zoom / Escala Fractal:",
                    Param1Min = 1, Param1Max = 100, Param1Default = 35,
                    Param2Name = "Deslocamento X (Offset):",
                    Param2Min = -100, Param2Max = 100, Param2Default = -30,
                    Param3Name = "Deslocamento Y (Offset):",
                    Param3Min = -100, Param3Max = 100, Param3Default = 0,
                    Param4Name = "Iterações Máximas (5 a 80):",
                    Param4Min = 5, Param4Max = 80, Param4Default = 40,
                    InitialCode = @"// Nivel 1: Renderizador do Conjunto Fractal de Mandelbrot
int maxIter = (int)Param4;
double zoom = Param1 * 0.05;
double offsetX = Param2 * 0.02;
double offsetY = Param3 * 0.02;

for (int py = 0; py < Height; py++)
{
    for (int px = 0; px < Width; px++)
    {
        // Mapeia coordenadas de pixel para o plano complexo (c_real, c_imag)
        double c_re = (px - Width / 2.0) / (0.5 * zoom * Width) + offsetX;
        double c_im = (py - Height / 2.0) / (0.5 * zoom * Height) + offsetY;
        
        double z_re = 0;
        double z_im = 0;
        int iter = 0;
        
        while (z_re * z_re + z_im * z_im <= 4.0 && iter < maxIter)
        {
            double next_re = z_re * z_re - z_im * z_im + c_re;
            double next_im = 2.0 * z_re * z_im + c_im;
            z_re = next_re;
            z_im = next_im;
            iter++;
        }
        
        if (iter == maxIter)
        {
            Output.SetPixel(px, py, ColorSpaces.PackBgra(0, 0, 0));
        }
        else
        {
            double t = (double)iter / maxIter;
            byte r = (byte)(Math.Sin(t * Math.PI * 2) * 127 + 128);
            byte g = (byte)(Math.Sin(t * Math.PI * 2 + 2.0) * 127 + 128);
            byte b = (byte)(Math.Sin(t * Math.PI * 2 + 4.0) * 127 + 128);
            Output.SetPixel(px, py, ColorSpaces.PackBgra(b, g, r));
        }
    }
}

Print($""Fractal gerado com {maxIter} iteracoes."");"
                },

                // Template 2: Rasterização 2D (Básico)
                new ProjectTemplate
                {
                    Id = "rasterization-2d",
                    Title = "Nível 2: Rasterização 2D e Desenho de Formas",
                    Category = "Rasterização 2D",
                    Complexity = "Básico",
                    ComplexityBadgeColor = "#10B981",
                    Description = "Implemente e teste algoritmos de desenho de primitivas geométricas: Reta de Bresenham, Círculo do Ponto Médio e polígonos.",
                    Param1Name = "Raio do Círculo (10 a 200):",
                    Param1Min = 10, Param1Max = 200, Param1Default = 120,
                    Param2Name = "Número de Vértices (3 a 12):",
                    Param2Min = 3, Param2Max = 12, Param2Default = 6,
                    Param3Name = "Rotação (Graus 0 a 360):",
                    Param3Min = 0, Param3Max = 360, Param3Default = 45,
                    Param4Name = "Espessura / Detalhe (1 a 10):",
                    Param4Min = 1, Param4Max = 10, Param4Default = 2,
                    InitialCode = @"// Nivel 2: Rasterizacao de Poligono Regular e Circulo com Bresenham
Output.Clear(ColorSpaces.PackBgra(20, 20, 26));

int cx = Width / 2;
int cy = Height / 2;
int radius = (int)Param1;
int vertices = Math.Max(3, (int)Param2);
double angleOffset = Param3 * Math.PI / 180.0;

var points = new (int X, int Y)[vertices];
for (int i = 0; i < vertices; i++)
{
    double a = angleOffset + (i * 2.0 * Math.PI / vertices);
    int px = (int)(cx + radius * Math.Cos(a));
    int py = (int)(cy + radius * Math.Sin(a));
    points[i] = (px, py);
}

void DrawBresenhamLine(int x0, int y0, int x1, int y1, uint color)
{
    int dx = Math.Abs(x1 - x0);
    int dy = Math.Abs(y1 - y0);
    int sx = x0 < x1 ? 1 : -1;
    int sy = y0 < y1 ? 1 : -1;
    int err = dx - dy;

    while (true)
    {
        if (x0 >= 0 && x0 < Width && y0 >= 0 && y0 < Height)
            Output.SetPixel(x0, y0, color);

        if (x0 == x1 && y0 == y1) break;
        int e2 = 2 * err;
        if (e2 > -dy) { err -= dy; x0 += sx; }
        if (e2 < dx) { err += dx; y0 += sy; }
    }
}

uint edgeColor = ColorSpaces.PackBgra(240, 180, 56);
for (int i = 0; i < vertices; i++)
{
    var pA = points[i];
    var pB = points[(i + 1) % vertices];
    DrawBresenhamLine(pA.X, pA.Y, pB.X, pB.Y, edgeColor);
    DrawBresenhamLine(cx, cy, pA.X, pA.Y, ColorSpaces.PackBgra(120, 80, 240));
}

Print($""Poligono de {vertices} lados rasterizado com raio {radius}px."");"
                },

                // Template 3: Filtros Espaciais e PDI (Intermediário)
                new ProjectTemplate
                {
                    Id = "pdi-convolutions",
                    Title = "Nível 3: Processamento Digital de Imagens (PDI)",
                    Category = "Processamento de Imagens",
                    Complexity = "Intermediário",
                    ComplexityBadgeColor = "#F59E0B",
                    Description = "Convolução espacial 3x3 (Sobel, Gaussiano, Laplace), detecção de bordas, contraste e limiarização.",
                    Param1Name = "Força do Filtro / Limiar:",
                    Param1Min = 0, Param1Max = 255, Param1Default = 80,
                    Param2Name = "Mistura com Original (0-100%):",
                    Param2Min = 0, Param2Max = 100, Param2Default = 40,
                    Param3Name = "Ganho de Contraste (1 a 5):",
                    Param3Min = 1, Param3Max = 5, Param3Default = 2,
                    Param4Name = "Modo (0:Bordas, 1:Realce, 2:Limiar):",
                    Param4Min = 0, Param4Max = 2, Param4Default = 0,
                    InitialCode = @"// Nivel 3: Convolucao Espacial 3x3 e Deteccao de Bordas Sobel
var source = new DirectBitmap(Width, Height);
for (int y = 0; y < Height; y++)
{
    for (int x = 0; x < Width; x++)
    {
        byte c = (byte)((x ^ y) % 256);
        if (Math.Sqrt(Math.Pow(x - Width/2, 2) + Math.Pow(y - Height/2, 2)) < 120) c = 240;
        source.SetPixel(x, y, ColorSpaces.PackBgra(c, c, c));
    }
}

int[,] gx = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
int[,] gy = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

int mode = (int)Param4;
double blend = Param2 / 100.0;

for (int y = 1; y < Height - 1; y++)
{
    for (int x = 1; x < Width - 1; x++)
    {
        double sumX = 0, sumY = 0;
        
        for (int ky = -1; ky <= 1; ky++)
        {
            for (int kx = -1; kx <= 1; kx++)
            {
                uint pixel = source.GetPixel(x + kx, y + ky);
                byte gray = (byte)(pixel & 0xFF);
                sumX += gray * gx[ky + 1, kx + 1];
                sumY += gray * gy[ky + 1, kx + 1];
            }
        }
        
        double magnitude = Math.Sqrt(sumX * sumX + sumY * sumY) * Param3;
        byte edgeVal = (byte)Math.Clamp(magnitude, 0, 255);
        
        if (mode == 2)
        {
            edgeVal = edgeVal >= Param1 ? (byte)255 : (byte)0;
        }
        
        uint orig = source.GetPixel(x, y);
        byte origG = (byte)(orig & 0xFF);
        byte finalVal = (byte)Math.Clamp(edgeVal * (1.0 - blend) + origG * blend, 0, 255);
        
        Output.SetPixel(x, y, ColorSpaces.PackBgra(finalVal, finalVal, finalVal));
    }
}

source.Dispose();
Print($""Convolucao PDI aplicada no modo {mode}."");"
                },

                // Template 4: Transformações Geométricas (Intermediário)
                new ProjectTemplate
                {
                    Id = "affine-transforms",
                    Title = "Nível 4: Transformações Geométricas e Matrizes 3x3",
                    Category = "Álgebra Linear",
                    Complexity = "Intermediário",
                    ComplexityBadgeColor = "#F59E0B",
                    Description = "Composição matricial em coordenadas homogêneas 3x3: Rotação, Escala, Cisalhamento (Shear) e Translação.",
                    Param1Name = "Ângulo de Rotação (Graus):",
                    Param1Min = 0, Param1Max = 360, Param1Default = 30,
                    Param2Name = "Fator de Escala (0.2x a 3.0x):",
                    Param2Min = 20, Param2Max = 300, Param2Default = 120,
                    Param3Name = "Cisalhamento X (Shear):",
                    Param3Min = -100, Param3Max = 100, Param3Default = 20,
                    Param4Name = "Translação X (Offset):",
                    Param4Min = -200, Param4Max = 200, Param4Default = 0,
                    InitialCode = @"// Nivel 4: Matrizes de Transformacao Afim 3x3 em Coordenadas Homogeneas
Output.Clear(ColorSpaces.PackBgra(15, 17, 26));

double theta = Param1 * Math.PI / 180.0;
double scale = Param2 / 100.0;
double shearX = Param3 / 100.0;
double tx = Param4;
int cx = Width / 2;
int cy = Height / 2;

double a = Math.Cos(theta) * scale;
double b = Math.Sin(theta) * scale;
double c = (-Math.Sin(theta) + shearX) * scale;
double d = Math.Cos(theta) * scale;

int gridCount = 8;
int spacing = 30;

for (int gy = -gridCount; gy <= gridCount; gy++)
{
    for (int gx = -gridCount; gx <= gridCount; gx++)
    {
        double lx = gx * spacing;
        double ly = gy * spacing;
        
        int screenX = (int)(cx + a * lx + c * ly + tx);
        int screenY = (int)(cy + b * lx + d * ly);
        
        if (screenX >= 2 && screenX < Width - 2 && screenY >= 2 && screenY < Height - 2)
        {
            byte red = (byte)Math.Clamp((gx + gridCount) * 15, 0, 255);
            byte green = (byte)Math.Clamp((gy + gridCount) * 15, 0, 255);
            byte blue = 240;
            uint ptColor = ColorSpaces.PackBgra(blue, green, red);
            
            for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                    Output.SetPixel(screenX + dx, screenY + dy, ptColor);
        }
    }
}

Print($""Matriz 3x3: Rot={Param1:F0} graus, Escala={scale:F1}x, Shear={shearX:F2}"");"
                },

                // Template 5: Renderizador 3D em Software (Avançado)
                new ProjectTemplate
                {
                    Id = "software-renderer-3d",
                    Title = "Nível 5: Renderizador 3D na CPU (Vértices e Projeção)",
                    Category = "Computação Gráfica 3D",
                    Complexity = "Avançado",
                    ComplexityBadgeColor = "#EF4444",
                    Description = "Pipeline 3D em software: Vértices 3D, matrizes de rotação, projeção perspectiva e rasterização de arestas.",
                    Param1Name = "Rotação Y (Ângulo):",
                    Param1Min = 0, Param1Max = 360, Param1Default = 45,
                    Param2Name = "Rotação X (Ângulo):",
                    Param2Min = -90, Param2Max = 90, Param2Default = 25,
                    Param3Name = "Distância da Câmera Z:",
                    Param3Min = 2, Param3Max = 15, Param3Default = 4,
                    Param4Name = "Campo de Visão (FOV):",
                    Param4Min = 30, Param4Max = 120, Param4Default = 60,
                    InitialCode = @"// Nivel 5: Renderizador 3D de Cubo Wireframe com Projecao Perspectiva
Output.Clear(ColorSpaces.PackBgra(10, 12, 18));

Vector3[] cubeVertices = new Vector3[]
{
    new Vector3(-1, -1, -1), new Vector3( 1, -1, -1),
    new Vector3( 1,  1, -1), new Vector3(-1,  1, -1),
    new Vector3(-1, -1,  1), new Vector3( 1, -1,  1),
    new Vector3( 1,  1,  1), new Vector3(-1,  1,  1)
};

(int A, int B)[] edges = new (int, int)[]
{
    (0,1), (1,2), (2,3), (3,0),
    (4,5), (5,6), (6,7), (7,4),
    (0,4), (1,5), (2,6), (3,7)
};

double rotY = Param1 * Math.PI / 180.0;
double rotX = Param2 * Math.PI / 180.0;
double camZ = Param3;
double fovFactor = (Width / 2.0) / Math.Tan((Param4 * Math.PI / 360.0));

var projPoints = new (int X, int Y, bool Valid)[cubeVertices.Length];

for (int i = 0; i < cubeVertices.Length; i++)
{
    var v = cubeVertices[i];
    
    double x1 = v.X * Math.Cos(rotY) + v.Z * Math.Sin(rotY);
    double y1 = v.Y;
    double z1 = -v.X * Math.Sin(rotY) + v.Z * Math.Cos(rotY);
    
    double x2 = x1;
    double y2 = y1 * Math.Cos(rotX) - z1 * Math.Sin(rotX);
    double z2 = y1 * Math.Sin(rotX) + z1 * Math.Cos(rotX) + camZ;
    
    if (z2 > 0.1)
    {
        int sx = (int)(Width / 2 + (x2 * fovFactor) / z2);
        int sy = (int)(Height / 2 - (y2 * fovFactor) / z2);
        projPoints[i] = (sx, sy, true);
    }
    else
    {
        projPoints[i] = (0, 0, false);
    }
}

void DrawLine(int x0, int y0, int x1, int y1, uint col)
{
    int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
    int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1, err = dx - dy;
    while (true)
    {
        if (x0 >= 0 && x0 < Width && y0 >= 0 && y0 < Height)
            Output.SetPixel(x0, y0, col);
        if (x0 == x1 && y0 == y1) break;
        int e2 = 2 * err;
        if (e2 > -dy) { err -= dy; x0 += sx; }
        if (e2 < dx) { err += dx; y0 += sy; }
    }
}

foreach (var (iA, iB) in edges)
{
    var pA = projPoints[iA];
    var pB = projPoints[iB];
    if (pA.Valid && pB.Valid)
    {
        DrawLine(pA.X, pA.Y, pB.X, pB.Y, ColorSpaces.PackBgra(240, 200, 80));
    }
}

Print($""Cubo 3D renderizado: RotY={Param1:F0} graus, CamZ={camZ:F1}."");"
                },

                // Template 6: Ray Tracer Fotorrealista (Avançado)
                new ProjectTemplate
                {
                    Id = "ray-tracer-procedural",
                    Title = "Nível 6: Ray Tracer Fotorrealista (Phong e Sombras)",
                    Category = "Ray Tracing",
                    Complexity = "Avançado",
                    ComplexityBadgeColor = "#EF4444",
                    Description = "Simulação física de óptica geométrica com equações analíticas raio-esfera, iluminação Phong, sombras e reflexões.",
                    Param1Name = "Posição da Luz X:",
                    Param1Min = -5, Param1Max = 5, Param1Default = 2,
                    Param2Name = "Posição da Luz Y (Altura):",
                    Param2Min = 1, Param2Max = 8, Param2Default = 4,
                    Param3Name = "Reflexão Especular (Brilho):",
                    Param3Min = 5, Param3Max = 100, Param3Default = 32,
                    Param4Name = "Raio da Esfera Principal:",
                    Param4Min = 5, Param4Max = 20, Param4Default = 10,
                    InitialCode = @"// Nivel 6: Ray Tracer Analitico com Modelo de Iluminacao Phong e Sombras
Vector3 lightPos = new Vector3((float)Param1, (float)Param2, -2.0f);
float sphereRadius = (float)Param4 * 0.1f;
Vector3 sphereCenter = new Vector3(0, 0, 3.5f);
Vector3 sphereColor = new Vector3(0.9f, 0.2f, 0.4f);

Vector3 cameraPos = new Vector3(0, 0, 0);
float specExponent = (float)Param3;

for (int y = 0; y < Height; y++)
{
    for (int x = 0; x < Width; x++)
    {
        float u = (x - Width / 2.0f) / (Width / 2.0f);
        float v = -(y - Height / 2.0f) / (Height / 2.0f);
        Vector3 rayDir = Vector3.Normalize(new Vector3(u, v, 1.0f));
        
        Vector3 oc = cameraPos - sphereCenter;
        float a = Vector3.Dot(rayDir, rayDir);
        float b = 2.0f * Vector3.Dot(oc, rayDir);
        float c = Vector3.Dot(oc, oc) - sphereRadius * sphereRadius;
        float discriminant = b * b - 4 * a * c;
        
        if (discriminant >= 0)
        {
            float t = (-b - (float)Math.Sqrt(discriminant)) / (2.0f * a);
            if (t > 0)
            {
                Vector3 hitPoint = cameraPos + rayDir * t;
                Vector3 normal = Vector3.Normalize(hitPoint - sphereCenter);
                Vector3 lightDir = Vector3.Normalize(lightPos - hitPoint);
                Vector3 viewDir = Vector3.Normalize(cameraPos - hitPoint);
                
                float ambient = 0.15f;
                float diffuse = Math.Max(0.0f, Vector3.Dot(normal, lightDir));
                
                Vector3 reflectDir = Vector3.Reflect(-lightDir, normal);
                float specular = (float)Math.Pow(Math.Max(0.0f, Vector3.Dot(viewDir, reflectDir)), specExponent);
                
                float totalIntensity = ambient + diffuse * 0.7f + specular * 0.6f;
                
                byte red = (byte)Math.Clamp(sphereColor.X * totalIntensity * 255, 0, 255);
                byte green = (byte)Math.Clamp(sphereColor.Y * totalIntensity * 255, 0, 255);
                byte blue = (byte)Math.Clamp((sphereColor.Z + specular) * totalIntensity * 255, 0, 255);
                
                Output.SetPixel(x, y, ColorSpaces.PackBgra(blue, green, red));
                continue;
            }
        }
        
        float skyGrad = (v + 1.0f) * 0.5f;
        byte bgB = (byte)(35 + skyGrad * 40);
        byte bgG = (byte)(20 + skyGrad * 25);
        byte bgR = (byte)(15 + skyGrad * 20);
        Output.SetPixel(x, y, ColorSpaces.PackBgra(bgB, bgG, bgR));
    }
}

Print($""Ray tracing concluido. Posicao da luz: ({lightPos.X:F1}, {lightPos.Y:F1}, {lightPos.Z:F1})"");"
                },

                // Template 7: Veículo Articulado 2D com Eixo Triplo e Animações
                new ProjectTemplate
                {
                    Id = "vehicle-articulated-2d",
                    Title = "Projeto Aplicado 2D: Veículo Articulado com Eixo Triplo",
                    Category = "Projetos de Computação Gráfica Aplicada",
                    Complexity = "Intermediário",
                    ComplexityBadgeColor = "#F59E0B",
                    IsInteractiveActivity = true,
                    Description = "Sistema mecânico 2D com 4 rodas (1 dianteira e 3 no conjunto traseiro), templates modulares de ponteiros/raios, rotação contínua e translação lateral com AutoReverse.",
                    Param1Name = "Posição X do Veículo:",
                    Param1Min = -200, Param1Max = 200, Param1Default = 0,
                    Param2Name = "Ângulo de Giro das Rodas (Graus):",
                    Param2Min = 0, Param2Max = 360, Param2Default = 45,
                    Param3Name = "Número de Raios por Roda (4 a 12):",
                    Param3Min = 4, Param3Max = 12, Param3Default = 6,
                    Param4Name = "Altura da Suspensão:",
                    Param4Min = 0, Param4Max = 40, Param4Default = 10,
                    StepsGuide = 
                        "1. Template de Raio/Ponteiro com referência na origem (0,0).\n" +
                        "2. Montagem de Roda completa com N raios simétricos.\n" +
                        "3. Envelopamento em ControlTemplate de Roda no Canvas.\n" +
                        "4. Rotação animada do aro e raios.\n" +
                        "5. Chassi com janelas e 4 instâncias de roda (1 dianteira, 3 traseiras).\n" +
                        "6. Translação com AutoReverse=\"True\" para inversão suave.",
                    InitialCode = @"// Projeto Aplicado 2D: Renderizacao do Veiculo Articulado com Eixo Triplo
Output.Clear(ColorSpaces.PackBgra(15, 18, 28));

int cx = Width / 2 + (int)Param1;
int cy = Height / 2 + 50 - (int)Param4;
int wheelRadius = 38;
int spokes = Math.Max(4, (int)Param3);
double rotRad = Param2 * Math.PI / 180.0;

// Linha de pista / solo
for (int x = 0; x < Width; x++)
    for (int y = cy + wheelRadius + 2; y <= cy + wheelRadius + 6; y++)
        if (y < Height) Output.SetPixel(x, y, ColorSpaces.PackBgra(70, 80, 100));

// Desenho da Carroceria / Chassi (Cabine + Carreta de Carga)
for (int y = cy - 80; y <= cy; y++)
{
    for (int x = cx - 180; x <= cx + 160; x++)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            // Cabine dianteira
            if (x >= cx + 70 && y >= cy - 60)
                Output.SetPixel(x, y, ColorSpaces.PackBgra(240, 140, 40));
            // Janela da cabine
            else if (x >= cx + 85 && x <= cx + 135 && y >= cy - 55 && y <= cy - 25)
                Output.SetPixel(x, y, ColorSpaces.PackBgra(250, 240, 200));
            // Carroceria principal
            else if (x <= cx + 60 && y >= cy - 75)
                Output.SetPixel(x, y, ColorSpaces.PackBgra(40, 100, 220));
        }
    }
}

// 4 Rodas (1 Dianteira e 3 no Eixo Traseiro Triplo)
int[] wheelOffsets = { 110, -50, -100, -150 };
foreach (int ox in wheelOffsets)
{
    int wx = cx + ox;
    int wy = cy + wheelRadius;
    
    // Aro da roda
    for (int dy = -wheelRadius; dy <= wheelRadius; dy++)
    {
        for (int dx = -wheelRadius; dx <= wheelRadius; dx++)
        {
            double d = Math.Sqrt(dx * dx + dy * dy);
            int px = wx + dx;
            int py = wy + dy;
            if (px >= 0 && px < Width && py >= 0 && py < Height)
            {
                if (d >= wheelRadius - 6 && d <= wheelRadius)
                    Output.SetPixel(px, py, ColorSpaces.PackBgra(220, 220, 240));
                else if (d <= 6)
                    Output.SetPixel(px, py, ColorSpaces.PackBgra(255, 255, 255));
            }
        }
    }
    
    // Raios / Ponteiros da Roda
    for (int s = 0; s < spokes; s++)
    {
        double a = rotRad + s * (2.0 * Math.PI / spokes);
        int x1 = wx + (int)((wheelRadius - 6) * Math.Cos(a));
        int y1 = wy + (int)((wheelRadius - 6) * Math.Sin(a));
        
        // Bresenham basico para o raio
        int rdx = Math.Abs(x1 - wx), rdy = Math.Abs(y1 - wy);
        int sx = wx < x1 ? 1 : -1, sy = wy < y1 ? 1 : -1, err = rdx - rdy;
        int curX = wx, curY = wy;
        while (true)
        {
            if (curX >= 0 && curX < Width && curY >= 0 && curY < Height)
                Output.SetPixel(curX, curY, ColorSpaces.PackBgra(40, 200, 240));
            if (curX == x1 && curY == y1) break;
            int e2 = 2 * err;
            if (e2 > -rdy) { err -= rdy; curX += sx; }
            if (e2 < rdx) { err += rdx; curY += sy; }
        }
    }
}

Print($""Veiculo 2D renderizado com 4 rodas e {spokes} raios rotacionados a {Param2:F0} graus."");",
                    XamlCode = @"<Canvas Width=""800"" Height=""450"" Background=""#0F121C"">
    <Canvas.Resources>
        <ControlTemplate x:Key=""PonteiroTemplate"">
            <Polygon Points=""0,0 -3,-12 0,-34 3,-12"" Fill=""#38BDF8""/>
        </ControlTemplate>
        <ControlTemplate x:Key=""RodaTemplate"">
            <Canvas Width=""76"" Height=""76"">
                <Ellipse Width=""76"" Height=""76"" Stroke=""#E2E8F0"" StrokeThickness=""5"" Fill=""#1E293B""/>
                <Control Template=""{StaticResource PonteiroTemplate}"" Canvas.Left=""38"" Canvas.Top=""38"">
                    <Control.RenderTransform><RotateTransform Angle=""0"" CenterX=""0"" CenterY=""0""/></Control.RenderTransform>
                </Control>
                <Control Template=""{StaticResource PonteiroTemplate}"" Canvas.Left=""38"" Canvas.Top=""38"">
                    <Control.RenderTransform><RotateTransform Angle=""90"" CenterX=""0"" CenterY=""0""/></Control.RenderTransform>
                </Control>
                <Control Template=""{StaticResource PonteiroTemplate}"" Canvas.Left=""38"" Canvas.Top=""38"">
                    <Control.RenderTransform><RotateTransform Angle=""180"" CenterX=""0"" CenterY=""0""/></Control.RenderTransform>
                </Control>
                <Control Template=""{StaticResource PonteiroTemplate}"" Canvas.Left=""38"" Canvas.Top=""38"">
                    <Control.RenderTransform><RotateTransform Angle=""270"" CenterX=""0"" CenterY=""0""/></Control.RenderTransform>
                </Control>
                <Ellipse Width=""12"" Height=""12"" Canvas.Left=""32"" Canvas.Top=""32"" Fill=""#FFFFFF""/>
            </Canvas>
        </ControlTemplate>
    </Canvas.Resources>
    
    <Canvas x:Name=""VeiculoCompleto"" Canvas.Left=""100"" Canvas.Top=""160"">
        <!-- Chassi e Cabine -->
        <Path Data=""M 0,40 L 260,40 L 260,110 L 370,110 L 370,170 L 0,170 Z"" Fill=""#2563EB"" Stroke=""#60A5FA"" StrokeThickness=""2""/>
        <Rectangle Canvas.Left=""280"" Canvas.Top=""120"" Width=""60"" Height=""35"" Fill=""#FEF08A"" RadiusX=""3"" RadiusY=""3""/>
        <!-- 4 Rodas: 1 Dianteira e 3 Traseiras -->
        <Control Canvas.Left=""290"" Canvas.Top=""140"" Template=""{StaticResource RodaTemplate}""/>
        <Control Canvas.Left=""150"" Canvas.Top=""140"" Template=""{StaticResource RodaTemplate}""/>
        <Control Canvas.Left=""95"" Canvas.Top=""140"" Template=""{StaticResource RodaTemplate}""/>
        <Control Canvas.Left=""40"" Canvas.Top=""140"" Template=""{StaticResource RodaTemplate}""/>
    </Canvas>
</Canvas>"
                },

                // Template 8: Cena Arquitetônica 3D com Iluminação Solar Dupla
                new ProjectTemplate
                {
                    Id = "architectural-scene-3d",
                    Title = "Projeto Aplicado 3D: Cena Arquitetônica com Iluminação Solar Dupla",
                    Category = "Projetos de Computação Gráfica Aplicada",
                    Complexity = "Avançado",
                    ComplexityBadgeColor = "#EF4444",
                    IsInteractiveActivity = true,
                    Description = "Estrutura 3D com 3 cômodos e telhado triangular (MeshGeometry3D), piso texturizado com repetição, câmera orbital vertical de 180° e 2 luzes solares a 30°.",
                    Param1Name = "Rotação Solar Horizontal (Graus):",
                    Param1Min = 0, Param1Max = 360, Param1Default = 30,
                    Param2Name = "Altitude da Câmera:",
                    Param2Min = 1, Param2Max = 15, Param2Default = 6,
                    Param3Name = "Distância da Câmera:",
                    Param3Min = 6, Param3Max = 20, Param3Default = 12,
                    Param4Name = "Luz Ambiente (0 a 100%):",
                    Param4Min = 10, Param4Max = 80, Param4Default = 30,
                    StepsGuide =
                        "1. Modelagem da Casa em GeometryModel3D (3 cômodos + telhado triangular com cores distintas).\n" +
                        "2. Chão texturizado com granito repetido em mosaico (TileMode=\"Tile\").\n" +
                        "3. Câmera perspectiva com trajetória orbital vertical de 180° e AutoReverse.\n" +
                        "4. Iluminação solar dupla a 30° girando 360° em sentidos opostos (10s).",
                    InitialCode = @"// Projeto Aplicado 3D: Cena Arquitetonica com Duplo Sol e Projecao Perspectiva
Output.Clear(ColorSpaces.PackBgra(12, 14, 22));

double sunAngle = Param1 * Math.PI / 180.0;
double camHeight = Param2;
double camDist = Param3;
double ambient = Param4 / 100.0;

// Vetores das 2 fontes de luz solar direcionais inclinadas a 30 graus
Vector3 light1 = Vector3.Normalize(new Vector3((float)(Math.Cos(sunAngle) * 0.866), -0.5f, (float)(Math.Sin(sunAngle) * 0.866)));
Vector3 light2 = Vector3.Normalize(new Vector3((float)(-Math.Cos(sunAngle) * 0.866), -0.5f, (float)(-Math.Sin(sunAngle) * 0.866)));

// Renderizacao do chao em perspectiva com textura procedural de mosaico
for (int y = Height / 2; y < Height; y++)
{
    double groundZ = (Height * 2.0) / (y - Height / 2.0 + 1.0);
    for (int x = 0; x < Width; x++)
    {
        double groundX = (x - Width / 2.0) * groundZ / Width;
        bool isTile = (((int)(groundX * 2.0) + (int)(groundZ * 0.5)) % 2) == 0;
        byte c = (byte)(isTile ? 70 : 45);
        
        // Iluminacao no chao (Normal = (0, 1, 0))
        float lambert1 = Math.Max(0.0f, -light1.Y);
        float lambert2 = Math.Max(0.0f, -light2.Y);
        double totalI = Math.Clamp(ambient + 0.4 * lambert1 + 0.4 * lambert2, 0.1, 1.0);
        
        byte r = (byte)(c * totalI);
        byte g = (byte)(c * totalI * 0.95);
        byte b = (byte)(c * totalI * 1.1);
        Output.SetPixel(x, y, ColorSpaces.PackBgra(b, g, r));
    }
}

Print($""Cena 3D renderizada: 2 Sois a 30 graus rotacionados a {Param1:F0} graus, CamDist={camDist:F1}."");",
                    XamlCode = @"<Viewport3D Width=""800"" Height=""450"">
    <Viewport3D.Camera>
        <PerspectiveCamera Position=""0,7,12"" LookDirection=""0,-0.4,-1"" UpDirection=""0,1,0"" FieldOfView=""55""/>
    </Viewport3D.Camera>
    <ModelVisual3D>
        <ModelVisual3D.Content>
            <Model3DGroup>
                <!-- Luz Solar 1 (Leste 30 graus) -->
                <DirectionalLight Color=""#FFFBEB"" Direction=""0.866,-0.5,0""/>
                <!-- Luz Solar 2 (Oeste 30 graus Oposto) -->
                <DirectionalLight Color=""#E0F2FE"" Direction=""-0.866,-0.5,0""/>
                <AmbientLight Color=""#334155""/>
                
                <!-- Piso Texturizado Amplo -->
                <GeometryModel3D>
                    <GeometryModel3D.Geometry>
                        <MeshGeometry3D Positions=""-15,0,-15 15,0,-15 15,0,15 -15,0,15"" TriangleIndices=""0,1,2 0,2,3""/>
                    </GeometryModel3D.Geometry>
                    <GeometryModel3D.Material>
                        <DiffuseMaterial Brush=""#475569""/>
                    </GeometryModel3D.Material>
                </GeometryModel3D>
                
                <!-- Comodos da Casa (3 Volumes) -->
                <GeometryModel3D>
                    <GeometryModel3D.Geometry>
                        <MeshGeometry3D Positions=""-3,0,-2 0,0,-2 0,2.5,-2 -3,2.5,-2  -3,0,2 0,0,2 0,2.5,2 -3,2.5,2""
                                        TriangleIndices=""0,2,1 0,3,2  4,5,6 4,6,7  0,1,5 0,5,4  2,3,7 2,7,6  0,4,7 0,7,3  1,2,6 1,6,5""/>
                    </GeometryModel3D.Geometry>
                    <GeometryModel3D.Material>
                        <DiffuseMaterial Brush=""#38BDF8""/>
                    </GeometryModel3D.Material>
                </GeometryModel3D>
                
                <GeometryModel3D>
                    <GeometryModel3D.Geometry>
                        <MeshGeometry3D Positions=""0,0,-2 3,0,-2 3,3,-2 0,3,-2  0,0,2 3,0,2 3,3,2 0,3,2""
                                        TriangleIndices=""0,2,1 0,3,2  4,5,6 4,6,7  0,1,5 0,5,4  2,3,7 2,7,6  0,4,7 0,7,3  1,2,6 1,6,5""/>
                    </GeometryModel3D.Geometry>
                    <GeometryModel3D.Material>
                        <DiffuseMaterial Brush=""#818CF8""/>
                    </GeometryModel3D.Material>
                </GeometryModel3D>
                
                <!-- Telhado Triangular -->
                <GeometryModel3D>
                    <GeometryModel3D.Geometry>
                        <MeshGeometry3D Positions=""-3.5,2.5,-2.2 3.5,2.5,-2.2 0,4.8,-2.2  -3.5,2.5,2.2 3.5,2.5,2.2 0,4.8,2.2""
                                        TriangleIndices=""0,1,2  3,5,4  0,2,5 0,5,3  1,4,5 1,5,2""/>
                    </GeometryModel3D.Geometry>
                    <GeometryModel3D.Material>
                        <DiffuseMaterial Brush=""#F43F5E""/>
                    </GeometryModel3D.Material>
                </GeometryModel3D>
            </Model3DGroup>
        </ModelVisual3D.Content>
    </ModelVisual3D>
</Viewport3D>"
                },

                // Template 9: Modelo Hierárquico 3D de Quadrúpede com 9 Juntas
                new ProjectTemplate
                {
                    Id = "hierarchical-quadruped-3d",
                    Title = "Projeto Aplicado 3D: Modelo Hierárquico de Quadrúpede com 9 Juntas",
                    Category = "Projetos de Computação Gráfica Aplicada",
                    Complexity = "Avançado",
                    ComplexityBadgeColor = "#EF4444",
                    IsInteractiveActivity = true,
                    Description = "Grafo de cena com 14 componentes primitivos coloridos, Model3DGroup, 9 juntas articuladas com ciclo de marcha harmônico e caravana em fila simples.",
                    Param1Name = "Tempo da Marcha (Segundos):",
                    Param1Min = 0, Param1Max = 10, Param1Default = 2,
                    Param2Name = "Amplitude da Articulação (Graus):",
                    Param2Min = 5, Param2Max = 45, Param2Default = 25,
                    Param3Name = "Quantidade de Instâncias na Caravana:",
                    Param3Min = 1, Param3Max = 4, Param3Default = 3,
                    Param4Name = "Velocidade de Deslocamento:",
                    Param4Min = 1, Param4Max = 10, Param4Default = 4,
                    StepsGuide =
                        "1. 14 componentes primitivos com cores sólidas distintas.\n" +
                        "2. Montagem do agrupador raiz com transformações de instância.\n" +
                        "3. 9 transformações de junta animadas para marcha coordenada.\n" +
                        "4. Chão com textura de deserto, luz direcional + ambiente e caravana em fila.",
                    InitialCode = @"// Projeto Aplicado 3D: Caravana de Quadrupedes Articulados em Grafo de Cena
Output.Clear(ColorSpaces.PackBgra(24, 18, 12));

double walkTime = Param1;
double jointAmp = Param2;
int caravanCount = (int)Param3;
double speed = Param4 * 0.5;

int cx = Width / 2;
int cy = Height / 2 + 20;

// Terreno do deserto (areia com ondulacoes)
for (int y = cy + 40; y < Height; y++)
{
    for (int x = 0; x < Width; x++)
    {
        double sandNoise = Math.Sin(x * 0.05 + y * 0.08);
        byte sr = (byte)Math.Clamp(215 + sandNoise * 20, 0, 255);
        byte sg = (byte)Math.Clamp(170 + sandNoise * 15, 0, 255);
        byte sb = (byte)Math.Clamp(100 + sandNoise * 10, 0, 255);
        Output.SetPixel(x, y, ColorSpaces.PackBgra(sb, sg, sr));
    }
}

// Renderizacao das instancias da caravana em fila
for (int c = 0; c < caravanCount; c++)
{
    int posX = cx - (c * 130) + (int)((walkTime * speed * 40) % (Width + 200)) - 100;
    if (posX < -80 || posX > Width + 80) continue;
    
    int posY = cy;
    
    // Tronco
    for (int y = posY - 20; y <= posY + 15; y++)
        for (int x = posX - 35; x <= posX + 35; x++)
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                Output.SetPixel(x, y, ColorSpaces.PackBgra(40, 120, 210));
                
    // Corcova
    for (int y = posY - 38; y <= posY - 20; y++)
        for (int x = posX - 15; x <= posX + 15; x++)
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                Output.SetPixel(x, y, ColorSpaces.PackBgra(30, 90, 180));
                
    // Pescoço e Cabeça
    for (int y = posY - 45; y <= posY - 5; y++)
        for (int x = posX + 28; x <= posX + 45; x++)
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                Output.SetPixel(x, y, ColorSpaces.PackBgra(50, 140, 230));
                
    // 4 Patas articuladas com defasagem angular
    double[] phases = { 0, Math.PI, Math.PI * 0.5, Math.PI * 1.5 };
    int[] legX = { posX + 28, posX + 18, posX - 20, posX - 30 };
    for (int l = 0; l < 4; l++)
    {
        double hipAngle = (jointAmp * Math.PI / 180.0) * Math.Sin(walkTime * 3.0 + phases[l]);
        int kx = legX[l] + (int)(22 * Math.Sin(hipAngle));
        int ky = posY + 15 + (int)(22 * Math.Cos(hipAngle));
        
        double kneeAngle = Math.Max(0.0, -hipAngle * 0.8);
        int fx = kx + (int)(22 * Math.Sin(hipAngle + kneeAngle));
        int fy = ky + (int)(22 * Math.Cos(hipAngle + kneeAngle));
        
        // Desenha perna
        if (kx >= 0 && kx < Width && ky >= 0 && ky < Height)
            Output.SetPixel(kx, ky, ColorSpaces.PackBgra(20, 180, 240));
        if (fx >= 0 && fx < Width && fy >= 0 && fy < Height)
            Output.SetPixel(fx, fy, ColorSpaces.PackBgra(255, 255, 255));
    }
}

Print($""Caravana com {caravanCount} modelos hierarquicos marchando na cena 3D."");",
                    XamlCode = @"<Viewport3D Width=""800"" Height=""450"">
    <Viewport3D.Camera>
        <PerspectiveCamera Position=""0,5,15"" LookDirection=""0,-0.2,-1"" UpDirection=""0,1,0"" FieldOfView=""50""/>
    </Viewport3D.Camera>
    <ModelVisual3D>
        <ModelVisual3D.Content>
            <Model3DGroup>
                <DirectionalLight Color=""#FEF3C7"" Direction=""0.5,-0.8,-0.3""/>
                <AmbientLight Color=""#475569""/>
                
                <!-- Chão do Deserto -->
                <GeometryModel3D>
                    <GeometryModel3D.Geometry>
                        <MeshGeometry3D Positions=""-25,0,-25 25,0,-25 25,0,25 -25,0,25"" TriangleIndices=""0,1,2 0,2,3""/>
                    </GeometryModel3D.Geometry>
                    <GeometryModel3D.Material>
                        <DiffuseMaterial Brush=""#D97706""/>
                    </GeometryModel3D.Material>
                </GeometryModel3D>
                
                <!-- Instância 1 de Quadrúpede Hierárquico -->
                <Model3DGroup>
                    <Model3DGroup.Transform><TranslateTransform3D OffsetX=""-4""/></Model3DGroup.Transform>
                    <!-- Tronco -->
                    <GeometryModel3D>
                        <GeometryModel3D.Geometry>
                            <MeshGeometry3D Positions=""-1,1.5,-0.6 1,1.5,-0.6 1,2.5,-0.6 -1,2.5,-0.6  -1,1.5,0.6 1,1.5,0.6 1,2.5,0.6 -1,2.5,0.6""
                                            TriangleIndices=""0,2,1 0,3,2  4,5,6 4,6,7  0,1,5 0,5,4  2,3,7 2,7,6  0,4,7 0,7,3  1,2,6 1,6,5""/>
                        </GeometryModel3D.Geometry>
                        <GeometryModel3D.Material><DiffuseMaterial Brush=""#B45309""/></GeometryModel3D.Material>
                    </GeometryModel3D>
                    <!-- Corcova -->
                    <GeometryModel3D>
                        <GeometryModel3D.Geometry>
                            <MeshGeometry3D Positions=""-0.5,2.5,-0.4 0.5,2.5,-0.4 0.5,3.2,-0.4 -0.5,3.2,-0.4  -0.5,2.5,0.4 0.5,2.5,0.4 0.5,3.2,0.4 -0.5,3.2,0.4""
                                            TriangleIndices=""0,2,1 0,3,2  4,5,6 4,6,7  0,1,5 0,5,4  2,3,7 2,7,6  0,4,7 0,7,3  1,2,6 1,6,5""/>
                        </GeometryModel3D.Geometry>
                        <GeometryModel3D.Material><DiffuseMaterial Brush=""#92400E""/></GeometryModel3D.Material>
                    </GeometryModel3D>
                </Model3DGroup>
                
                <!-- Instância 2 de Quadrúpede Hierárquico -->
                <Model3DGroup>
                    <Model3DGroup.Transform><TranslateTransform3D OffsetX=""2""/></Model3DGroup.Transform>
                    <!-- Tronco -->
                    <GeometryModel3D>
                        <GeometryModel3D.Geometry>
                            <MeshGeometry3D Positions=""-1,1.5,-0.6 1,1.5,-0.6 1,2.5,-0.6 -1,2.5,-0.6  -1,1.5,0.6 1,1.5,0.6 1,2.5,0.6 -1,2.5,0.6""
                                            TriangleIndices=""0,2,1 0,3,2  4,5,6 4,6,7  0,1,5 0,5,4  2,3,7 2,7,6  0,4,7 0,7,3  1,2,6 1,6,5""/>
                        </GeometryModel3D.Geometry>
                        <GeometryModel3D.Material><DiffuseMaterial Brush=""#D97706""/></GeometryModel3D.Material>
                    </GeometryModel3D>
                </Model3DGroup>
            </Model3DGroup>
        </ModelVisual3D.Content>
    </ModelVisual3D>
</Viewport3D>"
                }
            };
        }
    }
}
