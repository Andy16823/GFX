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

        /// <summary>
        /// Create a new Storage instance
        /// </summary>
        public Storage()
        {
            ManagedElements = new List<GameElement>();
        }

        /// <summary>
        /// Add an element which get managed
        /// </summary>
        /// <param name="element"></param>
        public void ManageElement(GameElement element)
        {
            ManagedElements.Add(element);
        }

        /// <summary>
        /// Removes disabled elements from the game and the garbage collector
        /// </summary>
        /// <param name="game"></param>
        /// <param name="scene"></param>
        public void Process(Game game, Scene scene)
        {
            List<GameElement> etd = new List<GameElement>();
            foreach (var element in ManagedElements)
            {
                if(!element.Enabled)
                {
                    element.OnDestroy(game);
                    scene.RemoveElement(element);
                    etd.Add(element);
                }
            }
            foreach (var element in etd)
            {
                ManagedElements.Remove(element);
            }
            etd.Clear();
        }
    }
}
