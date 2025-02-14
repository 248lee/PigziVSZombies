using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatusEntry
{
    string message;
    bool isDone;

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
    public TextMeshProUGUI textBar;
    private List<StatusEntry> statusStack;
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
    public StatusEntry AddStatusEntry(string message)
    {
        StatusEntry tmp = new StatusEntry(message);
        this.statusStack.Add(tmp);
        return tmp;
    }
}
