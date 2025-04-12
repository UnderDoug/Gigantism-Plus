# Gigantism Plus
This mod enhances greatly and enables for character creation, the physical defect Gigantism. It retains it's base game functionality while introducing MANY new mechanics. With this mod, Gigantism can be improved with mutation points and rapid advanced, and interacts with base game mutations and everything added by this mod in various ways.

## Updates
**Total refactoring of the entire mod. Damage scaling, class names, and almost every feature of this mod has been redone, with a massive list of new additions and outright changes.**

## Features

### Gigantism
- The ability to tinker objects to be Gigantic. Natural weapons that scale with Gigantism's rank. New bonuses from Gigantism in addition to the ones from the base game. The ability to fit into smaller spaces!

### Elongated Paws
- A new mutation that allows you to use gigantic weapons items, without being gigantic yourself. If you're already gigantic, allows you to wield two-handed weapons one-handed.

### New and Altered Cybernetics
- Exoframes that give you ranks of Gigantism depending upon their Tier. New interactions with hand replacement cybernetics.

### Altered Vanilla Mutations
- Causes a few mutations to interact in new ways, like Crystallinity and Burrowing Claws. Perhaps even adding a few new features to them!

### Modular
- In the options menu, you can toggle several features of the mod, custom tailoring it a bit to your own personal taste.

# File and Class Guide
This section provides an overview of the key files and classes in the `Gigantism-Plus` mod, explaining their purpose and functionality.

### Core Files
- **`StartUp.cs`**: Handles the initialization of the mod, setting up necessary hooks and configurations for the Gigantism-Plus mod.
- **`Options.cs` and `Options.xml`**: Define and manage configurable options for the mod. These include toggling features like tinkering gigantic items, adjusting item rarity, and enabling or disabling specific mechanics.
- **`Const.cs`**: Contains constants used throughout the mod, such as default values for scaling damage or mutation ranks.
- **`Utils.cs`**: Provides utility functions and helpers used across various parts of the mod, such as common calculations or data transformations.

### Modifications
Located in the `Modifications/` folder, these files define specific modifications applied to natural weapons and equipment:
- **`ModGiganticNaturalWeapon.cs`**: Implements the behavior of gigantic natural weapons, including scaling damage, hit penalties, and other effects.
- **`ModNaturalEquipmentBase.cs`**: Serves as the base class for managing natural equipment modifications, providing shared functionality for other modification classes.
- **`ModImprovedGigantismPlus.cs`**: Enhances the core functionality of the Gigantism mutation, adding new mechanics and interactions.
- **Other Files**: Includes additional modifications for specific natural weapons, such as `ModBurrowingNaturalWeapon.cs` and `ModCrystallineNaturalWeapon.cs`, which add unique effects to these weapon types.

### Mutations
Located in the `Mutations/` folder, these files define new or modified mutations:
- **`GigantismPlus.cs`**: Implements the core functionality of the Gigantism mutation, including scaling effects, interactions with other mechanics, and rank-based bonuses.
- **`ElongatedPaws.cs`**: Adds a mutation for elongated paws, allowing characters to wield gigantic weapons without being gigantic themselves.
- **`BaseManagedDefaultEquipmentMutation.cs`**: Serves as a base class for mutations that manage default equipment, providing shared functionality for other mutation classes.
- **Other Files**: Includes additional mutations, such as `UD_ManagedBurrowingClaws.cs` and `UD_ManagedCrystallinity.cs`, which add unique abilities and interactions.

### Harmony Patches
Located in the `Harmony/` folder, these files modify or extend the behavior of the base game using Harmony:
- **`Gigantism_Patches.cs`**: Contains patches related to the Gigantism mutation, such as adjusting character size, weight, and interactions with the environment.
- **`MeleeWeapon_Patches.cs`**: Modifies melee weapon behavior to support gigantic weapons, including damage scaling and hit penalties.
- **`Body_Patches.cs`**: Adjusts body-related mechanics to accommodate gigantic characters, such as furniture interactions and movement restrictions.
- **Other Files**: Includes patches for specific mechanics, such as `Tinkering_Disassemble_Patches.cs` for tinkering interactions and `Physics_Patches.cs` for weight and collision adjustments.

### Events
Located in the `Events/` folder, these files handle specific events triggered during gameplay:
- **`UpdateNaturalEquipmentModsEvent.cs`**: Manages updates to natural equipment modifications, ensuring they remain consistent with the character's current state.
- **`BodyPartsUpdated/`**: Contains event handlers for body part updates, such as adding or removing natural weapons.
- **`DescribeModGigantic/`**: Handles descriptions and tooltips for the Gigantism mod, providing detailed information to the player.
- **Other Subfolders**: Includes additional event handlers for specific mechanics, such as `ManageDefaultEquipment/` and `RapidAdvancement/`.

### Cybernetics
Located in the `Cybernetics/` folder, these files define cybernetic enhancements:
- **`CyberneticsGiganticExoframe.cs`**: Implements a cybernetic exoframe designed for gigantic characters, providing bonuses based on its tier.
- **`BaseManagedDefaultEquipmentCybernetic.cs`**: Serves as a base class for cybernetic equipment management, providing shared functionality for other cybernetic classes.

### Secrets
Located in the `Secrets/` folder, these files add hidden content and lore:
- **`SecretGiganticExoframe.cs`**: Introduces a secret cybernetic exoframe, providing unique bonuses and interactions.
- **`SeriouslyThickStew.cs`**: Adds a unique recipe related to gigantic characters, offering special effects when consumed.
- **Other Files**: Includes additional secrets, such as `Books.xml` for hidden lore and `Conversations.xml` for unique dialogue options.

### Object Builders and Blueprints
- **`ObjectBuilders/`**: Contains logic for constructing objects, such as `Gigantified.cs` for creating gigantic items and equipment.
- **`ObjectBlueprints/`**: Defines XML blueprints for creatures, items, and data used in the mod, such as `Creatures.xml` and `Items.xml`.

### Textures
Located in the `Textures/` folder, these files provide visual assets for abilities, creatures, items, mutations, and natural weapons. Subfolders include:
- **`Abilities/`**: Icons and visuals for new abilities added by the mod.
- **`Creatures/`**: Sprites for new or modified creatures.
- **`Items/`**: Visuals for gigantic items and equipment.
- **`Mutations/`**: Icons for new mutations, such as Gigantism and Elongated Paws.
- **`NaturalWeapons/`**: Visuals for natural weapons, such as gigantic fists and claws.

### Miscellaneous
- **`manifest.json`**: Metadata for the mod, including its name, version, and dependencies.
- **`README.md`**: Documentation for the mod, including features, file guides, and contribution guidelines.
- **`workshop.json`**: Configuration for publishing the mod to the Steam Workshop.

This guide should help you navigate the mod's structure and understand the purpose of its components. Feel free to explore and modify the files to suit your needs!

## To-Do List
- Settle on damage for the natural weapons.
- Finish secret events and objects.

## Contributing

If you have any ideas to improve the mod, or want to contribute to the [experimental branch](https://https://github.com/hyd-n-plyn-syt/Gigantism-Plus/tree/experimental), feel free to let me know or make a request! This mod has primarily been tested by myself and UnderDoug, so please report any issues, and we'll work to resolve them!

**Enjoy your adventure with your gigantic mutants, and have fun!**

If you'd like to contribute to any of my [public mods](https://github.com/hyd-n-plyn-syt?tab=repositories), or [buy me a coffee](https://ko-fi.com/hydnplynsyt), feel free!