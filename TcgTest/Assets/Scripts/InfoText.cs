using Assets.Customs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MB_Singleton<InfoText>
{
    [SerializeField] private GameObject InfoTextObj;
    private Image Panel;
    [SerializeField] private TMP_Text Text;
    protected new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
    protected new void OnDestroy()
    {
        base.OnDestroy();
    }
    private void Start()
    {
        Panel = InfoTextObj.GetComponentInChildren<Image>();
    }
    public void ShowInfoText(string text, float size)
    {
        Text.text = text;
        InfoTextObj.SetActive(true);
        StartCoroutine(Fade());
    }
    private IEnumerator Fade()
    {
        float coolDown = 5;

        while(coolDown > 0)
        {
            float a = 1 - (5 - coolDown) / 5;
            Panel.color = new Color(Panel.color.r,Panel.color.g,Panel.color.b,a);
            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b,a);
            coolDown -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        InfoTextObj.SetActive(false);
    }
}
