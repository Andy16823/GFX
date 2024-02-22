using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    public delegate void ListItemBehavior(Game game, Scene scene, ListItem item);
    public delegate void ListSelectorBehavior(Game game, Scene scene, ListSelector selector);

    /// <summary>
    /// Represents a list item with a name, text, and a callback function for selection.
    /// </summary>
    public class ListItem
    {
        /// <summary>
        /// Gets or sets the name of the list item.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the text content of the list item.
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Gets or sets the callback function invoked when the item is selected.
        /// </summary>
        public ListItemBehavior OnSelect { get; set; }

        /// <summary>
        /// Creates a new instance of the ListItem class.
        /// </summary>
        /// <param name="name">The name of the list item.</param>
        /// <param name="text">The text content of the list item.</param>
        /// <param name="func">The callback function invoked when the item is selected.</param>
        public ListItem(String name, String text, ListItemBehavior func)
        {
            this.Name = name;
            this.Text = text;
            OnSelect = func;
        }
    }

    /// <summary>
    /// Represents a list selector widget in the UI, allowing the user to navigate and select items.
    /// </summary>
    public class ListSelector : Widget
    {
        /// <summary>
        /// Gets or sets the list of items in the selector.
        /// </summary>
        public List<ListItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the index of the currently selected item.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the height of each line in the list.
        /// </summary>
        public float LineHeight { get; set; } = 20f;

        /// <summary>
        /// Gets or sets the font used for rendering text in the list.
        /// </summary>
        public Genesis.Graphics.Font Font { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text should be centered within each line.
        /// </summary>
        public bool CenterText { get; set; }

        /// <summary>
        /// Gets or sets the font size used for rendering text in the list.
        /// </summary>
        public float FontSize { get; set; } = 16.0f;

        /// <summary>
        /// Gets or sets the font spacing used for rendering text in the list.
        /// </summary>
        public float FontSpacing { get; set; } = 0.5f;

        /// <summary>
        /// Gets or sets the behavior invoked when the selected index changes.
        /// </summary>
        public ListSelectorBehavior IndexChangedBehavior { get; set; }

        private long lastSelection;

        /// <summary>
        /// Creates a new instance of the ListSelector class.
        /// </summary>
        /// <param name="name">The name of the list selector.</param>
        /// <param name="location">The location of the list selector.</param>
        /// <param name="font">The font used for rendering text in the list.</param>
        public ListSelector(String name, Vec3 location, Genesis.Graphics.Font font)
        {
            Items = new List<ListItem>();
            this.Name = name;
            this.Location = location;
            this.Size = this.CalcualteSize();
            this.Font = font;
        }

        /// <summary>
        /// Adds an item to the list selector.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(ListItem item)
        {
            this.Items.Add(item);
            this.Size = this.CalcualteSize();
        }

        /// <summary>
        /// Renders the list selector on the screen.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device.</param>
        /// <param name="scene">The scene instance.</param>
        /// <param name="canvas">The canvas containing the list selector.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            Vec3 loc = GetRelativePos(canvas);
            this.Size = this.CalcualteSize();

            if (this.Debug)
            {
                renderDevice.FillRect(new Rect(loc.X, loc.Y, Size.X, Size.Y), Color.Blue);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                ListItem item = Items[i];
                float x = loc.X;

                if(CenterText)
                {
                    float stringWidth = Utils.GetStringWidth(item.Text, FontSize, FontSpacing);
                    x = x - (stringWidth / 2);
                    Console.WriteLine(item.Text + " = " + stringWidth + " X: " + x);
                }

                float y = loc.Y - (LineHeight / 2) + (i * LineHeight);
                if(i == SelectedIndex)
                {
                    renderDevice.DrawString(item.Text, new Vec3(loc.X, y), FontSize, FontSpacing, Font, System.Drawing.Color.Yellow);
                }
                else
                {
                    renderDevice.DrawString(item.Text, new Vec3(loc.X, y), FontSize, FontSpacing, Font, System.Drawing.Color.White);
                }
                
            }
        }

        /// <summary>
        /// Updates the list selector.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="scene">The scene instance.</param>
        /// <param name="canvas">The canvas containing the list selector.</param>
        public override void OnUpdate(Game game, Scene scene, Canvas canvas)
        {
            base.OnUpdate(game, scene, canvas);
            long now = Utils.GetCurrentTimeMillis();
            if(now > lastSelection + 100)
            {
                if (Input.IsKeyDown(System.Windows.Forms.Keys.Up))
                {
                    this.SelectedIndex--;
                    if (this.SelectedIndex < 0)
                    {
                        this.SelectedIndex = Items.Count - 1;
                    }
                    if(IndexChangedBehavior != null)
                    {
                        IndexChangedBehavior(game, scene, this);
                    }
                }
                if (Input.IsKeyDown(System.Windows.Forms.Keys.Down))
                {
                    this.SelectedIndex++;
                    if (this.SelectedIndex >= Items.Count)
                    {
                        this.SelectedIndex = 0;
                    }
                    if (IndexChangedBehavior != null)
                    {
                        IndexChangedBehavior(game, scene, this);
                    }
                }
                if(Input.IsKeyDown(System.Windows.Forms.Keys.Enter))
                {
                    Items[SelectedIndex].OnSelect(game, scene, Items[SelectedIndex]);
                }
                lastSelection = now;
            }
        }

        /// <summary>
        /// Calculates the size of the list selector based on its content.
        /// </summary>
        /// <returns>The calculated size of the list selector.</returns>
        private Vec3 CalcualteSize()
        {
            float height = (this.Items.Count * LineHeight);
            float width = 0f;

            foreach( var item in this.Items )
            {
                float itemWidth = Utils.GetStringWidth(item.Text, FontSize, FontSpacing);
                if(itemWidth > width) 
                { 
                    width = itemWidth;
                }
            }

            return new Vec3(width, height);
        }

    }
}
