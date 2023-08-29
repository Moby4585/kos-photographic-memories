using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Newtonsoft.Json.Linq;
using OpenTK.Graphics.OpenGL;
using Vintagestory.API.Server;
using Vintagestory;
using System;
using Vintagestory.API.Config;

namespace kosphotography
{
    /// <summary>
    /// Makes you all happy and giddy when you hold a chick in your hands
    /// A sample on how to load your own custom shader and how to render with it with a quad model during the ortho/2d gui render pass
    /// </summary>
    public class ShaderSysMod : ModSystem
    {
        ICoreClientAPI capi;
        IShaderProgram overlayShaderProg;
        CameraAimRenderer renderer;

        public static bool didJustSnapped = false;
        public static bool isTakingPicture = false;

        public static bool settingsLoaded = false;

        public override bool ShouldLoad(EnumAppSide side)
        {
            return side == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            this.capi = api;

            api.Event.ReloadShader += LoadShader;
            LoadShader();

            renderer = new CameraAimRenderer(api, overlayShaderProg);
            api.Event.RegisterRenderer(renderer, EnumRenderStage.Ortho);

            //gameTickListenerID = api.Event.RegisterGameTickListener(TryRegister, 100);
        }

        public bool LoadShader()
        {
            overlayShaderProg = capi.Shader.NewShaderProgram();
            overlayShaderProg.VertexShader = capi.Shader.NewShader(EnumShaderType.VertexShader);
            overlayShaderProg.FragmentShader = capi.Shader.NewShader(EnumShaderType.FragmentShader);

            overlayShaderProg.VertexShader.Code = GetVertexShaderCode();
            overlayShaderProg.FragmentShader.Code = GetFragmentShaderCode();

            capi.Shader.RegisterMemoryShaderProgram("exampleoverlay", overlayShaderProg);
            //overlayShaderProg.PrepareUniformLocations("time");
            overlayShaderProg.Compile();

            if (renderer != null)
            {
                renderer.overlayShaderProg = overlayShaderProg;
            }

            return true;
        }


        #region Shader Code

        public string GetVertexShaderCode()
        {
            return @"
#version 330 core
#extension GL_ARB_explicit_attrib_location: enable

layout(location = 0) in vec3 vertex;

out vec2 uv;

void main(void)
{
    gl_Position = vec4(vertex.xy, 0, 1);
    uv = (vertex.xy + 1.0) / 2.0;
}
";
        }

        public string GetFragmentShaderCode()
        {
            return @"
#version 330 core

in vec2 uv;

out vec4 outColor;

uniform float time;
uniform float xs;
uniform float ys; // texture resolution
uniform int isWhite;
uniform sampler2D colorTexture;

void main () {
    vec4 color = texture2D(colorTexture, uv);

    if (uv[0] * xs < (xs/2) - (ys/2) - 2 || uv[0] * xs > (xs/2) + (ys/2) + 2) {
        color = color * 0.2;
    }

    outColor = (isWhite == 1) ? vec4(1, 1, 1, 1) : color;
}
";
        }

        #endregion

    }

    public class CameraAimRenderer : IRenderer
    {
        MeshRef quadRef;
        ICoreClientAPI capi;
        public IShaderProgram overlayShaderProg;


        public CameraAimRenderer(ICoreClientAPI capi, IShaderProgram overlayShaderProg)
        {
            this.capi = capi;
            this.overlayShaderProg = overlayShaderProg;

            MeshData quadMesh = QuadMeshUtil.GetCustomQuadModelData(-1, -1, 0, 2, 2);
            quadMesh.Rgba = null;

            quadRef = capi.Render.UploadMesh(quadMesh);
        }

        public double RenderOrder
        {
            get { return 1.1; }
        }

        public int RenderRange { get { return 1; } }

        public void Dispose()
        {
            capi.Render.DeleteMesh(quadRef);
            overlayShaderProg.Dispose();
        }

        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
        {
            ItemStack stack = capi.World.Player.InventoryManager.ActiveHotbarSlot.Itemstack;
            if (stack == null || (!(stack?.Collectible?.Attributes?.IsTrue("isCamera") ?? false))) return;
            ItemStack offHandStack = capi.World.Player.InventoryManager.GetHotbarInventory()[10].Itemstack;
            if ((offHandStack == null || !((offHandStack?.Collectible?.Code?.ToString() ?? "") == "kosphotography:photographicpaper")) && !ShaderSysMod.isTakingPicture) return;
            //if (!ShaderSysMod.isTakingPicture) return;


            IShaderProgram curShader = capi.Render.CurrentActiveShader;
            curShader?.Stop();

            overlayShaderProg.Use();

            capi.Render.GlToggleBlend(true);
            //IServerPlayer splayer;

            overlayShaderProg.Uniform("isWhite", ShaderSysMod.didJustSnapped ? 1 : 0);
            //overlayShaderProg.Uniform("isWhite", 0);

            overlayShaderProg.Uniform("xs", (float)capi.Render.FrameWidth);
            overlayShaderProg.Uniform("ys", (float)capi.Render.FrameHeight);
            overlayShaderProg.BindTexture2D("colorTexture", capi.Render.FrameBuffers[(int)EnumFrameBuffer.Primary].ColorTextureIds[0], 0);
            //overlayShaderProg.BindTexture2D("UITexture", capi.Render.FrameBuffers[(int)EnumFrameBuffer.Primary].ColorTextureIds[0], 0);

            // Primary : 0 : world, 1 : glow, 2 : Normal, 3 : eye space coords ?, 

            capi.Render.RenderMesh(quadRef);
            overlayShaderProg.Stop();


            curShader?.Use();

            if (ShaderSysMod.didJustSnapped)
            {
                ShaderSysMod.didJustSnapped = false;
                ShaderSysMod.isTakingPicture = false;
            }
        }
    }
}