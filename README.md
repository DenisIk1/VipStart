# VipStart
An EXILED plugin that allows create loadouts for server roles
```Yaml
group_loadouts:
    #Server Role or Group Badge Text
    admin:
    # Choose the role when player get items
    - role: ClassD
      # Health(Override Max Health)
      h_p: 120
      # Item dictionary, ItemType/Chance
      loadout:
        KeycardJanitor: 100
      # Ammo Dictionary, AmmoType/Amount
      ammo:
        Nato9: 100
     # Yes, should clear default loadout
      should_clear_default_loadout: false
     # Creating Loadout for another role
    - role: Scientist
      h_p: 999
      loadout:
      - Jailbird
      - KeycardO5
      should_clear_default_loadout: true
    # Creating new Group/Loadout Dictionary
    MODERATOR:
    - role: NtfCaptain
      h_p: 125
      # Item dictionary, ItemType/Chance
      loadout:
        KeycardJanitor: 100
      # Ammo Dictionary, AmmoType/Amount
      ammo:
        Nato9: 100
      should_clear_default_loadout: true
```
