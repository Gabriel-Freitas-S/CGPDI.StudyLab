# 🌟 Capítulo 6: Ray Tracing & Renderização Realística

Implementado em [`Graphics3D/Raytracer3D.cs`](file:///D:/source/repos/teste/teste/Graphics3D/Raytracer3D.cs).

---

## 1. O Modelo Óptico de Whitted (1980)

Enquanto a rasterização convencional projeta triângulos para a tela (*Forward Rendering*), o **Ray Tracing** simula o caminho inverso da luz: raios primários são disparados a partir do foco da câmera através de cada pixel da tela em direção à cena 3D.

```
       [ Câmera ]
           │
           │  (Raio Primário)
           ▼
    [ Pixel da Tela ]
           │
           ▼
   [ Superfície do Objeto ]
     ├──> [ Raio de Sombra (Shadow Ray) ] ──> [ Fonte de Luz ]
     ├──> [ Raio de Reflexão Especular (Bounce) ] ──> [ Outro Objeto ]
     └──> [ Raio de Refração (Lei de Snell) ] ──> [ Interior do Objeto ]
```

---

## 2. Equações Matemáticas Fundamentais

### 2.1 Equação Paramétrica do Raio
$$\vec{r}(t) = \vec{O} + t \cdot \vec{D}, \quad t > 0$$
Onde $\vec{O}$ é a origem do raio e $\vec{D}$ é o vetor diretor normalizado ($|\vec{D}| = 1$).

### 2.2 Interseção Analítica Raio-Esfera
Uma esfera com centro $\vec{C}$ e raio $R$ é definida por:
$$|\vec{P} - \vec{C}|^2 = R^2$$
Substituindo a equação do raio $\vec{P} = \vec{O} + t \vec{D}$:
$$(\vec{D} \cdot \vec{D}) t^2 + 2 \vec{D} \cdot (\vec{O} - \vec{C}) t + |\vec{O} - \vec{C}|^2 - R^2 = 0$$
Sendo uma equação quadrática $a t^2 + b t + c = 0$:
$$a = 1, \quad b = 2 \vec{D} \cdot (\vec{O} - \vec{C}), \quad c = |\vec{O} - \vec{C}|^2 - R^2$$
$$\Delta = b^2 - 4ac$$
* Se $\Delta < 0$: Não há interseção.
* Se $\Delta \ge 0$: A menor raiz positiva $t = \frac{-b - \sqrt{\Delta}}{2}$ determina o ponto de impacto mais próximo.

### 2.3 Vetor de Reflexão Especular
Dado o vetor de incidência $\vec{D}$ e a normal da superfície $\vec{N}$:
$$\vec{R} = \vec{D} - 2 (\vec{D} \cdot \vec{N}) \vec{N}$$

### 2.4 Lei de Refração de Snell e Equação de Fresnel
$$n_1 \sin\theta_1 = n_2 \sin\theta_2$$
Aproximação de Schlick para o coeficiente de reflexão em dielétricos (vidros/água):
$$R_0 = \left( \frac{n_1 - n_2}{n_1 + n_2} \right)^2$$
$$R(\theta) = R_0 + (1 - R_0)(1 - \cos\theta)^5$$

---

## 3. Algoritmo Recursivo em C#

```csharp
private static Vec3 TraceRay(Ray3D ray, Scene3D scene, int depth, int maxDepth)
{
    if (depth > maxDepth || !scene.Intersect(ray, out HitRecord hit))
        return scene.BackgroundColor;

    Vec3 color = scene.AmbientLight * hit.Material.Color;

    // 1. Sombras e Iluminação Direta
    foreach (var light in scene.Lights)
    {
        Vec3 lightDir = (light.Position - hit.Point).Normalized();
        Ray3D shadowRay = new Ray3D(hit.Point + hit.Normal * 1e-4, lightDir);

        if (!scene.IntersectShadow(shadowRay, (light.Position - hit.Point).Length()))
        {
            // Iluminação Difusa Lambertiana
            double diff = Math.Max(0.0, Vec3.Dot(hit.Normal, lightDir));
            color += hit.Material.Color * light.Color * diff * hit.Material.Diffuse;

            // Especular de Blinn-Phong
            Vec3 halfVec = (lightDir - ray.Direction).Normalized();
            double spec = Math.Pow(Math.Max(0.0, Vec3.Dot(hit.Normal, halfVec)), hit.Material.Shininess);
            color += light.Color * spec * hit.Material.Specular;
        }
    }

    // 2. Reflexão Recursiva (Espelhos / Metais)
    if (hit.Material.Reflectivity > 0)
    {
        Vec3 reflectDir = Vec3.Reflect(ray.Direction, hit.Normal);
        Ray3D reflectRay = new Ray3D(hit.Point + hit.Normal * 1e-4, reflectDir);
        Vec3 reflectColor = TraceRay(reflectRay, scene, depth + 1, maxDepth);
        color = Vec3.Lerp(color, reflectColor, hit.Material.Reflectivity);
    }

    return color;
}
```
