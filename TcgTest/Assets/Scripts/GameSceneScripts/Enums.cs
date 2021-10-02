using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardEvent
{
    FollowMouse_MouseDown,
    FollowMouse_MouseDrag,
    Block,
    Attack,
    Summon,
    Discard,
    Destroy,
    Play,
    Burn,
    DrawLine_MouseDown,
    DrawLine_MouseDrag,
    Recall
}
public enum MouseEvent
{
    Down,
    Drag,
    Up
}
public enum NetworkTarget
{
    Local,
    Other,
    All
}
public enum DuelistType
{
    Player,
    Enemy,
    Both
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
    Running,
    GameOver
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
public enum TurnPhase
{
    DrawPhase,
    MainPhase,
    AttackPhase,
    EndPhase
}
public enum MonsterCardButton
{
    Attack,
    Summon
}
public enum GameManagerStates
{
    StartPhase,
    HandCardSelected,
    Summoning,
    Aborting,
    AttackPhase,
    Blocking,
    Tributing,
    Busy,
    Recalling,
    Discarding,
    Destroying,
    EndPhase
}
public enum MonsterCardLocation
{
    InDeck,
    OnField,
    InGraveyard,
    InHand
}