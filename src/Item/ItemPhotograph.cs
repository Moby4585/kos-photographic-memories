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
    class ItemPhotograph : Item
    {
        ModSystemPhotograph photoModSys;
        ICoreClientAPI capi;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            capi = api as ICoreClientAPI;

            photoModSys = api.ModLoader.GetModSystem<ModSystemPhotograph>();
        }

        public override string GetHeldItemName(ItemStack itemStack)
        {
            string itemName = base.GetHeldItemName(itemStack);
            if (itemStack.Attributes.HasAttribute("width"))
            {
                itemName += " " + itemStack.Attributes.GetInt("width").ToString() + "x";
            }
            return itemName;
        }
    }
}
