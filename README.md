**GFX Game Engine 0.1.2**

Welcome to the GFX Game Engine, version 0.1.2!

**Overview:**
GFX Game Engine is a robust framework developed in C# for creating both 2D and 3D games. Designed with simplicity in mind, it facilitates easy implementation and management of scenes, game elements, and their components. The engine abstracts rendering, physics, and components to enable seamless customization.

**Key Features:**
- Streamlined scene rendering with focus on one active scene.
- Flexible layer system for adding game elements: sprites, 3D objects, and empties.
- Extensive customization using "GameBehaviors" for game logic implementation.
- Support for custom GameBehaviors and game elements via abstract classes.
- Default collection of essential elements included.
- 2D lighting for enhanced game atmosphere.
- 3D animations.
- 2D particle emitter.

**Rendering:**
GFX uses OpenGL 4.5 as the default renderer, akin to DirectX 11. Custom renderers are possible, and future plans include Vulkan and DirectX 12 integration. Supported 3D formats include Wavefront, FBX, Collada, and GLTF files.

**Physics:**
Physics simulation utilizes PhysicHandlers—PhysicHandler3D and PhysicHandler2D—leveraging BulletSharp, a Bullet Physics Wrapper. Custom PhysicHandlers can be developed as needed.

**Supported Games:**
GFX is ideal for creating 2D games across various genres. The 3D functionalities in version 0.1.2 include basic features like loading and rendering Wavefront and FBX files. However, additional features such as shadow maps are planned for future updates.

**Asset Loading:**
Textures and fonts can be loaded easily by placing them in the Resources folder, managed automatically by the Asset Manager. GFX supports texture archiving to streamline project management without the need for additional RAR files. Ensure proper configuration for file copying during the build process.

**External Libraries:**
GFX integrates open-source libraries including:
- **AssimpNet:** [AssimpNet Repository](https://bitbucket.org/Starnick/assimpnet/src/master/)
- **BulletSharp:** [BulletSharp Repository](https://bitbucket.org/Starnick/assimpnet/src/master/)
- **GlmSharp:** [GlmSharp Repository](https://github.com/Philip-Trettner/GlmSharp)
- **NetGL:** [NetGL Repository](https://github.com/Andy16823/NetGL-2023)
- **Newtonsoft.Json:** [Newtonsoft.Json Repository](https://github.com/JamesNK/Newtonsoft.Json)

See the License folder for individual licenses; the GFX Game Engine itself is licensed under MIT.

**Community and Resources:**
- **Website:** [https://gfx-engine.org](https://gfx-engine.org)
- **Forum:** [forum.gfx-engine.org](https://forum.gfx-engine.org)
- **Documentation:** [docs.gfx-engine.org](https://docs.gfx-engine.org)

**Getting Started:**
Clone the repository and consult the comprehensive documentation in the Wiki to begin developing your games with GFX.

**License:**
GFX Game Engine is licensed under the MIT License. Refer to the License folder for detailed terms.

For the latest updates and community support, visit [GFX Game Engine on GitHub](https://github.com/Andy16823/GFX/tree/main/GFX/GFX%200.1.0.1).

Let your creativity flourish with GFX Game Engine 0.1.2!
