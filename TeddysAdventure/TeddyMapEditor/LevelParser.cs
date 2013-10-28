using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeddysAdventureLibrary;

namespace TeddyMapEditor
{
    class LevelParser
    {
        public static void WriteLevel(string fileName, Color backgroundColor, List<Background> backgrounds, List<Surface> surfaces, List<GameObject> gameObjects, List<Enemy> enemies, int levelWidth, int levelHeight)
        {

            if (fileName == "")
                return;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = Environment.NewLine;

            using (var screenWriter = System.Xml.XmlTextWriter.Create(  new System.IO.StreamWriter(fileName), settings))
            {
                System.Xml.XmlDocument screenDoc = new System.Xml.XmlDocument();

                XmlNode xnaContent = screenDoc.CreateElement("XnaContent");
                screenDoc.AppendChild(xnaContent);

                XmlNode assetNode = screenDoc.CreateElement("Asset");
                xnaContent.AppendChild(assetNode);

                assetNode.Attributes.Append(screenDoc.CreateAttribute("Type"));
                assetNode.Attributes[0].Value = "TeddysAdventureLibrary.ScreenHelper";

                XmlNode teddyStartNode = screenDoc.CreateElement("TeddyStart");

                foreach (GameObject i in gameObjects)
                {
                    if (i.Name == "TeddyStart")
                    {
                        teddyStartNode.InnerText = i.Location.X.ToString() + " " + i.Location.Y.ToString();
                    }
                }

                if (teddyStartNode.InnerText == "")
                {
                    teddyStartNode.InnerText = "0 0";
                }
                
                assetNode.AppendChild(teddyStartNode);

                XmlNode backgroundColorNode = screenDoc.CreateElement("BackgroundColor");
                backgroundColorNode.InnerText = HexColorConverter(backgroundColor);
                assetNode.AppendChild(backgroundColorNode);


                XmlNode backgroundsNode = screenDoc.CreateElement("Backgrounds");
                foreach (Background i in backgrounds)
                {
                    var itemDoc = new XmlDocument();
                    itemDoc.LoadXml(i.GetXMLString());
                    backgroundsNode.AppendChild(screenDoc.ImportNode(itemDoc.DocumentElement, true));  
                    

                }
                assetNode.AppendChild(backgroundsNode);


                XmlNode surfacesNode = screenDoc.CreateElement("Surfaces");
                foreach (Surface s in surfaces)
                {
                    var itemDoc = new XmlDocument();
                    itemDoc.LoadXml( s.GetXMLString() );
                    surfacesNode.AppendChild(  screenDoc.ImportNode(  itemDoc.DocumentElement, true));         
                }
                assetNode.AppendChild(surfacesNode);


                XmlNode gameObjectsNode = screenDoc.CreateElement("ListOfObjects");
                foreach (GameObject i in gameObjects)
                {
                    if (i.Name != "TeddyStart")
                    {
                        var itemDoc = new XmlDocument();
                        itemDoc.LoadXml(i.GetXMLString());
                        gameObjectsNode.AppendChild(screenDoc.ImportNode(itemDoc.DocumentElement, true));   
                    }
                }
                assetNode.AppendChild(gameObjectsNode);

                XmlNode enemiesNode = screenDoc.CreateElement("ListOfEnemies");
                foreach (Enemy i in enemies)
                {
                    var itemDoc = new XmlDocument();
                    itemDoc.LoadXml( i.GetXMLString() );
                    enemiesNode.AppendChild(screenDoc.ImportNode(itemDoc.DocumentElement, true));         
                }
                assetNode.AppendChild(enemiesNode);

                XmlNode levelTypeNode = screenDoc.CreateElement("LevelType");
                levelTypeNode.InnerText = "Normal";
                assetNode.AppendChild(levelTypeNode);

                XmlNode levelSizeNode = screenDoc.CreateElement("LevelSize");
                levelSizeNode.InnerText = string.Format("{0} {1}", levelWidth.ToString(), levelHeight.ToString());
                assetNode.AppendChild(levelSizeNode);


                screenDoc.WriteTo(screenWriter);
            }

        }

