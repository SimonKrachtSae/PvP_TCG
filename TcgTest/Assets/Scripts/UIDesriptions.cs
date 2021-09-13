using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class UIDesriptions : MonoBehaviour
{
    public static UIDesriptions Instance;

    public TMP_Text PlayerSummonPowerText;

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else Instance = this;
    }
}
