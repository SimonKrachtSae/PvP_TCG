using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public enum Team
{
    A,
    B,
    None
}
public enum ConnectionStatus
{
    Connecting,
    ConnectionFailed,
    Connected,
    HostingOrJoiningRoom,
    InRoomSelection,
    InRoom,
    DeckBuilder
}
public enum PlayerType
{
    Player,
    NPC
}

public enum NPCstate
{
	Wander,
	Chase,
	Shoot,
	Flee,

	GameOver,
	MasterLeft
}
