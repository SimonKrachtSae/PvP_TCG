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
public enum MainPhaseStates
{
    StartPhase,
    HandCardSelected,
    Summoning,
    Aborting,
    AttackPhase,
    Blocking,
    Tributing
}
public enum MonsterCardLocation
{
    InDeck,
    OnField,
    InGraveyard,
    InHand
}