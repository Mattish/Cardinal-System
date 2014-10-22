Cardinal-System
===============

Scalable MMO Engine. Working project in spare time. Please feel free to comment on any of the ideas or design here.

## Design

Potentially need to split the components deeper:

* Nodes - The hard work, connected via Circuits and can split/join based on workload
* Circuits - The pipes to connect everything, connect between nodes which have edges and for admin/debug connecting. Ideally all traffic will go over Circuits for easier metrics/debugging.

Kind of have an idea about what I want for these components, although still unsure:

* Yui(?) - Admin/Debug controller. Can connect to all other component to do and know everything
* Heathcliff(?) - Some sort of higher level management for overall system to manage load etc.

Things that need to be considered:

* NerveGear(?) - Outside connection/server and security for that
* Ruru(?) - Redundency of somekind, thinking of placing in, or connected to history into circuits so can easily plug-in & dump to new Node

## Cardinal-System-Shared

This contains all the transferable common objects. Internal domain objects can be physical or information. DTOs currently are labeled as a 'Change', might just change to/append 'DTO' for simplicity.
