using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.Graphics3D
{
    public class Mesh3D
    {
        public List<Vec3> Vertices { get; set; } = new List<Vec3>();
        public List<int> Triangles { get; set; } = new List<int>(); // Índices de 3 em 3
        public List<Vec3> Normals { get; set; } = new List<Vec3>();
        public Color BaseColor { get; set; } = Color.FromRgb(80, 150, 240);

        public void CalculateNormals()
        {
            Normals = new List<Vec3>(new Vec3[Vertices.Count]);

            for (int i = 0; i < Triangles.Count; i += 3)
            {
                int i0 = Triangles[i];
                int i1 = Triangles[i + 1];
                int i2 = Triangles[i + 2];

                Vec3 v0 = Vertices[i0];
                Vec3 v1 = Vertices[i1];
                Vec3 v2 = Vertices[i2];

                Vec3 edge1 = v1 - v0;
                Vec3 edge2 = v2 - v0;
                Vec3 faceNormal = Vec3.Cross(edge1, edge2).Normalized;

                Normals[i0] = (Normals[i0] + faceNormal).Normalized;
                Normals[i1] = (Normals[i1] + faceNormal).Normalized;
                Normals[i2] = (Normals[i2] + faceNormal).Normalized;
            }
        }

        #region Geradores de Malhas Procedurais

        public static Mesh3D CreateCube(double size = 1.0, Color? color = null)
        {
            Mesh3D mesh = new Mesh3D();
            mesh.BaseColor = color ?? Color.FromRgb(90, 160, 240);
            double h = size / 2.0;

            // 8 Vértices
            mesh.Vertices.AddRange(new Vec3[]
            {
                new Vec3(-h, -h, -h), // 0
                new Vec3( h, -h, -h), // 1
                new Vec3( h,  h, -h), // 2
                new Vec3(-h,  h, -h), // 3
                new Vec3(-h, -h,  h), // 4
                new Vec3( h, -h,  h), // 5
                new Vec3( h,  h,  h), // 6
                new Vec3(-h,  h,  h)  // 7
            });

            // 12 Triângulos (6 Faces)
            int[] indices = new int[]
            {
                0, 2, 1,  0, 3, 2, // Frente
                5, 6, 4,  4, 6, 7, // Trás
                4, 7, 0,  0, 7, 3, // Esquerda
                1, 2, 5,  5, 2, 6, // Direita
                3, 7, 2,  2, 7, 6, // Topo
                0, 1, 4,  4, 1, 5  // Base
            };
            mesh.Triangles.AddRange(indices);
            mesh.CalculateNormals();
            return mesh;
        }

        public static Mesh3D CreatePyramid(double size = 1.0)
        {
            Mesh3D mesh = new Mesh3D();
            mesh.BaseColor = Color.FromRgb(240, 130, 80);
            double h = size / 2.0;

            mesh.Vertices.AddRange(new Vec3[]
            {
                new Vec3(-h, -h, -h), // 0
                new Vec3( h, -h, -h), // 1
                new Vec3( h, -h,  h), // 2
                new Vec3(-h, -h,  h), // 3
                new Vec3( 0,  h,  0)  // 4 Topo
            });

            int[] indices = new int[]
            {
                0, 2, 1,  0, 3, 2, // Base quadrada
                0, 1, 4,           // Lado 1
                1, 2, 4,           // Lado 2
                2, 3, 4,           // Lado 3
                3, 0, 4            // Lado 4
            };
            mesh.Triangles.AddRange(indices);
            mesh.CalculateNormals();
            return mesh;
        }

        public static Mesh3D CreateUvSphere(double radius = 1.0, int latitudeBands = 20, int longitudeBands = 20)
        {
            Mesh3D mesh = new Mesh3D();
            mesh.BaseColor = Color.FromRgb(80, 220, 140);

            for (int lat = 0; lat <= latitudeBands; lat++)
            {
                double theta = lat * Math.PI / latitudeBands;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int lon = 0; lon <= longitudeBands; lon++)
                {
                    double phi = lon * 2.0 * Math.PI / longitudeBands;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = cosPhi * sinTheta;
                    double y = cosTheta;
                    double z = sinPhi * sinTheta;

                    mesh.Vertices.Add(new Vec3(x * radius, y * radius, z * radius));
                    mesh.Normals.Add(new Vec3(x, y, z));
                }
            }

            for (int lat = 0; lat < latitudeBands; lat++)
            {
                for (int lon = 0; lon < longitudeBands; lon++)
                {
                    int first = (lat * (longitudeBands + 1)) + lon;
                    int second = first + longitudeBands + 1;

                    mesh.Triangles.Add(first);
                    mesh.Triangles.Add(first + 1);
                    mesh.Triangles.Add(second);

                    mesh.Triangles.Add(second);
                    mesh.Triangles.Add(first + 1);
                    mesh.Triangles.Add(second + 1);
                }
            }

            return mesh;
        }

        public static Mesh3D CreateTorus(double rMajor = 1.0, double rMinor = 0.35, int majorSegments = 24, int minorSegments = 16)
        {
            Mesh3D mesh = new Mesh3D();
            mesh.BaseColor = Color.FromRgb(220, 90, 200);

            for (int i = 0; i <= majorSegments; i++)
            {
                double u = i * 2.0 * Math.PI / majorSegments;
                double cosU = Math.Cos(u);
                double sinU = Math.Sin(u);

                for (int j = 0; j <= minorSegments; j++)
                {
                    double v = j * 2.0 * Math.PI / minorSegments;
                    double cosV = Math.Cos(v);
                    double sinV = Math.Sin(v);

                    double x = (rMajor + rMinor * cosV) * cosU;
                    double y = rMinor * sinV;
                    double z = (rMajor + rMinor * cosV) * sinU;

                    Vec3 pos = new Vec3(x, y, z);
                    Vec3 center = new Vec3(rMajor * cosU, 0, rMajor * sinU);
                    Vec3 normal = (pos - center).Normalized;

                    mesh.Vertices.Add(pos);
                    mesh.Normals.Add(normal);
                }
            }

            for (int i = 0; i < majorSegments; i++)
            {
                for (int j = 0; j < minorSegments; j++)
                {
                    int first = (i * (minorSegments + 1)) + j;
                    int second = first + minorSegments + 1;

                    mesh.Triangles.Add(first);
                    mesh.Triangles.Add(second);
                    mesh.Triangles.Add(first + 1);

                    mesh.Triangles.Add(second);
                    mesh.Triangles.Add(second + 1);
                    mesh.Triangles.Add(first + 1);
                }
            }

            return mesh;
        }

        #endregion
    }

    /// <summary>
    /// Renderizador 3D em Software (Pipeline Gráfico 3D 100% CPU construído do zero).
    /// Executa:
    /// - Transformações Matriciais (Model -> View -> Projection)
    /// - Divisão Perspectiva (w-divide)
    /// - Mapeamento para Espaço de Tela (Viewport)
    /// - Back-Face Culling (Descarte de faces traseiras via Dot Product)
    /// - Rasterização de Triângulos com Coordenadas Baricêntricas
    /// - Depth-Buffering (Z-Buffer com interpolação de profundidade 1/Z)
    /// - Modelo de Iluminação de Phong (Ambiente, Difuso Lambertiano, Especular)
    /// </summary>
    public static class SoftwareRenderer3D
    {
        public static DirectBitmap RenderScene(
            Mesh3D mesh,
            int width = 512,
            int height = 512,
            double rotX = 0.3,
            double rotY = 0.5,
            double rotZ = 0.0,
            double cameraDist = 3.5,
            bool wireframe = false)
        {
            DirectBitmap dst = new DirectBitmap(width, height);
            dst.Lock();
            dst.Clear(Color.FromRgb(18, 18, 22)); // Fundo escuro elegante

            float[] zBuffer = new float[width * height];
            for (int i = 0; i < zBuffer.Length; i++)
                zBuffer[i] = float.MaxValue;

            // 1. Matriz de Modelo (Rotação)
            Mat4x4 matModel = Mat4x4.CreateRotationX(rotX) * Mat4x4.CreateRotationY(rotY) * Mat4x4.CreateRotationZ(rotZ);

            // 2. Matriz de Visualização (Câmera LookAt)
            Vec3 eye = new Vec3(0, 0, -cameraDist);
            Vec3 target = Vec3.Zero;
            Vec3 up = Vec3.Up;
            Mat4x4 matView = Mat4x4.CreateLookAt(eye, target, up);

            // 3. Matriz de Projeção Perspectiva (FOV 60 graus)
            double fov = 60.0 * Math.PI / 180.0;
            double aspect = (double)width / height;
            Mat4x4 matProj = Mat4x4.CreatePerspective(fov, aspect, 0.1, 100.0);

            Mat4x4 matMVP = matProj * matView * matModel;

            // Direção da luz no espaço de mundo
            Vec3 lightDir = new Vec3(0.5, 1.0, -0.8).Normalized;
            Vec3 viewDir = (eye - target).Normalized;

            int numTriangles = mesh.Triangles.Count / 3;

            for (int t = 0; t < numTriangles; t++)
            {
                int i0 = mesh.Triangles[t * 3 + 0];
                int i1 = mesh.Triangles[t * 3 + 1];
                int i2 = mesh.Triangles[t * 3 + 2];

                Vec3 v0 = mesh.Vertices[i0];
                Vec3 v1 = mesh.Vertices[i1];
                Vec3 v2 = mesh.Vertices[i2];

                // Transforma vértices de mundo
                Vec3 w0 = matModel.TransformPoint(v0);
                Vec3 w1 = matModel.TransformPoint(v1);
                Vec3 w2 = matModel.TransformPoint(v2);

                // Normal da face no mundo
                Vec3 faceNormal = Vec3.Cross(w1 - w0, w2 - w0).Normalized;

                // Back-face Culling: se a normal apontar para longe da câmera, descarta
                if (Vec3.Dot(faceNormal, (w0 - eye).Normalized) > 0)
                    continue;

                // Projeta vértices para tela (Clip Space -> NDC -> Screen)
                Vec4 clip0 = matMVP.Transform(new Vec4(v0.X, v0.Y, v0.Z, 1.0));
                Vec4 clip1 = matMVP.Transform(new Vec4(v1.X, v1.Y, v1.Z, 1.0));
                Vec4 clip2 = matMVP.Transform(new Vec4(v2.X, v2.Y, v2.Z, 1.0));

                if (clip0.W <= 0.01 || clip1.W <= 0.01 || clip2.W <= 0.01)
                    continue; // Frustum clipping próximo

                Vec3 ndc0 = clip0.ToVec3();
                Vec3 ndc1 = clip1.ToVec3();
                Vec3 ndc2 = clip2.ToVec3();

                // Mapeamento para coordenadas de tela
                double sx0 = (ndc0.X + 1.0) * 0.5 * width;
                double sy0 = (1.0 - ndc0.Y) * 0.5 * height;
                double sx1 = (ndc1.X + 1.0) * 0.5 * width;
                double sy1 = (1.0 - ndc1.Y) * 0.5 * height;
                double sx2 = (ndc2.X + 1.0) * 0.5 * width;
                double sy2 = (1.0 - ndc2.Y) * 0.5 * height;

                if (wireframe)
                {
                    // Renderização em Wireframe (Arestas com Bresenham)
                    Color wireColor = Color.FromRgb(100, 220, 255);
                    DrawLineFast(dst, (int)sx0, (int)sy0, (int)sx1, (int)sy1, wireColor);
                    DrawLineFast(dst, (int)sx1, (int)sy1, (int)sx2, (int)sy2, wireColor);
                    DrawLineFast(dst, (int)sx2, (int)sy2, (int)sx0, (int)sy0, wireColor);
                }
                else
                {
                    // Iluminação Blinn-Phong por Triângulo
                    double ambient = 0.2;
                    double diffuse = Math.Max(0.0, Vec3.Dot(faceNormal, lightDir));

                    Vec3 halfVec = (lightDir + viewDir).Normalized;
                    double specular = Math.Pow(Math.Max(0.0, Vec3.Dot(faceNormal, halfVec)), 32.0) * 0.6;

                    byte r = (byte)Math.Clamp(mesh.BaseColor.R * (ambient + diffuse * 0.7) + specular * 255.0, 0, 255);
                    byte g = (byte)Math.Clamp(mesh.BaseColor.G * (ambient + diffuse * 0.7) + specular * 255.0, 0, 255);
                    byte b = (byte)Math.Clamp(mesh.BaseColor.B * (ambient + diffuse * 0.7) + specular * 255.0, 0, 255);
                    uint colorBgra = (uint)((255 << 24) | (r << 16) | (g << 8) | b);

                    // Rasterização de Triângulo por Coordenadas Baricêntricas
                    RasterizeTriangle(dst, zBuffer, width, height, sx0, sy0, clip0.W, sx1, sy1, clip1.W, sx2, sy2, clip2.W, colorBgra);
                }
            }

            dst.Unlock(true);
            return dst;
        }

        private static void RasterizeTriangle(
            DirectBitmap dst, float[] zBuffer, int width, int height,
            double x0, double y0, double z0,
            double x1, double y1, double z1,
            double x2, double y2, double z2,
            uint colorBgra)
        {
            // Bounding box do triângulo 2D
            int minX = (int)Math.Max(0, Math.Min(x0, Math.Min(x1, x2)));
            int maxX = (int)Math.Min(width - 1, Math.Max(x0, Math.Max(x1, x2)));
            int minY = (int)Math.Max(0, Math.Min(y0, Math.Min(y1, y2)));
            int maxY = (int)Math.Min(height - 1, Math.Max(y0, Math.Max(y1, y2)));

            double area = EdgeFunction(x0, y0, x1, y1, x2, y2);
            if (Math.Abs(area) < 1e-6) return; // Triângulo degenerado

            double invArea = 1.0 / area;

            unsafe
            {
                byte* buf = dst.BackBuffer;
                int stride = dst.Stride;

                for (int y = minY; y <= maxY; y++)
                {
                    uint* row = (uint*)(buf + (y * stride));

                    for (int x = minX; x <= maxX; x++)
                    {
                        // Coordenadas Baricêntricas (w0, w1, w2)
                        double w0 = EdgeFunction(x1, y1, x2, y2, x + 0.5, y + 0.5) * invArea;
                        double w1 = EdgeFunction(x2, y2, x0, y0, x + 0.5, y + 0.5) * invArea;
                        double w2 = EdgeFunction(x0, y0, x1, y1, x + 0.5, y + 0.5) * invArea;

                        // Se o ponto está dentro do triângulo (todas baricêntricas >= 0)
                        if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                        {
                            // Interpolação de profundidade Z
                            float z = (float)(w0 * z0 + w1 * z1 + w2 * z2);
                            int zIdx = y * width + x;

                            // Teste de Z-Buffer
                            if (z < zBuffer[zIdx])
                            {
                                zBuffer[zIdx] = z;
                                row[x] = colorBgra;
                            }
                        }
                    }
                }
            }
        }

        private static double EdgeFunction(double ax, double ay, double bx, double by, double cx, double cy)
        {
            return (cx - ax) * (by - ay) - (cy - ay) * (bx - ax);
        }

        private static void DrawLineFast(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color color)
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
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }
    }
}
