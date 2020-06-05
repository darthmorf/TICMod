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
    - [Spawn NPC by Name](#spawn-npc-by-name)
    - [Spawn NPC by ID](#spawn-npc-by-id)
    - [Give Item to Player](#give-item-to-player)
    - [Force Give Item to Player](#force-give-item-to-player)
    - [Draw World Text](#draw-world-text)
    - [Draw UI Text](#draw-ui-text)
    - [Respawn Player](#respawn-player)
    - [Kill Player](#kill-player)
- [Conditionals](#conditionals)
  - [Conditional Commands](#conditional-commands)
    - [Is Day?](#is-day)
- [Entity Selection](#entity-selection)
  - [Player Selection](#player-selection)
    - [All Players](#all-players)
    - [Stored Players](#stored-players)
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

#### Spawn NPC by Name
`spawnnpc x,y, name`<br/>
Spawns the NPC with specified name at world position x,y. [NPC Names can be found on the Wiki](https://terraria.gamepedia.com/NPC_IDs). Note: Some NPCs, like Zombies have multiple variants. To control which one spawns more accurately, use `spawnpcid` instead.<br/>
EG: `spawnnpc 26,10, Dr Man Fly`

#### Spawn NPC by ID
`spawnnpcid x,y, id`<br/>
Spawns the NPC with specified ID at world position x,y. [NPC IDs can be found on the Wiki](https://terraria.gamepedia.com/NPC_IDs).<br/>
EG: `spawnnpcid 26,10, 53`

#### Give Item to Player
`giveitem id, count, player(s)`<br/>
Gives [targeted player](#player-selection) the specified count of item with specified id.<br/>
EG: `giveitem 26, 10, @a`

#### Force Give Item to Player
`forcegiveitem id, count, player(s)`<br/>
Gives [targeted player](#player-selection) the specified count of item with specified id. Unlike [giveitem](#give-item-to-player), places the item directly in the inventory, rather than triggering the usual pickup text. Works when players are dead.<br/>
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
`respawn player`<br/>
Respawns the current player if they are dead. Does nothing if they are not.<br/>
EG: `respawn @a`

#### Kill Player
`kill player, reason`<br/>
Kills the targeted player, with `reason` as the message displayed in chat. The string `#name` can be included within `reason` to refer to the name of the player that was killed.<br/>
EG: `kill @n, darthmorf, Killed #name\, they deserved it!`

## Conditionals

Conditionals check whether something is true or false, and then give an output depending on that result, for example, checking if it's day, or if a player has an item, or if a tile exists at a certain spot.

![](/readme-img/conditional.png)

- Sending a signal throught the IN tile activates the Conditional.
- Once activated, if the condition is true a signal is sent through OUT 1.
- Once activated, if the condition is false a signal is sent through OUT 2. 

### Conditional Commands
#### Is Day?
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


## Data Stores

Some commands (mostly Triggers) will allow you to output an effected entity so that they can be effected by another TIC block, like an influencer. This is acheived through Data Stores. To create a store, simply reference it in a command that can output to one. Currently supported entity types are: players.

## Examples

#### Give 20 Lesser Healing Potions to players when they die.
First, we need a Trigger to trigger when a player dies, and store that player in a [datastore](#data-stores). Let's call this store `deadplayer`. This is achieved by using the [playerdeath](#player-death) command in a trigger:<br/>
`playerdeath deadplayer`<br/>
This Trigger's output should then be wired to an Influencer with the [giveitem](#give-item-to-player) command, targeting the [datastore](#data-stores) we created in the the Trigger:<br/>
`giveitem 28, 20, @s, deadplayer`<br/>
Now, when a player dies, the trigger will add them to the datastore and send the influencer a signal, which will give them 20 Lesser Healing Potions to the player we just stoed in the datastore!


## Credits
- Thanks to jopojelly for use of his UI Classes.
- Thanks to jopojelly, Rartrin and direwolf420 for miscellaneous support.
- Thanks to Khaios for the inspiration!

All other work by darthmorf / Sam Poirier