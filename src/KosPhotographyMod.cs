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

            //Check for Existing Config file, create one if none exists
            try
            {
                var Config = api.LoadModConfig<KosPhotographyConfig>("kosphotography.json");
                if (Config != null)
                {
                    api.Logger.Notification("Mod Config successfully loaded.");
                    KosPhotographyConfig.Current = Config;
                }
                else
                {
                    api.Logger.Notification("No Mod Config specified. Falling back to default settings");
                    KosPhotographyConfig.Current = KosPhotographyConfig.GetDefault();
                }
            }
            catch
            {
                KosPhotographyConfig.Current = KosPhotographyConfig.GetDefault();
                api.Logger.Error("Failed to load custom mod configuration. Falling back to default settings!");
            }
            finally
            {
                api.StoreModConfig(KosPhotographyConfig.Current, "kosphotography.json");
            }
        }
    }
}
