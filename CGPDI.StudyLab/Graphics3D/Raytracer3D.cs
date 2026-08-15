using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.Graphics3D
{
    public class MaterialRay
    {
        public Vec3 Color { get; set; } = new Vec3(1, 1, 1);
        public double Ambient { get; set; } = 0.1;
        public double Diffuse { get; set; } = 0.7;
        public double Specular { get; set; } = 0.3;
        public double Shininess { get; set; } = 32.0;
        public double Reflectivity { get; set; } = 0.0;
        public double Transparency { get; set; } = 0.0;
        public double RefractiveIndex { get; set; } = 1.5; // Vidro = 1.5, Água = 1.33
        public bool IsCheckerboard { get; set; } = false;
    }

    public abstract class SceneObject
    {
        public MaterialRay Material { get; set; } = new MaterialRay();
        public abstract bool Intersect(Ray3D ray, out double t, out Vec3 normal);
    }

    public class SphereObject : SceneObject
    {
        public Vec3 Center { get; set; }
        public double Radius { get; set; }

        public SphereObject(Vec3 center, double radius, MaterialRay material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        /// <summary>
        /// Interseção Raio-Esfera analítica: |O + t*D - C|^2 = R^2
        /// Equação quadrática at^2 + bt + c = 0 onde a = D.D = 1, b = 2*D.(O - C), c = (O - C).(O - C) - R^2
        /// </summary>
        public override bool Intersect(Ray3D ray, out double t, out Vec3 normal)
        {
            t = 0;
            normal = Vec3.Zero;

            Vec3 oc = ray.Origin - Center;
            double b = Vec3.Dot(oc, ray.Direction);
            double c = Vec3.Dot(oc, oc) - Radius * Radius;
            double discriminant = b * b - c;

            if (discriminant < 0) return false; // Raio não atinge a esfera

            double sqrtDisc = Math.Sqrt(discriminant);
            double t0 = -b - sqrtDisc;
            double t1 = -b + sqrtDisc;

            if (t0 > 1e-4)
                t = t0;
            else if (t1 > 1e-4)
                t = t1;
            else
                return false;

            Vec3 hitPoint = ray.PointAt(t);
            normal = (hitPoint - Center).Normalized;
            return true;
        }
    }

    public class PlaneObject : SceneObject
    {
        public Vec3 Point { get; set; }
        public Vec3 Normal { get; set; }

        public PlaneObject(Vec3 point, Vec3 normal, MaterialRay material)
        {
            Point = point;
            Normal = normal.Normalized;
            Material = material;
        }

        /// <summary>
        /// Interseção Raio-Plano: t = (P0 - O) . N / (D . N)
        /// </summary>
        public override bool Intersect(Ray3D ray, out double t, out Vec3 normal)
        {
            t = 0;
            normal = Normal;

            double denom = Vec3.Dot(Normal, ray.Direction);
            if (Math.Abs(denom) > 1e-6)
            {
                double tTest = Vec3.Dot(Point - ray.Origin, Normal) / denom;
                if (tTest > 1e-4)
                {
                    t = tTest;
                    return true;
                }
            }
            return false;
        }
    }

    public class PointLight
    {
        public Vec3 Position { get; set; }
        public Vec3 Color { get; set; } = new Vec3(1, 1, 1);
        public double Intensity { get; set; } = 1.0;

        public PointLight(Vec3 pos, Vec3 color, double intensity = 1.0)
        {
            Position = pos;
            Color = color;
            Intensity = intensity;
        }
    }

    /// <summary>
    /// Renderizador por Traçado de Raios (Whitted-style Ray Tracer).
    /// Simula a física do transporte de luz traçando raios da câmera para a cena:
    /// - Raios primários (Eye rays)
    /// - Raios de sombra (Shadow rays) com oclusão e penumbra
    /// - Raios secundários de reflexão especular perfeita (Recursive bounce)
    /// - Raios de refração dielétrica (Lei de Snell e Coeficientes de Fresnel)
    /// </summary>
    public static class Raytracer3D
    {
        public static DirectBitmap Render(
            int width = 512,
            int height = 512,
            double cameraAngle = 0.0,
            int maxDepth = 3)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            // Configuração da Cena com Esferas e Plano Xadrez
            List<SceneObject> objects = new List<SceneObject>();

            // Chão Xadrez
            objects.Add(new PlaneObject(
                new Vec3(0, -1.0, 0),
                new Vec3(0, 1, 0),
                new MaterialRay
                {
                    Color = new Vec3(0.9, 0.9, 0.9),
                    Ambient = 0.1,
                    Diffuse = 0.7,
                    Reflectivity = 0.25,
                    IsCheckerboard = true
                }
            ));

            // Esfera 1: Metálica Espelhada Cromada (Reflexão pura)
            objects.Add(new SphereObject(
                new Vec3(-1.4, 0.0, 3.0),
                1.0,
                new MaterialRay
                {
                    Color = new Vec3(0.95, 0.95, 0.95),
                    Ambient = 0.05,
                    Diffuse = 0.1,
                    Specular = 0.9,
                    Shininess = 128.0,
                    Reflectivity = 0.85
                }
            ));

            // Esfera 2: Dielétrica de Vidro Translúcido (Refração + Reflexão Fresnel)
            objects.Add(new SphereObject(
                new Vec3(0.0, -0.2, 2.0),
                0.8,
                new MaterialRay
                {
                    Color = new Vec3(0.2, 0.7, 1.0),
                    Ambient = 0.05,
                    Diffuse = 0.2,
                    Specular = 0.8,
                    Shininess = 64.0,
                    Reflectivity = 0.3,
                    Transparency = 0.7,
                    RefractiveIndex = 1.45
                }
            ));

            // Esfera 3: Vermelha Rubi Brilhante
            objects.Add(new SphereObject(
                new Vec3(1.3, -0.3, 2.4),
                0.7,
                new MaterialRay
                {
                    Color = new Vec3(0.9, 0.15, 0.2),
                    Ambient = 0.15,
                    Diffuse = 0.7,
                    Specular = 0.6,
                    Shininess = 48.0,
                    Reflectivity = 0.2
                }
            ));

            // Luzes pontuais
            List<PointLight> lights = new List<PointLight>
            {
                new PointLight(new Vec3(-2.0, 4.0, 0.0), new Vec3(1.0, 0.95, 0.9), 1.2),
                new PointLight(new Vec3(3.0, 3.0, 1.0), new Vec3(0.4, 0.6, 1.0), 0.6) // Luz de preenchimento azulada
            };

            // Câmera
            double camX = Math.Sin(cameraAngle) * 4.0;
            double camZ = -Math.Cos(cameraAngle) * 4.0 + 2.0;
            Vec3 camPos = new Vec3(camX, 1.2, camZ);
            Vec3 camTarget = new Vec3(0, 0, 2.2);

            Mat4x4 camView = Mat4x4.CreateLookAt(camPos, camTarget, Vec3.Up);
            double aspect = (double)width / height;
            double fov = 60.0 * Math.PI / 180.0;
            double tanHalfFov = Math.Tan(fov / 2.0);

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * bmp.Stride));

                    for (int x = 0; x < width; x++)
                    {
                        // NDC normalizado [-1, 1]
                        double px = (2.0 * (x + 0.5) / width - 1.0) * aspect * tanHalfFov;
                        double py = (1.0 - 2.0 * (y + 0.5) / height) * tanHalfFov;

                        Vec3 rayDirLocal = new Vec3(px, py, 1.0).Normalized;
                        // Transforma direção do raio pelo inverso da matriz de visualização
                        Vec3 rayDir = (camView.M00 * rayDirLocal.X + camView.M10 * rayDirLocal.Y + camView.M20 * rayDirLocal.Z) * Vec3.Right +
                                      (camView.M01 * rayDirLocal.X + camView.M11 * rayDirLocal.Y + camView.M21 * rayDirLocal.Z) * Vec3.Up +
                                      (camView.M02 * rayDirLocal.X + camView.M12 * rayDirLocal.Y + camView.M22 * rayDirLocal.Z) * Vec3.Forward;
                        rayDir = rayDir.Normalized;

                        Ray3D primaryRay = new Ray3D(camPos, rayDir);
                        Vec3 color = TraceRay(primaryRay, objects, lights, 0, maxDepth);

                        // Tone mapping simples e Gamma Correction sRGB (2.2)
                        byte r = (byte)Math.Clamp(Math.Pow(color.X, 1.0 / 2.2) * 255.0, 0, 255);
                        byte g = (byte)Math.Clamp(Math.Pow(color.Y, 1.0 / 2.2) * 255.0, 0, 255);
                        byte b = (byte)Math.Clamp(Math.Pow(color.Z, 1.0 / 2.2) * 255.0, 0, 255);

                        row[x] = (uint)((255 << 24) | (r << 16) | (g << 8) | b);
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        private static Vec3 TraceRay(Ray3D ray, List<SceneObject> objects, List<PointLight> lights, int depth, int maxDepth)
        {
            if (depth > maxDepth)
                return Vec3.Zero;

            double closestT = double.MaxValue;
            SceneObject? hitObject = null;
            Vec3 hitNormal = Vec3.Zero;

            foreach (var obj in objects)
            {
                if (obj.Intersect(ray, out double t, out Vec3 normal))
                {
                    if (t < closestT)
                    {
                        closestT = t;
                        hitObject = obj;
                        hitNormal = normal;
                    }
                }
            }

            // Se não atingiu nenhum objeto: Cor do Céu / Gradiente de Fundo
            if (hitObject == null)
            {
                double tSky = 0.5 * (ray.Direction.Y + 1.0);
                return Vec3.Lerp(new Vec3(0.05, 0.05, 0.08), new Vec3(0.2, 0.4, 0.7), tSky);
            }

            Vec3 hitPoint = ray.PointAt(closestT);
            MaterialRay mat = hitObject.Material;

            // Textura Xadrez procedural para planos
            Vec3 baseColor = mat.Color;
            if (mat.IsCheckerboard)
            {
                int cx = (int)Math.Floor(hitPoint.X * 1.5);
                int cz = (int)Math.Floor(hitPoint.Z * 1.5);
                bool isEven = (cx + cz) % 2 == 0;
                baseColor = isEven ? new Vec3(0.85, 0.85, 0.85) : new Vec3(0.15, 0.15, 0.18);
            }

            Vec3 finalColor = baseColor * mat.Ambient;
            Vec3 viewDir = -ray.Direction;

            // Loop de Iluminação Direta e Sombras
            foreach (var light in lights)
            {
                Vec3 lightDir = (light.Position - hitPoint).Normalized;
                double distToLight = (light.Position - hitPoint).Length;

                // Raio de Sombra (Shadow Ray)
                Ray3D shadowRay = new Ray3D(hitPoint + hitNormal * 1e-4, lightDir);
                bool inShadow = false;

                foreach (var obj in objects)
                {
                    if (obj.Intersect(shadowRay, out double tShadow, out _) && tShadow < distToLight)
                    {
                        inShadow = true;
                        break;
                    }
                }

                if (!inShadow)
                {
                    // Difuso Lambertiano
                    double nDotL = Math.Max(0.0, Vec3.Dot(hitNormal, lightDir));
                    Vec3 diffuse = baseColor * mat.Diffuse * nDotL * light.Color * light.Intensity;

                    // Especular Blinn-Phong
                    Vec3 halfVector = (lightDir + viewDir).Normalized;
                    double nDotH = Math.Max(0.0, Vec3.Dot(hitNormal, halfVector));
                    double spec = Math.Pow(nDotH, mat.Shininess) * mat.Specular * light.Intensity;
                    Vec3 specular = light.Color * spec;

                    finalColor += diffuse + specular;
                }
            }

            // Reflexão Especular Recursiva
            if (mat.Reflectivity > 0)
            {
                Vec3 reflectDir = Vec3.Reflect(ray.Direction, hitNormal).Normalized;
                Ray3D reflectRay = new Ray3D(hitPoint + hitNormal * 1e-4, reflectDir);
                Vec3 reflectColor = TraceRay(reflectRay, objects, lights, depth + 1, maxDepth);
                finalColor = Vec3.Lerp(finalColor, reflectColor, mat.Reflectivity);
            }

            // Refração Dielétrica Translúcida (Vidro)
            if (mat.Transparency > 0)
            {
                Vec3? refractDir = Vec3.Refract(ray.Direction, hitNormal, mat.RefractiveIndex);
                if (refractDir.HasValue)
                {
                    Ray3D refractRay = new Ray3D(hitPoint - hitNormal * 1e-4, refractDir.Value.Normalized);
                    Vec3 refractColor = TraceRay(refractRay, objects, lights, depth + 1, maxDepth);
                    finalColor = Vec3.Lerp(finalColor, refractColor, mat.Transparency);
                }
            }

            return finalColor;
        }
    }
}
