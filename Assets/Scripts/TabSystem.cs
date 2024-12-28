using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSystem : MonoBehaviour
{
    [System.Serializable]
    public class TabPage
    {
        public string tabName;
        public Color backgroundColor;
        public Button tabButton;
        public GameObject objectToShow;
    }
    public List<TabPage> tabPages;
    // Start is called before the first frame update
    void Start()
    {
        foreach (TabPage tabPage in this.tabPages)
        {
            tabPage.tabButton.onClick.AddListener(() => this.switchToTab(tabPage.tabName));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void switchToTab(string targetTabName)
    {
        foreach (TabPage tabPage in this.tabPages)
        {
            if (tabPage.tabName == targetTabName)
            {
                tabPage.objectToShow.SetActive(true);
                GetComponent<Image>().color = tabPage.backgroundColor;
            }
            else
                tabPage.objectToShow.SetActive(false);
        }
    }
    public void switchToDefaultTab()
    {
        if (tabPages.Count == 0)
        {
            Debug.LogWarning("NO tabs in the TabSystem!");
            return;
        }
        tabPages[0].objectToShow.SetActive(true);
        GetComponent<Image>().color = tabPages[0].backgroundColor;

        for (int i = 1; i < tabPages.Count; i++)  // turn off all other tabs
        {
            tabPages[i].objectToShow.SetActive(false);
        }
    }
}
