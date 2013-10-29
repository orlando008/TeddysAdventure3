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
using System.IO;
using Microsoft.Win32;
using System.Xml;

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
        private EditMode _previousEditMode = EditMode.none;

        private EditorViewModel _vm;
        private List<TeddysAdventureLibrary.Background> _backgrounds = new List<TeddysAdventureLibrary.Background>();
        private List<Surface> _surfaces = new List<Surface>();
        private List<GameObject> _gameObjects = new List<GameObject>();
        private List<Enemy> _enemies = new List<Enemy>();

        private Rectangle _surfaceAdditionRectangle;

        public EditMode PreviousEditMode
        {
            get { return _previousEditMode; }
            set { _previousEditMode = value; }
        }
        private float _selectionOpacity = 0.5f;

        public EditMode CurrentEditMode
        {
            get { return _currentEditMode; }
            set 
            {
                UnselectAllElements();
                _previousEditMode = _currentEditMode;
                _currentEditMode = value;
            }
        }


        private SolidColorBrush _surfaceBrushOutline;
        private SolidColorBrush _surfaceSelectedOutline;

        private SolidColorBrush _buttonSelectionTextColor = Brushes.Navy;
        private SolidColorBrush _buttonUnselectedTextColor = Brushes.Black;

        private SolidColorBrush _buttonSelectionBackColor = Brushes.LightBlue;
        private SolidColorBrush _buttonUnselectedBackColor = Brushes.LightGray;

        private bool _surfaceDraggingStarted = false;

        public MainWindow()
        {
            InitializeComponent();
            CurrentEditMode = EditMode.none;

            _vm = new EditorViewModel( this);
            this.DataContext = _vm;

            Color c = new Color();
            c = Colors.DarkGray;
            c.A = 250;
            _surfaceBrushOutline = new SolidColorBrush(c);

            c = Colors.Navy;
            c.A = 250;
            _surfaceSelectedOutline = new SolidColorBrush(c);
            
            this.PreviewKeyDown += cnvs_KeyPrev;
        }

        public Rectangle CurrentSurface
        {
            get
            {
                if (_vm.CurrentSelection != null && _vm.CurrentSelection.GetType() == typeof(Surface))
                    return _vm.CurrentSelection.Parent;
                else
                    return null;
            }


        }


        public void DrawGrid()
        {
            for (int i = cnvsMap.Children.Count - 1; i > 0; i--)
            {
               if(cnvsMap.Children[i].GetType() == typeof(Line))
               {
                   cnvsMap.Children.RemoveAt(i);
               }
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

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGrid();
        }


        private void cnvsMap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == cnvsMap)
            {
                if (e.Source.GetType() != typeof(Rectangle))
                {
                    SetAllCurrentObjectsToNull();
                    UnselectAllElements();
                    if(CurrentEditMode == EditMode.selection)
                        CurrentEditMode = PreviousEditMode;
                }
            }

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
                case EditMode.selection:

                    break;
            }

        }

        private void cnvs_KeyPrev(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                for (int i = cnvsMap.Children.Count -1; i >= 0; i--)
                {
                    if (cnvsMap.Children[i].GetType() == typeof(Rectangle))
                    {
                        if (((Rectangle)cnvsMap.Children[i]).Opacity == Convert.ToDouble(_selectionOpacity))
                        {

                            if (((Rectangle)cnvsMap.Children[i]).Tag.GetType() == typeof(GameObject))
                            {
                                _gameObjects.Remove((GameObject)((Rectangle)cnvsMap.Children[i]).Tag);
                            }
                            else if (((Rectangle)cnvsMap.Children[i]).Tag.GetType() == typeof(Enemy))
                            {
                                _enemies.Remove((Enemy)((Rectangle)cnvsMap.Children[i]).Tag);
                            }
                            else if (((Rectangle)cnvsMap.Children[i]).Tag.GetType() == typeof(Surface))
                            {
                                _surfaces.Remove((Surface)((Rectangle)cnvsMap.Children[i]).Tag);
                            }
                            

                            cnvsMap.Children.RemoveAt(i);
                        }
                    }
                }
                
            }
        }

        private void SurfaceModeMouseMove(MouseEventArgs e)
        {
            if (_clickCaptured)
            {

                cnvsMap.Children.Remove(_surfaceAdditionRectangle);
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
                _surfaceAdditionRectangle = r;
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

        private void SurfaceModeMouseUp(MouseEventArgs e)
        {
            if (_clickCaptured)
            {
                Point p = e.MouseDevice.GetPosition(cnvsMap);
                int x = (int)(p.X / BASIC_UNIT);
                int y = (int)(p.Y / BASIC_UNIT);

                Point p2 = new Point(x * BASIC_UNIT, y * BASIC_UNIT);


                cnvsMap.Children.Remove(_surfaceAdditionRectangle);

                _clickEnd = p2;
                UnselectAllElements();

                Rectangle r = new Rectangle();
                r.Width = Math.Abs(_clickEnd.X - _clickStart.X);
                r.Height = Math.Abs(_clickEnd.Y - _clickStart.Y);
                r.Stroke = _surfaceSelectedOutline;
                r.StrokeThickness = .5;

                if (txtSurfaceTexture.Text == "")
                    txtSurfaceTexture.Text = "SurfaceTexture1";

                Surface s = new Surface() { SurfaceTexture = txtSurfaceTexture.Text };
                s.SurfaceBounds = r;
                r.Tag = s;


                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("..\\..\\Images\\" + s.SurfaceTexture  + ".png", UriKind.Relative));
                ib.TileMode = TileMode.Tile;
                ib.Viewport = new Rect(0, 0, ib.ImageSource.Width/r.Width, ib.ImageSource.Height/r.Height);
                r.Fill = ib;
                r.Opacity = _selectionOpacity;
                cnvsMap.Children.Add(r);

                cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.LeftProperty, _clickStart.X);
                cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.TopProperty, _clickStart.Y);

                _surfaces.Add(s);
                _surfaceDraggingStarted = false;
                _vm.CurrentSelection = s;
                                
            }
        }

        private void EnemyModeMouseUp(MouseEventArgs e)
        {
            if (cboEnemies.Text == "")
                return;

            Point p = e.MouseDevice.GetPosition(cnvsMap);
            Rectangle r = new Rectangle();

            r.Stroke = _surfaceSelectedOutline;
            r.StrokeThickness = .5;
            r.Opacity = _selectionOpacity;
            r.Fill = Brushes.Beige;
            Enemy en = new Enemy( r,cboEnemies.Text, p, 0, 0);

            r.Tag = en;



            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("..\\..\\Images\\" + cboEnemies.Text + ".png", UriKind.Relative));
            r.Fill = ib;

            r.Width = ib.ImageSource.Width;
            r.Height = ib.ImageSource.Height;

            en.SomethingChanged += new EventHandler(MoveableObjectChanged);
            cnvsMap.Children.Add(r);

            cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.LeftProperty, p.X);
            cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.TopProperty, p.Y);

            _enemies.Add(en);
            _vm.CurrentSelection = en;

            en.Location = p;
            en.VelocityX = 0;
            en.VelocityY = 0;
        }

        private void ObjectModeMouseUp(MouseEventArgs e)
        {
            if (cboObjects.Text == "")
                return;

            Point p = e.MouseDevice.GetPosition(cnvsMap);
            Rectangle r = new Rectangle();
            r.Stroke = _surfaceSelectedOutline;
            r.StrokeThickness = .5;
            r.Fill = Brushes.GhostWhite;
            r.Opacity = _selectionOpacity;

            GameObject go = new GameObject(r,cboObjects.Text, p);

            r.Tag = go;

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("..\\..\\Images\\" + cboObjects.Text + ".png", UriKind.Relative));
            r.Fill = ib;
            r.Width = ib.ImageSource.Width;
            r.Height = ib.ImageSource.Height;

            ((GameObject)r.Tag).SomethingChanged += new EventHandler(MoveableObjectChanged);
            cnvsMap.Children.Add(r);

            cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.LeftProperty, p.X);
            cnvsMap.Children[cnvsMap.Children.Count - 1].SetValue(Canvas.TopProperty, p.Y);
            _gameObjects.Add(go);

            _vm.CurrentSelection = go;
    
             go.Location = p;
        }


        private void MoveableObjectChanged(object sender, EventArgs e)
        {
            ((IMovableGameItem)sender).Parent.SetValue(Canvas.LeftProperty, ((IMovableGameItem)sender).Location.X);
            ((IMovableGameItem)sender).Parent.SetValue(Canvas.TopProperty, ((IMovableGameItem)sender).Location.Y);
            _vm.OnPropertyChanges(null);

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
                UnselectAllElements();
                SetAllCurrentObjectsToNull();
              //  spSurfaces.Width = 1000;
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
                UnselectAllElements();
                SetAllCurrentObjectsToNull();
                //Rectangle r = GetCurrentEnemysSize( null, 
                _vm.CurrentSelection = new Enemy(null, "Airplane", new Point(0, 0), 0, 0);

            //    spEnemies.Width = 1000;
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
                UnselectAllElements();
                SetAllCurrentObjectsToNull();
                _vm.CurrentSelection = new GameObject(null, "Fluff", new Point(0, 0));
             //   spObjects.Width = 1000;
            }
        }

        private void SetAllCurrentObjectsToNull()
        {
            _vm.CurrentSelection = null;


        //    spEnemies.Width = 0;
        //    spSurfaces.Width = 0;
        //    spObjects.Width = 0;
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
            btnSurfacesMode.Foreground = _buttonUnselectedTextColor;
            btnSurfacesMode.Background = _buttonUnselectedBackColor;

            btnObjects.Foreground = _buttonUnselectedTextColor;
            btnObjects.Background = _buttonUnselectedBackColor;

            btnEnemyMode.Foreground = _buttonUnselectedTextColor;
            btnEnemyMode.Background = _buttonUnselectedBackColor;

         //   spEnemies.Width = 0;
         //   spSurfaces.Width = 0;
        }



        private void btnIncreaseWidth_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentSurface != null)
                CurrentSurface.Width += BASIC_UNIT;
        }

        private void btnDecreaseWidth_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurface != null)
                CurrentSurface.Width -= BASIC_UNIT;
        }

        private void btnIncreaseHeight_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurface != null)
                CurrentSurface.Height += BASIC_UNIT;
        }

        private void btnDecreaseHeight_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurface != null)
                CurrentSurface.Height -= BASIC_UNIT;
        }

        private void btnIncreaseX_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurface != null)
                CurrentSurface.SetValue(Canvas.LeftProperty, Convert.ToDouble(CurrentSurface.GetValue(Canvas.LeftProperty)) + (double)BASIC_UNIT);
        }

        private void btnDecreaseX_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurface != null)
                CurrentSurface.SetValue(Canvas.LeftProperty, Convert.ToDouble(CurrentSurface.GetValue(Canvas.LeftProperty)) - (double)BASIC_UNIT);
        }

        private void btnDecreaseY_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurface != null)
                CurrentSurface.SetValue(Canvas.TopProperty, Convert.ToDouble(CurrentSurface.GetValue(Canvas.TopProperty)) - (double)BASIC_UNIT);
        }

        private void btnIncreaseY_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurface != null)
                CurrentSurface.SetValue(Canvas.TopProperty, Convert.ToDouble(CurrentSurface.GetValue(Canvas.TopProperty)) + (double)BASIC_UNIT);
        }


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            cnvsMap.Children.Clear();
            txtLevelName.Text = "{NEW}";
            txtLevelWidth.Text = "1250";
            txtLevelHeight.Text = "750";
            DrawGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofg = new SaveFileDialog();

            ofg.AddExtension = true;
            ofg.DefaultExt = "*.xml";
            ofg.FileName = txtLevelName.Text;
            ofg.Filter = "XML (.xml)|*.xml"; // Filter files by extension 
            if (ofg.ShowDialog(this) == false)
            {
                return;
            }

            LevelParser.WriteLevel(ofg.FileName, _vm.BackgroundColor, _backgrounds, _surfaces, _gameObjects, _enemies, _vm.LevelWidth, _vm.LevelHeight);

        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofg = new OpenFileDialog();

            ofg.AddExtension = true;
            ofg.DefaultExt = "*.xml";
            ofg.FileName = txtLevelName.Text;
            ofg.Filter = "XML (.xml)|*.xml"; // Filter files by extension 
            if (ofg.ShowDialog(this) == false)
            {
                return;
            }

            if (ofg.FileName == "")
                return;

            cnvsMap.Children.Clear();

            Color backgroundColor = Colors.White;
            Size levelSize = new Size();

            LevelParser.LoadLevel(   ofg.FileName, _surfaces, _enemies, _gameObjects, _backgrounds, ref backgroundColor, ref levelSize);
            _vm.LevelWidth = (int) levelSize.Width;
            _vm.LevelHeight = (int)levelSize.Height;
            _vm.BackgroundColor = backgroundColor;
            LoadScreenElementsToUI(_surfaces, _gameObjects, _enemies, _backgrounds,  ofg.FileName);


        }




        private void LoadScreenElementsToUI(List<Surface> surfaces, List<GameObject> gameObjects, List<Enemy> enemies, List<TeddysAdventureLibrary.Background> backgrounds, string filePath)
        {

            cnvsMap.Children.Clear();

            foreach (TeddysAdventureLibrary.Background b in backgrounds)
            {


            }

            foreach( Surface s in surfaces){
                //Add to UI
                Rectangle r = s.SurfaceBounds;
                r.Tag = s;

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(string.Format("..\\..\\Images\\{0}.png", s.SurfaceTexture), UriKind.Relative));
                ib.TileMode = TileMode.Tile;
                ib.Viewport = new Rect(0, 0, ib.ImageSource.Width / r.Width, ib.ImageSource.Height / r.Height);
                r.Fill = ib;
                r.Stroke = _surfaceBrushOutline;
                r.Opacity = 1;
                cnvsMap.Children.Add(r);
            }

            foreach (GameObject go in gameObjects)
            {

                Rectangle r = go.Parent;
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("..\\..\\Images\\" + go.Name + ".png", UriKind.Relative));
                r.Fill = ib;

                r.Width = ib.ImageSource.Width;
                r.Height = ib.ImageSource.Height;
                r.Stroke = _surfaceSelectedOutline;


                go.SomethingChanged += new EventHandler(MoveableObjectChanged);
                cnvsMap.Children.Add(r);

            }

            foreach( Enemy e in enemies) {
                Rectangle r = e.Parent;
                r.Tag = e;

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("..\\..\\Images\\" + e.Name + ".png", UriKind.Relative));
                r.Fill = ib;

                r.Width = ib.ImageSource.Width;
                r.Height = ib.ImageSource.Height;
                r.Stroke = _surfaceBrushOutline;

                ((Enemy)r.Tag).SomethingChanged += new EventHandler(MoveableObjectChanged);
                cnvsMap.Children.Add(r);
            }

            txtLevelName.Text = filePath.Replace(".xml", "");
            txtLevelName.Text = txtLevelName.Text.Substring(txtLevelName.Text.LastIndexOf("\\") + 1);
        }


        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(Rectangle) ) {
                Rectangle r = (Rectangle)e.OriginalSource;

                if (r.Tag != null)
                {

                    if (CurrentEditMode != EditMode.selection)
                    {
                        CurrentEditMode = EditMode.selection;
                    }

                    SetAllCurrentObjectsToNull();
                    UnselectAllElements();

                    r.Opacity = _selectionOpacity;

                    _clickCaptured = true;
                    _clickStart = e.GetPosition(cnvsMap);


                    _vm.CurrentSelection = (IMovableGameItem)r.Tag;

                    switch (r.Tag.GetType().Name)
                    {
                        case "Enemy":
                            MoveableObjectChanged(r.Tag, null);
                            break;

                        case "GameObject":
                            MoveableObjectChanged(r.Tag, null);
                            break;

                        case "Surface":
                            break;
                            
                    }

                }
            }

        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (_vm.CurrentSelection != null)
            {
                Rectangle r = _vm.CurrentSelection.Parent;

                if (_clickCaptured && r != null)
                {
                    Point p = e.GetPosition(cnvsMap);
                    double xDiff = p.X - _clickStart.X;
                    double yDiff = p.Y - _clickStart.Y;
                    _clickStart = p;

                    r.SetValue(Canvas.LeftProperty, Convert.ToDouble(r.GetValue(Canvas.LeftProperty)) + xDiff);
                    r.SetValue(Canvas.TopProperty, Convert.ToDouble(r.GetValue(Canvas.TopProperty)) + yDiff);
                }

                if ( _clickCaptured)
                {
                    _vm.CurrentSelection.Location = new Point(Convert.ToInt32(r.GetValue(Canvas.LeftProperty)), Convert.ToInt32(r.GetValue(Canvas.TopProperty)));
                }
            }

        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _clickCaptured = false;
        }

    }
}
