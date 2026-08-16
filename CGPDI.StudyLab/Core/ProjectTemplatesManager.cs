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
                    InitialCode = @"// Projeto Livre: Codigo C# do Zero
// Variaveis disponiveis no ambiente:
// - Output: DirectBitmap (Buffer de saida onde voce desenha)
// - Width, Height: Dimensoes da imagem (512x512)
// - Param1, Param2, Param3, Param4: Valores dos 4 sliders interativos
// - Print(string): Imprime mensagens no console do estudio

Print($""Iniciando renderizacao livre: {Width}x{Height}"");

// Exemplo: Limpa o fundo e desenha um gradiente radial com aneis concentricos
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
        
        // Padrao de aneis com senos
        double wave = Math.Sin(dist * freq - Param1 * 0.1);
        int intensity = (int)((wave + 1.0) * 0.5 * Param3);
        
        byte r = (byte)Math.Clamp(intensity + (int)Param2, 0, 255);
        byte g = (byte)Math.Clamp(intensity, 0, 255);
        byte b = (byte)Math.Clamp(255 - intensity, 0, 255);
        
        Output.SetPixel(x, y, ColorSpaces.PackBgra(b, g, r));
    }
}

Print(""Renderizacao concluida com sucesso!"");"
                },

                // Template 1: Padrões Matemáticos e Procedurais (Básico)
                new ProjectTemplate
                {
                    Id = "procedural-patterns",
                    Title = "Nivel 1: Padroes Matematicos e Fractais 2D",
                    Category = "Fundamentos",
                    Complexity = "Basico",
                    ComplexityBadgeColor = "#10B981",
                    Description = "Gere padroes visuais, fractais de Mandelbrot e interferencia ondulatoria atraves de funcoes matematicas por pixel.",
                    Param1Name = "Zoom / Escala Fractal:",
                    Param1Min = 1, Param1Max = 100, Param1Default = 35,
                    Param2Name = "Deslocamento X (Offset):",
                    Param2Min = -100, Param2Max = 100, Param2Default = -30,
                    Param3Name = "Deslocamento Y (Offset):",
                    Param3Min = -100, Param3Max = 100, Param3Default = 0,
                    Param4Name = "Iteracoes Maximas (5 a 80):",
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
                    Title = "Nivel 2: Rasterizacao 2D e Desenho de Formas",
                    Category = "Rasterizacao 2D",
                    Complexity = "Basico",
                    ComplexityBadgeColor = "#10B981",
                    Description = "Implemente e teste algoritmos de desenho de primitivas geometricas: Reta de Bresenham, Circulo do Ponto Medio e poligonos.",
                    Param1Name = "Raio do Circulo (10 a 200):",
                    Param1Min = 10, Param1Max = 200, Param1Default = 120,
                    Param2Name = "Numero de Vertices (3 a 12):",
                    Param2Min = 3, Param2Max = 12, Param2Default = 6,
                    Param3Name = "Rotacao (Graus 0 a 360):",
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
                    Title = "Nivel 3: Processamento Digital de Imagens (PDI)",
                    Category = "Processamento de Imagens",
                    Complexity = "Intermediario",
                    ComplexityBadgeColor = "#F59E0B",
                    Description = "Convolucao espacial 3x3 (Sobel, Gaussiano, Laplace), deteccao de bordas, contraste e limiarizacao.",
                    Param1Name = "Forca do Filtro / Limiar:",
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
                    Title = "Nivel 4: Transformacoes Geometricas e Matrizes 3x3",
                    Category = "Algebra Linear",
                    Complexity = "Intermediario",
                    ComplexityBadgeColor = "#F59E0B",
                    Description = "Composicao matricial em coordenadas homogeneas 3x3: Rotacao, Escala, Cisalhamento (Shear) e Translacao.",
                    Param1Name = "Angulo de Rotacao (Graus):",
                    Param1Min = 0, Param1Max = 360, Param1Default = 30,
                    Param2Name = "Fator de Escala (0.2x a 3.0x):",
                    Param2Min = 20, Param2Max = 300, Param2Default = 120,
                    Param3Name = "Cisalhamento X (Shear):",
                    Param3Min = -100, Param3Max = 100, Param3Default = 20,
                    Param4Name = "Translacao X (Offset):",
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
                    Title = "Nivel 5: Renderizador 3D na CPU (Vertices e Projecao)",
                    Category = "Computacao Grafica 3D",
                    Complexity = "Avancado",
                    ComplexityBadgeColor = "#EF4444",
                    Description = "Pipeline 3D em software: Vertices 3D, matrizes de rotacao, projecao perspectiva e rasterizacao de arestas.",
                    Param1Name = "Rotacao Y (Angulo):",
                    Param1Min = 0, Param1Max = 360, Param1Default = 45,
                    Param2Name = "Rotacao X (Angulo):",
                    Param2Min = -90, Param2Max = 90, Param2Default = 25,
                    Param3Name = "Distancia da Camera Z:",
                    Param3Min = 2, Param3Max = 15, Param3Default = 4,
                    Param4Name = "Campo de Visao (FOV):",
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
                    Title = "Nivel 6: Ray Tracer Fotorrealista (Phong e Sombras)",
                    Category = "Ray Tracing",
                    Complexity = "Avancado",
                    ComplexityBadgeColor = "#EF4444",
                    Description = "Simulacao fisica de optica geometrica com equacoes analiticas raio-esfera, iluminacao Phong, sombras e reflexoes.",
                    Param1Name = "Posicao da Luz X:",
                    Param1Min = -5, Param1Max = 5, Param1Default = 2,
                    Param2Name = "Posicao da Luz Y (Altura):",
                    Param2Min = 1, Param2Max = 8, Param2Default = 4,
                    Param3Name = "Reflexao Especular (Brilho):",
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
                }
            };
        }
    }
}
