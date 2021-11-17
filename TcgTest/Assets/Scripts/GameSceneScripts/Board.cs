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
    [SerializeField] private List<GameObject> playerMonsterFields;
    public List<GameObject> PlayerMonsterFields { get => playerMonsterFields; set => playerMonsterFields = value; }

    [SerializeField] private List<GameObject> enemyMonsterFields;
    public List<GameObject> EnemyMonsterFields { get => enemyMonsterFields; set => enemyMonsterFields = value; }
    [SerializeField] private TMP_Text enemyDeckText;
    [SerializeField] private TMP_Text playerDeckText;
    public GameObject PlayerHandParent { get => playerHandParent; set => playerHandParent = value; }

    [SerializeField] private GameObject playerHandParent;
    public GameObject EnemyHandParent { get => enemyHandParent; set => enemyHandParent = value; }

    [SerializeField] private GameObject enemyHandParent;
    public GameObject HandFieldPrefab { get => handFieldPrefab; set => handFieldPrefab = value; }
    [SerializeField] private GameObject handFieldPrefab;
    public DuelistUIs PlayerUIs { get; set; }
    public DuelistUIs EnemyUIs { get; set; }
    public TMP_Text TurnCount { get => turnCount; set => turnCount = value; }
    public List<Button> MonsterCardControls { get => monsterCardControls; set => monsterCardControls = value; }

    [SerializeField] private TMP_Text turnCount;
    [SerializeField] private List<Button> monsterCardControls;

    [SerializeField] TMP_Text playerCardsInDeckCount;
    [SerializeField] Transform playerManaPos;
    [SerializeField] TMP_Text playerCardsInGraveyardCount;

    [SerializeField] TMP_Text enemyCardsInDeckCount;
    [SerializeField] TMP_Text enemyCardsInGraveyardCount;
    [SerializeField] Transform enemyManaPos;

    [SerializeField] private TMP_Text playerInfoText;
    public TMP_Text PlayerInfoText { get => playerInfoText; set => playerInfoText = value; }

    [SerializeField] private GameObject blockRequest;
    public GameObject BlockRequest { get => blockRequest; set => blockRequest = value; }

    [SerializeField] private GameObject playerDeckFieldObj;
    public GameObject PlayerDeckFieldObj { get => playerDeckFieldObj; set => playerDeckFieldObj = value; }
    [SerializeField] private GameObject enemyDeckFieldObj;
    public GameObject EnemyDeckFieldObj { get => enemyDeckFieldObj; set => enemyDeckFieldObj = value; }

    [SerializeField] private GameObject playerGraveyard;
    public GameObject PlayerGraveyard { get => playerGraveyard; set => playerGraveyard = value; }
    [SerializeField] private GameObject enemyGraveyard;
    public GameObject EnemyGraveyard { get => enemyGraveyard; set => enemyGraveyard = value; }

    [SerializeField] private GameObject burnField;
    public GameObject BurnField { get => burnField; set => burnField = value; }
    public TMP_Text EnemyDeckText { get => enemyDeckText; set => enemyDeckText = value; }
    public TMP_Text PlayerDeckText { get => playerDeckText; set => playerDeckText = value; }

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else Instance = this;

        PlayerUIs = new DuelistUIs(playerCardsInDeckCount, playerCardsInGraveyardCount,playerManaPos);
        EnemyUIs = new DuelistUIs(enemyCardsInDeckCount, enemyCardsInGraveyardCount, enemyManaPos);
    }
}
