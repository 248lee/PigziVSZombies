using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JohnUtils;

/// <summary>
/// Manages an input field with an auto-complete functionality.
/// Displays a list of word suggestions as the user types and allows selection via mouse or keyboard.
/// </summary>
[RequireComponent(typeof(TabSystem))]
public class AutoCompleteInput : MonoBehaviour
{
    [SerializeField] GameObject ctrl_icon;  // This is the icon that shows the player that he/she can press Ctrl to select a suggestion
    public static AutoCompleteInput instance;
    /// <summary>
    /// Reference to the TMP_InputField where the user types.
    /// </summary>
    public TMP_InputField inputField;

    /// <summary>
    /// Panel used to display auto-complete suggestions.
    /// </summary>
    public GameObject suggestionPanel;

    /// <summary>
    /// Prefab used to create suggestion items dynamically.
    /// The prefab should contain a Button and TextMeshProUGUI component.
    /// </summary>
    public GameObject suggestionPrefab;

    /// <summary>
    /// List of words available for auto-completion.
    /// </summary>
    private List<string> wordBank = new List<string>();

    /// <summary>
    /// List of currently active suggestion GameObjects.
    /// </summary>
    private List<GameObject> activeSuggestions = new List<GameObject>();

    /// <summary>
    /// Index of the currently selected suggestion. Used for keyboard navigation.
    /// </summary>
    private int selectedIndex = -1;

    /// <summary>
    /// To record the state whether the current input change is caused by accepting a suggestion.
    /// </summary>
    private bool isJustAutoFilled = false;

    /// <summary>
    /// This handler is called when the player press Enter or click on a suggestion vocabulary.
    /// </summary>
    public event EventHandlerWithString inputCompleteHandler;

    private TabSystem tabSystem;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    /// <summary>
    /// Initializes the component and sets up event listeners.
    /// </summary>
    void Start()
    {
        wordBank.Sort();
        this.tabSystem = GetComponent<TabSystem>();
        this.tabSystem.switchToDefaultTab();
        inputField.onValueChanged.AddListener(OnInputChanged);
    }

    /// <summary>
    /// Called when the input field value changes. Updates suggestions based on the input.
    /// </summary>
    /// <param name="input">The current text in the input field.</param>
    void OnInputChanged(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            OnInputEmpty();
            return;
        }

        if (isJustAutoFilled)  // If this observation of input-changed is based on the selection of the suggestions, then do not update suggestion list
        {
            isJustAutoFilled = false;
            return;
        }

        this.tabSystem.switchToTab("±ÀÂË¦r");

        UpdateSuggestions();

