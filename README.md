# CodeMonkey2023-Overcooked-Clone Online with netcode / Lobby / relay

/WORK IN PROGRESS/
 This is the multiplayer complete tutorial covered by Code Monkey in this video: https://www.youtube.com/watch?v=7glCsF9fv3s
This game project started as a Singleplayer only game, then netcode support was added later on, I've also uploaded the complete sinpleplayer repo in github: https://github.com/OzgenKoklu/KitchenChaosSingleplayer
In this readme, I only mention the Netcode/Lobby/Relay related features and coding, the repository for Singleplayer project include the details on main game mechanics.

Disclaimer: All assets were borrowed from Code Monkey and are not used for commercial use, this is a learning project and I've built this on my own under instructions by Code Monkey. 
This built wont run on your computer as is unless linked to Unity Services, to link go to Edit > Project Settings > Services and link to a project in which Lobby and Relay services are enabled.

ABOUT THE GAME: This is a small scale casual game that is highly inspired by OverCooked, where you have to deliver specific orders in a time limit. 
It is a complete package with scene cycle, sound and animation, options menu, savable settings and key bindings with added multiplayer! 

/IMAGES AND VIDEO TO BE ADDED/
Screenshots: 
-main menu ss
-lobby list ss
-lobby scene ss
-Game play ss

Videos & Gifs: 
-Lobby color change gif
-Gameplay gif 

-YT Link: 

TLDR: What I've learned from this project:
-Implementing Netcode For Gameobject , Lobby & Relay by Unity to an already existing singleplayer game
-Basic understanding of multiplayer game logic: Server/Client authoritative design meaning, RPC's, basic solutions for bad connection
-Making a fully working game scene flow using netcode for gameObjects, then using lobby and relay services to connect online
-

On my decision to seperate the project in two repositories:
Short answer: The source code changed too much. And since I'm much new to multiplayer development, I'm not much familiar with structures and design choiches that were implemented in this current state.
Before this project, I havent done any multiplayer game project, I've heard of Photon Network and I think it still is a very popular option to make a multiplayer game.
The architectural needs of a multiplayer game is completely different and the programmer has to decide whether the game would be server authoritative or client authoritative and this alone changes the structure of the project. 
In singleplayer games, especially when you are in the very beggining in your developer journey, you only deal with whats on the screen, you make things change in behaviour to make the game play, however, 
the entire communication has to be tought out clearly for multiplayer development. Theres some essence of backend development in this sense where requests should be dealt with in particular ways. 
This project helped me further understand the C# and OOP concepts, I've understood while building it, but yet I feel unfamiliar still.

More about this project for those who have more time to spend: 
The project is a course project by Code Monkey and has its curriculum in this link: https://unitycodemonkey.com/kitchenchaosmultiplayercourse.php

However, I want to re-visit my own commits and make my own list of what I've learned: 
1)NetworkBehaviour, inherits Monobehavior, needed for networkObjects to work. Holds Network methods, properties and fields like IsHost etc.
All things gameplay related needs to be synced to other players somehow. This is one of the ways.
2)Server authoritative / Client authoritative desing. Programmer needs to decide whether code is validated on server side or client side. For casual game like this its OK to go with client authoritative, but its prone to hacks, not ideal for hardcore games.
3)ServerRPC / ClientRPC and the way they are deployed. ServerRPC only runs on server while ClientRPC runs on clients too. 
Validations, instantiation of network objects(SpawnKitchenObjectServerRpc), and syncing of method execution is handheld in serverRPC while the final execution of the logic is generally happens in Client side. 
4)IsServer bool is super useful to make some logic run just on server to avoid sync issues, IsOwner is great for doing some local logic like animation
5)In RPC's you can ony use Serializable parameter, thus sending int value index's work, but sending custom scriptable objects dont*
Making int/index returning functions, or reverse of that, returning class from int.
6)Singleton's cant be NetworkBehaviour, because there will be multiple instances of them in runtime. 
Player wont be a singleton but it can use static events for similar results, or have a LocalInstance. 
7)NetworkManager's StartHost(), StartClient(), etc 
8)Spawning Kitchen Object On Network Logic Expanation Step by step: 
..
Destroying KitchenObject via RPC's: 
..
9)Animation Syncing via RPC's Explained step by step:
..
10) Network Variables and details about their usage. 
..
11) Syncing game states, scene flow in multiplayer (lobby readdy, start game etc, pause state) 
12) PlayerData struct is shared through KitchenGameMultiplayer, usage of INetwrokSerializable and IEquatable 
13) Lobbies for connecting players. Awaitable Async methods, lobby related terminology such as queryResponse, queryFilter, heartbeat signals
...
14) Relay / Allocation to the host, Task<T> type as an async return type. 
...

ABSTRACT NOTES//





/WORK IN PROGRESS/