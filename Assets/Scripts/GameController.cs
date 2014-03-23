using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class GameController : MonoBehaviour {
	
	public int gridWidth = 40;
	public int gridHeight = 30;
	public float gridSize = 30;
	
	public Transform solid, brick, ladder, player, pickup, rope, enemy;
	
	public int pickups = 0;

	private CameraController cameraController;
	
	void Start () {
		cameraController = Camera.main.GetComponent<CameraController>();

		try {
			using (StreamReader sr = new StreamReader(Application.dataPath + "/Levels/" + "01.txt")) {
				string line;
				int gridY = gridHeight-1;
				
				while ((line = sr.ReadLine()) != null) {
					// if line.Lenght != gridWidth-1 and other conditions
					char[] chars = new char[gridWidth];
					using (StringReader sgr = new StringReader(line)) {
						sgr.Read(chars, 0, gridWidth);
						int gridX = 0;
						foreach (char c in chars) {
							CreateInstance(c, gridX, gridY);
							gridX++;
						}
					}
					gridY--;
				}
			}
		} catch (Exception e) {
			Debug.LogException(e);
		}
	}
	
	void CreateInstance(char c, float x, float y) {
		x *= gridSize;
		y *= gridSize;
		switch (char.ToLower(c)) {
		case 'x':
			Instantiate(solid, new Vector3(x, y, 0), Quaternion.identity);
			break;
		case '#':
			Instantiate(brick, new Vector3(x, y, 0), Quaternion.identity);
			break;
		case 'h':
			Instantiate(ladder, new Vector3(x, y, 0), Quaternion.identity);
			break;
		case '-':
			Instantiate(rope, new Vector3(x, y, 0), Quaternion.identity);
			break;
		case 'o':
			Instantiate(pickup, new Vector3(x, y, 0), Quaternion.identity);
			pickups++;
			break;
		case 'p':
			Transform playerObject = (Transform) Instantiate(player, new Vector3(x, y, -1), Quaternion.identity);
			cameraController.player = playerObject;

			break;
		case 'e':
			Instantiate(enemy, new Vector3(x, y, -1), Quaternion.identity);
			break;
		}	

		gameObject.SendMessage("SetPickups", pickups);
	}
	
	void PickedUp() {
		pickups--;
		gameObject.SendMessage("SetPickups", pickups);
	}
}