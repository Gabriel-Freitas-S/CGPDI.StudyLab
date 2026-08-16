using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CGPDI.StudyLab.Graphics3D
{
    /// <summary>
    /// Gerenciador do subsistema 3D acelerado por hardware do WPF (DirectX / Viewport3D).
    /// Controla malhas procedurais, iluminação de Phong avançada, materiais e câmera orbital Arcball interativa.
    /// </summary>
    public class WpfViewport3DManager
    {
        private readonly Viewport3D _viewport;
        private readonly Model3DGroup _modelGroup;
        private PerspectiveCamera _perspectiveCamera;
        private OrthographicCamera _orthographicCamera;
        private bool _isPerspective = true;
        private GeometryModel3D _currentGeometryModel;
        private Model3DGroup? _hierarchicalGroup;

        // Câmera Orbital (Coordenadas Esféricas)
        private double _yaw = 45.0;     // Rotação horizontal (graus)
        private double _pitch = 30.0;   // Rotação vertical (graus)
        private double _distance = 6.0; // Distância da câmera ao centro
        private Point _lastMousePos;
        private bool _isMouseDragging;

        // Luzes
        private DirectionalLight _directionalLight;
        private System.Windows.Media.Media3D.PointLight _pointLight;
        private AmbientLight _ambientLight;

        public WpfViewport3DManager(Viewport3D viewport)
        {
            _viewport = viewport;
            _modelGroup = new Model3DGroup();
            ModelVisual3D visual = new ModelVisual3D { Content = _modelGroup };
            _viewport.Children.Clear();
            _viewport.Children.Add(visual);

            // Câmera Perspectiva
            _perspectiveCamera = new PerspectiveCamera
            {
                FieldOfView = 55.0,
                NearPlaneDistance = 0.1,
                FarPlaneDistance = 100.0
            };

            // Câmera Ortográfica (Projeção Paralela)
            _orthographicCamera = new OrthographicCamera
            {
                Width = 6.0,
                NearPlaneDistance = 0.1,
                FarPlaneDistance = 100.0
            };

            _viewport.Camera = _perspectiveCamera;
            UpdateCameraPosition();

            // Configuração das Luzes
            _ambientLight = new AmbientLight(Color.FromRgb(40, 40, 50));
            _modelGroup.Children.Add(_ambientLight);

            _directionalLight = new DirectionalLight(Color.FromRgb(240, 240, 255), new Vector3D(-1, -1.5, -1));
            _modelGroup.Children.Add(_directionalLight);

            _pointLight = new System.Windows.Media.Media3D.PointLight(Color.FromRgb(255, 180, 100), new Point3D(2, 3, 2))
            {
                Range = 15.0,
                ConstantAttenuation = 1.0,
                LinearAttenuation = 0.1,
                QuadraticAttenuation = 0.05
            };
            _modelGroup.Children.Add(_pointLight);

            // Modelo padrão inicial (Esfera)
            _currentGeometryModel = new GeometryModel3D();
            _modelGroup.Children.Add(_currentGeometryModel);

            // Eventos do Mouse para controle orbital
            _viewport.MouseDown += Viewport_MouseDown;
            _viewport.MouseMove += Viewport_MouseMove;
            _viewport.MouseUp += Viewport_MouseUp;
            _viewport.MouseWheel += Viewport_MouseWheel;

            SetShape("Torus");
        }

        #region Controle de Câmera Orbital (Arcball)

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                _isMouseDragging = true;
                _lastMousePos = e.GetPosition(_viewport);
                _viewport.CaptureMouse();
            }
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDragging)
            {
                Point currentPos = e.GetPosition(_viewport);
                double dx = currentPos.X - _lastMousePos.X;
                double dy = currentPos.Y - _lastMousePos.Y;

                _yaw += dx * 0.4;
                _pitch = Math.Clamp(_pitch - dy * 0.4, -89.0, 89.0);

                _lastMousePos = currentPos;
                UpdateCameraPosition();
            }
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDragging = false;
            _viewport.ReleaseMouseCapture();
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _distance = Math.Clamp(_distance - (e.Delta / 120.0) * 0.5, 1.5, 20.0);
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            double yawRad = _yaw * Math.PI / 180.0;
            double pitchRad = _pitch * Math.PI / 180.0;

            double x = _distance * Math.Cos(pitchRad) * Math.Sin(yawRad);
            double y = _distance * Math.Sin(pitchRad);
            double z = _distance * Math.Cos(pitchRad) * Math.Cos(yawRad);

            Point3D pos = new Point3D(x, y, z);
            Vector3D lookDir = new Vector3D(-x, -y, -z);
            Vector3D upDir = new Vector3D(0, 1, 0);

            _perspectiveCamera.Position = pos;
            _perspectiveCamera.LookDirection = lookDir;
            _perspectiveCamera.UpDirection = upDir;

            _orthographicCamera.Position = pos;
            _orthographicCamera.LookDirection = lookDir;
            _orthographicCamera.UpDirection = upDir;
            _orthographicCamera.Width = _distance * 0.9;
        }

        public void RotateCamera(double deltaYawDegrees)
        {
            _yaw = (_yaw + deltaYawDegrees) % 360.0;
            UpdateCameraPosition();
        }

        public void SetCameraProjection(bool isPerspective)
        {
            _isPerspective = isPerspective;
            _viewport.Camera = isPerspective ? _perspectiveCamera : _orthographicCamera;
            UpdateCameraPosition();
        }

        public void SetHierarchicalScene(Model3DGroup group)
        {
            if (_hierarchicalGroup != null)
                _modelGroup.Children.Remove(_hierarchicalGroup);

            _modelGroup.Children.Remove(_currentGeometryModel);
            _hierarchicalGroup = group;
            _modelGroup.Children.Add(_hierarchicalGroup);
        }

        public void ClearHierarchicalScene()
        {
            if (_hierarchicalGroup != null)
            {
                _modelGroup.Children.Remove(_hierarchicalGroup);
                _hierarchicalGroup = null;
            }
            if (!_modelGroup.Children.Contains(_currentGeometryModel))
            {
                _modelGroup.Children.Add(_currentGeometryModel);
            }
        }

        public void SetDistance(double distance)
        {
            _distance = Math.Clamp(distance, 1.5, 25.0);
            UpdateCameraPosition();
        }

        public void ResetCamera()
        {
            _yaw = 45.0;
            _pitch = 30.0;
            _distance = 6.0;
            UpdateCameraPosition();
        }

        #endregion

        #region Materiais e Iluminação

        public void UpdateMaterial(Color baseColor, double specularPower = 40.0, bool doubleSided = true)
        {
            MaterialGroup group = new MaterialGroup();

            // Componente Difusa
            DiffuseMaterial diffuse = new DiffuseMaterial(new SolidColorBrush(baseColor));
            group.Children.Add(diffuse);

            // Componente Especular (Brilho metálico/plástico com expoente de brilho)
            SpecularMaterial specular = new SpecularMaterial(new SolidColorBrush(Colors.White), specularPower);
            group.Children.Add(specular);

            _currentGeometryModel.Material = group;
            if (doubleSided)
                _currentGeometryModel.BackMaterial = group; // Permite ver o interior de superfícies abertas como Möbius
            else
                _currentGeometryModel.BackMaterial = null;
        }

        public void UpdateLights(Color dirColor, Color pointColor, double ambientIntensity = 0.2)
        {
            _directionalLight.Color = dirColor;
            _pointLight.Color = pointColor;
            byte amb = (byte)Math.Clamp(ambientIntensity * 255.0, 0, 255);
            _ambientLight.Color = Color.FromRgb(amb, amb, amb);
        }

        #endregion

        #region Formas Geométricas 3D Paramétricas

        public void SetShape(string shapeName, Color? color = null)
        {
            ClearHierarchicalScene();
            Color c = color ?? Color.FromRgb(60, 150, 240);
            MeshGeometry3D mesh;
            string lower = (shapeName ?? "").ToLower();

            if (lower.Contains("cub"))
            {
                mesh = BuildCubeMesh(2.0);
            }
            else if (lower.Contains("esfer") || lower.Contains("spher"))
            {
                mesh = BuildSphereMesh(1.5, 32, 32);
            }
            else if (lower.Contains("tor") || lower.Contains("donut"))
            {
                mesh = BuildTorusMesh(1.4, 0.55, 36, 24);
            }
            else if (lower.Contains("cilind") || lower.Contains("cylind"))
            {
                mesh = BuildCylinderMesh(1.0, 2.5, 32);
            }
            else if (lower.Contains("con"))
            {
                mesh = BuildConeMesh(1.2, 2.5, 32);
            }
            else if (lower.Contains("mob") || lower.Contains("m\u00f6b") || lower.Contains("topolog"))
            {
                mesh = BuildMobiusStripMesh(1.6, 0.6, 60, 10);
            }
            else
            {
                mesh = BuildTorusMesh(1.4, 0.55, 36, 24);
            }

            _currentGeometryModel.Geometry = mesh;
            UpdateMaterial(c, 40.0, true);
        }

        private MeshGeometry3D BuildCubeMesh(double size)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            double h = size / 2.0;

            Point3D[] p = new Point3D[]
            {
                new Point3D(-h, -h, -h), new Point3D( h, -h, -h), new Point3D( h,  h, -h), new Point3D(-h,  h, -h), // Frente
                new Point3D(-h, -h,  h), new Point3D( h, -h,  h), new Point3D( h,  h,  h), new Point3D(-h,  h,  h)  // Trás
            };

            void AddQuad(int i0, int i1, int i2, int i3, Vector3D normal)
            {
                int baseIdx = mesh.Positions.Count;
                mesh.Positions.Add(p[i0]);
                mesh.Positions.Add(p[i1]);
                mesh.Positions.Add(p[i2]);
                mesh.Positions.Add(p[i3]);

                for (int k = 0; k < 4; k++)
                    mesh.Normals.Add(normal);

                mesh.TriangleIndices.Add(baseIdx + 0);
                mesh.TriangleIndices.Add(baseIdx + 2);
                mesh.TriangleIndices.Add(baseIdx + 1);

                mesh.TriangleIndices.Add(baseIdx + 0);
                mesh.TriangleIndices.Add(baseIdx + 3);
                mesh.TriangleIndices.Add(baseIdx + 2);
            }

            AddQuad(0, 1, 2, 3, new Vector3D(0, 0, -1)); // Frente
            AddQuad(5, 4, 7, 6, new Vector3D(0, 0, 1));  // Trás
            AddQuad(4, 0, 3, 7, new Vector3D(-1, 0, 0)); // Esquerda
            AddQuad(1, 5, 6, 2, new Vector3D(1, 0, 0));  // Direita
            AddQuad(3, 2, 6, 7, new Vector3D(0, 1, 0));  // Topo
            AddQuad(4, 5, 1, 0, new Vector3D(0, -1, 0)); // Base

            return mesh;
        }

        private MeshGeometry3D BuildSphereMesh(double radius, int latBands, int lonBands)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int lat = 0; lat <= latBands; lat++)
            {
                double theta = lat * Math.PI / latBands;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int lon = 0; lon <= lonBands; lon++)
                {
                    double phi = lon * 2.0 * Math.PI / lonBands;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = cosPhi * sinTheta;
                    double y = cosTheta;
                    double z = sinPhi * sinTheta;

                    mesh.Positions.Add(new Point3D(x * radius, y * radius, z * radius));
                    mesh.Normals.Add(new Vector3D(x, y, z));
                    mesh.TextureCoordinates.Add(new Point((double)lon / lonBands, (double)lat / latBands));
                }
            }

            for (int lat = 0; lat < latBands; lat++)
            {
                for (int lon = 0; lon < lonBands; lon++)
                {
                    int first = (lat * (lonBands + 1)) + lon;
                    int second = first + lonBands + 1;

                    mesh.TriangleIndices.Add(first);
                    mesh.TriangleIndices.Add(first + 1);
                    mesh.TriangleIndices.Add(second);

                    mesh.TriangleIndices.Add(second);
                    mesh.TriangleIndices.Add(first + 1);
                    mesh.TriangleIndices.Add(second + 1);
                }
            }

            return mesh;
        }

        private MeshGeometry3D BuildTorusMesh(double rMajor, double rMinor, int majorSegments, int minorSegments)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

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

                    Point3D pos = new Point3D(x, y, z);
                    Point3D center = new Point3D(rMajor * cosU, 0, rMajor * sinU);
                    Vector3D normal = pos - center;
                    normal.Normalize();

                    mesh.Positions.Add(pos);
                    mesh.Normals.Add(normal);
                }
            }

            for (int i = 0; i < majorSegments; i++)
            {
                for (int j = 0; j < minorSegments; j++)
                {
                    int first = (i * (minorSegments + 1)) + j;
                    int second = first + minorSegments + 1;

                    mesh.TriangleIndices.Add(first);
                    mesh.TriangleIndices.Add(second);
                    mesh.TriangleIndices.Add(first + 1);

                    mesh.TriangleIndices.Add(second);
                    mesh.TriangleIndices.Add(second + 1);
                    mesh.TriangleIndices.Add(first + 1);
                }
            }

            return mesh;
        }

        private MeshGeometry3D BuildCylinderMesh(double radius, double height, int segments)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            double halfH = height / 2.0;

            for (int i = 0; i <= segments; i++)
            {
                double angle = i * 2.0 * Math.PI / segments;
                double x = radius * Math.Cos(angle);
                double z = radius * Math.Sin(angle);

                mesh.Positions.Add(new Point3D(x, -halfH, z));
                mesh.Positions.Add(new Point3D(x,  halfH, z));

                Vector3D normal = new Vector3D(x, 0, z);
                normal.Normalize();
                mesh.Normals.Add(normal);
                mesh.Normals.Add(normal);
            }

            for (int i = 0; i < segments; i++)
            {
                int baseIdx = i * 2;
                mesh.TriangleIndices.Add(baseIdx + 0);
                mesh.TriangleIndices.Add(baseIdx + 1);
                mesh.TriangleIndices.Add(baseIdx + 2);

                mesh.TriangleIndices.Add(baseIdx + 1);
                mesh.TriangleIndices.Add(baseIdx + 3);
                mesh.TriangleIndices.Add(baseIdx + 2);
            }

            return mesh;
        }

        private MeshGeometry3D BuildConeMesh(double radius, double height, int segments)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            double halfH = height / 2.0;
            Point3D apex = new Point3D(0, halfH, 0);

            for (int i = 0; i <= segments; i++)
            {
                double angle = i * 2.0 * Math.PI / segments;
                double x = radius * Math.Cos(angle);
                double z = radius * Math.Sin(angle);

                mesh.Positions.Add(new Point3D(x, -halfH, z));
                Vector3D n = new Vector3D(x, radius / height, z);
                n.Normalize();
                mesh.Normals.Add(n);
            }

            int apexIdx = mesh.Positions.Count;
            mesh.Positions.Add(apex);
            mesh.Normals.Add(new Vector3D(0, 1, 0));

            for (int i = 0; i < segments; i++)
            {
                mesh.TriangleIndices.Add(i);
                mesh.TriangleIndices.Add(apexIdx);
                mesh.TriangleIndices.Add(i + 1);
            }

            return mesh;
        }

        /// <summary>
        /// Faixa de Möbius Paramétrica 3D:
        /// Superfície topológica não-orientável com apenas 1 lado e 1 borda.
        /// x(u,v) = (1 + (v/2)*cos(u/2)) * cos(u)
        /// y(u,v) = (v/2)*sin(u/2)
        /// z(u,v) = (1 + (v/2)*cos(u/2)) * sin(u)
        /// </summary>
        private MeshGeometry3D BuildMobiusStripMesh(double radius, double width, int uSegments, int vSegments)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int i = 0; i <= uSegments; i++)
            {
                double u = i * 2.0 * Math.PI / uSegments;

                for (int j = 0; j <= vSegments; j++)
                {
                    double v = (j * 2.0 / vSegments - 1.0) * (width / 2.0);

                    double x = (radius + v * Math.Cos(u / 2.0)) * Math.Cos(u);
                    double y = v * Math.Sin(u / 2.0);
                    double z = (radius + v * Math.Cos(u / 2.0)) * Math.Sin(u);

                    mesh.Positions.Add(new Point3D(x, y, z));
                    mesh.Normals.Add(new Vector3D(Math.Cos(u), Math.Sin(u / 2.0), Math.Sin(u)));
                }
            }

            for (int i = 0; i < uSegments; i++)
            {
                for (int j = 0; j < vSegments; j++)
                {
                    int first = (i * (vSegments + 1)) + j;
                    int second = first + vSegments + 1;

                    mesh.TriangleIndices.Add(first);
                    mesh.TriangleIndices.Add(first + 1);
                    mesh.TriangleIndices.Add(second);

                    mesh.TriangleIndices.Add(second);
                    mesh.TriangleIndices.Add(first + 1);
                    mesh.TriangleIndices.Add(second + 1);
                }
            }

            return mesh;
        }

        #endregion
    }
}
