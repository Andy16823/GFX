using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Class for dynamic clearing not used elements.
    /// </summary>
    public class Storage
    {
        public List<GameElement> ManagedElements { get; set; }

        public Storage()
        {
            ManagedElements = new List<GameElement>();
        }

        public void ManageElement(GameElement element)
        {
            ManagedElements.Add(element);
        }

        public void Process(Game game, Scene scene)
        {
            List<GameElement> etd = new List<GameElement>();
            foreach (var element in ManagedElements)
            {
                if(!element.Enabled)
                {
                    scene.RemoveElement(element);
                    etd.Add(element);
                }
            }
            etd.Clear();
        }
    }
}
