using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using Exiled.Permissions.Extensions;
using Exiled.Permissions.Features;
using InventorySystem;
using System.Collections.Generic;
using System.ComponentModel;
using Utils.NonAllocLINQ;

namespace VipStart
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("Dictionary where key is a group name, badge text or permission")]
        public Dictionary<string, List<GroupLoadout>> GroupLoadouts { get; set; } = new Dictionary<string, List<GroupLoadout>>()
        {
            {
                "admin",
                new List<GroupLoadout>()
                {
                    new GroupLoadout()
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
            foreach (string groupname in Instance.Config.GroupLoadouts.Keys)
            {
                if (!Instance.GroupNames.Contains(groupname))
                {
                    GroupNames.Add(groupname);
                }
            }
            Exiled.Events.Handlers.Player.Spawned += handlers.Spawned;
        }
        public override void OnDisabled()
        {
            GroupNames.Clear();
            Exiled.Events.Handlers.Player.Spawned -= handlers.Spawned;
            handlers = null;
            Instance = null;
        }
    }
    public class EventHandlers
    {
        private bool TryFindCheckName(Player checkplayer, out string checkname)
        {
            if(!checkplayer.IsVerified)
            {
                checkname = string.Empty;
                return false;
            }
            if(checkplayer.Role.Base is not IInventoryRole)
            {
                checkname = string.Empty;
                return false;
            }
            if (VipStartMain.Instance.GroupNames.Contains(checkplayer.GroupName))
            {
                checkname = checkplayer.GroupName;
                Log.Warn($"Yey, I somehow find {checkplayer.GroupName} to {checkname}");
                return true;
            }
            if(VipStartMain.Instance.GroupNames.Contains(checkplayer.Group.BadgeText))
            {
                checkname = checkplayer.Group.BadgeText;
                return true;
            }
            checkname = string.Empty;
            return false;

        }
        public void Spawned(SpawnedEventArgs ev)
        {
            if(TryFindCheckName(ev.Player, out string checkname) && VipStartMain.Instance.Config.GroupLoadouts[checkname].TryGetFirst(gl => gl.Role == ev.Player.Role.Type, out GroupLoadout groupLoadout))
            {
                Log.Warn(checkname);
                Log.Debug("Found Player checkname");
                if(groupLoadout.ShouldClearDefaultLoadout)
                {
                    ev.Player.ClearInventory();
                }
                foreach(ItemType item in groupLoadout.Loadout.Keys)
                {
                    if (Loader.Random.Next(0, 100) < groupLoadout.Loadout[item])
                    {
                        ev.Player.AddItem(item);
                    }
                }
                ev.Player.AddAmmo(groupLoadout.Ammo);
                ev.Player.Health = groupLoadout.HP;
                ev.Player.MaxHealth = groupLoadout.HP;

            }
        }
    }
}