        private static String HexColorConverter(Color c)
        {
            return  c.A.ToString("X2") + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static void LoadLevel(string filePath, List<Surface> surfaces, List<Enemy> enemies, List<GameObject> gameObjects, List<TeddysAdventureLibrary.Background> backgrounds, ref Color backgroundColor, ref Size levelSize)
        {

            surfaces.Clear();
            enemies.Clear();
            gameObjects.Clear();
            backgrounds.Clear();

            using (System.Xml.XmlReader screenReader = System.Xml.XmlReader.Create(new System.IO.StreamReader(filePath)))
            {
                System.Xml.XmlDocument screenDoc = new System.Xml.XmlDocument();
                screenDoc.Load(screenReader);

                //TeddyStart
                XmlNodeList teddyStartNode = ((XmlNodeList)screenDoc.GetElementsByTagName("TeddyStart"));
                if (teddyStartNode.Count > 0)
                {
                    string sPosition = teddyStartNode.Item(0).InnerText;

                    List<string> lvecparts = sPosition.Split(' ').ToList();
                    Point p = new Point(Double.Parse(lvecparts[0]), Double.Parse(lvecparts[1]));

                    Rectangle r = new Rectangle();

                    int width = 50;
                    int height = 75;
                    //   GetCurrentObjectSize(sType, ref width, ref height);
                    r.Width = width;
                    r.Height = height;
                    // r.Stroke = _surfaceSelectedOutline;
                    r.StrokeThickness = .5;
                    r.Fill = Brushes.GhostWhite;

                    r.SetValue(Canvas.LeftProperty, p.X);
                    r.SetValue(Canvas.TopProperty, p.Y);

                    var go = new GameObject(r, "TeddyStart", p);
                    r.Tag = go;

                    gameObjects.Add(go);
                }

                //Background Color
                XmlNodeList backgroundColorNode = ((XmlNodeList)screenDoc.GetElementsByTagName("BackgroundColor"));
                if (backgroundColorNode.Count > 0)
                {
                    string argbString = backgroundColorNode.Item(0).InnerText;
                    backgroundColor = ParseARGBColorString(argbString);
                }

                //Load Backgrounds
                foreach (XmlNode backgroundHeaderNode in screenDoc.GetElementsByTagName("Backgrounds"))
                {
                    foreach (XmlNode backgroundItemNode in backgroundHeaderNode.ChildNodes)
                    {
                        TeddysAdventureLibrary.Background b = LoadBackgroundFromXML(backgroundItemNode);
                        backgrounds.Add(b);
                    }
                }

                //Load Surfaces
                foreach (XmlNode surfacesHeaderNode in screenDoc.GetElementsByTagName("Surfaces"))
                {
                    foreach (XmlNode surfaceItemNode in surfacesHeaderNode.ChildNodes)
                    {
                        Surface s = LoadSurfaceFromXML(surfaceItemNode);
                        surfaces.Add(s);
                    }
                }


                foreach (XmlNode objectsHeaderNode in screenDoc.GetElementsByTagName("ListOfObjects"))
                {
                    foreach (XmlNode objectItemNode in objectsHeaderNode.ChildNodes)
                    {
                        GameObject go = LoadGameObjectFromXML(objectItemNode);
                        gameObjects.Add(go);
                    }
                }

                foreach (XmlNode enemiesHeaderNode in screenDoc.GetElementsByTagName("ListOfEnemies"))
                {
                    foreach (XmlNode enemyItemNode in enemiesHeaderNode.ChildNodes)
                    {
                        Enemy e = LoadEnemyFromXML(enemyItemNode);
                        enemies.Add(e);
                    }
                }

                XmlNodeList levelSizeNode = ((XmlNodeList)screenDoc.GetElementsByTagName("LevelSize"));
                if (levelSizeNode.Count > 0)
                {
                    List<string> levelSizeString = levelSizeNode.Item(0).InnerText.Trim().Split(" ".ToArray()).ToList();
                    levelSize = new Size(Int32.Parse(levelSizeString[0]), Int32.Parse(levelSizeString[1]));
                }
            }

        }

        private enum SurfaceElementsEnum
        {
            Rect = 0,
            Sprite = 1
        }

        private enum ObjectElementEnum
        {
            Type = 0,
            Position = 1,
            Velocity = 2,
            IsSpawnPoint = 3,
            SpawnInterval = 4
        }

        private enum BackgroundElementsEnum
        {
            Image = 0,
            RepeatX = 1,
            RepeatY = 2,
            Scrolls = 3,
            Offset = 4
        }
        
        private static TeddysAdventureLibrary.Background LoadBackgroundFromXML(XmlNode backgroundItemNode)
        {
            string simageName = backgroundItemNode.ChildNodes.Item((int)BackgroundElementsEnum.Image).InnerText;
            string sRepeatX = backgroundItemNode.ChildNodes.Item((int)BackgroundElementsEnum.RepeatX).InnerText;
            string sRepeatY = backgroundItemNode.ChildNodes.Item((int)BackgroundElementsEnum.RepeatY).InnerText;
            string sScrolls = backgroundItemNode.ChildNodes.Item((int)BackgroundElementsEnum.Scrolls).InnerText;
            string sOffset = backgroundItemNode.ChildNodes.Item((int)BackgroundElementsEnum.Offset).InnerText;

            var b = new TeddysAdventureLibrary.Background();

            b.BackgroundName = simageName;
            b.RepeatX = (sRepeatX == "1" ? true : false);
            b.RepeatY = (sRepeatY == "1" ? true : false);
            b.Scrolls = (sScrolls == "1" ? true : false);

            string[] offsets = sOffset.Split(" ".ToArray());

            int offsetX = Convert.ToInt32(offsets[0]);
            int offsetY = Convert.ToInt32(offsets[1]);
            b.SetOffsets(offsetX, offsetY);

            return b;
        }

        private static Surface LoadSurfaceFromXML(XmlNode surfaceItemNode)
        {
            //Create each surface item
            string sRect = surfaceItemNode.ChildNodes.Item((int)SurfaceElementsEnum.Rect).InnerText;
            string sSprite = surfaceItemNode.ChildNodes.Item((int)SurfaceElementsEnum.Sprite).InnerText;

            //Parse Rect string into Rectangle object
            List<string> lrecparts = sRect.Split(" ".ToArray()).ToList();
            Rectangle r = new Rectangle();
            r.Width = Convert.ToInt32(lrecparts[2]);
            r.Height = Convert.ToInt32(lrecparts[3]);
           // r.Stroke = _surfaceBrushOutline;
            r.StrokeThickness = 1;
            r.SetValue(Canvas.LeftProperty, Convert.ToDouble(lrecparts[0]));
            r.SetValue(Canvas.TopProperty, Convert.ToDouble(lrecparts[1]));

            Surface s = new Surface() { SurfaceTexture = sSprite, SurfaceBounds = r };

            return s;
        }

        private static GameObject LoadGameObjectFromXML(XmlNode objectItemNode)
        {

            string sType = objectItemNode.ChildNodes.Item((int)ObjectElementEnum.Type).InnerText;
            string sPosition = objectItemNode.ChildNodes.Item((int)ObjectElementEnum.Position).InnerText;

            List<string> lvecparts = sPosition.Split(' ').ToList();
            Point p = new Point(Int32.Parse(lvecparts[0]), Int32.Parse(lvecparts[1]));

            Rectangle r = new Rectangle();

            int width = 0;
            int height = 0;
         //   GetCurrentObjectSize(sType, ref width, ref height);
            r.Width = width;
            r.Height = height;
           // r.Stroke = _surfaceSelectedOutline;
            r.StrokeThickness = .5;
            r.Fill = Brushes.GhostWhite;

            r.SetValue(Canvas.LeftProperty, p.X);
            r.SetValue(Canvas.TopProperty, p.Y);
            
            var go = new GameObject(r,  sType, p);
            r.Tag = go;
            
            return go;
        }

        private static Enemy LoadEnemyFromXML(XmlNode enemyItemNode)
        {

            string sType = enemyItemNode.ChildNodes.Item((int)ObjectElementEnum.Type).InnerText;
            List<string> sPosition = enemyItemNode.ChildNodes.Item((int)ObjectElementEnum.Position).InnerText.Split(" ".ToArray()).ToList(); ;
            List<string> sVelocity = enemyItemNode.ChildNodes.Item((int)ObjectElementEnum.Velocity).InnerText.Split(" ".ToArray()).ToList(); ;
            string sSpawnPoint = enemyItemNode.ChildNodes.Item((int)ObjectElementEnum.IsSpawnPoint).InnerText;
            string sSpawnInterval = enemyItemNode.ChildNodes.Item((int)ObjectElementEnum.SpawnInterval).InnerText;


            Point p = new Point(Convert.ToDouble(sPosition[0]), Convert.ToDouble(sPosition[1]));
            Rectangle r = new Rectangle();
            int width = 0;
            int height = 0;
            r.Width = width;
            r.Height = height;
            r.StrokeThickness = 1;
            r.Fill = Brushes.Beige;

            r.SetValue(Canvas.LeftProperty, p.X);
            r.SetValue(Canvas.TopProperty, p.Y);

            Enemy e = new Enemy(r,  sType, p, Convert.ToSingle(sVelocity[0]), Convert.ToSingle(sVelocity[1]));
            e.IsSpawnPoint = (sSpawnPoint == "1" ? true : false);

            Int32 interval;
            Int32.TryParse(sSpawnInterval, out interval);
            e.SpawnInterval = interval;

            return e;
        }

        private static Color ParseARGBColorString(string argbString)
        {
            byte a = byte.Parse(argbString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte r = byte.Parse(argbString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(argbString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(argbString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

    }
}
