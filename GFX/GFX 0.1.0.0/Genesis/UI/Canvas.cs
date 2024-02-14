using BulletSharp;
using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    public class Canvas
    {
        public String Name { get; set; }
        public List<Widget> Widgets { get; set; }
        public Vec3 Location { get; set; }
        public Vec3 Size { get; set; }
        public bool Enabled { get; set; } = true;
        private Game game;

        /// <summary>
        /// Creates a new canvas instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public Canvas(String name, Vec3 location, Vec3 size)
        {
            Name = name;
            Widgets = new List<Widget>();
            Location = location;
            Size = size;
        }

        /// <summary>
        /// Adds a entity to the canvas
        /// </summary>
        /// <param name="entity"></param>
        public void AddWidget(Widget entity)
        {
            Widgets.Add(entity);
        }

        /// <summary>
        /// Initial the canvas
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        public void OnInit(Game game, Scene scene)
        {
            this.game = game;
            foreach (var item in Widgets)
            {
                item.OnInit(game, scene, this);
            }
        }

        /// <summary>
        /// Update the canvas
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        public void OnUpdate(Game game, Scene scene)
        {
            if(this.Enabled)
            {
                foreach (var item in Widgets)
                {
                    if(item.Enabled)
                    {
                        item.OnUpdate(game, scene, this);
                    }
                }
            }
        }

        /// <summary>
        /// Renders the canvas
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        /// <param name="scene"></param>
        public void OnRender(Game game, IRenderDevice renderDevice, Scene scene)
        {
            if(this.Enabled)
            {
                foreach (var item in Widgets)
                {
                    if (item.Enabled)
                    {
                        item.OnRender(game, renderDevice, scene, this);
                    }
                }
            }
        }

        /// <summary>
        /// Dispose the canvas
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        public void OnDispose(Game game, Scene scene)
        {
            foreach (var item in Widgets)
            {
                item.OnDispose(game, scene, this);
            }
        }

        /// <summary>
        /// Return a widget with the specified name, searching within entities and their children.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The widget with the specified name, or null if not found.</returns>
        public Widget GetWidget(String name)
        {
            Widget widget = null;
            foreach (var entity in Widgets)
            {
                if(entity.Name.Equals(name))
                {
                    return entity;
                }
                widget = entity.FindChildren(name);
                if(widget != null)
                {
                    return widget;
                }
            }
            return widget;
        }

        /// <summary>
        /// Returns the bounds from the canvas
        /// </summary>
        /// <returns></returns>
        public Rect GetBounds()
        {
            return new Rect(Location.X, Location.Y, Size.X, Size.Y);
        }

        /// <summary>
        /// Returns the screen bounds from the canvas
        /// </summary>
        /// <returns></returns>
        public Rect GetScreenBounds()
        {
            return new Rect(this.Location.X, this.Location.Y, Size.X, Size.Y);
        }

        /// <summary>
        /// Centers the widget in the middle of the canvas
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="widget"></param>
        public static void CenterWidget(Canvas canvas, Widget widget)
        {
            float cX = canvas.Size.X / 2;
            float cY = canvas.Size.Y / 2;

            float wX = cX - (widget.Size.X / 2);
            float wY = cY - (widget.Size.Y / 2);

            widget.Location.Set(wX, wY, 0f);
        }

        /// <summary>
        /// Loads a canvas from a specified JSON file using a UI parser and asset manager.
        /// </summary>
        /// <param name="File">Path to the JSON file containing canvas configuration.</param>
        /// <param name="assets">Asset manager for managing resources such as images, fonts, etc.</param>
        /// <param name="parser">UI parser that contains the logic for parsing the JSON data.</param>
        /// <returns>The created canvas based on the JSON configuration.</returns>
        public static Canvas LoadCanvas(String File, AssetManager assets, UIParser parser)
        {
            var jObject = JObject.Parse(System.IO.File.ReadAllText(File));
            var canvasObject = jObject["Canvas"];
            var widgets = canvasObject["widgets"];
            var name = canvasObject["name"].ToString();
            var location = new Vec3(canvasObject["x"].Value<float>(), canvasObject["y"].Value<float>());
            var size = new Vec3(canvasObject["width"].Value<float>(), canvasObject["height"].Value<float>());

            Canvas canvas = new Canvas(name, location, size);

            foreach (JObject item in widgets)
            {
                var widget = LoadWidget(item, assets, parser);
                if(widget != null)
                {
                    canvas.AddWidget(widget);
                } 
            }

            return canvas;
        }

        /// <summary>
        /// Loads a widget from provided JSON data using a UI parser and asset manager.
        /// </summary>
        /// <param name="data">JSON data containing the configuration of the widget to be created.</param>
        /// <param name="assets">Asset manager for managing resources such as images, fonts, etc.</param>
        /// <param name="parser">UI parser that contains the logic for parsing the JSON data.</param>
        /// <returns>The created widget based on the JSON data.</returns>
        public static Widget LoadWidget(JObject data, AssetManager assets, UIParser parser)
        {
            Widget widget = null;
            foreach (var p in parser.UIHandler)
            {
                if (p.Key.Equals(data["type"].ToString()))
                {
                    widget = p.Value(data, assets);
                    break;
                }
            }

            if (data["childs"] != null)
            {
                foreach (JObject item in data["childs"])
                {
                    var child = LoadWidget(item, assets, parser);
                    if (child != null)
                    {
                        widget.AddChildren(child);
                    }
                }
            }

            return widget;
        }

    }
}
