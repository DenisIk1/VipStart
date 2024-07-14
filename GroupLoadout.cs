using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipStart
{
    public class GroupLoadout
    {

        public RoleTypeId Role { get; set; }

        public int HP { get; set; }

        public List<ItemType> Loadout { get; set; }

        public bool ShouldClearDefaultLoadout { get; set; }

        
    }
}
