# **GFX Game Engine 1.5.0**  
Welcome to the GFX Game Engine – an open-source framework designed to make game development simple, powerful, and accessible. With version 1.5.0, GFX now takes a big leap into 3D game development while continuing to provide a solid foundation for 2D projects.

If you have any questions about GFX, join our community on [Discord](https://discord.gg/qZRgRKedBs).
 
---

## **Overview**  
GFX Game Engine is a lightweight yet robust framework built in **C#** for developing 2D and 3D games. Whether you're an indie developer or a hobbyist, GFX empowers you with tools for seamless scene management, rendering, physics simulation, and game logic customization.  

**Why Choose GFX?**  
- Simplifies game development with intuitive tools and APIs.  
- Combines performance with flexibility for 2D and 3D workflows.  
- Open-source with MIT licensing for unlimited creative freedom.  

---

## **What's New in Version 1.5.0?**
- **Dynamic Shadows**: Add realism with soft, adjustable shadows.  
- **Specular Shader**: Reflective materials for visually polished 3D environments.  
- **New 3D Game Elements**: Now includes spheres, capsules, and compound mesh colliders.  
- **Physics Overhaul**: Enhanced collision handling, new rigidbodies, and collision groups.  
- **Performance Boosts**: Instanced rendering and raycast accuracy improvements.  

For a complete list of features, check the [release notes](https://github.com/Andy16823/GFX/releases).

---

## **Core Features**
### **Rendering**  
GFX leverages **OpenGL 4.5** for high-performance rendering, with support for custom shaders and materials. Supported 3D file formats include **Wavefront (.obj)**, **FBX**, **Collada**, and **GLTF**. Future plans include support for **Vulkan** and **DirectX 12**.  

### **Physics**  
Physics are powered by **BulletSharp**, a wrapper for the Bullet Physics library. The framework supports:  
- **PhysicHandler3D** and **PhysicHandler2D** for seamless simulations.  
- Custom physics handlers to suit advanced gameplay needs.  

### **2D & 3D Game Development**  
- Fully integrated **layer system** for managing game elements such as sprites, 3D objects, and empty game objects.  
- Support for **GameBehaviors** to simplify custom game logic implementation.  
- Ready-to-use 2D particle emitters and 2D lighting systems to bring your games to life.  

### **Asset Management**  
Easily manage textures, models, and fonts by placing them in the `Resources` folder. The **Asset Manager** ensures streamlined loading without additional dependencies.  

### **External Libraries**  
GFX integrates some of the best open-source libraries for game development:  
- **AssimpNet**: Model loading ([AssimpNet Repository](https://bitbucket.org/Starnick/assimpnet/src/master/))  
- **BulletSharp**: Physics simulation ([BulletSharp Repository](https://github.com/AndresTraks/BulletSharp))  
- **GlmSharp**: Vector and matrix operations ([GlmSharp Repository](https://github.com/Philip-Trettner/GlmSharp))  
- **NetGL**: OpenGL wrapper ([NetGL Repository](https://github.com/Andy16823/NetGL-2023))  
- **Newtonsoft.Json**: JSON parsing ([Newtonsoft.Json Repository](https://github.com/JamesNK/Newtonsoft.Json))  

Refer to the `LICENSE` folder for detailed library licenses.

---

## **Getting Started**
### **Clone the Repository**
```bash
git clone https://github.com/Andy16823/GFX.git
```

### Run the Demo Project
- A complete 3D example project is included to help you get started.
- Follow the instructions in the documentation to create your first game!

### Documentation & Resources
- Website:  [gfx-engine.org](https://gfx-engine.org)
- Forum:  [community.gfx-engine.org](https://community.gfx-engine.org) 
- Docs & Tutorials: [docs.gfx-engine.org](https://docs.gfx-engine.org) 

### Contribute to GFX
GFX Game Engine thrives on community contributions! Whether it’s reporting bugs, submitting feature requests, or contributing code, your input is always welcome. Check out our contribution guidelines to get involved.

### License
The GFX Game Engine is released under the MIT License, ensuring complete freedom for commercial and personal projects. See the LICENSE folder for full terms.
