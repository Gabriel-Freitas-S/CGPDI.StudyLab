using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CGPDI.StudyLab.Views
{
    /// <summary>
    /// Base para janelas sem borda (WindowStyle="None") com barra de título própria.
    /// Centraliza os controles de minimizar/maximizar/fechar e a abertura maximizada
    /// respeitando a WorkArea (não cobre a taskbar). O botão maximizar alterna entre
    /// o estado restaurado e a tela cheia (fullscreen) nativa.
    /// </summary>
    public abstract class BorderlessWindow : Window
    {
        private bool _isMaximized = false;

        protected BorderlessWindow()
        {
            StateChanged += OnBorderlessStateChanged;
        }

        protected Button? MaximizeButton => FindName("BtnMaximize") as Button;

        private void OnBorderlessStateChanged(object? sender, EventArgs e)
        {
            UpdateMaximizeButton();
        }

        /// <summary>Estende a janela sobre a WorkArea (taskbar visível). Chamar no Loaded.</summary>
        protected void MaximizeOnOpen()
        {
            var wa = SystemParameters.WorkArea;
            Left = wa.Left;
            Top = wa.Top;
            Width = wa.Width;
            Height = wa.Height;
            _isMaximized = true;
            UpdateMaximizeButton();
        }

        protected void ToggleMaximize()
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
            UpdateMaximizeButton();
        }

        /// <summary>Limita a janela à área de trabalho (se exceder) e a centraliza na tela.</summary>
        protected void CenterOnScreen()
        {
            var wa = SystemParameters.WorkArea;
            if (Width > wa.Width) Width = wa.Width;
            if (Height > wa.Height) Height = wa.Height;
            Left = wa.Left + (wa.Width - Width) / 2;
            Top = wa.Top + (wa.Height - Height) / 2;
        }

        protected virtual void UpdateMaximizeButton()
        {
            if (MaximizeButton is Button btn)
            {
                bool extended = _isMaximized || WindowState == WindowState.Maximized;
                btn.Content = extended ? "🗗" : "🗖";
            }
        }

        protected void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            // Não interfere com o clique dos botões da barra de título
            if (IsOverButton(e.OriginalSource as DependencyObject))
                return;

            if (e.ClickCount == 2)
            {
                ToggleMaximize();
            }
            else if (!_isMaximized && WindowState != WindowState.Maximized)
            {
                DragMove();
            }
        }

        private static bool IsOverButton(DependencyObject? obj)
        {
            while (obj != null)
            {
                if (obj is ButtonBase)
                    return true;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return false;
        }

        protected void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        protected virtual void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
