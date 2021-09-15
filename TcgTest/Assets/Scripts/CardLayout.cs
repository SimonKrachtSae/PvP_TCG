using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class CardLayout : MonoBehaviour
{
    [SerializeField] private Duelist player;
    [SerializeField] private protected TMP_Text NameTextUI;
    [SerializeField] private protected TMP_Text EffectTextUI;
    [SerializeField] private protected TMP_Text AttackTextUI;
    [SerializeField] private protected TMP_Text DefenseTextUI;
    [SerializeField] private protected TMP_Text PlayCostTextUI;
    [SerializeField] private Button button;
    private MonsterCardStats monsterCard;
    private FieldState state;
    public MonsterCardStats MonsterCard
    {
        get => monsterCard;
        set
        {
            NameTextUI.text = value.CardName;
            if (value.Effect != null) EffectTextUI.text = value.Effect.EffectDescription;
            else EffectTextUI.text = "";
            DefenseTextUI.text = value.Defense.ToString();
            AttackTextUI.text = value.Attack.ToString();
            PlayCostTextUI.text = value.PlayCost.ToString();
            monsterCard = value;
        }
    }
    private void Start()
    {
        if (button != null) button.onClick.AddListener(() => { OnFieldButtonClick(); });
        state = FieldState.Unselected;
    }
    public void Update()
    {
        if (state == FieldState.Selected)
        {

        }
    }
   public void OnFieldButtonClick()
   {
  //
  //     GameManager gm = GameManager.Instance;
  //     if (gm.TurnState == TurnState.Normal)
  //     {
  //         Board.Instance.CardInfo.MonsterCard = monsterCard;
  //         if (monsterCard != null)
  //         {
  //             if (player.SummonPower >= monsterCard.PlayCost)
  //             {
  //                 gm.TurnState = TurnState.Summoning;
  //                 gm.CardToBeSummoned = this;
  //             }
  //         }
  //     }
  //     else if (gm.TurnState == TurnState.Summoning)
  //     {
  //         if (isMonsterField && monsterCard == null)
  //         {
  //             MonsterCard = gm.CardToBeSummoned.MonsterCard;
  //             gm.TurnState = TurnState.Normal;
  //             gm.CardToBeSummoned.MonsterCard = null;
  //             gm.CardToBeSummoned = null;
  //             gm.CurrentPlayer.SummonPower -= monsterCard.PlayCost;
  //         }
  //     }
   }
}
