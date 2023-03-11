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
    class ItemCamera : Item
    {
        ModSystemPhotograph photoModSys;
        ICoreClientAPI capi;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            capi = api as ICoreClientAPI;

            photoModSys = api.ModLoader.GetModSystem<ModSystemPhotograph>();
        }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);

            IPlayer player = (byEntity as EntityPlayer).Player;

            if (byEntity.Controls.Sneak) return;

            if (player != null)
            {

                if (api is ICoreClientAPI capi)
                {
                    capi.ShowChatMessage("photo client");

                    if (player.InventoryManager.GetHotbarInventory()[10].Itemstack.Collectible.Code.ToString() == "kosphotography:photographicpaper")
                    {

                        PhotoBitmap bitmap = new PhotoBitmap();

                        Bitmap bmp = ModSystemPhotograph.GrabScreenshot(capi, false, true, false);

                        var graphics = Graphics.FromImage(bmp);
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                        bmp = new Bitmap(bmp, new Size(bmp.Width / (bmp.Height / 256), 256));

                        bitmap.setBitmap(bmp);

                        photoModSys.takePhoto(player, bitmap.PixelsGrayscale, bitmap.Width, bitmap.Height);
                    }
                }
            }
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction
                {
                    //HotKeyCode = "shift",
                    ActionLangCode = "heldhelp-takephoto",
                    MouseButton = EnumMouseButton.Right
                }
            };
        }
    }
}
