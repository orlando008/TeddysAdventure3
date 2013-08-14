﻿using System;
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

        private const int BASIC_UNIT = 25;
        private bool _clickCaptured = false;
        private Point _clickStart;
        private Point _clickEnd;
        private EditMode _currentEditMode;
        private float _selectionOpacity = 0.5f;

        public EditMode CurrentEditMode
        {
            get { return _currentEditMode; }
            set 
            {
                UnselectAllElements();
                spEnemies.Width = 0;
                spObjects.Width = 0;
                spSurfaces.Width = 0;
                _currentEditMode = value; 
            }
        }

        //private SolidColorBrush _surfaceBrush;
        private SolidColorBrush _surfaceBrushOutline;
        //private SolidColorBrush _surfaceSelectedBrush;
        private SolidColorBrush _surfaceSelectedOutline;

        private SolidColorBrush _enemyBrush;
        private SolidColorBrush _enemyBrushOutline;
        private SolidColorBrush _enemySelectedBrush;
        private SolidColorBrush _enemySelectedOutline;

        private SolidColorBrush _buttonSelectionTextColor = Brushes.Navy;
        private SolidColorBrush _buttonUnselectedTextColor = Brushes.Black;

        private SolidColorBrush _buttonSelectionBackColor = Brushes.LightBlue;
        private SolidColorBrush _buttonUnselectedBackColor = Brushes.LightGray;

        private bool _surfaceDraggingStarted = false;
        private Rectangle _currentSurface;
        private Rectangle _currentEnemy;
        private Rectangle _currentObject;

        public MainWindow()
        {
            InitializeComponent();
            CurrentEditMode = EditMode.none;

            Color c = new Color();
            c = Colors.Gray;
            c.A = 50;
            //_surfaceBrush = new SolidColorBrush(c);

            c = Colors.DarkGray;
            c.A = 250;
            _surfaceBrushOutline = new SolidColorBrush(c);

            c = Colors.LightBlue;
            c.A = 150;
            //_surfaceSelectedBrush = new SolidColorBrush(c);

            c = Colors.Navy;
            c.A = 250;
            _surfaceSelectedOutline = new SolidColorBrush(c);
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
                case EditMode.enemies:
                    EnemyModeMouseUp(e);
                    break;
                case EditMode.objects:
                    ObjectModeMouseUp(e);
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

        private void EnemyModeMouseUp(MouseEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(cnvsMap);
            Rectangle r = new Rectangle();
            int width =0;
            int height =0;
            GetCurrentEnemysSize(ref width, ref height);
            r.Width = width;
            r.Height = height;
            r.Stroke = _surfaceSelectedOutline;
            r.StrokeThickness = .5;
            r.Fill = Brushes.Beige;
            r.Tag = new Enemy(cboEnemies.Text, p, 0, 0);

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("..\\..\\Images\\" + cboEnemies.Text + ".png", UriKind.Relative));
            r.Fill = ib;

            r.MouseDown += new MouseButtonEventHandler(enemy_MouseDown);

            cnvsMap.Children.Add(r);

            cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.LeftProperty, p.X);
            cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.TopProperty, p.Y);
        }

        private void ObjectModeMouseUp(MouseEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(cnvsMap);
            Rectangle r = new Rectangle();
            int width = 0;
            int height = 0;
            GetCurrentObjectSize(ref width, ref height);
            r.Width = width;
            r.Height = height;
            r.Stroke = _surfaceSelectedOutline;
            r.StrokeThickness = .5;
            r.Fill = Brushes.GhostWhite;
            r.Tag = new GameObject(cboObjects.Text, p);

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("..\\..\\Images\\" + cboObjects.Text + ".png", UriKind.Relative));
            r.Fill = ib;

            r.MouseDown += new MouseButtonEventHandler(object_MouseDown);
            cnvsMap.Children.Add(r);

            cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.LeftProperty, p.X);
            cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.TopProperty, p.Y);
        }

        private void GetCurrentEnemysSize(ref int width, ref int height)
        {
            switch (cboEnemies.Text.ToString())
            {
                case "BowlingBall":
                    width = 50;
                    height = 50;
                    break;
                case "FlyingBook":
                    width = 150;
                    height = 68;
                    break;
                case "MatchBoxCar":
                    width = 67;
                    height = 25;
                    break;
                case "Airplane":
                    width = 75;
                    height = 36;
                    break;
            }
        }

        private void GetCurrentObjectSize(ref int width, ref int height)
        {
            switch (cboObjects.Text)
            {
                case "Fluff":
                    width = 25;
                    height = 25;
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
                UnselectAllElements();

                Rectangle r = new Rectangle();
                r.Width = Math.Abs(_clickEnd.X - _clickStart.X);
                r.Height = Math.Abs(_clickEnd.Y - _clickStart.Y);
                r.Stroke = _surfaceSelectedOutline;
                r.StrokeThickness = .5;

                r.Tag = new Surface() { SurfaceTexture = txtSurfaceTexture.Text };


                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("..\\..\\Images\\" + txtSurfaceTexture.Text  + ".png", UriKind.Relative));
                ib.TileMode = TileMode.Tile;
                ib.Viewport = new Rect(0, 0, ib.ImageSource.Width/r.Width, ib.ImageSource.Height/r.Height);
                r.Fill = ib;
                r.Opacity = _selectionOpacity;
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
                //r.Fill = _surfaceBrush;
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
                UnselectAllElements();

                ((Rectangle)sender).Opacity = _selectionOpacity;
                _currentSurface = ((Rectangle)sender);
                spSurfaces.Width = 1000;
            }

        }

        private void enemy_MouseDown(object sender, MouseEventArgs e)
        {
            if (_currentEditMode == EditMode.selection)
            {
                UnselectAllElements();

                ((Rectangle)sender).Stroke = _surfaceSelectedOutline;
                ((Rectangle)sender).Opacity = _selectionOpacity;

                _currentEnemy = ((Rectangle)sender);
                spEnemies.Width = 1000;
            }

        }

        private void object_MouseDown(object sender, MouseEventArgs e)
        {
            if (_currentEditMode == EditMode.selection)
            {
                UnselectAllElements();

                ((Rectangle)sender).Stroke = _surfaceSelectedOutline;
                ((Rectangle)sender).Opacity = _selectionOpacity;

                _currentObject = ((Rectangle)sender);
                spObjects.Width = 1000;
            }

        }



        private void UnselectAllElements()
        {
            foreach (UIElement item in cnvsMap.Children)
            {
                if (item.GetType() == typeof(Rectangle))
                {
                    ((Rectangle)item).Opacity = 1;

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
                CurrentEditMode = EditMode.selection;
                btnSelectionMode.Foreground = _buttonSelectionTextColor;
                btnSelectionMode.Background = _buttonSelectionBackColor;
                this.Cursor = Cursors.Hand;
            }

            
        }

        private void btnSurfacesMode_Click(object sender, RoutedEventArgs e)
        {
            if (btnSurfacesMode.Foreground == _buttonSelectionTextColor)
            {
                SetButtonUnselected(ref btnSurfacesMode);
            }
            else
            {
                UnselectAllButtons();
                CurrentEditMode = EditMode.surfaces;
                btnSurfacesMode.Foreground = _buttonSelectionTextColor;
                btnSurfacesMode.Background = _buttonSelectionBackColor;
                this.Cursor = Cursors.Cross;
                UnselectAllElements();
                SetAllCurrentObjectsToNull();
                spSurfaces.Width = 1000;
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
                CurrentEditMode = EditMode.enemies;
                btnEnemyMode.Foreground = _buttonSelectionTextColor;
                btnEnemyMode.Background = _buttonSelectionBackColor;
                this.Cursor = Cursors.Pen;
                UnselectAllElements();
                SetAllCurrentObjectsToNull();
                spEnemies.Width = 1000;
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
                CurrentEditMode = EditMode.objects;
                btnObjects.Foreground = _buttonSelectionTextColor;
                btnObjects.Background = _buttonSelectionBackColor;
                this.Cursor = Cursors.SizeAll;
                UnselectAllElements();
                SetAllCurrentObjectsToNull();
                spObjects.Width = 1000;
            }
        }

        private void SetAllCurrentObjectsToNull()
        {
            _currentSurface = null;
            _currentEnemy = null;
            _currentObject = null;
        }

        private void SetButtonUnselected(ref Button b)
        {
            CurrentEditMode = EditMode.none;
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

            spEnemies.Width = 0;
            spSurfaces.Width = 0;
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
