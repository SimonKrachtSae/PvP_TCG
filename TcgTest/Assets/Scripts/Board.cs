using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] private List<HandField> playerHandCards;
    public List<HandField> PlayerHandCards { get => playerHandCards; set => playerHandCards = value; }
    [SerializeField] private List<MonsterField> playerMonsterFields;
    public List<MonsterField> PlayerMonsterFields { get => playerMonsterFields; set => playerMonsterFields = value; }

    [SerializeField] private List<MonsterField> enemyMonsterFields;
    public List<MonsterField> EnemyMonsterFields { get => enemyMonsterFields; set => enemyMonsterFields = value; }
    public List<HandField> EnemyHandCards;
    [SerializeField] private TMP_Text enemyDeck;
    [SerializeField] private TMP_Text enemyGraveyard;
    public GameObject PlayerHandParent { get => playerHandParent; set => playerHandParent = value; }

    [SerializeField] private GameObject playerHandParent;
    public GameObject EnemyHandParent { get => enemyHandParent; set => enemyHandParent = value; }

    [SerializeField] private GameObject enemyHandParent;
    public GameObject HandFieldPrefab { get => handFieldPrefab; set => handFieldPrefab = value; }
    [SerializeField] private GameObject handFieldPrefab;

    public HandField CardInfo;
    public DuelistUIs PlayerUIs { get; set; }
    public DuelistUIs EnemyUIs { get; set; }
    public TMP_Text TurnCount { get => turnCount; set => turnCount = value; }
    public List<Button> MonsterCardControls { get => monsterCardControls; set => monsterCardControls = value; }

    [SerializeField] private TMP_Text turnCount;
    [SerializeField] private List<Button> monsterCardControls;

    [SerializeField] TMP_Text playerCardsInDeckCount;
    [SerializeField] TMP_Text playerSummonPowerText;
    [SerializeField] TMP_Text playerCardsInGraveyardCount;

    [SerializeField] TMP_Text enemyCardsInDeckCount;
    [SerializeField] TMP_Text enemyCardsInGraveyardCount;
    [SerializeField] TMP_Text enemySummonPowerText;

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else Instance = this;

        PlayerUIs = new DuelistUIs(playerCardsInDeckCount, playerCardsInGraveyardCount, playerSummonPowerText);
        EnemyUIs = new DuelistUIs(enemyCardsInDeckCount, enemyCardsInGraveyardCount, enemySummonPowerText);
    }
   // public HandField GetFreeMonsterField(DuelistType type)
   // {
   //     List<HandField> MonsterFields = playerMonsterFields;
   //     if (type == DuelistType.Enemy) MonsterFields = enemyMonsterFields;
   //     for (int i = 0; i < 5; i++)
   //     {
   //         if (MonsterFields[i].Card == null) return MonsterFields[i];
   //     }
   //     return null;
   // }
    public void AddMonsterCardCommandButton(MonsterCardButton monsterCardButton)
    {
        int index = 0;
        foreach (Button b in MonsterCardControls) if (b.isActiveAndEnabled) index++;
        if (index > 2) return;
        switch(monsterCardButton)
        {
            case MonsterCardButton.Summon:
                MonsterCardControls[index].gameObject.SetActive(true);
                MonsterCardControls[index].GetComponentInChildren<TMP_Text>().text = "Summon";
                for (int i = 0; i < PlayerMonsterFields.Count; i++)
                {
                    //PlayerMonsterFields[i].Button.onClick.AddListener(() => { GameManager.Instance.SummonMonsterCard(cardLayout, i); });
                }
                break;
        }
    }
}
