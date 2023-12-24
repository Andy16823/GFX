using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    public delegate void ListItemBehavior(Game game, Scene scene, ListItem item);
    public delegate void ListSelectorBehavior(Game game, Scene scene, ListSelector selector);

    public class ListItem
    {
        public String Name { get; set; }
        public String Text { get; set; }
        public ListItemBehavior OnSelect { get; set; }

        public ListItem(String name, String text, ListItemBehavior func)
        {
            this.Name = name;
            this.Text = text;
            OnSelect = func;
        }
    }

    public class ListSelector : Entity
    {
        public List<ListItem> Items { get; set; }
        public int SelectedIndex { get; set; }
        public float LineHeight { get; set; } = 20f;
        public Font Font { get; set; }
        public bool CenterText { get; set; }
        public float FontSize { get; set; } = 16.0f;
        public float FontSpacing { get; set; } = 0.5f;
        public ListSelectorBehavior IndexChangedBehavior { get; set; }

        private long lastSelection;

        public ListSelector(String name, Vec3 location, Vec3 size, Font font)
        {
            Items = new List<ListItem>();
            this.Name = name;
            this.Location = location;
            this.Size = size;
            this.Font = font;
        }

        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            Vec3 loc = GetRelativePos(canvas);

            for (int i = 0; i < Items.Count; i++)
            {
                ListItem item = Items[i];
                float x = loc.X;

                if(CenterText)
                {
                    float stringWidth = Utils.GetStringWidht(item.Text, FontSize, FontSpacing);
                    x = x - (stringWidth / 2);
                }

                float y = loc.Y + (i * LineHeight);
                if(i == SelectedIndex)
                {
                    renderDevice.DrawString(item.Text, new Vec3(x, y), FontSize, FontSpacing, Font, System.Drawing.Color.Yellow);
                }
                else
                {
                    renderDevice.DrawString(item.Text, new Vec3(x, y), FontSize, FontSpacing, Font, System.Drawing.Color.White);
                }
                
            }
        }

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

    }
}
