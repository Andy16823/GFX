﻿using BulletSharp;
using BulletSharp.Math;
using Genesis.Core.Behaviors.Physics2D;
using Genesis.Core.GameElements;
using Genesis.Graphics;
using Genesis.Math;
using Genesis.Physics;
using Genesis.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public delegate void SceneEventHandler(Scene scene, Game game, IRenderDevice renderDevice);
    public delegate void SceneSizeEvenHandler(Scene scene, Viewport viewport);

    /// <summary>
    /// Represents a game scene in the Genesis framework.
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// Gets or sets the name of the scene.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the list of layers within the scene.
        /// </summary>
        public List<Layer> Layer { get; set; }

        /// <summary>
        /// Gets or sets the camera used for rendering the scene.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the list of UI canvases within the scene.
        /// </summary>
        public List<Canvas> Canvas { get; set; }

        /// <summary>
        /// Gets or sets the physics handler for the scene.
        /// </summary>
        public PhysicHandler PhysicHandler { get; set; }

        /// <summary>
        /// Event handler triggered before scene preparation.
        /// </summary>
        public SceneEventHandler BeforeScenePreperation { get; set; }

        /// <summary>
        /// Event handler triggered before scene rendering.
        /// </summary>
        public SceneEventHandler BeforeSceneRender { get; set; }

        /// <summary>
        /// Event handler triggered after scene rendering.
        /// </summary>
        public SceneEventHandler AfterSceneRender { get; set; }

        /// <summary>
        /// Event handler triggered before UI canvas preparation.
        /// </summary>
        public SceneEventHandler BeforeCanvasPreperation { get; set; }

        /// <summary>
        /// Event handler triggered before UI canvas rendering.
        /// </summary>
        public SceneEventHandler BeforeCanvasRender { get; set; }

        /// <summary>
        /// Event handler triggered after UI canvas rendering.
        /// </summary>
        public SceneEventHandler AfterCanvasRender { get; set; }

        /// <summary>
        /// Event handler triggered when the scene is resized.
        /// </summary>
        public SceneSizeEvenHandler OnSceneResize { get; set; }

        /// <summary>
        /// Set or gets the background texture for the scene
        /// </summary>
        public Texture BackgroundTexture { get; set; }

        /// <summary>
        /// Creates a new game scene.
        /// </summary>
        public Scene()
        {
            Layer= new List<Layer>();
            Canvas= new List<Canvas>();
        }

        /// <summary>
        /// Creates a new game scene with the specified name.
        /// </summary>
        public Scene(String name)
        {
            this.Name= name;
            Layer= new List<Layer>();
            Canvas= new List<Canvas>();
        }

        /// <summary>
        /// Adds a layer to the scene
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(Layer layer) 
        { 
            Layer.Add(layer);
        }

        /// <summary>
        /// Adds a layer to the scene
        /// </summary>
        /// <param name="layerName"></param>
        public void AddLayer(String layerName)
        {
            Layer.Add(new Layer(layerName));
        }

        /// <summary>
        /// Adds a new ui canvas to the scene
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public Canvas AddCanvas(Canvas canvas) 
        {
            Canvas.Add(canvas);
            return canvas;
        }

        /// <summary>
        /// Removes a layer from the scene
        /// </summary>
        /// <param name="layer"></param>
        public void RemoveLayer(Layer layer)
        {
            Layer.Remove(layer);
        }

        /// <summary>
        /// Gets the layer with the given name
        /// </summary>
        /// <param name="layername"></param>
        /// <returns></returns>
        public Layer GetLayer(String layername)
        {
            foreach (var item in this.Layer)
            {
                if(item.Name.Equals(layername))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a GameElement in the scene. It will be placed in the given layer
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="gameElement"></param>
        public void AddGameElement(String layerName, GameElement gameElement)
        {
            Layer layer = GetLayer(layerName);
            if(layer != null)
            {
                gameElement.Scene = this;
                layer.Elements.Add(gameElement);
            }
        }

        /// <summary>
        /// Adds GameElements into the scene. The elements will be placed in the given layer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="gameElements"></param>
        public void AddGameElements(String layer, List<GameElement> gameElements)
        {
            foreach (var item in gameElements)
            {
                this.AddGameElement(layer, item);
            }
        }

        /// <summary>
        /// Initial the scene
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public virtual void Init(Game game, IRenderDevice renderDevice)
        {
            foreach (var item in Layer)
            {
                item.Init(game, renderDevice);
            }

            foreach (var item in Canvas)
            {
                item.OnInit(game, this);
            }
        }

        /// <summary>
        /// Update the scene and the elements. Called every frame
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public virtual void OnUpdate(Game game, IRenderDevice renderDevice) 
        {
            foreach (var item in Layer)
            {
                item.OnUpdate(game, renderDevice);
            }

            foreach (var item in Canvas)
            {
                item.OnUpdate(game, this);
            }

            if (this.PhysicHandler != null)
            {
                this.PhysicHandler.Process(this, game);
            }
        }

        /// <summary>
        /// Renders the scene
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        public virtual void OnRender(Game game, IRenderDevice renderDevice)
        {
            if(this.Camera != null)
                renderDevice.SetCamera(game.Viewport, this.Camera);

            // Before scene preperation even
            if (this.BeforeScenePreperation != null) 
                this.BeforeScenePreperation(this, game, renderDevice);

            // Scene rendering
            renderDevice.PrepareSceneRendering(this);

            if (this.BeforeSceneRender != null)
                this.BeforeSceneRender(this, game, renderDevice);

            foreach (var item in Layer)
            {
                item.OnRender(game, renderDevice);
            }

            if(this.AfterSceneRender != null)
                this.AfterSceneRender(this, game, renderDevice);

            renderDevice.FinishSceneRendering(this);


            // Canvas canvas preperation
            if(this.BeforeCanvasPreperation != null)
                this.BeforeCanvasPreperation(this, game, renderDevice);

            // Cavas rendering
            renderDevice.PrepareCanvasRendering(this, null);

            if(this.BeforeCanvasRender != null)
                this.BeforeCanvasRender(this, game, renderDevice);

            foreach (var item in Canvas)
            {
                item.OnRender(game, renderDevice, this);
            }

            if(this.AfterCanvasRender != null)
                this.AfterCanvasRender(this, game, renderDevice);

            renderDevice.FinishCanvasRendering(this, null);
        }

        /// <summary>
        /// Destroys the scene data
        /// </summary>
        /// <param name="game"></param>
        public virtual void OnDestroy(Game game)
        {
            foreach(var item in Layer)
            {
                item.OnDestroy(game);
            }

            foreach (var item in Canvas)
            {
                item.OnDispose(game, this);
            }
        }

        /// <summary>
        /// Gets the elements from the given layer
        /// </summary>
        /// <param name="layername"></param>
        /// <returns></returns>
        public List<GameElement> GetElements(String layername)
        {
            foreach (var item in Layer)
            {
                if(item.Name.Equals(layername))
                {
                    return item.Elements;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the elements from the given layers
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public List<GameElement> GetElements(String[] layer)
        {
            List<GameElement> elements = new List<GameElement>();
            foreach (var item in Layer) { 
                if(layer.Contains(item.Name))
                {
                    elements.AddRange(item.Elements);
                }
            }
            return elements;
        }

        /// <summary>
        /// Gets the element with the given name. This function searchs
        /// in every layer until it finds a element with an equal name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameElement GetElement(String name)
        {
            foreach (var layer in Layer)
            {
                foreach (var element in layer.Elements)
                {
                    if(element.Name.Equals(name))
                    {
                        return element;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the element with the given name out of the given layer.
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameElement GetElement(String layerName, String name)
        {
            Layer layer = this.GetLayer(layerName);
            if(layer != null)
            {
                foreach (var element in layer.Elements)
                {
                    if(element.Name.Equals(name))
                    {
                        return element;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the canvas with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Canvas GetCanvas(String name)
        {
            foreach (var item in Canvas)
            {
                if(item.Name.Equals(name))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a entity with the given name from the canvas
        /// </summary>
        /// <param name="canvasName"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public Widget GetWidget(String canvasName, String entityName)
        {
            Canvas canvas = this.GetCanvas(canvasName);
            if(canvas != null)
            {
                return canvas.GetWidget(entityName);
            }
            return null;
        }

        /// <summary>
        /// Removes a element from the scene. This function will look in all 
        /// layer for the element.
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElement(GameElement element)
        {
            foreach (var item in Layer)
            {
                if(item.Elements.Contains(element))
                {
                    item.Elements.Remove(element);
                }
            }
        }

        /// <summary>
        /// Removes a element from the given layer
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="element"></param>
        public void RemoveElement(String layerName, GameElement element)
        {
            Layer layer = GetLayer(layerName);
            if(layer != null)
            {
                if(layer.Elements.Contains(element))
                {
                    layer.Elements.Remove(element);
                }
            }
        }

        /// <summary>
        /// Removes the ui canvas from the scene
        /// </summary>
        /// <param name="canvas"></param>
        public void RemoveCanvas(Canvas canvas)
        {
            this.Canvas.Remove(canvas);
        }

        /// <summary>
        /// Removes the ui canvas with the given name from the scene
        /// </summary>
        /// <param name="canvasName"></param>
        public void RemoveCanvas(String canvasName)
        {
            Canvas canvas = this.GetCanvas(canvasName);
            if(canvas != null)
            {
                this.RemoveCanvas(canvas);
            }
        }

        /// <summary>
        /// Resizes the scene by invoking the event handler for scene resize.
        /// </summary>
        /// <param name="viewport">The viewport containing information about the new dimensions.</param>
        public virtual void ResizeScene(Viewport viewport)
        {
            if(this.OnSceneResize != null)
            {
                this.OnSceneResize(this, viewport);
            }
        } 

        /// <summary>
        /// Loads the given scene file into this scene
        /// </summary>
        /// <param name="filename">The file name</param>
        /// <param name="assetManager">The asset manager</param>
        public void ImportScene2D(String filename, AssetManager assetManager)
        {
            var root = JObject.Parse(File.ReadAllText(filename));
            var bitmap = Utils.ConvertBase64ToBitmap(root["tileSets"][0]["bitmap"].Value<String>());
            var texture = assetManager.AddTexture(root["tileSets"][0]["name"].Value<String>(), bitmap);
            
            foreach (JObject jLayer in root["layer"])
            {
                var layerName = jLayer["name"].Value<String>();
                var layerType = jLayer["type"].Value<String>();

                Layer layer = new Layer(layerName);
                if (layerType.Equals("tiledLayer"))
                {
                    BufferedSprite bufferedSprite = new BufferedSprite(layerName + "_bufferedSprite", new Vec3(0, 0), texture);
                    foreach (var jElement in jLayer["elements"])
                    {
                        var name = jElement["name"].Value<String>();
                        var location = new Vec3(jElement["x"].Value<float>(), jElement["y"].Value<float>());
                        var size = new Vec3(jElement["width"].Value<float>(), jElement["height"].Value<float>());

                        var texcords = new TexCoords();
                        texcords.TopLeft = new Vec3(jElement["texCords"]["topLeftX"].Value<float>(), jElement["texCords"]["topLeftY"].Value<float>());
                        texcords.TopRight = new Vec3(jElement["texCords"]["topRightX"].Value<float>(), jElement["texCords"]["topRightY"].Value<float>());
                        texcords.BottomLeft = new Vec3(jElement["texCords"]["bottomLeftX"].Value<float>(), jElement["texCords"]["bottomLeftY"].Value<float>());
                        texcords.BottomRight = new Vec3(jElement["texCords"]["bottomRightX"].Value<float>(), jElement["texCords"]["bottomRightY"].Value<float>());

                        bufferedSprite.AddShape(location, size, texcords);
                    }

                    JArray props = (JArray)jLayer["properties"];
                    var propetery = this.GetSceneFilePropetery(props, "Collision");
                    if (propetery != null)
                    {
                        var isCollider = propetery.Value<bool>();
                        if (isCollider)
                        {
                            var collider = bufferedSprite.AddBehavior(new BufferedSpriteCollider(this.PhysicHandler));
                            collider.CreateCollider();
                        }
                        
                    }
                    layer.Elements.Add(bufferedSprite);
                }
                else if (layerType.Equals("objectLayer"))
                {
                    foreach (var jElement in jLayer["elements"])
                    {
                        var element = new Empty();
                        element.Name = jElement["name"].Value<String>();
                        element.Location = new Vec3(jElement["x"].Value<float>(), jElement["y"].Value<float>());
                        element.Size = new Vec3(jElement["width"].Value<float>(), jElement["height"].Value<float>());
                        element.Rotation = Vec3.Zero();

                        if (jElement["type"].Value<String>().Equals("RectObject"))
                        {
                            var collider = element.AddBehavior<PhysicsBox2D>(new PhysicsBox2D());
                            collider.CreateRigidbody(this.PhysicHandler, 0f);
                        }
                        else if (jElement["type"].Value<String>().Equals("PolyObject"))
                        {
                            List<float> points = new List<float>();
                            foreach (var item in jElement["points"])
                            {
                                float ptX = item["x"].Value<float>();
                                float ptY = item["y"].Value<float>();
                                points.Add(ptX);
                                points.Add(ptY);
                                points.Add(0f);
                            }
                            var collider = element.AddBehavior<PhysicsPolygon2D>(new PhysicsPolygon2D());
                            collider.CreateRigidbody(this.PhysicHandler, 0f, points.ToArray());
                        }
                        layer.Elements.Add(element);
                    }
                }
                this.Layer.Add(layer);
            }
        }

        /// <summary>
        /// Gets a scene file proptery. 
        /// </summary>
        /// <param name="array">The propteries jarray</param>
        /// <param name="key">The key for the propetery</param>
        /// <returns>the jobject if the key exist. null if the key dont exist</returns>
        private JToken GetSceneFilePropetery(JArray array, String key)
        {
            foreach (var item in array)
            {
                if (item[key] != null)
                {
                    return item[key];
                }
            }
            return null;
        }

    }
}