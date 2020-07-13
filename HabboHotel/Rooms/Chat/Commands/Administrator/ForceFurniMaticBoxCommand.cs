﻿using Neon.Communication.Packets.Outgoing;
using Neon.Communication.Packets.Outgoing.Catalog;
using Neon.Communication.Packets.Outgoing.Inventory.Furni;
using Neon.HabboHotel.Items;
using System;

namespace Neon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class ForceFurniMaticBoxCommand : IChatCommand
    {
        public string PermissionRequired => "command_furnimatic";

        public string Parameters => "";

        public string Description => "TEST Command - Rewards";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().Username != "Custom")
            {
                Session.SendWhisper("Uy mijito qué intentas.");
                return;
            }

            int Boxes = int.Parse(Params[1]);

            for (int i = 1; i <= Boxes; i++)
            {
                Catalog.FurniMatic.FurniMaticRewards reward = NeonEnvironment.GetGame().GetFurniMaticRewardsMnager().GetRandomReward();
                if (reward == null)
                {
                    return;
                }

                int rewardId;
                int furniMaticBoxId = 4692;
                NeonEnvironment.GetGame().GetItemManager().GetItem(furniMaticBoxId, out ItemData data);
                string maticData = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                using (Database.Interfaces.IQueryAdapter dbClient = NeonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES ('" + data.Id + "', '" + Session.GetHabbo().Id + "', @extra_data)");
                    dbClient.AddParameter("extra_data", maticData);
                    rewardId = Convert.ToInt32(dbClient.InsertQuery());
                    dbClient.runFastQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + rewardId + "', '" + reward.GetBaseItem().Id + "', '')");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = " + rewardId + " LIMIT 1;");
                }

                Item GiveItem = ItemFactory.CreateGiftItem(data, Session.GetHabbo(), maticData, maticData, rewardId, 0, 0);
                if (GiveItem != null)
                {
                    Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                    Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                    Session.SendMessage(new PurchaseOKComposer());
                    Session.SendMessage(new FurniListAddComposer(GiveItem));
                    Session.SendMessage(new FurniListUpdateComposer());
                }

                ServerPacket response = new ServerPacket(ServerPacketHeader.FurniMaticReceiveItem);
                response.WriteInteger(1);
                response.WriteInteger(GiveItem.Id); // received item id
                Session.SendMessage(response);
            }
        }
    }
}
