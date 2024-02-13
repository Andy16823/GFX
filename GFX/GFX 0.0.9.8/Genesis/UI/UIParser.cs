using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Genesis.UI
{
    public delegate Widget UIParseHandler(JObject data, AssetManager assets);

    /// <summary>
    /// This class provides funtions to parse an json object as an widget.
    /// </summary>
    public class UIParser
    {
        public Dictionary<String, UIParseHandler> UIHandler { get; set; }

        public UIParser()
        {
            this.UIHandler = new Dictionary<string, UIParseHandler>();

            // ImageButton
            this.UIHandler.Add("ImageButton", new UIParseHandler((data, assets) =>
            {
                var name = data["name"].ToString();
                var baseTexture = assets.GetTexture(data["baseTexture"].ToString());
                var hoverTexture = assets.GetTexture(data["hoverTexture"].ToString());
                var location = new Vec3(data["x"].Value<float>(), data["y"].Value<float>());
                var size = new Vec3(data["width"].Value<float>(), data["height"].Value<float>());
                var widget = new ImageButton(name, location, size, baseTexture, hoverTexture);

                return widget;
            }));

            // Panel
            this.UIHandler.Add("Panel", new UIParseHandler((data, assets) =>
            {
                var name = data["name"].ToString();
                var location = new Vec3(data["x"].Value<float>(), data["y"].Value<float>());
                var size = new Vec3(data["width"].Value<float>(), data["height"].Value<float>());
                var widget = new Genesis.UI.Panel(name, location, size);

                if (data["hasBackgroundColor"] != null)
                {
                    widget.HasBackgroundColor = data["hasBackgroundColor"].Value<bool>();
                }

                if (data["backgroundColor"] != null)
                {
                    var color = System.Drawing.Color.FromArgb(data["backgroundColor"][0].Value<int>(), data["backgroundColor"][1].Value<int>(), data["backgroundColor"][2].Value<int>(), data["backgroundColor"][3].Value<int>());
                    widget.BackgroundColor = color;
                }

                if (data["backgroundImage"] != null)
                {
                    var backgroundImage = assets.GetTexture(data["backgroundImage"].ToString());
                    widget.BackgroundImage = backgroundImage;
                }

                return widget;
            }));

            // Label
            this.UIHandler.Add("Label", new UIParseHandler((data, assets) =>
            {
                var name = data["name"].ToString();
                var location = new Vec3(data["x"].Value<float>(), data["y"].Value<float>());
                var text = data["text"].ToString();
                var font = assets.GetFont(data["font"].ToString());
                var color = System.Drawing.Color.FromArgb(data["color"][0].Value<int>(), data["color"][1].Value<int>(), data["color"][2].Value<int>(), data["color"][3].Value<int>());
                var widget = new Genesis.UI.Label(name, location, text, font, color);

                return widget;
            }));

            // ProgressBar
            this.UIHandler.Add("ProgressBar", new UIParseHandler((data, assets) =>
            {
                var name = data["name"].ToString();
                var location = new Vec3(data["x"].Value<float>(), data["y"].Value<float>());
                var size = new Vec3(data["width"].Value<float>(), data["height"].Value<float>());
                var value = data["value"].Value<float>();
                var maxValue = data["maxValue"].Value<float>();
                var widget = new Genesis.UI.ProgressBar(name, location, size);
                widget.MaxValue = maxValue;
                widget.Value = value;

                return widget;
            }));
        }

    }
}
