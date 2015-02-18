Cardinal-System
===============

### Currently blocked on working out how to split a world up on the fly between nodes, while still maintaining quad tree for local

Scalable MMO Engine. Created to assess my own ability to design a system and codebase. Based on SAO/GGO naming because why not. Working project in spare time. Please feel free to comment on any of the ideas or design here.

### Design

Potentially need to split the components deeper:

* Nodes - The hard work, connected via Circuits and can advertise/join/ditch/register/more things with circuits
* Circuits - The pipes to connect everything, connect between nodes which have edges and for admin/debug connecting. Ideally all traffic will go over Circuits for easier metrics/debugging.

Kind of have an idea about what I want for these components, although still unsure:

* Yui(?) - Admin/Debug controller. Can connect to all other component to do and know everything
* Heathcliff(?) - Some sort of higher level management for overall system to manage load etc. Commands to be sent over circuits, some might just be broadcast(e.g. Looking for a node to handle X) or just to a single node to do something(e.g. Send me your current info)

Things that need to be considered:

* NerveGear(?) - Outside connection/server and security for that
* Ruru(?) - Redundency of somekind, thinking of placing in, or connected to history into circuits so can easily plug-in & dump to new Node

![Potential architecture](http://puu.sh/crNIy/f697b3ae25.png)

### Cardinal-System-Shared

This contains all the transferable common objects. Internal domain objects can be physical or information. Message base type for every message. 

### Cardinal-System-Common

As it says on the tin.

### Cardinal-System-Test

Currently just used for testing out things. Most likely will add in formal unit tests at some point with NUnit.

### To Do
 * Receive incoming messages to somewhere and manage if they need to be delayed further(Entity messages) or not(Component messages)
 * Area/grid/quadtree for Nodes. 
 * Bordering, advertising to network for bordering, border feathering
 * The rest of it

### To think about
* How does bordering work between nodes? Current idea is 'RegisterOwner' and 'RegisterInterest' and potentially a 'EntityOwnerSwap'
* Should there be 'idle' components waiting for allocation?
* Dto stuff is okay and fast enough?
* How much are we really going to be sending over Circuits/out from nodes? We want to limit as much network traffic which is dangerous with entities lingering at borders
