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
    class CommandsSystem : ModSystem
    {
        public override bool ShouldLoad(EnumAppSide side)
        {
            return side == EnumAppSide.Server;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);

            var parsers = api.ChatCommands.Parsers;
            api.ChatCommands.Create("photoname")
                .RequiresPrivilege(Privilege.chat)
                .WithArgs(parsers.Word("name"))
                .HandleWith(setPhotoName);
        }

        private TextCommandResult setPhotoName(TextCommandCallingArgs args)
        {
            IPlayer player = args.Caller.Player;

            if (player.InventoryManager.ActiveHotbarSlot.Itemstack.Collectible.Code.ToString() == "kosphotography:photograph")
            {
                player.InventoryManager.ActiveHotbarSlot.Itemstack.Attributes.SetString("photoname", args[0].ToString());
                player.InventoryManager.ActiveHotbarSlot.MarkDirty();
                return TextCommandResult.Success("Ok, photograph name set to " + args[0].ToString());
            }

            return TextCommandResult.Success("Error: item is not a photograph");
        }
    }
}
