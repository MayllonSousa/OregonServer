﻿using Neon.Communication.Packets.Incoming;
using Neon.HabboHotel.Rooms;
using Neon.HabboHotel.Users;
using System.Collections.Concurrent;

namespace Neon.HabboHotel.Items.Wired.Boxes.Conditions
{
    internal class ActorHasNotCreditsBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.ConditionActorHasNotCredits;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public ActorHasNotCreditsBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;

            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Credits = Packet.PopString();

            StringData = Credits;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0 || Instance == null || string.IsNullOrEmpty(StringData))
            {
                return false;
            }

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
            {
                return false;
            }

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
            {
                return false;
            }

            if (User.GetClient().GetHabbo().Credits > int.Parse(StringData))
            {
                return false;
            }

            return true;
        }
    }
}