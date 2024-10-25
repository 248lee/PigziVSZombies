using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimalController : MonoBehaviour
{
    public int index; //  The no. of this animal. We identify each animal by this variable.
    public float runSpeed = 10f;
    Vector3 runDirection = Vector3.zero;
    AnimalSystem animalSystem;
    Animator stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        this.animalSystem = FindAnyObjectByType<AnimalSystem>();
        this.stateMachine = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Busy updating all the variables needed by the condition checkings
        this.stateMachine.SetBool("isLeftHomeEscapable", this.isLeftHomeEscapable);
        this.stateMachine.SetBool("isRightHomeEscapable", this.isRightHomeEscapable);
        this.stateMachine.SetBool("isSelfHomeAvailable", this.isSelfHomeAvailable);
    }
    private void FixedUpdate()
    {
        if (this.runDirection != Vector3.zero)
        {
            transform.position += this.runDirection * this.runSpeed * Time.deltaTime;
        }
        this.runDirection = Vector3.zero;
    }
    public bool isLeftHomeEscapable
    {
        get => this.index > 0 ? this.animalSystem.IsHomeEscapable(this.index - 1) : false;
    }
    public bool isRightHomeEscapable
    {
        get => this.index < this.animalSystem.countsOfAnimalControllersOnScene - 1 ? this.animalSystem.IsHomeEscapable(this.index + 1) : false;
    }
    public bool isSelfHomeAvailable
    {
        get => this.animalSystem.IsHomeAvailable(this.index);
    }
    public Vector3 leftHomeTargetPosition
    {
        get => this.animalSystem.GetHomePositionRight(this.index - 1);  // Left tree's right position
    }
    public Vector3 rightHomeTargetPosition
    {
        get => this.animalSystem.GetHomePositionLeft(this.index + 1);  // Right tree's left position
    }
    public Vector3 selfHomePosition
    {
        get => this.animalSystem.GetHomePosition(this.index);
    }
    /// <summary>
    /// Request AnimalSystem to let me occupy the <u>LEFT</u> home.
    /// </summary>
    public void OccupyLeftHome()
    {
        this.animalSystem.SetOccupyHomeLeft(this.index - 1, is_occupying: true);
    }
    /// <summary>
    /// Request AnimalSystem to let me occupy the <u>RIGHT</u> home.
    /// </summary>
    public void OccupyRightHome()
    {
        this.animalSystem.SetOccupyHomeLeft(this.index + 1, is_occupying: true);
    }
    /// <summary>
    /// Request AnimalSystem to let me release the <u>LEFT</u> home.
    /// </summary>
    public void LeaveLeftHome()
    {
        this.animalSystem.SetOccupyHomeLeft(this.index - 1, is_occupying: false);
    }
    /// <summary>
    /// Request AnimalSystem to let me release the <u>RIGHT</u> home.
    /// </summary>
    public void LeaveRightHome()
    {
        this.animalSystem.SetOccupyHomeLeft(this.index + 1, is_occupying: false);
    }
    /// <summary>
    /// Ask the animal to <u>run a small step</u> for this frame at a given direction.
    /// </summary>
    /// <param name="direction">NOTICE:Please normalize before passing this parameter!</param>
    public void SetRunThisFrame(Vector3 direction)
    {
        this.runDirection = direction;
    }
}
