using Exiled.API.Enums;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipStart
{
    public class GroupLoadout
    {

        public RoleTypeId Role { get; set; } = RoleTypeId.NtfCaptain;

        public int HP { get; set; } = 250;

        public Dictionary<ItemType, float> Loadout { get; set; } = new Dictionary<ItemType, float>()
        {
             { ItemType.KeycardJanitor , 100},
             { ItemType.Painkillers , 50 }
        };

        public Dictionary<AmmoType, ushort> Ammo { get; set; } = new Dictionary<AmmoType, ushort>()
        {
            { AmmoType.Nato556, 100 }
            
        };
        public bool ShouldClearDefaultLoadout { get; set; } = false;

        
    }
}
