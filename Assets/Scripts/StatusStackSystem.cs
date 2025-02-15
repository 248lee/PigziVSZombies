using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatusEntry
{
    public string message { get; private set; }
    public bool isDone { get; private set; }

    public StatusEntry(string message)
    {
        this.message = message;
        this.isDone = false;
    }
    public void SetDone()
    {
        this.isDone = true;
    }
}
public class StatusStackSystem : MonoBehaviour
{
    public static StatusStackSystem instance { get; private set; }
    public TextMeshProUGUI barText;
    private Stack<StatusEntry> statusStack;
    private Stack<StatusEntry> level2_statusStack;
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
    private void Start()
    {
        this.statusStack = new Stack<StatusEntry>();
        this.level2_statusStack = new Stack<StatusEntry>();
    }
    private void Update()
    {
        // Deal with level2 status
        while (this.level2_statusStack.Count > 0 && this.level2_statusStack.Peek().isDone)
        {
            this.level2_statusStack.Pop();
        }
        StatusEntry tail;
        if (this.level2_statusStack.TryPeek(out tail))  // if there is still something in the stack
        {
            this.barText.text = tail.message;
            return;  // Skip the normal status.
        }
        else
            this.barText.text = "";

        // Deal with normal status
        while (this.statusStack.Count > 0 && this.statusStack.Peek().isDone)
        {
            this.statusStack.Pop();
        }
        if (this.statusStack.TryPeek(out tail))  // if there is still something in the stack
            this.barText.text = tail.message;
        else
            this.barText.text = "";
    }
    public StatusEntry AddStatusEntry(string message)
    {
        StatusEntry tmp = new StatusEntry(message);
        this.statusStack.Push(tmp);
        return tmp;
    }
    public StatusEntry AddStatusEntry(string message, bool is_emergency)
    {
        StatusEntry tmp = new StatusEntry(message);
        if (is_emergency)
            this.level2_statusStack.Push(tmp);
        else
            this.statusStack.Push(tmp);
        return tmp;
    }
}
