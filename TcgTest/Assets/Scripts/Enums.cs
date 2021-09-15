using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DuelistType
{
    Player,
    Enemy
}
public enum TurnState
{
    Normal,
    Summoning
}
public enum FieldState
{
    Selected,
    Unselected
}
public enum GameState
{
    CoinFlip,
    Running
}
public enum CoinState
{
    Heads,
    Tails
}
public enum ClientType
{
    Host,
    Client
}