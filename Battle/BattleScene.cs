using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class BattleScene : MonoBehaviour {

	public static BattleScene battleScene;

	private Camera cam;

	[SerializeField] private GameObject allyPositionOne;
	[SerializeField] private GameObject allyPositionTwo;
	[SerializeField] private GameObject allyPositionThree;
	
	[SerializeField] private GameObject allyPositionOneOfOne;
	
	[SerializeField] private GameObject enemyPositionOne;
	[SerializeField] private GameObject enemyPositionTwo;
	[SerializeField] private GameObject enemyPositionThree;
	[SerializeField] private GameObject enemyPositionFour;
	
	[SerializeField] private GameObject enemyPositionOneOfOne;
	[SerializeField] private GameObject enemyPositionOneOfTwo;
	[SerializeField] private GameObject enemyPositionTwoOfTwo;
	[SerializeField] private GameObject enemyPositionThreeOfThree;

//	private Queue<Vector3> allyPositions = new Queue<Vector3>();
//	private Queue<Vector3> enemyPositions = new Queue<Vector3>();

	private List<Vector3> allyPositions = new List<Vector3>();
	private List<Vector3> enemyPositions = new List<Vector3>();
	
	private Vector3 aPosOne;
	private Vector3 aPosTwo;
	private Vector3 aPostThree;
	private Vector3 ePosOne;
	private Vector3 ePosTwo;
	private Vector3 ePosThree;
	private Vector3 ePosFour;
	
	private Vector3 aPosOneOfOne;
	
	private Vector3 ePosOneOfTwo;
	private Vector3 ePosTwoOfTwo;
	
	private Vector3 ePosOneOfOne;
	private Vector3 ePosThreeOfThree;

	private void Start() {
		if (battleScene == null) {
			battleScene = this;
		} else if (battleScene != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		cam = Camera.main;
//		aPosOne = allyPositionOne.transform.position;
//		aPosTwo = allyPositionTwo.transform.position;
//		aPostThree = allyPositionThree.transform.position;
//
//		aPosOneOfOne = allyPositionOneOfOne.transform.position;
//		
//		ePosOne = enemyPositionOne.transform.position;
//		ePosTwo = enemyPositionTwo.transform.position;
//		ePosThree = enemyPositionThree.transform.position;
//		ePosFour = enemyPositionFour.transform.position;
//
//		ePosOneOfOne = enemyPositionOneOfOne.transform.position;
//		ePosOneOfTwo = enemyPositionOneOfTwo.transform.position;
//		ePosTwoOfTwo = enemyPositionTwoOfTwo.transform.position;
//		ePosThreeOfThree = enemyPositionThreeOfThree.transform.position;
	}

	// Update is called once per frame
//	private void Update() {
//		Vector3 pos = new Vector3(cam.transform.position.x, cam.transform.position.y, -10);
//		transform.position = pos;
//	}

	public void InitBattleScene() {
		Vector3 position = cam.transform.position;
		Vector3 pos = new Vector3(position.x, position.y, -10);
		transform.position = pos;
		aPosOne = allyPositionOne.transform.position;
		aPosTwo = allyPositionTwo.transform.position;
		aPostThree = allyPositionThree.transform.position;

		aPosOneOfOne = allyPositionOneOfOne.transform.position;
		
		ePosOne = enemyPositionOne.transform.position;
		ePosTwo = enemyPositionTwo.transform.position;
		ePosThree = enemyPositionThree.transform.position;
		ePosFour = enemyPositionFour.transform.position;

		ePosOneOfOne = enemyPositionOneOfOne.transform.position;
		ePosOneOfTwo = enemyPositionOneOfTwo.transform.position;
		ePosTwoOfTwo = enemyPositionTwoOfTwo.transform.position;
		ePosThreeOfThree = enemyPositionThreeOfThree.transform.position;
	}

	public void UpdateAllyPositions() {
		switch (PlayerManager.pm.AllyCount()) {
			case 1:
				allyPositionOne.transform.position = aPosOneOfOne;
				break;
			case 2:
				allyPositionOne.transform.position = aPosOne;
				allyPositionTwo.transform.position = aPostThree;
				break;
			case 3:
				allyPositionOne.transform.position = aPosOne;
				allyPositionTwo.transform.position = aPosTwo;
				allyPositionThree.transform.position = aPostThree;
				break;
			default:
				Debug.Log("Invalid party count");
				break;
		}
	}

	public void UpdateEnemyPositions(int enemyCount) {
		switch (enemyCount) {
			case 1:
//				enemyPositionOne.transform.position = ePosOneOfOne;
				enemyPositions.Add(ePosOneOfOne);
				break;
			case 2:
//				enemyPositionOne.transform.position = ePosOne;
//				enemyPositionTwo.transform.position = ePosTwo;
				enemyPositions.Add(ePosOneOfTwo);
				enemyPositions.Add(ePosTwoOfTwo);
				break;
			case 3:
//				enemyPositionOne.transform.position = ePosOne;
//				enemyPositionTwo.transform.position = ePosTwo;
//				enemyPositionThree.transform.position = ePosThreeOfThree;
				enemyPositions.Add(ePosOne);
				enemyPositions.Add(ePosTwo);
				enemyPositions.Add(ePosThreeOfThree);
				break;
			case 4:
//				enemyPositionOne.transform.position = ePosOne;
//				enemyPositionTwo.transform.position = ePosTwo;
//				enemyPositionThree.transform.position = ePosThree;
//				enemyPositionFour.transform.position = ePosFour;
				enemyPositions.Add(ePosOne);
				enemyPositions.Add(ePosTwo);
				enemyPositions.Add(ePosThree);
				enemyPositions.Add(ePosFour);
				break;
			default:
				Debug.Log("Invalid enemy count");
				break;
		}
	}

	public Vector3 GetEnemyPosition(int index) {
		return enemyPositions[index];
	}

	public void ResetEnemyPositions() {
		enemyPositions.Clear();
	}
}
