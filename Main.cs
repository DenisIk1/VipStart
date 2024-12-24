using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
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
            if(VipStartMain.Instance.GroupNames.Count == 0)
            {
                checkname = string.Empty;
                return false;
            }
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
            if(checkplayer.Group == null)
            {
                checkname = string.Empty;
                return false;
            }
            if (VipStartMain.Instance.GroupNames.Contains(checkplayer.GroupName))
            {
                checkname = checkplayer.GroupName;
                Log.Debug($"Found by groupname");
                return true;
            }
            if(VipStartMain.Instance.GroupNames.Contains(checkplayer.Group.BadgeText))
            {
                Log.Debug("Found by Badge text");
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
                Log.Debug(checkname);
                Log.Debug("Found Player checkname");
                if(groupLoadout.ShouldClearDefaultLoadout)
                {
                    ev.Player.ClearInventory();
                }
                if(groupLoadout.Loadout.Count != 0)
                {
                    foreach (ItemType item in groupLoadout.Loadout.Keys)
                    {
                        if (Loader.Random.Next(0, 100) < groupLoadout.Loadout[item])
                        {
                            ev.Player.AddItem(item);
                        }
                    }
                }
                if (groupLoadout.Ammo.Count != 0)
                {

                    ev.Player.AddAmmo(groupLoadout.Ammo);
                }
                ev.Player.Health = groupLoadout.HP;
                ev.Player.MaxHealth = groupLoadout.HP;

            }
        }
    }
}
