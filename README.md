# VipStart
An EXILED plugin that allows create loadouts for server roles
```Yaml
group_loadouts:
    #Server Role or Permission
    admin:
    # Choose the role when player get items
    - role: ClassD
      # Health(Override Max Health)
      h_p: 120
    # Loadout
      loadout: 
      - KeycardJanitor
      - Painkillers
     # Yes, should clear default loadout
      should_clear_default_loadout: false
    - role: Scientist
      h_p: 999
      loadout:
      - Jailbird
      - KeycardO5
      should_clear_default_loadout: true
    owner:
    - role: FacilityGuard
      h_p: 110
      loadout:
      - GunAK
      should_clear_default_loadout: false
```
