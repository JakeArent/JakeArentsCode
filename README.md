# JakeArentsCode
Code for projects that I have worked on for classes at Florida Polytechnic.

All assets used in the creation of these games were found for free on the Unity Asset Store.

<b>Quickdraw</b>

The first project, Quickdraw, is a western themed First Person Shooter with a twist. The player character feels the need to always spin their gun. When the player shoots, the bullet will travel forwards from wherever the gun is currently pointing. There are 4 levels created for this game.
The first level is a quick tutorial, where you have to shoot a few bottles on a counter, and then are ambushed by two bandits.
On the second level, you have to rescue the mayor from the bandits, but the stairs are blocked by debris. There is dynamite hidden around the first floor to clear the stairway.
The third level has you clear the town of any remaining bandits, introducing a new enemy that can shoot back.
The final level has you go to the bandits camp to clear them out once and for all.

Most of these levels were designed to test out the gun spinning mechanic in different environments, and while it was an interesting phyiscal timing challenge, it did not work well in every environment. Notably, on the third level it is more difficult than intended to shoot the snipers on an elevated position.


<b>RTS Project</b>

This is an unnamed Real Time Strategy game built simply to use Unity Networking. Because unity is depreciating HLAPI, I do not know how long this project will actually be runnable.

The game concept was a simple RTS game to test a new resource mechanic, Mana. While Mana can be flavored for any genre, most of the assets found fit a medieval era, hence the naming convention. The idea is that all building take an amount of mana every second to produce units or research upgrades. If you have less Mana than is required, or other buildings are also working, buildings will work at a lower rate, based on available mana. For example, a Base that produces a worker may take five mana per second. If you only gain four mana per second, then the base will work at 80% of it's normal rate.
In this game, Mana is uniformly distributed among all buildings. A variation of this could see players giving different buildings a priority between 1 and 10, and Mana would be distributed accordingly. This gives the player a better control over which buildings will be running innefectively, if they are using more Mana than they are producing.
In order to play the game, once it runs, you will need to have one computer act as a server. The next player to join will be player one, and the second player that joins will be automatically assigned player 2.
