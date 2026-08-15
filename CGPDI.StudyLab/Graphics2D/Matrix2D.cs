using System;
using System.Windows;

namespace CGPDI.StudyLab.Graphics2D
{
    /// <summary>
    /// Matriz 3x3 de Coordenadas Homogêneas para Transformações Geométricas 2D.
    /// 
    /// TEORIA DAS COORDENADAS HOMOGÊNEAS 2D:
    /// Em 2D cartesiano, a translação é uma operação aditiva: p' = p + t.
    /// Já a rotação e escala são multiplicativas: p' = M * p.
    /// Para unificar todas as transformações afins em uma única operação de MULTIPLICAÇÃO MATRICIAL,
    /// adiciona-se uma 3ª dimensão homogênea w=1: [x, y, 1]^T.
    /// 
    /// [ x' ]   [ m00  m01  m02 ] [ x ]
    /// [ y' ] = [ m10  m11  m12 ] [ y ]
    /// [ 1  ]   [  0    0    1  ] [ 1 ]
    /// </summary>
    public struct Matrix3x3
    {
        public double M00, M01, M02; // Linha 0 (ex: Escala X, Shear X, Translação X)
        public double M10, M11, M12; // Linha 1 (ex: Shear Y, Escala Y, Translação Y)
        public double M20, M21, M22; // Linha 2 (Sempre [0, 0, 1] em afim 2D)

        public static Matrix3x3 Identity => new Matrix3x3(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );

        public Matrix3x3(
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20 = 0, double m21 = 0, double m22 = 1)
        {
            M00 = m00; M01 = m01; M02 = m02;
            M10 = m10; M11 = m11; M12 = m12;
            M20 = m20; M21 = m21; M22 = m22;
        }

        /// <summary> Cria matriz de translação por vetor (tx, ty). </summary>
        public static Matrix3x3 CreateTranslation(double tx, double ty)
        {
            return new Matrix3x3(
                1, 0, tx,
                0, 1, ty,
                0, 0, 1
            );
        }

        /// <summary> Cria matriz de rotação por ângulo em radianos. </summary>
        public static Matrix3x3 CreateRotation(double angleRad)
        {
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);
            return new Matrix3x3(
                cos, -sin, 0,
                sin,  cos, 0,
                0,    0,   1
            );
        }

        /// <summary> Cria matriz de escala por fatores (sx, sy). </summary>
        public static Matrix3x3 CreateScale(double sx, double sy)
        {
            return new Matrix3x3(
                sx, 0,  0,
                0,  sy, 0,
                0,  0,  1
            );
        }

        /// <summary> Cria matriz de cisalhamento (shear) (shX, shY). </summary>
        public static Matrix3x3 CreateShear(double shX, double shY)
        {
            return new Matrix3x3(
                1,   shX, 0,
                shY, 1,   0,
                0,   0,   1
            );
        }

        /// <summary> Multiplicação matricial C = A * B (Composição de Transformações). </summary>
        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
        {
            return new Matrix3x3(
                a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20,
                a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21,
                a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22,

                a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20,
                a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21,
                a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22,

                a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20,
                a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21,
                a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22
            );
        }

        /// <summary> Transforma um ponto 2D pelo vetor homogêneo [x, y, 1]^T. </summary>
        public Point TransformPoint(Point p)
        {
            double x = M00 * p.X + M01 * p.Y + M02;
            double y = M10 * p.X + M11 * p.Y + M12;
            double w = M20 * p.X + M21 * p.Y + M22;

            if (Math.Abs(w - 1.0) > 1e-7 && Math.Abs(w) > 1e-7)
            {
                x /= w;
                y /= w;
            }

            return new Point(x, y);
        }
    }
}
