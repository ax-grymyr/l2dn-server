# L2Dn Server Project

**L2Dn Server** is an open-source server emulator fully written in .NET for the famous Korean MMORPG Lineage 2.

This page only gives very basic information, 
for the detailed information about building, developing and using
L2Dn Server, please visit [the documentation](https://l2dn.readthedocs.io/).

The project code initially is the port of the L2J server written in Java. 

#### Milestones

- [x] Make code compile
- [x] Make the server able to run without crashes
- [ ] Make main functionality work, see [Test Plan](https://l2dn.readthedocs.io/en/latest/TestPlan/) 
  * Teleports, skills, fighting are working         
- [ ] Port scripts (quests, events, etc)
  * Most of the handlers have been ported, including GM commands 
  * Tutorial quest for Dwarf fighters has been ported as a quest example 
- [ ] Datapack and Geodata

#### Client

The development branch is for protocol 447. 
The instruction how to set up the client is [here](https://l2dn.readthedocs.io/en/latest/Client/).

#### Other Documentation Pages
- [Changelog](https://l2dn.readthedocs.io/en/latest/Changelog/)
- [Test Plan](https://l2dn.readthedocs.io/en/latest/TestPlan/)
