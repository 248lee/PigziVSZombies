using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JohnUtils;

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
    public Color colorOfSelectedButtonText;
    public List<TabPage> tabPages;
    public event EventHandlerWithString onTabSwitched;
    public string currentTabName { get; private set; }
    private List<Color> original_button_colors_background = new List<Color>(), original_button_colors_text = new List<Color>();
    [SerializeField] private bool changeButtonBackgroundColor = true;
    // Start is called before the first frame update
    void Start()
    {
        foreach (TabPage tabPage in this.tabPages)
        {
            tabPage.tabButton.onClick.AddListener(() => this.switchToTab(tabPage.tabName));
            this.original_button_colors_background.Add(tabPage.tabButton.GetComponent<Image>().color);
            this.original_button_colors_text.Add(tabPage.tabButton.GetComponentInChildren<TextMeshProUGUI>().color);
        }
        switchToDefaultTab();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void switchToTab(string targetTabName)
    {
        for (int i = 0; i < this.tabPages.Count; i++)
        {
            TabPage tabPage = this.tabPages[i];
            if (tabPage.tabName == targetTabName)
            {
                tabPage.objectToShow.SetActive(true);
                GetComponent<Image>().color = tabPage.backgroundColor;
                if (this.changeButtonBackgroundColor)
                    tabPage.tabButton.GetComponent<Image>().color = tabPage.backgroundColor;
                tabPage.tabButton.GetComponentInChildren<TextMeshProUGUI>().color = this.colorOfSelectedButtonText;
                currentTabName = tabPage.tabName;

                // Trigger the event of switching tab
                if (this.onTabSwitched != null)
                    this.onTabSwitched.Invoke(targetTabName);
            }
            else
            {
                tabPage.objectToShow.SetActive(false);
                if (this.changeButtonBackgroundColor)
                    tabPage.tabButton.GetComponent<Image>().color = this.original_button_colors_background[i];
                tabPage.tabButton.GetComponentInChildren<TextMeshProUGUI>().color = this.original_button_colors_text[i];
            }
        }
    }
    public void switchToDefaultTab()
    {
        if (tabPages.Count == 0)
        {
            Debug.LogWarning("NO tabs in the TabSystem!");
            return;
        }
        switchToTab(this.tabPages[0].tabName);
    }
}
