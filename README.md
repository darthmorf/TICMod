![](/readme-img/logo.png)

TICMod (Trigger, Influencer, Conditional Mod) is a mod for Terraria using tModloader that adds blocks that function similar to Command Blocks from Minecraft. It is not balanced around a standard playthrough; it's intended use is for Adventure Maps, Servers and Testing/Building Worlds.

Functionality is implemented through three seperate tiles; the Trigger, the Influencer and the Conditional:

![](/readme-img/ticmod.png)

### Table of Contents <!-- omit in toc -->
- [User Interface](#user-interface)
- [Triggers](#triggers)
  - [Trigger Commands](#trigger-commands)
- [Influencers](#influencers)
  - [Influencer Commands](#influencer-commands)
- [Conditionals](#conditionals)
  - [Conditional Commands](#conditional-commands)


## User Interface

Commands are entered through each block's own UI, which can be opened by right clicking on the tile. The UI can be dragged around the screen, and multiple TIC UIs can be open at once for ease of use.
![](/readme-img/validui.png)

If a command is invalid, the text becomes red, and an error code is displayed upon hover:
![](/readme-img/invalidui.png)

The 'show debug output' checkbox determines whether debug output is displayed in the chat when the tile activates, for example:
![](/readme-img/debugout.png)



## Triggers

Triggers function as an 'event' - they are used when you want something happening to send a signal, for example a boss being spawned, a tile being edited, an item being picked up, etc.

![](/readme-img/trigger.png)

- Sending a signal through the IN tile enables or disables the Trigger, depending on it's current state.
- A signal is sent to the OUT tile when the Trigger is triggered.

### Trigger Commands
**Set Time**<br/>
`time hh:mm`<br/>
Sets the world's time. Must be in 24 hour format.<br/>
EG: `time 14:30`


## Influencers

Influencers function as the main heavy lifters of the mod; they are the ones that actually *do* things, for example spawning an NPC, creating a tile, or dropping an item.

![](/readme-img/influencer.png)

- Sending a signal throught the IN tile activates the Influencer.

### Influencer Commands
**Say**<br/>
`say rrr,ggg,bbb message`<br/>
Sends a message to the chat with the colour specified by `rrr,ggg,bbb`, in RGB format. Message can contain `\n` which will create a line break in the message.<br/>
EG: `say 0,160,255 Hello World`

**Spawn NPC by Name**<br/>
`spawnnpc x,y name`<br/>
Spawns the NPC with specified name at world position x,y. [NPC Names can be found on the Wiki](https://terraria.gamepedia.com/NPC_IDs). Note: Some NPCs, like Zombies have multiple variants. To control which one spawns more accurately, use `spawnpcid` instead.<br/>
EG: `spawnnpc 26,10 Dr Man Fly`

**Spawn NPC by ID**<br/>
`spawnnpcid x,y id`<br/>
Spawns the NPC with specified ID at world position x,y. [NPC IDs can be found on the Wiki](https://terraria.gamepedia.com/NPC_IDs).<br/>
EG: `spawnnpcid 26,10 53`


## Conditionals

Conditionals check whether something is true or false, and then give an output depending on that result, for example, checking if it's day, or if a player has an item, or if a tile exists at a certain spot.

![](/readme-img/conditional.png)

- Sending a signal throught the IN tile activates the Conditional.
- Once activated, if the condition is true a signal is sent through OUT 1.
- Once activated, if the condition is false a signal is sent through OUT 2. 

### Conditional Commands
**Is Day?**<br/>
`day`<br/>
Checks to see if it is currently Day.<br/>