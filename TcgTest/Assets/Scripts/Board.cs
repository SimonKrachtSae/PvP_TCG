using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] private List<Field> playerHandCards;
    public List<Field> PlayerHandCards { get => playerHandCards; set => playerHandCards = value; }
    [SerializeField] private List<Field> playerMonsterFields;
    [SerializeField] private TMP_Text playerCardsInDeckCount;
    [SerializeField] private TMP_Text playerCardsInGraveyardCount;

    [SerializeField] private TMP_Text playerGraveyard;

    [SerializeField] private List<Field> enemyMonsterFields;
    public List<Field> EnemyHandCards;
    [SerializeField] private TMP_Text enemyDeck;
    [SerializeField] private TMP_Text enemyGraveyard;
    [SerializeField] private TMP_Text enemyCardsInDeckCount;
    [SerializeField] private TMP_Text enemyCardsInGraveyardCount;
    public TMP_Text EnemyDeckText { get => enemyCardsInDeckCount; set => enemyCardsInDeckCount = value; }
    public GameObject PlayerHandParent { get => playerHandParent; set => playerHandParent = value; }

    [SerializeField] private GameObject playerHandParent;
    public GameObject EnemyHandParent { get => enemyHandParent; set => enemyHandParent = value; }

    [SerializeField] private GameObject enemyHandParent;
    public GameObject HandFieldPrefab { get => handFieldPrefab; set => handFieldPrefab = value; }
    [SerializeField] private GameObject handFieldPrefab;

    public Field CardInfo;
    [SerializeField] private TMP_Text playerSummonPowerText;
    [SerializeField] private TMP_Text enemySummonPowerText;
    public DuelistUIs PlayerUIs { get; set; }
    public DuelistUIs EnemyUIs { get; set; }
    public TMP_Text TurnCount { get => turnCount; set => turnCount = value; }
    public List<Button> MonsterCardControls { get => monsterCardControls; set => monsterCardControls = value; }

    [SerializeField] private TMP_Text turnCount;
    [SerializeField] private List<Button> monsterCardControls;
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else Instance = this;
        PlayerUIs = new DuelistUIs(playerCardsInDeckCount,playerCardsInGraveyardCount,playerSummonPowerText);
        EnemyUIs = new DuelistUIs(enemyCardsInDeckCount,enemyCardsInGraveyardCount,enemySummonPowerText);
    }
    public Field GetFreeMonsterField(DuelistType type)
    {
        List<Field> MonsterFields = playerMonsterFields;
        if (type == DuelistType.Enemy) MonsterFields = enemyMonsterFields;
        for (int i = 0; i < 5; i++)
        {
            if (MonsterFields[i].Card == null) return MonsterFields[i];
        }
        return null;
    }
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
                //monsterCardControls[index].onClick.AddListener(() => { OnHeadsClicked(); });
                break;
        }
    }
}
