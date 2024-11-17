using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
 
public class LevelManager : MonoBehaviour {
 
	public void GoBackToLevelSelection() {
        SceneManager.LoadScene("LevelSelection");
    }
}