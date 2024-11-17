using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UniversalLevelManager : MonoBehaviour {

    public void GoBackToLevelSelection() {
        SceneManager.LoadScene("LevelSelection");
    }
}