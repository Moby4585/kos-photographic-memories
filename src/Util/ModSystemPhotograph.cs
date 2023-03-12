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
using ProtoBuf;

namespace kosphotography
{
    class ModSystemPhotograph : ModSystem
    {
        ICoreAPI api;

        ItemStack photographStack;

        public Dictionary<string, TextureAtlasPosition> atlasPositions = new Dictionary<string, TextureAtlasPosition>();

        public override double ExecuteOrder()
        {
            return 0.3d;
        }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            this.api = api;
            api.Network.RegisterChannel("takephoto").RegisterMessageType<TakePhotoPacket>();
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
        }

        public TextureAtlasPosition GetAtlasPosition(PhotoBitmap photo, ICoreClientAPI capi, string picture) 
        {
            if (atlasPositions.ContainsKey(picture)) return atlasPositions[picture];

            TextureAtlasPosition atlasPosition;
            int texSubId;

            capi.BlockTextureAtlas.InsertTexture(photo, out texSubId, out atlasPosition);

            atlasPositions.Add(picture, atlasPosition);

            return atlasPosition;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Network.GetChannel("takephoto").SetMessageHandler<TakePhotoPacket>(onTakePhotoPacket);
        }

        public void takePhoto(IPlayer player, byte[] photo, int width, int height)
        {
            if (player.InventoryManager.GetHotbarInventory()[10].Itemstack.Collectible.Code.ToString() == "kosphotography:photographicpaper")
            {
                photographStack = new ItemStack(api.World.GetItem(new AssetLocation("kosphotography", "photograph")));

                player.InventoryManager.GetHotbarInventory().MarkSlotDirty(10);

                ItemStack exposedPhoto = photographStack.Clone();
                exposedPhoto.Attributes.SetInt("width", width);
                exposedPhoto.Attributes.SetInt("height", height);
                exposedPhoto.Attributes.SetString("photo", Encoding.GetEncoding(28591).GetString(photo));


                if (api is ICoreClientAPI capi)
                {
                    capi.Network.GetChannel("takephoto").SendPacket(new TakePhotoPacket() { Picture = photo, Width = width, Height = height });
                }
                else
                {
                    api.World.PlaySoundAt(new AssetLocation("kosphotography", "sounds/shutter"), player.Entity, null, true, 16);

                    int takeOutAmount = player.InventoryManager.GetHotbarInventory()[10].TakeOut(1).StackSize;
                    if (takeOutAmount >= 1)
                    {
                        //player.InventoryManager.TryGiveItemstack(exposedPhoto);
                        if (!player.InventoryManager.TryGiveItemstack(exposedPhoto))
                        {
                            api.World.SpawnItemEntity(exposedPhoto, player.Entity.Pos.XYZ);
                        }
                    }
                }
            }
        }

        public void onTakePhotoPacket(IServerPlayer player, TakePhotoPacket packet)
        {
            takePhoto(player, packet.Picture, packet.Width, packet.Height);
            //player.SendMessage(GlobalConstants.GeneralChatGroup, "Message photo serveur", EnumChatType.AllGroups);
        }

        public static Bitmap GrabScreenshot(ICoreClientAPI capi, bool scaleScreenshot, bool flip, bool withAlpha)
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
    [ProtoContract]
    public class TakePhotoPacket
    {
        [ProtoMember(1)]
        public byte[] Picture;
        [ProtoMember(2)]
        public int Width;
        [ProtoMember(3)]
        public int Height;
    }
}
