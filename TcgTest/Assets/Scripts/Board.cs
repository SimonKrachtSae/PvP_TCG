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
    [SerializeField] private TMP_Text playerDeckText;
    public TMP_Text PlayerDeckText { get => playerDeckText; set => playerDeckText = value; }

    [SerializeField] private TMP_Text playerGraveyard;

    [SerializeField] private List<Field> enemyMonsterFields;
    public List<Field> EnemyHandCards;
    [SerializeField] private TMP_Text enemyDeck;
    [SerializeField] private TMP_Text enemyGraveyard;
    [SerializeField] private TMP_Text enemyDeckText;
    public TMP_Text EnemyDeckText { get => enemyDeckText; set => enemyDeckText = value; }

    public Field CardInfo;
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else Instance = this;
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
}
