**Genesis Game Engine 0.9.9.9**

Welcome to the Genesis Game Engine, version 0.1.0.0!

**Overview:**
The Genesis Game Engine is a powerful framework built in C# for creating both 2D and 3D games. Designed with a focus on simplicity, it provides easy implementation and management of scenes, game elements, and their components. The engine abstracts rendering, physics, and components to facilitate seamless customization.

**Key Features:**
- Streamlined scene rendering with a single active scene at a time.
- Flexible layer system for adding game elements, including sprites, 3D objects, and empties.
- Extensive customization through the use of "GameBehaviors" to implement game logic.
- Support for custom GameBehaviors and game elements using abstract classes.
- Default collection of essential elements included with the engine.

**Rendering:**

Genesis currently employs OpenGL 4.5 as the default rendering device, comparable to DirectX 11. Custom renderers can be implemented, and future plans include integration with Vulkan and DirectX 12. 3D format support includes Wavefront (.obj) files.

**Physics:**

Physics simulation within the game world is handled by PhysicHandlers, featuring PhysicHandler3D and PhysicHandler2D using BulletSharp, a Bullet Physics Wrapper. Custom PhysicHandlers can also be created.

**Supported Games:**

Genesis is ideal for creating 2D games across various genres. While 3D support is under development in version 0.9.9.9, some features such as animations, loading FBX objects, and efficient shadow implementation are still pending. Future updates will address these aspects.

**Current 3D Features:**

Version 0.9.9.9 includes basic 3D functionality like loading and rendering Wavefront (.obj) files and foundational physics simulation (collision, raycasts).

**Asset Loading:**

Textures and fonts can be easily loaded by placing them in the Resources folder, automatically managed by the Asset Manager. Genesis supports archiving textures to avoid including RAR files with the project. Ensure proper configuration for file copying during the build process.

**External Libraries:**

Genesis utilizes open-source libraries such as glmSharp, bulletSharp, NetGL, and OpenObjectLoader. Refer to the License folder for individual licenses; the project is licensed under MIT.

**Getting Started:**

Clone the repository and explore the extensive documentation in the Wiki to start building your games with Genesis.

**License:**

Genesis Game Engine is licensed under the MIT License. See the License folder for details.

For the latest updates and community support, visit [Genesis Game Engine on GitHub](https://github.com/Andy16823/GFX/tree/main/GFX/GFX%200.1.0.0).

Let your creativity thrive with Genesis Game Engine 0.1.0.0!
