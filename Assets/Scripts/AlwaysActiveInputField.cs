using TMPro;
using UnityEngine;

public class AlwaysActiveInputField : MonoBehaviour
{
    public static AlwaysActiveInputField instance { get; private set; }
    public delegate void InputHandler(string input); // Delegate for handling input
    public event InputHandler OnInputSubmitted; // Event to call when input is submitted (event makes it only able to be called in the current class)
    private TMP_InputField inputField;

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
    void Start()
    {
        this.inputField = GetComponent<TMP_InputField>();
        ReactivateInputField();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            HandleSubmit();
        }
        if (Input.GetMouseButtonDown(0))  // Also activate the input field even when the player click outside the input field.
        {
            ReactivateInputField();
        }
        inputField.selectionAnchorPosition = inputField.selectionFocusPosition;  // This prevents user from selecting (inverse-whiting) words
    }

    void HandleSubmit()
    {
        string inputText = inputField.text;

        if (!string.IsNullOrEmpty(inputText))
        {
            // Pass the input text to your function
            OnInputSubmitted?.Invoke(inputText);
        }

        // Clear the input field and reactivate it
        inputField.text = string.Empty;
        ReactivateInputField();
    }

    private void ReactivateInputField()
    {
        inputField.ActivateInputField();
        inputField.Select();
    }
}
