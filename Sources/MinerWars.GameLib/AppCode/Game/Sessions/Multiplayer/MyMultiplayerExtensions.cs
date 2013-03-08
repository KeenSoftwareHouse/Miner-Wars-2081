using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.AppCode.Game.Sessions
{
    static class MyMultiplayerExtensions
    {
        public static readonly string CoopPlayerPrefix = "MP_CoopPlayer_";

        public static StringBuilder EmptyStringBuilder = new StringBuilder(String.Empty);

        public static MyPlayerRemote GetPlayer(this NetConnection connection)
        {
            return (connection != null && connection.Tag is MyPlayerRemote) ? ((MyPlayerRemote)connection.Tag) : null;
        }

        public static StringBuilder GetPlayerName(this NetConnection connection)
        {
            var player = connection.GetPlayer();
            return player != null ? player.GetDisplayName() : EmptyStringBuilder;
        }

        public static MyMwcObjectBuilder_Player LoadCoopPlayer(this MyMwcObjectBuilder_Checkpoint checkpoint, string displayName)
        {
            string name = CoopPlayerPrefix + displayName;
            if (checkpoint != null && checkpoint.InventoryObjectBuilder != null && checkpoint.InventoryObjectBuilder.InventoryItems != null)
            {
                var result = checkpoint.InventoryObjectBuilder.InventoryItems.Select(s => s.ItemObjectBuilder).OfType<MyMwcObjectBuilder_Player>().FirstOrDefault(s => s.Name == name);
                if (result != null)
                {
                    result.Name = displayName;
                    return result;
                }
            }
            return null;
        }

        public static void CopyCoopPlayers(this MyMwcObjectBuilder_Checkpoint loadFrom, MyMwcObjectBuilder_Checkpoint copyTo)
        {
            if (loadFrom != null && loadFrom.InventoryObjectBuilder != null && loadFrom.InventoryObjectBuilder.InventoryItems != null)
            {
                var result = loadFrom.InventoryObjectBuilder.InventoryItems.Select(s => s.ItemObjectBuilder).OfType<MyMwcObjectBuilder_Player>().Where(s => s.Name.StartsWith(CoopPlayerPrefix));
                foreach(var pl in result.ToArray())
                {
                    copyTo.StoreCoopPlayer((MyMwcObjectBuilder_Player) pl.Clone());
                }
            }
        }

        public static void StoreCoopPlayer(this MyMwcObjectBuilder_Checkpoint checkpoint, MyMwcObjectBuilder_Player playerBuilder, string displayName)
        {
            playerBuilder.Name = CoopPlayerPrefix + displayName;
            StoreCoopPlayer(checkpoint, playerBuilder);
        }

        private static void StoreCoopPlayer(this MyMwcObjectBuilder_Checkpoint checkpoint, MyMwcObjectBuilder_Player playerBuilder)
        {
            if (checkpoint != null)
            {
                if (checkpoint.InventoryObjectBuilder == null)
                    checkpoint.InventoryObjectBuilder = (MyMwcObjectBuilder_Inventory)MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.Inventory, null);

                // Remove old and add new
                checkpoint.InventoryObjectBuilder.InventoryItems.RemoveAll(new Predicate<MyMwcObjectBuilder_InventoryItem>(s => s.ItemObjectBuilder.Name == playerBuilder.Name));
                checkpoint.InventoryObjectBuilder.InventoryItems.Add(new MyMwcObjectBuilder_InventoryItem(playerBuilder, 1));
            }
        }
    }
}
