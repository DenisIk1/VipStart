using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using LightContainmentZoneDecontamination;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.NonAllocLINQ;
using static PlayerList;

namespace VipStart
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("permissions or groups check. Use group for group check & use perms for permissions check")]

        public string CheckType { get; set; } = "group";

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
            if (Instance.Config.CheckType == "group")
            {
                foreach (string groupname in Instance.Config.GroupLoadouts.Keys)
                {
                    if (!Instance.GroupNames.Contains(groupname))
                    {
                        GroupNames.Add(groupname);
                    }
                }
                Exiled.Events.Handlers.Player.Spawned += handlers.SpawnedGroup;
            }
            else if(Instance.Config.CheckType == "perms")
            {
                Exiled.Events.Handlers.Player.Spawned += handlers.SpawnedPerms;
            }
            else
            {
                Log.Warn($"Unexpected type: {Instance.Config.CheckType}, using default group check");
                foreach (string groupname in Instance.Config.GroupLoadouts.Keys)
                {
                    if (!Instance.GroupNames.Contains(groupname))
                    {
                        GroupNames.Add(groupname);
                    }
                }
                Exiled.Events.Handlers.Player.Spawned += handlers.SpawnedGroup;
            }

        }
        public override void OnDisabled()
        {
            if(Instance.Config.CheckType == "perms")
            {
                Exiled.Events.Handlers.Player.Spawned -= handlers.SpawnedPerms;
            }
            else
            {
                GroupNames.Clear();
                Exiled.Events.Handlers.Player.Spawned -= handlers.SpawnedGroup;
            }
            handlers = null;
            Instance = null;
        }
    }
    public class EventHandlers
    {
        public void SpawnedGroup(SpawnedEventArgs ev)
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
        public void SpawnedPerms(SpawnedEventArgs ev)
        {
            foreach(string perm in VipStartMain.Instance.Config.GroupLoadouts.Keys)
            {
                if(ev.Player.CheckPermission(perm) && VipStartMain.Instance.Config.GroupLoadouts[perm].TryGetFirst(gl => gl.Role == ev.Player.Role.Type, out GroupLoadout groupLoadout))
                {
                    if (groupLoadout.ShouldClearDefaultLoadout)
                    {
                        ev.Player.ClearInventory();
                    }
                    foreach (ItemType item in groupLoadout.Loadout)
                    {
                        ev.Player.AddItem(item);
                    }
                    ev.Player.Health = groupLoadout.HP;
                    ev.Player.MaxHealth = groupLoadout.HP;
                }
            }
        }
    }
}
