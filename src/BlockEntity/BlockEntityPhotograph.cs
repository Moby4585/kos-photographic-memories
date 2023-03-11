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
    class BlockEntityPhotograph : BlockEntityContainer
    {
        public override string InventoryClassName => "photographframe";

        public PhotoBitmap bitmap;
        TextureAtlasPosition atlasPosition;
        LoadedTexture loadedTex;
        int texSubId = 0;

        public InventoryGeneric inventory;
        public override InventoryBase Inventory => inventory;

        ModSystemPhotograph photoModSys;

        public int imageArraySize = -1;

        public string desc = "";

        bool isPhotoUpdated = false;

        public static AssetLocation photoBlock = new AssetLocation("kosphotography", "phototest");
        MeshData photoMesh;

        // Inventory : 0 - liquide, 1 - solide, 2 - lampe

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);


            RegisterGameTickListener(OnGameTick, 5);

            //apparatusComposition = GetApparatusComposition();
            photoModSys = api.ModLoader.GetModSystem<ModSystemPhotograph>();

            genPhoto();
            MarkDirty(true);
        }



        public BlockEntityPhotograph()
        {
            bitmap = new PhotoBitmap();

            inventory = new InventoryGeneric(1, null, null);

            inventory.SlotModified += inventory_SlotModified;
        }

        public void inventory_SlotModified(int slotModified)
        {
            MarkDirty(true);
            isPhotoUpdated = false;
            genPhoto();
            
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
            if (!inventory[0].Empty)
            {
                if (!byPlayer.InventoryManager.TryGiveItemstack(inventory[0].Itemstack))
                {
                    Api.World.SpawnItemEntity(inventory[0].Itemstack, byPlayer.Entity.Pos.XYZ);
                }
                inventory[0].TakeOutWhole();
                return true;
            }

            if ((byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack?.Collectible.Code.ToString() ?? "") != "kosphotography:photograph") return false;

            MarkDirty(true);

            int amountTransferred = byPlayer.InventoryManager.ActiveHotbarSlot.TryPutInto(Api.World, inventory[0]);

            if (amountTransferred > 0) return true;

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

        private void genPhoto()
        {
            if (Api.Side != EnumAppSide.Client) return;

            if (inventory[0].Empty)
            {
                atlasPosition = new TextureAtlasPosition();
                return;
            }

            ICoreClientAPI capi = Api as ICoreClientAPI;

            ITreeAttribute attributes = inventory[0].Itemstack.Attributes;

            Bitmap bmp = BitmapUtil.GrayscaleBitmapFromPixels(Encoding.GetEncoding(28591).GetBytes(attributes.GetString("photo", "")), attributes.GetInt("width", 0), attributes.GetInt("height", 0));

            //var graphics = Graphics.FromImage(bmp);
            //graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            bmp = new Bitmap(bmp, new Size(bmp.Width / (bmp.Height / 256), 256));

            bitmap.setBitmap(bmp);

            desc = bitmap.GetPixel(20, 20).ToString() + "\n";
            desc += bitmap.GetPixel(500, 500).ToString() + "\n";

            atlasPosition = photoModSys.GetAtlasPosition(bitmap, capi, attributes.GetString("photo", ""));

            loadMesh();
        }

        private void loadMesh()
        {
            if (Api.Side == EnumAppSide.Server) return;
            Block block = Api.World.GetBlock(photoBlock);
            ICoreClientAPI capi = Api as ICoreClientAPI;
            photoMesh = capi.TesselatorManager.GetDefaultBlockMesh(block);

            if (atlasPosition != null)
            {
                photoMesh = photoMesh.WithTexPos(atlasPosition);
                
                photoMesh.Uv[6] = atlasPosition.x1;
                photoMesh.Uv[7] = atlasPosition.y1;

                photoMesh.Uv[4] = atlasPosition.x2;
                photoMesh.Uv[5] = atlasPosition.y1;

                photoMesh.Uv[2] = atlasPosition.x2;
                photoMesh.Uv[3] = atlasPosition.y2;

                photoMesh.Uv[0] = atlasPosition.x1;
                photoMesh.Uv[1] = atlasPosition.y2;

                desc += "\n";
                foreach(float f in photoMesh.GetUv())
                {
                    desc += f.ToString() + " ; ";
                }
            }
            isPhotoUpdated = true;
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            if (!inventory.Empty && isPhotoUpdated) mesher.AddMeshData(photoMesh);

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
