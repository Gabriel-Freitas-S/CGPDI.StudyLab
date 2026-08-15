using System;

namespace CGPDI.StudyLab.Graphics3D
{
    /// <summary>
    /// Vetor 3D com operações completas de Álgebra Linear para Computação Gráfica.
    /// </summary>
    public struct Vec3
    {
        public double X, Y, Z;

        public Vec3(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        public static Vec3 Zero => new Vec3(0, 0, 0);
        public static Vec3 One => new Vec3(1, 1, 1);
        public static Vec3 Up => new Vec3(0, 1, 0);
        public static Vec3 Forward => new Vec3(0, 0, 1);
        public static Vec3 Right => new Vec3(1, 0, 0);

        public double LengthSquared => X * X + Y * Y + Z * Z;
        public double Length => Math.Sqrt(LengthSquared);

        public Vec3 Normalized
        {
            get
            {
                double len = Length;
                return len > 1e-8 ? new Vec3(X / len, Y / len, Z / len) : Zero;
            }
        }

        public static Vec3 operator +(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3 operator -(Vec3 a, Vec3 b) => new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vec3 operator -(Vec3 a) => new Vec3(-a.X, -a.Y, -a.Z);
        public static Vec3 operator *(Vec3 a, double s) => new Vec3(a.X * s, a.Y * s, a.Z * s);
        public static Vec3 operator *(double s, Vec3 a) => new Vec3(a.X * s, a.Y * s, a.Z * s);
        public static Vec3 operator *(Vec3 a, Vec3 b) => new Vec3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vec3 operator /(Vec3 a, double s) => new Vec3(a.X / s, a.Y / s, a.Z / s);

        /// <summary> Produto Escalar (Dot Product): a . b = |a||b| cos(theta) </summary>
        public static double Dot(Vec3 a, Vec3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        /// <summary> Produto Vetorial (Cross Product): a x b (Vetor ortogonal a ambos) </summary>
        public static Vec3 Cross(Vec3 a, Vec3 b) => new Vec3(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );

        /// <summary> Reflete um vetor incidente 'i' em relação à normal da superfície 'n'. </summary>
        public static Vec3 Reflect(Vec3 i, Vec3 n) => i - 2.0 * Dot(i, n) * n;

        /// <summary> Refrata um vetor incidente 'i' através da Lei de Snell (Índice de refração eta). </summary>
        public static Vec3? Refract(Vec3 i, Vec3 n, double eta)
        {
            double cosi = -Math.Max(-1.0, Math.Min(1.0, Dot(i, n)));
            double etai = 1, etat = eta;
            Vec3 nNorm = n;
            if (cosi < 0)
            {
                cosi = -cosi;
                (etai, etat) = (etat, etai);
                nNorm = -n;
            }
            double etaRatio = etai / etat;
            double k = 1.0 - etaRatio * etaRatio * (1.0 - cosi * cosi);
            if (k < 0) return null; // Reflexão Interna Total (TIR)
            return etaRatio * i + (etaRatio * cosi - Math.Sqrt(k)) * nNorm;
        }

        public static Vec3 Lerp(Vec3 a, Vec3 b, double t) => a + (b - a) * t;
    }

    /// <summary>
    /// Vetor Homogêneo 4D (x, y, z, w).
    /// </summary>
    public struct Vec4
    {
        public double X, Y, Z, W;

        public Vec4(double x, double y, double z, double w = 1.0)
        {
            X = x; Y = y; Z = z; W = w;
        }

        public Vec3 ToVec3() => Math.Abs(W) > 1e-7 ? new Vec3(X / W, Y / W, Z / W) : new Vec3(X, Y, Z);
    }

    /// <summary>
    /// Matriz 4x4 de Coordenadas Homogêneas para Transformações 3D (Model, View, Projection).
    /// </summary>
    public struct Mat4x4
    {
        public double M00, M01, M02, M03;
        public double M10, M11, M12, M13;
        public double M20, M21, M22, M23;
        public double M30, M31, M32, M33;

        public static Mat4x4 Identity => new Mat4x4
        {
            M00 = 1, M11 = 1, M22 = 1, M33 = 1
        };

        public static Mat4x4 CreateTranslation(double tx, double ty, double tz)
        {
            Mat4x4 m = Identity;
            m.M03 = tx;
            m.M13 = ty;
            m.M23 = tz;
            return m;
        }

        public static Mat4x4 CreateScale(double sx, double sy, double sz)
        {
            Mat4x4 m = Identity;
            m.M00 = sx;
            m.M11 = sy;
            m.M22 = sz;
            return m;
        }

        public static Mat4x4 CreateRotationX(double rad)
        {
            Mat4x4 m = Identity;
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            m.M11 = c;  m.M12 = -s;
            m.M21 = s;  m.M22 = c;
            return m;
        }

        public static Mat4x4 CreateRotationY(double rad)
        {
            Mat4x4 m = Identity;
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            m.M00 = c;   m.M02 = s;
            m.M20 = -s;  m.M22 = c;
            return m;
        }

        public static Mat4x4 CreateRotationZ(double rad)
        {
            Mat4x4 m = Identity;
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            m.M00 = c;  m.M01 = -s;
            m.M10 = s;  m.M11 = c;
            return m;
        }

        /// <summary>
        /// Matriz de Visualização da Câmera (View Matrix / LookAt):
        /// Converte coordenadas de Mundo (World Space) para o Espaço de Visão da Câmera (Camera/Eye Space).
        /// </summary>
        public static Mat4x4 CreateLookAt(Vec3 eye, Vec3 target, Vec3 up)
        {
            Vec3 zAxis = (target - eye).Normalized; // Forward
            Vec3 xAxis = Vec3.Cross(up, zAxis).Normalized; // Right
            Vec3 yAxis = Vec3.Cross(zAxis, xAxis); // Up

            Mat4x4 m = Identity;
            m.M00 = xAxis.X; m.M01 = xAxis.Y; m.M02 = xAxis.Z; m.M03 = -Vec3.Dot(xAxis, eye);
            m.M10 = yAxis.X; m.M11 = yAxis.Y; m.M12 = yAxis.Z; m.M13 = -Vec3.Dot(yAxis, eye);
            m.M20 = zAxis.X; m.M21 = zAxis.Y; m.M22 = zAxis.Z; m.M23 = -Vec3.Dot(zAxis, eye);
            return m;
        }

        /// <summary>
        /// Matriz de Projeção Perspectiva (Perspective Projection Matrix):
        /// Modela o cone de visão (Frustum) da câmera, projetando a cena 3D para o plano 2D da tela.
        /// Aplica distorção de perspectiva onde objetos distantes tornam-se menores proporcionalmente a 1/Z.
        /// </summary>
        public static Mat4x4 CreatePerspective(double fovRadians, double aspect, double near, double far)
        {
            double tanHalfFov = Math.Tan(fovRadians / 2.0);
            Mat4x4 m = new Mat4x4();
            m.M00 = 1.0 / (aspect * tanHalfFov);
            m.M11 = 1.0 / tanHalfFov;
            m.M22 = far / (far - near);
            m.M23 = (-far * near) / (far - near);
            m.M32 = 1.0;
            m.M33 = 0.0;
            return m;
        }

        public static Mat4x4 operator *(Mat4x4 a, Mat4x4 b)
        {
            Mat4x4 r = new Mat4x4();
            r.M00 = a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20 + a.M03 * b.M30;
            r.M01 = a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21 + a.M03 * b.M31;
            r.M02 = a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22 + a.M03 * b.M32;
            r.M03 = a.M00 * b.M03 + a.M01 * b.M13 + a.M02 * b.M23 + a.M03 * b.M33;

            r.M10 = a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20 + a.M13 * b.M30;
            r.M11 = a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            r.M12 = a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
            r.M13 = a.M10 * b.M03 + a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;

            r.M20 = a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20 + a.M23 * b.M30;
            r.M21 = a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            r.M22 = a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
            r.M23 = a.M20 * b.M03 + a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;

            r.M30 = a.M30 * b.M00 + a.M31 * b.M10 + a.M32 * b.M20 + a.M33 * b.M30;
            r.M31 = a.M30 * b.M01 + a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            r.M32 = a.M30 * b.M02 + a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
            r.M33 = a.M30 * b.M03 + a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;
            return r;
        }

        public Vec4 Transform(Vec4 v)
        {
            return new Vec4(
                M00 * v.X + M01 * v.Y + M02 * v.Z + M03 * v.W,
                M10 * v.X + M11 * v.Y + M12 * v.Z + M13 * v.W,
                M20 * v.X + M21 * v.Y + M22 * v.Z + M23 * v.W,
                M30 * v.X + M31 * v.Y + M32 * v.Z + M33 * v.W
            );
        }

        public Vec3 TransformPoint(Vec3 p) => Transform(new Vec4(p.X, p.Y, p.Z, 1.0)).ToVec3();

        public Vec3 TransformDirection(Vec3 d)
        {
            return new Vec3(
                M00 * d.X + M01 * d.Y + M02 * d.Z,
                M10 * d.X + M11 * d.Y + M12 * d.Z,
                M20 * d.X + M21 * d.Y + M22 * d.Z
            );
        }
    }

    /// <summary>
    /// Raio 3D para Ray Tracing e Ray Casting: r(t) = O + t * D.
    /// </summary>
    public struct Ray3D
    {
        public Vec3 Origin;
        public Vec3 Direction;

        public Ray3D(Vec3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction.Normalized;
        }

        public Vec3 PointAt(double t) => Origin + Direction * t;
    }
}
