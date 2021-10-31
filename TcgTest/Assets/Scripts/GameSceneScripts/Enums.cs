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
    Recall,
    CardOverField
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
public enum FieldState
{
    Selected,
    Unselected
}
public enum GameState
{
    CoinFlip,
    Running,
    GameOver,
    Paused
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
public enum TurnState
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
    EndPhase,
}
public enum MonsterCardLocation
{
    InDeck,
    OnField,
    InGraveyard,
    InHand
}
public enum CardName
{
  Monster1,
  Monster2,
  Monster3,
  Monster4,
  Monster5,
  Monster6,
  Magic1,
  Magic2,
  Magic3,
  Magic4,
  Magic5,
  Magic6,
}