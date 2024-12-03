using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI.Items
{
    public abstract class IItem
    {
        public String Name { get; set; }
        public String Text { get; set; }

        public abstract void OnRender(IRenderDevice renderer, Widget parent, Rect screenBounds);
    }
}
