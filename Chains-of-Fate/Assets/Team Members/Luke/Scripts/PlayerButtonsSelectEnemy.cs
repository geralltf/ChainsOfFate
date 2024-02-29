using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChainsOfFate.Gerallt;

public class PlayerButtonsSelectEnemy : MonoBehaviour
{
	
	public PlayerButtons PlayerButtons;

	public GameObject EnemyParty;
	private List<EnemyNPC> EnemyNPCs = new ();
	[SerializeField] private List<GameObject> Buttons;

	private void ResetView()
	{
		foreach (GameObject button in Buttons)
		{
			button.SetActive(false);
		}
		
		for (int i=0; i<EnemyNPCs.Count; i++)
		{
			Buttons[i].SetActive(true);
		}
	}

	public void SelectEnemy(int i)
	{
		CombatGameManager combatGameManager = CombatGameManager.Instance;

		combatGameManager.attackTarget = EnemyNPCs[i];
		
		PlayerButtons.EnemySelected();
	}
	
	public void BackButton_OnClick()
	{
		this.gameObject.SetActive(false);
		PlayerButtons.view.SetActive(true);
	}

	void OnEnable()
    {
	    EnemyNPCs.Clear();
	    EnemyNPCs.AddRange(EnemyParty.GetComponentsInChildren<EnemyNPC>());
	    ResetView();
    }
}