        // Select the first item that shows the players original input
        selectedIndex = 0;
        HighlightSuggestion();
    }

    /// <summary>
    /// Updates the suggestion panel according to the given input string.
    /// </summary>
    void UpdateSuggestions()
    {
        ClearSuggestions();
        string inputText = inputField.text;

        // Filter word bank to find matches starting with the input text.
        var suggestions = wordBank
            .Where(word => word.StartsWith(inputText, System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var suggestion in suggestions)
        {
            // Instantiate a suggestion item.
            var suggestionItem = Instantiate(suggestionPrefab, suggestionPanel.transform);

            // Set the text of the suggestion item.
            suggestionItem.GetComponentInChildren<TextMeshProUGUI>().text = suggestion;

            // Add a click listener to apply the suggestion.
            suggestionItem.GetComponent<Button>().onClick.AddListener(() => OnSuggestionClickedOrCtrlPressed(suggestion));

            this.activeSuggestions.Add(suggestionItem);
        }
        if (this.ctrl_icon != null)
        {
            // Show the Ctrl icon if there are suggestions available.
            ctrl_icon.SetActive(activeSuggestions.Count > 0);
        }
        //else
        //{
        //    Debug.LogWarning("Ctrl icon is not assigned in AutoCompleteInput.");
        //}
    }

    /// <summary>
    /// Handles the event when a suggestion is clicked.
    /// Sets the input field text to the selected suggestion.
    /// </summary>
    /// <param name="suggestion">The selected suggestion text.</param>
    void OnSuggestionClickedOrCtrlPressed(string suggestion)
    {
        FillSuggestion(suggestion);
    }
    /// <summary>
    /// Submit the answer to the handler. Called when the player press Enter.
    /// </summary>
    void HandleSubmit()
    {
        string inputText = inputField.text;

        if (!string.IsNullOrEmpty(inputText))
        {
            // Pass the input text to your function.
            if (this.inputCompleteHandler != null)
                this.inputCompleteHandler.Invoke(inputText);
        }

        // Clear the input field and get back all the suggesstions.
        inputField.text = string.Empty;
    }

    /// <summary>
    /// Clears all active suggestions from the suggestion panel.
    /// </summary>
    void ClearSuggestions()
    {
        foreach (var suggestion in activeSuggestions)
        {
            Destroy(suggestion);
        }
        activeSuggestions.Clear();
    }
    void OnInputEmpty()
    {
        ClearSuggestions();
        UpdateSuggestions();
        this.tabSystem.switchToDefaultTab();
    }

    /// <summary>
    /// Updates keyboard navigation for the suggestion list.
    /// Allows users to navigate suggestions using arrow keys and select with Enter.
    /// </summary>
    void Update()
    {
        // Press Enter to submit the answer
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            HandleSubmit();
        }

        if (this.tabSystem.currentTabName != "±ÀÂË¦r")  // If the current tab is not the suggesstion panel, the player cannot select any suggesstion
            return;
        // Navigate down through suggestions.
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % activeSuggestions.Count;
        }
        // Navigate up through suggestions.
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + activeSuggestions.Count) % activeSuggestions.Count;
        }
        // Select the highlighted suggestion.
        else if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && activeSuggestions.Count >= 0 && selectedIndex >= 0)
        {
            // Debug use, delete me later!
            if (activeSuggestions.Count == 0)
            {
                return;
            }
            if (selectedIndex >= activeSuggestions.Count)
                Debug.LogError("Bug here");
            OnSuggestionClickedOrCtrlPressed(activeSuggestions[selectedIndex].GetComponentInChildren<TextMeshProUGUI>().text);
        }
        HighlightSuggestion();
    }

    /// <summary>
    /// Highlights the currently selected suggestion in the suggestion list.
    /// Changes the text color of the selected suggestion for visual feedback.
    /// Sets the input field text to the selected suggestion.
    /// </summary>
    void HighlightSuggestion()
    {
        for (int i = 0; i < activeSuggestions.Count; i++)
        {
            var text = activeSuggestions[i].GetComponentInChildren<TextMeshProUGUI>();
            text.color = Color.white;
            if (i == selectedIndex)
            {
                text.color = Color.yellow;
                //FillSuggestion(text.text);
            }
        }        
    }
    /// <summary>
    /// Apply the suggested string into the input field.
    /// </summary>
    /// <param name="suggestion"></param>
    void FillSuggestion(string suggestion)
    {
        if (inputField.text != suggestion)
            isJustAutoFilled = true;  // This tells the observer of the input-changed not to refresh the suggestions if the player is choosing a suggestion
        inputField.text = suggestion;
        inputField.caretPosition = suggestion.Length;
    }

    /// <summary>
    /// Add a word(string) to the word bank, which will be suggested to the player in his/her future input.
    /// </summary>
    /// <param name="word">The string you want to add.</param>
    /// <returns>True if the word is new. False when the word is already in the word bank.</returns>
    public bool AddWordToWordBank(string word)
    {
        wordBank.Sort(); // just in case

        // Find the index to insert the new element
        int index = wordBank.BinarySearch(word);

        if (index >= 0)  // If the element is already in the wordBank, return false
        {
            return false;
        }

        // Insert the element at the correct position
        index = ~index;  // If the element is not found, BinarySearch returns a negative number which is the complement of the index it should be
        wordBank.Insert(index, word);

        UpdateSuggestions();
        return true;
    }
}
