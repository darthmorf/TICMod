![](/readme-img/logo.png)
<br/>**CURRENTLY WIP & UNRELEASED**<br/>
If you do compile + test this yourself, let me know but please don't share it around yet!<br/><br/>

TICMod (Trigger, Influencer, Conditional Mod) is a mod for Terraria that adds tiles that function similar to Command Blocks from Minecraft. It is not balanced around a standard playthrough; it's intended use is for Adventure Maps, Servers and Testing/Building Worlds.

Functionality is implemented through three seperate tiles; the Trigger, the Influencer and the Conditional:

![](/readme-img/ticmod.png)

### Table of Contents <!-- omit in toc -->
- [User Interface](#user-interface)
- [Triggers](#triggers)
  - [Trigger Commands](#trigger-commands)
    - [Time](#time)
    - [Player Death](#player-death)
- [Influencers](#influencers)
  - [Influencer Commands](#influencer-commands)
    - [Say](#say)
    - [Spawn NPC](#spawn-npc)
    - [Give Item to Player](#give-item-to-player)
    - [Force Give Item to Player](#force-give-item-to-player)
    - [Draw World Text](#draw-world-text)
    - [Draw UI Text](#draw-ui-text)
    - [Respawn Player](#respawn-player)
    - [Kill Player](#kill-player)
    - [Kill NPC](#kill-npc)
    - [Clear Dropped Items](#clear-dropped-items)
    - [Copy Tiles](#copy-tiles)
    - [Destroy Tiles](#destroy-tiles)
    - [Place Tiles](#place-tiles)
    - [Remove Inventory Item](#remove-inventory-item)
    - [Set Health](#set-health)
    - [Set Max Health](#set-max-health)
    - [Set Mana](#set-mana)
    - [Set Max Mana](#set-max-mana)
    - [Spawn World Item](#spawn-world-item)
    - [Teleport Player](#teleport-player)
- [Conditionals](#conditionals)
  - [Conditional Commands](#conditional-commands)
    - [Is Day?](#is-day)
- [Entity Selection](#entity-selection)
  - [Player Selection](#player-selection)
    - [All Players](#all-players)
    - [Stored Players](#stored-players)
  - [NPC Selection](#npc-selection)
    - [All NPCs](#all-npcs)
    - [By ID](#by-id)
    - [Enemy NPCs](#enemy-npcs)
    - [Town NPCs](#town-npcs)
    - [Friendly NPCs](#friendly-npcs)
- [Data Stores](#data-stores)
- [Examples](#examples)
    - [Give 20 Lesser Healing Potions to players when they die.](#give-20-lesser-healing-potions-to-players-when-they-die)
- [Credits](#credits)


## User Interface

Commands are entered through each block's own UI, which can be opened by right clicking on the tile. The UI can be dragged around the screen, and multiple TIC UIs can be open at once for ease of use.
![](/readme-img/validui.png)

If a command is invalid, the text becomes red, and an error code is displayed upon hover:
![](/readme-img/invalidui.png)

The user interface supports a range of shortcuts to ease the editing of commands. Arrow keys can be used to navigate, along with HOME and END to jump the front and back respectively.
Ctrl+C, Ctrl+V and Ctrl+X can be used to copy, paste and cut text too. Ctrl+Z can be used to undo changes, and tab can be pressed to navigate between different open TIC UIs.

The `,` character is used to seperate different command parameters. If you need to use one outside of this, such as in a text output command, they can be escaped by typing `\,`, which will be ignored by the interpreter. Whitespace between `,` and the start of the next parameter is also ignored so can be used to make commands easier to read. I like to group things like RGB or Coordinate parameters together without spaces, but leave them for everything else.

The 'show debug output' checkbox determines whether debug output is displayed in the chat when the tile activates, for example:<br/>
![](/readme-img/debugout.png)
These outputs can be disabled globally (ignoring individual tile settings) by using the 'Tile Output Toggler' item. Using it will toggle the world between the two states.

Some commands also require a world co-ordinate to be passed. As this does not directly correspond to the vanilla position items, you can use an accessory; the 'Mouse Coordinate Display'. With this equipped, your mouse cursor will display the co-ordinates of the tile beneath it:
<br>![](/readme-img/coordinateui.png)

## Triggers

Triggers function as an 'event' - they are used when you want something happening to send a signal, for example a boss being spawned, a tile being edited, an item being picked up, etc.

![](/readme-img/trigger.png)

- Sending a signal through the IN tile enables or disables the Trigger, depending on it's current state.
- A signal is sent to the OUT tile when the Trigger is triggered.

### Trigger Commands
#### Time
`time hh:mm`<br/>
Triggers once when time is equal to the specified time. Must be in 24 hour format.<br/>
EG: `time 14:30`

#### Player Death
`playerdeath datastore(optional)`<br/>
Triggers once when a player dies. Can optionally pass in a [datastore](#data-stores) string to write to.<br/>
EG: `playerdeath deadplayer`


## Influencers

Influencers function as the main heavy lifters of the mod; they are the ones that actually *do* things, for example spawning an NPC, creating a tile, or dropping an item.

![](/readme-img/influencer.png)

- Sending a signal throught the IN tile activates the Influencer.

### Influencer Commands
#### Say
`say rrr,ggg,bbb, message`<br/>
Sends a message to the chat with the colour specified by `rrr,ggg,bbb`, in RGB format. Message can contain `\n` which will create a line break in the message.<br/>
EG: `say 0,160,255, Hello World`

#### Spawn NPC
`spawnnpc x,y, id, datastore(optional)`<br/>
Spawns the NPC with specified ID at world position x,y. Stores it in the specified datastore, if supplied. [NPC IDs can be found on the Wiki](https://terraria.gamepedia.com/NPC_IDs).<br/>
EG: `spawnnpc 26,10, 53`

#### Give Item to Player
`giveitem id, count, player(s)`<br/>
Gives [targeted player(s)](#player-selection) the specified count of item with specified id.<br/>
EG: `giveitem 26, 10, @a`

#### Force Give Item To Player
`forcegiveitem id, count, player(s)`<br/>
Gives [targeted player(s)](#player-selection) the specified count of item with specified id. Unlike [giveitem](#give-item-to-player), places the item directly in the inventory, rather than triggering the usual pickup text. Also works when players are dead.<br/>
EG: `forecegiveitem 26, 10, @a`

#### Draw World Text
`drawworldtext rrr,ggg,bbb, x,y, duration, message`<br/>
Displays text with specified color at specified world coordinate. Sticks to tiles, acts as if it were part of the world. If given a duration of 0 or less, will last until the world is reloaded.<br/>
EG: `drawworldtext 0,160,255, 26,10, Hello World!`

#### Draw UI Text
`drawuitext rrr,ggg,bbb, x,y, duration, message`<br/>
Displays text with specified color at specified UI % position. Acts as if it were part of the User Interface. If given a duration of 0 or less, will last until the world is reloaded.<br/>
EG: `drawuitext 0,160,255, 50,10, Hello World!`

#### Respawn Player
`respawn player(s)`<br/>
Respawns the [targeted player(s)](#player-selection) if they are dead. Does nothing if they are not.<br/>
EG: `respawn @a`

#### Kill Player
`kill player(s), reason`<br/>
Kills the [targeted player(s)](#player-selection), with `reason` as the message displayed in chat. The string `#name` can be included within `reason` to refer to the name of the player that was killed.<br/>
EG: `kill @n, darthmorf, Killed #name\, they deserved it!`

#### Kill NPC
`killnpc npc`<br/>
Kills the [targeted NPC](#npc-selection).<br/>
EG: `killnpc @e`

#### Clear Dropped Items
`cleardroppeditems`<br/>
Removes all items that are dropped in the world.<br/>
EG: `cleardroppeditems`

#### Copy Tiles
`copytile startX,startY, endX,endY, destinationX,destionationY`<br/>
Copies the tiles within the region (startX,startY) - (endX,endY) to (destinationX,destinationY) where the destination coordinates are the new top-right of the tiles.<br/>
EG: `copytile 10,15, 50,60, 100,16`

#### Place Tiles
`placetile startX,startY, endX,endY, id, style (optional)`<br/>
Places tiles of specified id and style within the region (startX,startY) - (endX,endY). Can have difficulty placing non-valid tile or furniture parts.<br/>
EG: `destroytile 10,15, 50,60`

#### Destroy Tiles
`destroytile startX,startY, endX,endY`<br/>
Destroys the tiles within the specified region. No items are dropped.<br/>
EG: `destroytile 10,15, 50,60`

#### Remove Inventory Item
`removeitem id, count, player(s)`<br/>
Removes items from the [targeted player(s)](#player-selection)'s inventory, up to count.<br/>
EG: `removeitem 26, 10, @r`

#### Set Health
`sethealth value, player(s)`<br/>
Sets the current health of [targeted player(s)](#player-selection) to value. Unusual values may not persist after world save/rejoin to preserve vanilla compatibility.<br/>
EG: `sethealth 10, @a`

#### Set Max Health
`setmaxhealth value, player(s)`<br/>
Sets the maximum health of [targeted player(s)](#player-selection) to value. Unusual values may not persist after world save/rejoin to preserve vanilla compatibility.<br/>
EG: `setmaxhealth 200, @a`

#### Set Mana
`setmana value, player(s)`<br/>
Sets the current mana of [targeted player(s)](#player-selection) to value. Unusual values may not persist after world save/rejoin to preserve vanilla compatibility.<br/>
EG: `setmana 0, @a`

#### Set Max Mana
`setmaxmana value, player(s)`<br/>
Sets the maximum mana of [targeted player(s)](#player-selection) to value. Unusual values may not persist after world save/rejoin to preserve vanilla compatibility.<br/>
EG: `setmaxmana 200, @a`

#### Spawn World Item
`spawnitem x, y, id, count`<br/>
Spawns an item in the world at coordinates x,y with specified id and count.<br/>
EG: `spawnitem 26,10, 31, 20`

#### Teleport Player
`teleport x, y, player(s)`<br/>
Teleports [targeted player(s)](#player-selection) to x,y.<br/>
EG: `teleport 26,10, @a`

## Conditionals

Conditionals check whether something is true or false, and then give an output depending on that result, for example, checking if it's day, or if a player has an item, or if a tile exists at a certain spot.

![](/readme-img/conditional.png)

- Sending a signal throught the IN tile activates the Conditional.
- Once activated, if the condition is true a signal is sent through OUT 1.
- Once activated, if the condition is false a signal is sent through OUT 2. 

### Conditional Commands
#### Is Day
`day`<br/>
Checks to see if it is currently Day.


## Entity Selection

### Player Selection

If a command asks for a player as a parameter, a number of options can be put in, depending on who you wish to target.

#### All Players
`@a`<br/>
Applies TIC effect to all players.<br/>
EG: `giveitem 26, 10, @a`

#### Stored Players
`@s, store`<br/>
Applies TIC effect to the player within specified [datastore](#data-stores).<br/>
EG: `giveitem 26, 10, @s, store`

### NPC Selection

If a command asks for an NPC as a parameter, a number of options can be passed, depending on what you wish to target.

#### All NPCs
`@a`<br/>
Applies TIC effect to all NPCs.<br/>
EG: `killnpc @a`

#### By ID
`@i, id`<br/>
Applies TIC effect to all NPCs with specified id.<br/>
EG: `killnpc @i, 3`

#### Enemy NPCs
`@e`<br/>
Applies TIC effect to all enemy NPCs (IE NPCs that aren't town NPCs or Friendly NPCs).<br/>
EG: `killnpc @e`

#### Town NPCs
`@t`<br/>
Applies TIC effect to all town NPCs.<br/>
EG: `killnpc @e`

#### Friendly NPCs
`@f`<br/>
Applies TIC effect to all friendly NPCs.<br/>
EG: `killnpc @e`


## Data Stores

Some commands will allow you to output an effected entity so that they can be effected by another TIC block. This is acheived through Data Stores. To create a store, simply reference it in a command that can output to one, and to use one refer to the [entity-selection](#entity-selection) guide for the relevant entity. Currently supported entity types are: players, NPCs.

## Examples

#### Give 20 Lesser Healing Potions to players when they die
First, we need a Trigger to trigger when a player dies, and store that player in a [datastore](#data-stores). Let's call this store `deadplayer`. This is achieved by using the [playerdeath](#player-death) command in a trigger:<br/>
`playerdeath deadplayer`<br/>
This Trigger's output should then be wired to an Influencer with the [forcegiveitem](#force-give-item-to-player) command, targeting the [datastore](#data-stores) we created in the the Trigger:<br/>
`forcegiveitem 28, 20, @s, deadplayer`<br/>
Now, when a player dies, the trigger will add them to the datastore and send the influencer a signal, which will give them 20 Lesser Healing Potions to the player we just stoed in the datastore!


## Credits
- Thanks to jopojelly for use of his UI Classes.
- Thanks to jopojelly, Rartrin and direwolf420 for miscellaneous support.
- Thanks to Khaios for the inspiration!

All other work by darthmorf / Sam Poirier
