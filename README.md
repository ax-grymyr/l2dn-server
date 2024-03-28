# L2Dn Server Project

### L2Dn Server is an open-source server emulator fully written in .NET for the famous Korean MMORPG Lineage 2.

The source code initially is the port of the [L2J-Mobius Essence 7.3](https://l2jmobius.org).

This page only gives very basic information, 
for the detailed information about building 
and developing L2Dn Server, please visit the Wiki (TBD).

- main branch is the development version,
- release branches are the stable versions for specific protocols.

TBD: license, notes, etc.

#### Milestones

- [x] Make code compile
- [x] Make the server able to run without crashes
- [ ] Make main functionality work
  * Teleports, skills, fighting are working         
- [ ] Port scripts (quests, events, etc)
  * Most of the handlers have been ported, including GM commands 
  * Tutorial quest for Dwarf fighters has been ported as a quest example 
- [ ] Datapack and Geodata

#### Client

The development branch is for protocol 447. 
The instruction how to setup the client is [here](L2Dn/Wiki/Client.md).