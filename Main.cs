using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using LightContainmentZoneDecontamination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.NonAllocLINQ;

namespace VipStart
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        public Dictionary<string, List<GroupLoadout>> GroupLoadouts { get; set; } = new Dictionary<string, List<GroupLoadout>>()
        {
            {
                "admin",
                new List<GroupLoadout>()
                {
                    new GroupLoadout()
                    {
                       Role = PlayerRoles.RoleTypeId.ClassD,
                       HP = 120,
                       Loadout = new List<ItemType>()
                       {
                           ItemType.KeycardJanitor,
                           ItemType.Painkillers,
                       },
                       ShouldClearDefaultLoadout = false
                    },
                    new GroupLoadout()
                    {
                        Role = PlayerRoles.RoleTypeId.Scientist,
                        HP = 999,
                       Loadout = new List<ItemType>()
                       {
                           ItemType.Jailbird,
                           ItemType.KeycardO5,
                       },
                        ShouldClearDefaultLoadout = true
                    }
                }
            
            }
        };
    }
    public class VipStartMain : Plugin<Config>
    {
        public override string Author => "Unknown";

        public static VipStartMain Instance;

        public List<string> GroupNames = new List<string>();

        public EventHandlers handlers;
        public override void OnEnabled()
        {
            Instance = this;
            handlers = new EventHandlers();
            foreach(string groupname in Instance.Config.GroupLoadouts.Keys)
            {
                if(!Instance.GroupNames.Contains(groupname))
                {
                    GroupNames.Add(groupname);
                }
            }
            Exiled.Events.Handlers.Player.Spawned += handlers.Spawned;

        }
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= handlers.Spawned;
            GroupNames.Clear();
            handlers = null;
            Instance = null;
        }
    }
    public class EventHandlers
    {
        public void Spawned(SpawnedEventArgs ev)
        {
            if(VipStartMain.Instance.GroupNames.Contains(ev.Player.GroupName) && VipStartMain.Instance.Config.GroupLoadouts[ev.Player.GroupName].TryGetFirst(gl => gl.Role == ev.Player.Role.Type, out GroupLoadout groupLoadout))
            {
                if(groupLoadout.ShouldClearDefaultLoadout)
                {
                    ev.Player.ClearInventory();
                }
                foreach(ItemType item in groupLoadout.Loadout)
                {
                    ev.Player.AddItem(item);
                }
                ev.Player.Health = groupLoadout.HP;
                ev.Player.MaxHealth = groupLoadout.HP;

            }
        }
    }
}
