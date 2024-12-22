using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages an input field with an auto-complete functionality.
/// Displays a list of word suggestions as the user types and allows selection via mouse or keyboard.
/// </summary>
public class AutoCompleteInput : MonoBehaviour
{
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
    public List<string> wordBank;

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
    /// Initializes the component and sets up event listeners.
    /// </summary>
    void Start()
    {
        suggestionPanel.SetActive(false);
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
            ClearSuggestions();
            return;
        }

        if (isJustAutoFilled)  // If this observation of input-changed is based on the selection of the suggestions, then do not update suggestion list
        {
            isJustAutoFilled = false;
            return;
        }

        // Filter word bank to find matches starting with the input text.
        var suggestions = wordBank
            .Where(word => word.StartsWith(input, System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        UpdateSuggestions(input, suggestions);

        // Select the first item that shows the players original input
        selectedIndex = 0;
        HighlightSuggestion();
    }

    /// <summary>
    /// Updates the suggestion panel with the provided list of suggestions.
    /// </summary>
    /// <param name="originalString">The original input of the player.</param>
    /// <param name="suggestions">List of words to display as suggestions.</param>
    void UpdateSuggestions(string originalString, List<string> suggestions)
    {
        ClearSuggestions();

        suggestionPanel.SetActive(true);

        // Instantiate the item indicating the original word
        var originalWordItem = Instantiate(suggestionPrefab, suggestionPanel.transform);
        // Set the text of the suggestion item.
        originalWordItem.GetComponentInChildren<TextMeshProUGUI>().text = originalString;
        // Add a click listener to apply the suggestion.
        originalWordItem.GetComponent<Button>().onClick.AddListener(() => OnSuggestionClicked(originalString));
        this.activeSuggestions.Add(originalWordItem);

        foreach (var suggestion in suggestions)
        {
            // Instantiate a suggestion item.
            var suggestionItem = Instantiate(suggestionPrefab, suggestionPanel.transform);

            // Set the text of the suggestion item.
            suggestionItem.GetComponentInChildren<TextMeshProUGUI>().text = suggestion;

            // Add a click listener to apply the suggestion.
            suggestionItem.GetComponent<Button>().onClick.AddListener(() => OnSuggestionClicked(suggestion));

            this.activeSuggestions.Add(suggestionItem);
        }
    }

    /// <summary>
    /// Handles the event when a suggestion is clicked.
    /// Sets the input field text to the selected suggestion.
    /// </summary>
    /// <param name="suggestion">The selected suggestion text.</param>
    void OnSuggestionClicked(string suggestion)
    {
        FillSuggestion(suggestion);
        ClearSuggestions();
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
        suggestionPanel.SetActive(false);
    }

    /// <summary>
    /// Updates keyboard navigation for the suggestion list.
    /// Allows users to navigate suggestions using arrow keys and select with Enter.
    /// </summary>
    void Update()
    {
        if (activeSuggestions.Count == 0) return;

        // Navigate down through suggestions.
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % activeSuggestions.Count;
            HighlightSuggestion();
        }
        // Navigate up through suggestions.
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + activeSuggestions.Count) % activeSuggestions.Count;
            HighlightSuggestion();
        }
        // Select the highlighted suggestion.
        else if (Input.GetKeyDown(KeyCode.Return) && selectedIndex >= 0)
        {
            OnSuggestionClicked(activeSuggestions[selectedIndex].GetComponentInChildren<TextMeshProUGUI>().text);
        }
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
                FillSuggestion(text.text);
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
}
