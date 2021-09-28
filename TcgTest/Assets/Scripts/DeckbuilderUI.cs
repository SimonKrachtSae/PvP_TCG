using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject LobbyUI;
    [SerializeField] private GameObject DeckUI;

	public void BackToLobbyUI()
	{
		LobbyUI.SetActive(true);
		this.gameObject.SetActive(false);
	}

	public void ShowDetailedCardView()
	{

	}

	private void OnEnable()
	{

	}
}
