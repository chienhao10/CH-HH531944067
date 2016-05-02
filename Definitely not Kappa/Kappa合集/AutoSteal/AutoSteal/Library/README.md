# Project Name

Genesis Spell Library
By Genesis
https://github.com/Alweul/EloBuddy

## Installation

1) Download/Fork/Clone

2) Place inside your addon source (Note: If you are using the regular addon template already, use only SpellManager, SpellLibrary and the Spells folder or manually merge the files, do not overwrite your own work please.)

3) Add it to Visual Studio

4) Use it with ```using GenesisSpellLibrary;```

5) Initialize it with 

```
            SpellManager.Initialize();
			SpellLibrary.Initialize();
```



## Usage

```
SpellBase GetSpells(Champion heroChampion)
bool IsOnCooldown(AIHeroClient hero, SpellSlot slot)
float GetRange(SpellSlot slot, AIHeroClient sender)
```


## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## Credits

See the Spells/Spells.cs Credit region for credits for that file, all other credits will be here.

