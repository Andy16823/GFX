using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.UI
{
    /// <summary>
    /// Enum for the widget position
    /// </summary>
    public enum WidgetAnchor
    {
        TOP_LEFT,
        TOP_MID,
        TOP_RIGHT,
        MID_LEFT,
        MID_MID,
        MID_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_MID,
        BOTTOM_RIGHT,
    }

    /// <summary>
    /// Delegate for handling UI events.
    /// </summary>
    /// <param name="entity">The widget triggering the event.</param>
    /// <param name="game">The game instance.</param>
    /// <param name="scene">The current scene.</param>
    /// <param name="canvas">The canvas used for rendering.</param>
    public delegate void UIEvent(Widget entity, Game game, Scene scene, Canvas canvas);

    /// <summary>
    /// Base class for UI widgets.
    /// </summary>
    public class Widget
    {
        /// <summary>
        /// Gets or sets the name of the widget.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the location of the widget in 3D space.
        /// </summary>
        public Vec3 Location { get; set; }

        /// <summary>
        /// Gets or sets the size of the widget.
        /// </summary>
        public Vec3 Size { get; set; }

        /// <summary>
        /// Gets or sets the parent widget.
        /// </summary>
        public Widget Parent { get; set; }

        /// <summary>
        /// Gets or sets the list of child widgets.
        /// </summary>
        public List<Widget> Children { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the widget is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug mode is enabled for the widget.
        /// </summary>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets the anchor value for the widget
        /// </summary>
        public WidgetAnchor Anchor { get; set; } = WidgetAnchor.BOTTOM_LEFT;

        /// <summary>
        /// Event triggered when the mouse enters the widget.
        /// </summary>
        public event UIEvent MouseEnter;

        /// <summary>
        /// Event triggered when the mouse leaves the widget.
        /// </summary>
        public event UIEvent MouseLeave;

        /// <summary>
        /// Event triggered when the widget is clicked.
        /// </summary>
        public event UIEvent Click;

        private bool _isHover;
        private bool _isClick;

        /// <summary>
        /// Creates a new instance of the Widget class.
        /// </summary>
        public Widget()
        {
            Children = new List<Widget>();
        }

        /// <summary>
        /// Adds a child widget to the entity and sets the parent of the child entity.
        /// </summary>
        /// <param name="widget">The child widget to be added.</param>
        public void AddChildren(Widget widget) { 
            Children.Add(widget);
            widget.Parent = this;
        }

        /// <summary>
        /// Initializes the widget.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="scene">The current scene.</param>
        /// <param name="canvas">The canvas used for rendering.</param>
        public virtual void OnInit(Game game, Scene scene, Canvas canvas)
        {
            foreach (var item in Children)
            {
                item.OnInit(game, scene, canvas);
            }
        }

        /// <summary>
        /// Updates the widget.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="scene">The current scene.</param>
        /// <param name="canvas">The canvas used for rendering.</param>
        public virtual void OnUpdate(Game game, Scene scene, Canvas canvas)
        {
            if(IsHover(game, scene, canvas))
            {
                if(Input.IsKeyDown(Keys.LButton))
                {
                    if(Click != null) Click(this, game, scene, canvas);
                }
                if(!_isHover)
                {
                    if (MouseEnter != null)  MouseEnter(this, game, scene, canvas);
                    _isHover = true;
                }
            }
            else
            {
                if(_isHover)
                {
                    if (MouseLeave != null)  MouseLeave(this, game, scene, canvas);  
                    _isHover = false;
                }
            }
            foreach (var item in Children)
            {
                item.OnUpdate(game, scene, canvas);
            }
        }

        /// <summary>
        /// Renders the widget.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device used for rendering.</param>
        /// <param name="scene">The current scene.</param>
        /// <param name="canvas">The canvas used for rendering.</param>
        public virtual void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            foreach (var item in Children)
            {
                item.OnRender(game, renderDevice, scene, canvas);
            }
        }

        /// <summary>
        /// Disposes the widget.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="scene">The current scene.</param>
        /// <param name="canvas">The canvas used for rendering.</param>
        public virtual void OnDispose(Game game, Scene scene, Canvas canvas)
        {
            foreach (var item in Children)
            {
                item.OnDispose(game, scene, canvas);
            }
        }

        /// <summary>
        /// Gets the relative position of the widget on the screen.
        /// </summary>
        /// <param name="canvas">The canvas used for rendering.</param>
        /// <returns>The relative position of the widget.</returns>
        public Vec3 GetRelativePos(Canvas canvas)
        {
            Rect cRect = canvas.GetScreenBounds();
            Vec3 loc = new Vec3(cRect.X, cRect.Y);
            if(Parent != null)
            {
                Vec3 pLoc = Parent.GetRelativePos(canvas);
                loc.Add(Parent.Location);
            }
            loc.X += Location.X + (Size.X / 2);
            loc.Y += Location.Y + (Size.Y / 2);

            return loc;
        }

        /// <summary>
        /// Returns the child widget with the specified name.
        /// </summary>
        /// <param name="name">The name of the child widget to find.</param>
        /// <returns>The child widget with the specified name, or null if not found.</returns>
        public Widget GetChildren(String name)
        {
            foreach (var entity in Children)
            {
                if(entity.Name.Equals(name))
                {
                    return entity;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the relative bounds of the widget on the canvas.
        /// </summary>
        /// <param name="canvas">The canvas used for rendering.</param>
        /// <returns>The relative bounds of the widget.</returns>
        public Rect GetRelativeBounds2D(Canvas canvas)
        {
            Vec3 relativeLocation = this.GetRelativePos(canvas);
            var source = new Rect(relativeLocation.X - Size.X / 2, relativeLocation.Y - Size.Y / 2, Size.X, Size.Y);


            //return new Rect(relativeLocation.X, relativeLocation.Y, Size.X, Size.Y);
            return TransformBounds(source, this.Anchor);
        }

        /// <summary>
        /// Checks if the mouse is hovering over the widget.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="scene">The current scene.</param>
        /// <param name="canvas">The canvas used for rendering.</param>
        /// <returns>True if the mouse is hovering over the widget, otherwise false.</returns>
        public bool IsHover(Game game, Scene scene, Canvas canvas)
        {
            Vec3 mouse = Input.GetRefMousePos(game);
            Rect rect = GetRelativeBounds2D(canvas);

            if (rect.Contains(mouse.X, mouse.Y))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Recursively finds a child widget with the given name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The widget with the specified name, or null if not found.</returns>
        public Widget FindChildren(String name)
        {
            Widget widget = null;
            foreach (var item in this.Children)
            {
                if(item.Name.Equals(name))
                {
                    return item;
                }
                widget = item.FindChildren(name);
                if(widget != null)
                {
                    return widget;
                }
            }
            return widget;
        }

        /// <summary>
        /// Transform the bounds from the bottom left point to any anchor position
        /// </summary>
        /// <param name="source"></param>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public static Rect TransformBounds(Rect source, WidgetAnchor anchor)
        {
            switch (anchor)
            {
                case WidgetAnchor.TOP_LEFT:
                    return new Rect(source.X, source.Y - source.Height, source.Width, source.Height);
                case WidgetAnchor.TOP_MID:
                    return new Rect(source.X - (source.Width / 2), source.Y - source.Height, source.Width, source.Height);
                case WidgetAnchor.TOP_RIGHT:
                    return new Rect(source.X - source.Width, source.Y - source.Height, source.Width, source.Height);
                case WidgetAnchor.MID_LEFT:
                    return new Rect(source.X, source.Y - (source.Height / 2), source.Width, source.Height);
                case WidgetAnchor.MID_MID:
                    return new Rect(source.X - (source.Width / 2), source.Y - (source.Height / 2), source.Width, source.Height);
                case WidgetAnchor.MID_RIGHT:
                    return new Rect(source.X - source.Width, source.Y - (source.Height / 2), source.Width, source.Height);
                case WidgetAnchor.BOTTOM_LEFT:
                    return source;
                case WidgetAnchor.BOTTOM_MID:
                    return new Rect(source.X - (source.Width / 2), source.Y, source.Width, source.Height);
                case WidgetAnchor.BOTTOM_RIGHT:
                    return new Rect(source.X - source.Width, source.Y, source.Width, source.Height);
                default:
                    return source;
            }
        }

        /// <summary>
        /// Checks if the widget is hovered
        /// </summary>
        /// <returns></returns>
        public bool Hovered()
        {
            return _isHover;
        }

    }
}
