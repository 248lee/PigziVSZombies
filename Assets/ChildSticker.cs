using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EasyAnimation
{
    None,
    MovingDown
}
public class ChildSticker : MonoBehaviour
{
    public EasyAnimation easy_animation = EasyAnimation.None;

    Vector3 sticked_pos = Vector3.zero;
    bool sticking = false;
    float y_offset = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (this.sticking)
        {
            transform.position = this.sticked_pos + new Vector3(0f, this.y_offset, 0f);
            if (this.easy_animation == EasyAnimation.MovingDown)
                this.y_offset -= 0.04f * Time.deltaTime;
        }
    }
    public void SetStickPosition()
    {
        this.sticked_pos = transform.position;
        this.sticking = true;
        this.y_offset = 0f;
    }
    public void UnstickPosition()
    {
        this.sticking = false;
        this.y_offset = 0f;
    }
}
