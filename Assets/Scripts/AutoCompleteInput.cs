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

        // Filter word bank to find matches starting with the input text.
        var suggestions = wordBank
            .Where(word => word.StartsWith(input, System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        UpdateSuggestions(suggestions);
    }

    /// <summary>
    /// Updates the suggestion panel with the provided list of suggestions.
    /// </summary>
    /// <param name="suggestions">List of words to display as suggestions.</param>
    void UpdateSuggestions(List<string> suggestions)
    {
        ClearSuggestions();

        if (suggestions.Count == 0)
        {
            suggestionPanel.SetActive(false);
            return;
        }

        suggestionPanel.SetActive(true);

        foreach (var suggestion in suggestions)
        {
            // Instantiate a suggestion item.
            var suggestionItem = Instantiate(suggestionPrefab, suggestionPanel.transform);

            // Set the text of the suggestion item.
            suggestionItem.GetComponentInChildren<TextMeshProUGUI>().text = suggestion;

            // Add a click listener to apply the suggestion.
            var button = suggestionItem.GetComponent<Button>();
            button.onClick.AddListener(() => OnSuggestionClicked(suggestion));

            activeSuggestions.Add(suggestionItem);
        }
    }

    /// <summary>
    /// Handles the event when a suggestion is clicked.
    /// Sets the input field text to the selected suggestion.
    /// </summary>
    /// <param name="suggestion">The selected suggestion text.</param>
    void OnSuggestionClicked(string suggestion)
    {
        inputField.text = suggestion;
        inputField.caretPosition = suggestion.Length;
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
    /// </summary>
    void HighlightSuggestion()
    {
        for (int i = 0; i < activeSuggestions.Count; i++)
        {
            var text = activeSuggestions[i].GetComponentInChildren<TextMeshProUGUI>();
            text.color = i == selectedIndex ? Color.yellow : Color.white;
        }
    }
}
