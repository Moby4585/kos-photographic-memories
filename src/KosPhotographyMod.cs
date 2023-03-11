using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.API.Datastructures;
using System.Drawing;

namespace kosphotography
{
    public class KosPhotographyMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            /*(api as ICoreClientAPI).BlockTextureAtlas.InsertTexture();
            int testInt = 5;
            byte[] testIntByte = BitConverter.GetBytes(testInt);
            int[] framebuffer = ((api as ICoreClientAPI).Render.FrameBuffers[-1].ColorTextureIds);
            Bitmap bmp = BitmapUtil.GrayscaleBitmapFromPixels(, );*/

            api.RegisterBlockClass("BlockPhotograph", typeof(BlockPhotograph));
            api.RegisterBlockEntityClass("BlockEntityPhotograph", typeof(BlockEntityPhotograph));

            api.RegisterItemClass("ItemPhotograph", typeof(ItemPhotograph));
            api.RegisterItemClass("ItemCamera", typeof(ItemCamera));

            //api.RegisterMountable("vehicle", EntityVehicleSeat.GetMountable);
            //api.RegisterItemClass("ItemBoat", typeof(ItemBoat));
            //api.RegisterEntity("EntityVehicle", typeof(EntityVehicle));
            /*if (api is ICoreServerAPI sapi)
            { 
                sapi.World.Logger.StoryEvent("kosFireMod loaded");
            }*/
        }
    }
}
