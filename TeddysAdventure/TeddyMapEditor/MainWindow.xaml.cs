using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TeddyMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum EditMode
        {
            selection = 0,
            surfaces = 1,
            enemies = 2,
            objects = 3,
            none = 4
        }

        private const int BASIC_UNIT = 32;
        private bool _clickCaptured = false;
        private Point _clickStart;
        private Point _clickEnd;
        private EditMode _currentEditMode;
        private SolidColorBrush _surfaceBrush;
        private SolidColorBrush _surfaceBrushOutline;
        private SolidColorBrush _surfaceSelectedBrush;
        private SolidColorBrush _surfaceSelectedOutline;

        private SolidColorBrush _buttonSelectionTextColor = Brushes.Navy;
        private SolidColorBrush _buttonUnselectedTextColor = Brushes.Black;

        private SolidColorBrush _buttonSelectionBackColor = Brushes.LightBlue;
        private SolidColorBrush _buttonUnselectedBackColor = Brushes.LightGray;

        private bool _surfaceDraggingStarted = false;
        private Rectangle _currentSurface;

        public MainWindow()
        {
            InitializeComponent();
            _currentEditMode = EditMode.none;

            Color c = new Color();
            c = Colors.Gray;
            c.A = 50;
            _surfaceBrush = new SolidColorBrush(c);

            c = Colors.DarkGray;
            c.A = 250;
            _surfaceBrushOutline = new SolidColorBrush(c);

            c = Colors.LightBlue;
            c.A = 150;
            _surfaceSelectedBrush = new SolidColorBrush(c);

            c = Colors.Navy;
            c.A = 250;
            _surfaceSelectedOutline = new SolidColorBrush(c);

            spSurfaces.Visibility = Visibility.Hidden;
        }

        private void txtLevelWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            cnvsMap.Width = Convert.ToInt32(txtLevelWidth.Text);
            DrawGrid();
        }

        private void txtLevelHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            cnvsMap.Height = Convert.ToInt32(txtLevelHeight.Text);
            DrawGrid();
        }

        private void DrawGrid()
        {
            cnvsMap.Children.Clear();


            for (int i = 0; i < cnvsMap.Width / BASIC_UNIT; i++)
            {
                Line theVerticalLine = new Line();
                theVerticalLine.X1 = i * BASIC_UNIT;
                theVerticalLine.X2 = i * BASIC_UNIT;
                theVerticalLine.Y1 = 0;
                theVerticalLine.Y2 = cnvsMap.Height;
                theVerticalLine.StrokeThickness = 0.1;
                theVerticalLine.Stroke = Brushes.Black;
                cnvsMap.Children.Add(theVerticalLine);
            }


            for (int i = 0; i < cnvsMap.Height / BASIC_UNIT; i++)
            {
                Line theHorizontal = new Line();
                theHorizontal.X1 = 0;
                theHorizontal.X2 = cnvsMap.Width;
                theHorizontal.Y1 = i * BASIC_UNIT;
                theHorizontal.Y2 = i * BASIC_UNIT;
                theHorizontal.StrokeThickness = 0.1;
                theHorizontal.Stroke = Brushes.Black;
                cnvsMap.Children.Add(theHorizontal);
            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cnvsMap.Width = Convert.ToInt32(txtLevelWidth.Text);
            cnvsMap.Height = Convert.ToInt32(txtLevelHeight.Text);

            DrawGrid();
        }

        private void cnvsMap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(cnvsMap);
            int x = (int)(p.X / BASIC_UNIT);
            int y = (int)(p.Y / BASIC_UNIT);

            Point p2 = new Point(x * BASIC_UNIT, y * BASIC_UNIT);


            _clickStart = p2;
            _clickCaptured = true;
        }

        private void cnvsMap_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (_currentEditMode)
            {
                case EditMode.surfaces:
                    SurfaceModeMouseUp(e);
                    break;
            }

            _clickCaptured = false;
        }

        private void cnvsMap_MouseMove(object sender, MouseEventArgs e)
        {
            switch (_currentEditMode)
            {
                case EditMode.surfaces:
                    SurfaceModeMouseMove(e);
                    break;
            }

        }

        private void SurfaceModeMouseUp(MouseEventArgs e)
        {
            if (_clickCaptured)
            {
                Point p = e.MouseDevice.GetPosition(cnvsMap);
                int x = (int)(p.X / BASIC_UNIT);
                int y = (int)(p.Y / BASIC_UNIT);

                Point p2 = new Point(x * BASIC_UNIT, y * BASIC_UNIT);

                if (cnvsMap.Children.Count > 0)
                {
                    if (cnvsMap.Children[cnvsMap.Children.Count - 1].GetType() == typeof(Rectangle))
                        cnvsMap.Children.RemoveAt(cnvsMap.Children.Count - 1);
                }

                _clickEnd = p2;
                UnselectAllSurfaces();

                Rectangle r = new Rectangle();
                r.Width = Math.Abs(_clickEnd.X - _clickStart.X);
                r.Height = Math.Abs(_clickEnd.Y - _clickStart.Y);
                r.Stroke = _surfaceSelectedOutline;
                r.StrokeThickness = .5;
                r.Fill = _surfaceSelectedBrush;
                r.Tag = new Surface() { SurfaceTexture = txtSurfaceTexture.Text };
               
                r.MouseDown += new MouseButtonEventHandler(surface_MouseDown);
                cnvsMap.Children.Add(r);

                cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.LeftProperty, _clickStart.X);
                cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.TopProperty, _clickStart.Y);

                
                _surfaceDraggingStarted = false;
                _currentSurface = r;
            }
        }

        private void SurfaceModeMouseMove(MouseEventArgs e)
        {
            if (_clickCaptured)
            {
                if (_surfaceDraggingStarted && cnvsMap.Children.Count > 0)
                {
                    if (cnvsMap.Children[cnvsMap.Children.Count - 1].GetType() == typeof(Rectangle))
                        cnvsMap.Children.RemoveAt(cnvsMap.Children.Count - 1);
                }
                _surfaceDraggingStarted = true;

                Point p = e.MouseDevice.GetPosition(cnvsMap);
                int x = (int)(p.X / BASIC_UNIT);
                int y = (int)(p.Y / BASIC_UNIT);

                Point p2 = new Point(x * BASIC_UNIT, y * BASIC_UNIT);


                _clickEnd = p2;

                Rectangle r = new Rectangle();
                r.Width = Math.Abs(_clickEnd.X - _clickStart.X);
                r.Height = Math.Abs(_clickEnd.Y - _clickStart.Y);
                r.Stroke = _surfaceBrushOutline;
                r.StrokeThickness = .5;
                r.Fill = _surfaceBrush;
                cnvsMap.Children.Add(r);

                double startX;
                double startY;

                if (_clickEnd.X < _clickStart.X)
                {
                    startX = (int)_clickEnd.X;
                }
                else
                {
                    startX = (int)_clickStart.X;
                }

                if (_clickEnd.Y < _clickStart.Y)
                {
                    startY = (int)_clickEnd.Y;
                }
                else
                {
                    startY = (int)_clickStart.Y;
                }
                cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.LeftProperty, startX);
                cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.TopProperty, startY);
            }
        }

        private void surface_MouseDown(object sender, MouseEventArgs e)
        {
            if (_currentEditMode == EditMode.selection)
            {
                UnselectAllSurfaces();

                ((Rectangle)sender).Stroke = _surfaceSelectedOutline;
                ((Rectangle)sender).Fill = _surfaceSelectedBrush;

                _currentSurface = ((Rectangle)sender);
                spSurfaces.Visibility = Visibility.Visible;
            }

        }

        private void UnselectAllSurfaces()
        {
            foreach (UIElement item in cnvsMap.Children)
            {
                if (item.GetType() == typeof(Rectangle))
                {
                    ((Rectangle)item).Stroke = _surfaceBrushOutline;
                    ((Rectangle)item).Fill = _surfaceBrush;
                }

            }
        }

        private void btnSelectionMode_Click(object sender, RoutedEventArgs e)
        {
            
            if (btnSelectionMode.Foreground == _buttonSelectionTextColor)
            {
                SetButtonUnselected(ref btnSelectionMode);
            }
            else
            {
                UnselectAllButtons();
                _currentEditMode = EditMode.selection;
                btnSelectionMode.Foreground = _buttonSelectionTextColor;
                btnSelectionMode.Background = _buttonSelectionBackColor;
                this.Cursor = Cursors.Hand;
                spSurfaces.Visibility = Visibility.Hidden;
            }

            
        }

        private void btnSurfacesMode_Click(object sender, RoutedEventArgs e)
        {
            if (btnSurfacesMode.Foreground == _buttonSelectionTextColor)
            {
                SetButtonUnselected(ref btnSurfacesMode);
                spSurfaces.Visibility = Visibility.Hidden;
            }
            else
            {
                UnselectAllButtons();
                _currentEditMode = EditMode.surfaces;
                btnSurfacesMode.Foreground = _buttonSelectionTextColor;
                btnSurfacesMode.Background = _buttonSelectionBackColor;
                this.Cursor = Cursors.Cross;
                UnselectAllSurfaces();
                _currentSurface = null;
                spSurfaces.Visibility = Visibility.Visible;
            }
        }


        private void btnEnemyMode_Click(object sender, RoutedEventArgs e)
        {
            if (btnEnemyMode.Foreground == _buttonSelectionTextColor)
            {
                SetButtonUnselected(ref btnEnemyMode);
            }
            else
            {
                UnselectAllButtons();
                _currentEditMode = EditMode.enemies;
                btnEnemyMode.Foreground = _buttonSelectionTextColor;
                btnEnemyMode.Background = _buttonSelectionBackColor;
                this.Cursor = Cursors.Pen;
                UnselectAllSurfaces();
                _currentSurface = null;
            }
        }

        private void btnObjects_Click(object sender, RoutedEventArgs e)
        {
            if (btnObjects.Foreground == _buttonSelectionTextColor)
            {
                SetButtonUnselected(ref btnObjects);
            }
            else
            {
                UnselectAllButtons();
                _currentEditMode = EditMode.objects;
                btnObjects.Foreground = _buttonSelectionTextColor;
                btnObjects.Background = _buttonSelectionBackColor;
                this.Cursor = Cursors.SizeAll;
                UnselectAllSurfaces();
                _currentSurface = null;
            }
        }

        private void SetButtonUnselected(ref Button b)
        {
            _currentEditMode = EditMode.none;
            b.Foreground = _buttonUnselectedTextColor;
            b.Background = _buttonUnselectedBackColor;
            this.Cursor = Cursors.Arrow;
        }

        private void UnselectAllButtons()
        {
            btnSelectionMode.Foreground = _buttonUnselectedTextColor;
            btnSelectionMode.Background = _buttonUnselectedBackColor;

            btnSurfacesMode.Foreground = _buttonUnselectedTextColor;
            btnSurfacesMode.Background = _buttonUnselectedBackColor;

            btnObjects.Foreground = _buttonUnselectedTextColor;
            btnObjects.Background = _buttonUnselectedBackColor;

            btnEnemyMode.Foreground = _buttonUnselectedTextColor;
            btnEnemyMode.Background = _buttonUnselectedBackColor;
        }



        private void btnIncreaseWidth_Click(object sender, RoutedEventArgs e)
        {
            if(_currentSurface != null)
                _currentSurface.Width += BASIC_UNIT;
        }

        private void btnDecreaseWidth_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSurface != null)
                _currentSurface.Width -= BASIC_UNIT;
        }

        private void btnIncreaseHeight_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSurface != null)
                _currentSurface.Height += BASIC_UNIT;
        }

        private void btnDecreaseHeight_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSurface != null)
                _currentSurface.Height -= BASIC_UNIT;
        }

        private void btnIncreaseX_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSurface != null)
                _currentSurface.SetValue(Canvas.LeftProperty, Convert.ToDouble(_currentSurface.GetValue(Canvas.LeftProperty)) + (double)BASIC_UNIT);
        }

        private void btnDecreaseX_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSurface != null)
                _currentSurface.SetValue(Canvas.LeftProperty, Convert.ToDouble(_currentSurface.GetValue(Canvas.LeftProperty)) - (double)BASIC_UNIT);
        }

        private void btnDecreaseY_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSurface != null)
                _currentSurface.SetValue(Canvas.TopProperty, Convert.ToDouble(_currentSurface.GetValue(Canvas.TopProperty)) - (double)BASIC_UNIT);
        }

        private void btnIncreaseY_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSurface != null)
                _currentSurface.SetValue(Canvas.TopProperty, Convert.ToDouble(_currentSurface.GetValue(Canvas.TopProperty)) + (double)BASIC_UNIT);
        }

        private void btnRender_Click(object sender, RoutedEventArgs e)
        {
            cnvsMap.Width = Convert.ToInt32(txtLevelWidth.Text);
            cnvsMap.Height = Convert.ToInt32(txtLevelHeight.Text);
            DrawGrid();
        }


    }
}
