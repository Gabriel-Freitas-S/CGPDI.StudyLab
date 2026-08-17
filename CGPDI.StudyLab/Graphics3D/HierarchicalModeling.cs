using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CGPDI.StudyLab.Graphics3D
{
    /// <summary>
    /// Nó da Árvore de Modelagem Hierárquica (Scene Graph / Grafo de Cena).
    /// 
    /// TEORIA DA MODELAGEM HIERÁRQUICA & GRAFOS DE CENA:
    /// Em sistemas gráficos complexos, objetos são compostos por partes interconectadas (ex: robôs, veículos, esqueletos).
    /// Cada nó filho herda e acumula as transformações geométricas do seu nó pai:
    /// M_{global, filho} = M_{global, pai} \times M_{local, filho}
    /// 
    /// VANTAGENS:
    /// 1. Design Top-Down e Construção Bottom-Up
    /// 2. Reusabilidade de primitivas geométricas
    /// 3. Cinemática Direta (Forward Kinematics): Ao mover o ombro, o braço, antebraço e mão se movem juntos automaticamente.
    /// </summary>
    public class SceneNode3D
    {
        public string Name { get; set; }
        public Model3DGroup ModelGroup { get; } = new Model3DGroup();
        public Transform3DGroup TransformGroup { get; } = new Transform3DGroup();
        public List<SceneNode3D> Children { get; } = new List<SceneNode3D>();

        public SceneNode3D(string name)
        {
            Name = name;
            ModelGroup.Transform = TransformGroup;
        }

        public void AddChild(SceneNode3D child)
        {
            Children.Add(child);
            ModelGroup.Children.Add(child.ModelGroup);
        }

        public void AddGeometry(GeometryModel3D model)
        {
            ModelGroup.Children.Add(model);
        }
    }

    /// <summary>
    /// Gerador de Modelos Articulados Hierárquicos (Braço Robótico e Sistema Planetário).
    /// </summary>
    public class HierarchicalRobotArm
    {
        public SceneNode3D RootNode { get; }

        // Rotações das articulações (Cinemática Direta)
        private readonly RotateTransform3D _baseRotation;
        private readonly RotateTransform3D _shoulderRotation;
        private readonly RotateTransform3D _elbowRotation;
        private readonly RotateTransform3D _wristRotation;

        public HierarchicalRobotArm()
        {
            // 1. Nó Raiz: Centralizado verticalmente em y = -1.4 para que o robô fique exatamente no centro (0, 0, 0)
            RootNode = new SceneNode3D("Base_Rotativa");
            RootNode.TransformGroup.Children.Add(new TranslateTransform3D(0, -1.4, 0)); // Centralização do centro de massa

            _baseRotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0));
            RootNode.TransformGroup.Children.Add(_baseRotation);

            // Geometria da Base (Cilindro com tampas superior e inferior)
            GeometryModel3D baseGeom = CreateCylinderModel(0.9, 0.3, 24, Color.FromRgb(55, 60, 75));
            RootNode.AddGeometry(baseGeom);

            // 2. Nó Filho 1: Ombro / Braço Superior (Eixo Z)
            SceneNode3D shoulderNode = new SceneNode3D("Ombro_Braco");
            shoulderNode.TransformGroup.Children.Add(new TranslateTransform3D(0, 0.3, 0)); // Posiciona acima da base
            _shoulderRotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 25));
            shoulderNode.TransformGroup.Children.Add(_shoulderRotation);

            // Geometria do Braço Superior (Laranja Metálico)
            GeometryModel3D armGeom = CreateBoxModel(0.35, 1.3, 0.35, new Point3D(0, 0.65, 0), Color.FromRgb(235, 125, 35));
            shoulderNode.AddGeometry(armGeom);
            RootNode.AddChild(shoulderNode);

            // 3. Nó Filho 2: Cotovelo / Antebraço (Eixo Z)
            SceneNode3D elbowNode = new SceneNode3D("Cotovelo_Antebraco");
            elbowNode.TransformGroup.Children.Add(new TranslateTransform3D(0, 1.3, 0)); // Topo do braço
            _elbowRotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -45));
            elbowNode.TransformGroup.Children.Add(_elbowRotation);

            // Geometria do Antebraço (Azul Elétrico)
            GeometryModel3D forearmGeom = CreateBoxModel(0.28, 1.1, 0.28, new Point3D(0, 0.55, 0), Color.FromRgb(35, 145, 235));
            elbowNode.AddGeometry(forearmGeom);
            shoulderNode.AddChild(elbowNode);

            // 4. Nó Filho 3: Pulso / Garra Pinça (Eixo X)
            SceneNode3D wristNode = new SceneNode3D("Pulso_Garra");
            wristNode.TransformGroup.Children.Add(new TranslateTransform3D(0, 1.1, 0)); // Topo do antebraço
            _wristRotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 30));
            wristNode.TransformGroup.Children.Add(_wristRotation);

            // Geometria da Garra (Base dourada + Pinças rubi)
            GeometryModel3D clawBase = CreateBoxModel(0.45, 0.12, 0.18, new Point3D(0, 0.06, 0), Color.FromRgb(245, 205, 45));
            GeometryModel3D fingerLeft = CreateBoxModel(0.07, 0.32, 0.09, new Point3D(-0.16, 0.22, 0), Color.FromRgb(220, 50, 60));
            GeometryModel3D fingerRight = CreateBoxModel(0.07, 0.32, 0.09, new Point3D(0.16, 0.22, 0), Color.FromRgb(220, 50, 60));

            wristNode.AddGeometry(clawBase);
            wristNode.AddGeometry(fingerLeft);
            wristNode.AddGeometry(fingerRight);
            elbowNode.AddChild(wristNode);
        }

        public void SetJointAngles(double baseAngle, double shoulderAngle, double elbowAngle, double wristAngle)
        {
            ((AxisAngleRotation3D)_baseRotation.Rotation).Angle = baseAngle;
            ((AxisAngleRotation3D)_shoulderRotation.Rotation).Angle = shoulderAngle;
            ((AxisAngleRotation3D)_elbowRotation.Rotation).Angle = elbowAngle;
            ((AxisAngleRotation3D)_wristRotation.Rotation).Angle = wristAngle;
        }

        #region Construtores de Primitivas para Modelagem

        private static GeometryModel3D CreateBoxModel(double sx, double sy, double sz, Point3D center, Color color)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            double hx = sx / 2.0, hy = sy / 2.0, hz = sz / 2.0;
            double cx = center.X, cy = center.Y, cz = center.Z;

            Point3D[] p = new Point3D[]
            {
                new Point3D(cx - hx, cy - hy, cz - hz), new Point3D(cx + hx, cy - hy, cz - hz),
                new Point3D(cx + hx, cy + hy, cz - hz), new Point3D(cx - hx, cy + hy, cz - hz),
                new Point3D(cx - hx, cy - hy, cz + hz), new Point3D(cx + hx, cy - hy, cz + hz),
                new Point3D(cx + hx, cy + hy, cz + hz), new Point3D(cx - hx, cy + hy, cz + hz)
            };

            void AddFace(int i0, int i1, int i2, int i3)
            {
                int b = mesh.Positions.Count;
                mesh.Positions.Add(p[i0]); mesh.Positions.Add(p[i1]);
                mesh.Positions.Add(p[i2]); mesh.Positions.Add(p[i3]);
                mesh.TriangleIndices.Add(b + 0); mesh.TriangleIndices.Add(b + 2); mesh.TriangleIndices.Add(b + 1);
                mesh.TriangleIndices.Add(b + 0); mesh.TriangleIndices.Add(b + 3); mesh.TriangleIndices.Add(b + 2);
            }

            AddFace(0, 1, 2, 3); // Frente
            AddFace(5, 4, 7, 6); // Trás
            AddFace(4, 0, 3, 7); // Esquerda
            AddFace(1, 5, 6, 2); // Direita
            AddFace(3, 2, 6, 7); // Topo
            AddFace(4, 5, 1, 0); // Base

            MaterialGroup matGroup = new MaterialGroup();
            matGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(color)));
            matGroup.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 30));

            return new GeometryModel3D { Geometry = mesh, Material = matGroup, BackMaterial = matGroup };
        }

        private static GeometryModel3D CreateCylinderModel(double radius, double height, int segments, Color color)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Lateral do cilindro
            for (int i = 0; i <= segments; i++)
            {
                double angle = i * 2.0 * Math.PI / segments;
                double x = radius * Math.Cos(angle);
                double z = radius * Math.Sin(angle);

                mesh.Positions.Add(new Point3D(x, 0, z));
                mesh.Positions.Add(new Point3D(x, height, z));
                mesh.Normals.Add(new Vector3D(Math.Cos(angle), 0, Math.Sin(angle)));
                mesh.Normals.Add(new Vector3D(Math.Cos(angle), 0, Math.Sin(angle)));
            }

            for (int i = 0; i < segments; i++)
            {
                int b = i * 2;
                mesh.TriangleIndices.Add(b + 0);
                mesh.TriangleIndices.Add(b + 1);
                mesh.TriangleIndices.Add(b + 2);

                mesh.TriangleIndices.Add(b + 1);
                mesh.TriangleIndices.Add(b + 3);
                mesh.TriangleIndices.Add(b + 2);
            }

            // Tampa superior
            int topCenterIdx = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(0, height, 0));
            mesh.Normals.Add(new Vector3D(0, 1, 0));

            for (int i = 0; i < segments; i++)
            {
                double a1 = i * 2.0 * Math.PI / segments;
                double a2 = (i + 1) * 2.0 * Math.PI / segments;

                int p1 = mesh.Positions.Count;
                mesh.Positions.Add(new Point3D(radius * Math.Cos(a1), height, radius * Math.Sin(a1)));
                mesh.Positions.Add(new Point3D(radius * Math.Cos(a2), height, radius * Math.Sin(a2)));
                mesh.Normals.Add(new Vector3D(0, 1, 0));
                mesh.Normals.Add(new Vector3D(0, 1, 0));

                mesh.TriangleIndices.Add(topCenterIdx);
                mesh.TriangleIndices.Add(p1);
                mesh.TriangleIndices.Add(p1 + 1);
            }

            MaterialGroup matGroup = new MaterialGroup();
            matGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(color)));
            matGroup.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 30));

            return new GeometryModel3D { Geometry = mesh, Material = matGroup, BackMaterial = matGroup };
        }

        #endregion
    }
}
