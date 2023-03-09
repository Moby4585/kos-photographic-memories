using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;
using Vintagestory.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.API.Datastructures;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using OpenTK.Graphics;

namespace kosphotography
{
    class BlockEntityPhotograph : BlockEntity
    {
        public PhotoBitmap bitmap;
        TextureAtlasPosition atlasPosition;
        LoadedTexture loadedTex;
        int texSubId = 0;

        public int imageArraySize = -1;

        public string desc = "";

        public static AssetLocation oilLampBlock = new AssetLocation("kosphotography", "phototest");
        MeshData oilLampMesh;

        // Inventory : 0 - liquide, 1 - solide, 2 - lampe

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);


            RegisterGameTickListener(OnGameTick, 5);

            //apparatusComposition = GetApparatusComposition();
        }



        public BlockEntityPhotograph()
        {
            bitmap = new PhotoBitmap();
        }

        public void OnGameTick(float dt)
        {

        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {

            base.GetBlockInfo(forPlayer, dsc);
        }

        public bool OnInteract(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, bool onlyOilLamp = false)
        {
            if (Api.Side == EnumAppSide.Client)
            {
                ICoreClientAPI capi = Api as ICoreClientAPI;

                //bitmap.setBitmap(capi.Render.FrameBuffers[0].ColorTextureIds, capi.Render.FrameWidth, capi.Render.FrameHeight);

                
                //capi.Render.BindTexture2d(capi.Render.FrameBuffers[0].ColorTextureIds[2]);
                

                Bitmap bmp = GrabScreenshot(capi, false, true, false);
                desc = "\n" + bmp.PixelFormat.ToString();

                var graphics = Graphics.FromImage(bmp);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                bmp = new Bitmap(bmp, new Size(bmp.Width / (bmp.Height / 256), 256));

                bitmap.setBitmap(bmp);

                desc += bitmap.GetPixel(20, 20).ToString() + "\n";
                desc += bitmap.GetPixel(500, 500).ToString() + "\n";

                bmp = BitmapUtil.GrayscaleBitmapFromPixels(bitmap.PixelsGrayscale, bitmap.Width, bitmap.Height);

                LoadedTexture texture = new LoadedTexture(capi);

                texture.Width = bitmap.Width;
                texture.Height = bitmap.Height;

                capi.Render.LoadTexture(bitmap, ref texture);

                //capi.BlockTextureAtlas.AllocateTextureSpace(bitmap.Width, bitmap.Height, out texSubId, out atlasPosition);

                capi.BlockTextureAtlas.InsertTexture(bitmap, out texSubId, out atlasPosition);

                imageArraySize = texture.Width;
                desc += "\nAtlas positions:";
                desc += "\n" + atlasPosition.x1.ToString() + " ; " + atlasPosition.y1.ToString() + " ; " + atlasPosition.x2.ToString() + " ; " + atlasPosition.x2.ToString() + " ; ";




                /*TextureAtlasPosition position;
                //int textureSubId;
                if (capi.BlockTextureAtlas.AllocateTextureSpace(texture.Width, texture.Height, out texSubId, out position))
                {
                    capi.BlockTextureAtlas.RenderTextureIntoAtlas(texture, 0, 0, texture.Width, texture.Height, position.x1, position.y1, 0.5f);
                    desc += "\nCould allocate";
                }
                else
                {
                    desc += "\nCould NOT allocate";
                }

                atlasPosition = position;

                loadedTex = texture;*/

                loadMesh();

                bmp.Save("D:\\Documents\\Developpement\\_Vintage Story\\texture.bmp");
                //capi.BlockTextureAtlas.texture

                //capi.BlockTextureAtlas.InsertTexture((bitmap as IBitmap), out textureSubId, out atlasPosition);

                Block.Lod0Mesh?.SetTexPos(atlasPosition);
                Block.Lod2Mesh?.SetTexPos(atlasPosition);


                //Block.Textures["photograph"] ;
                return true;
            }
            return false;
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {

            //reactingRecipe = JsonUtil.FromString<RetortRecipe>(tree.GetString("reactingRecipe", ""));

            base.FromTreeAttributes(tree, worldAccessForResolve);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            //tree.SetString("reactingRecipe", JsonUtil.ToString(reactingRecipe));

            base.ToTreeAttributes(tree);
        }

        private void loadMesh()
        {
            if (Api.Side == EnumAppSide.Server) return;
            Block block = Api.World.GetBlock(oilLampBlock);
            ICoreClientAPI capi = Api as ICoreClientAPI;
            oilLampMesh = capi.TesselatorManager.GetDefaultBlockMesh(block);

            if (atlasPosition != null)
            {
                oilLampMesh = oilLampMesh.WithTexPos(atlasPosition);

                oilLampMesh.Uv[6] = atlasPosition.x1;
                oilLampMesh.Uv[7] = atlasPosition.y1;

                oilLampMesh.Uv[4] = atlasPosition.x2;
                oilLampMesh.Uv[5] = atlasPosition.y1;

                oilLampMesh.Uv[2] = atlasPosition.x2;
                oilLampMesh.Uv[3] = atlasPosition.y2;

                oilLampMesh.Uv[0] = atlasPosition.x1;
                oilLampMesh.Uv[1] = atlasPosition.y2;

                /*
                oilLampMesh.Uv[0] = atlasPosition.x1;
                oilLampMesh.Uv[1] = atlasPosition.y2;
                oilLampMesh.Uv[2] = atlasPosition.x2;
                oilLampMesh.Uv[3] = atlasPosition.y2;
                oilLampMesh.Uv[4] = atlasPosition.y2;
                oilLampMesh.Uv[5] = atlasPosition.y1;

                oilLampMesh.Uv[6] = atlasPosition.x1;
                oilLampMesh.Uv[7] = atlasPosition.y1;
                */

                //oilLampMesh.SetTexPos(atlasPosition);
                /*List<float> uvs = oilLampMesh.Uv.ToList<float>();
                bool isX = true;
                for (int i = 0; i < uvs.Count / 2; i++)
                {
                    if (isX)
                    {
                        uvs[i] = atlasPosition.x1;
                        uvs[i + 1] = atlasPosition.x2;
                    }
                    else
                    {
                        uvs[i] = atlasPosition.y1;
                        uvs[i + 1] = atlasPosition.y2;
                    }
                    isX = !isX;
                }*/
                /*uvs[0] = 0f;
                uvs[1] = 1f;
                uvs[2] = 0f;
                uvs[3] = 1f;
                oilLampMesh.SetUv(uvs.ToArray());*/
                desc += "\n";
                foreach(float f in oilLampMesh.GetUv())
                {
                    desc += f.ToString() + " ; ";
                }
            }
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {


            mesher.AddMeshData(oilLampMesh);

            return base.OnTesselation(mesher, tessThreadTesselator);
        }

        public Bitmap GrabScreenshot(ICoreClientAPI capi, bool scaleScreenshot, bool flip, bool withAlpha)
        {
            if (GraphicsContext.CurrentContext == null)
            {
                throw new GraphicsContextMissingException();
            }
            //capi.Render.BindTexture2d(capi.Render.FrameBuffers[1].ColorTextureIds[2]);
            Bitmap bmp = new Bitmap(capi.Render.FrameWidth, capi.Render.FrameHeight);
            Rectangle rect = new Rectangle(0, 0, capi.Render.FrameWidth, capi.Render.FrameHeight);
            //PixelFormat format = withAlpha ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;
            BitmapData data = bmp.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            GL.ReadPixels(0, 0, capi.Render.FrameWidth, capi.Render.FrameHeight, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.Byte, data.Scan0);
            bmp.UnlockBits(data);
            if (scaleScreenshot)
            {
                bmp = new Bitmap(bmp, new Size(capi.Render.FrameWidth, capi.Render.FrameHeight));
            }
            if (flip)
            {
                bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            }
            return bmp;
        }
    }
}
