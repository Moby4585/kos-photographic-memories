using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.API.Datastructures;

namespace kosphotography
{
    public class BlockPhotograph : Block
    {

        //public override bool AllowHeldLiquidTransfer => true;

        //public AssetLocation bubbleSound = new AssetLocation("game", "effect/bubbling");

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            BlockEntityPhotograph be = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BlockEntityPhotograph;

            if (base.OnBlockInteractStart(world, byPlayer, blockSel)) return true;

            if (be != null)
            {
                bool handled = be.OnInteract(world, byPlayer, blockSel);
                if (handled) return true;
            }

            return false;
        }

        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
        {
            return new ItemStack(this, 1);
        }

        public override string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
        {
            string info = base.GetPlacedBlockInfo(world, pos, forPlayer);

            BlockEntityPhotograph be = world.BlockAccessor.GetBlockEntity(pos) as BlockEntityPhotograph;

            if (be != null)
            {
                //info += "\n" + be.imageArraySize.ToString();
                //info += be.desc;
                info = "";
                if (!be.inventory[0].Empty)
                {
                    info = "Displaying: ";
                    if (be.inventory[0].Itemstack.Attributes.HasAttribute("photoname")) info += be.inventory[0].Itemstack.Attributes.GetString("photoname");
                    else info += "Photograph";
                }
            }

            return info;
        }
    }
}
