using TMPro;
using UnityEngine;

public class AlwaysActiveInputField : MonoBehaviour
{
    public static AlwaysActiveInputField instance { get; private set; }
    public delegate void InputHandler(string input); // Delegate for handling input
    private TMP_InputField inputField;
    private bool isAllowInput;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            this.isAllowInput = true;
        }
    }
    void Start()
    {
        this.inputField = GetComponent<TMP_InputField>();
        ReactivateInputField();
    }

    void Update()
    {
        if (!this.isAllowInput)
            return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ReactivateInputField();  // Activate the input field after the user submit the answer in AutoCompleteInput.cs
        }
        if (Input.GetMouseButtonDown(0))  // Also activate the input field even when the player click outside the input field.
        {
            ReactivateInputField();
        }
        inputField.selectionAnchorPosition = inputField.selectionFocusPosition;  // This prevents user from selecting (inverse-whiting) words
    }

    private void ReactivateInputField()
    {
        inputField.ActivateInputField();
        inputField.Select();
    }
    public void SetAllowInput(bool set)
    {
        this.isAllowInput = set;
        inputField.enabled = this.isAllowInput;
        if (this.isAllowInput)
            ReactivateInputField();
    }
}
