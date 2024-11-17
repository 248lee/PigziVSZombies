using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public enum Stage
    {
        Level1
    }
    public Stage stage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadScene()  // This is called by the button object LevelSelector
    {
        SceneManager.LoadScene(this.stage.ToString());
    }
}
