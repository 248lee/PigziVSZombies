using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimalController : MonoBehaviour
{
    public int index; //  The no. of this animal. We identify each animal by this variable.
    public float runSpeed = 10f;
    public float stepSize { get => runSpeed * Time.deltaTime; }
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
    /// <summary>
    /// Check whether this animal can escape to the left.
    /// </summary>
    public bool isLeftHomeEscapable
    {
        get => this.index > 0 ? this.animalSystem.IsHomeEscapable(this.index - 1) : false;
    }
    /// <summary>
    /// Check whether this animal can escape to the right.
    /// </summary>
    public bool isRightHomeEscapable
    {
        get => this.index < this.animalSystem.countsOfAnimalControllersOnScene - 1 ? this.animalSystem.IsHomeEscapable(this.index + 1) : false;
    }
    /// <summary>
    /// Check whether this animal can ccome back to its home.
    /// </summary>
    public bool isSelfHomeAvailable
    {
        get => this.animalSystem.IsHomeAvailable(this.index);
    }
    /// <summary>
    /// The destination for the animal when escaping to the left (Left tree's right position).
    /// </summary>
    public Vector3 leftHomeTargetPosition
    {
        get => this.animalSystem.GetRightPosition(this.index - 1);
    }
    /// <summary>
    /// The destination for the animal when escaping to the right (Right tree's left position).
    /// </summary>
    public Vector3 rightHomeTargetPosition
    {
        get => this.animalSystem.GetLeftPosition(this.index + 1);
    }
    /// <summary>
    /// The position of its warm home~
    /// </summary>
    public Vector3 selfHomeMiddlePosition
    {
        get => this.animalSystem.GetMiddlePosition(this.index);
    }
    /// <summary>
    /// The left_position of its home.
    /// </summary>
    public Vector3 selfHomeLeftPosition
    {
        get => this.animalSystem.GetLeftPosition(this.index);
    }
    /// <summary>
    /// The right_position of its home.
    /// </summary>
    public Vector3 selfHomeRightPosition
    {
        get => this.animalSystem.GetRightPosition(this.index);
    }
    /// <summary>
    /// Request AnimalSystem to let me occupy the <u>LEFT</u> home.
    /// </summary>
    public void OccupyLeftHome()
    {
        this.animalSystem.SetOccupyRightPosition(this.index - 1, is_occupying: true);
    }
    /// <summary>
    /// Request AnimalSystem to let me occupy the <u>RIGHT</u> home.
    /// </summary>
    public void OccupyRightHome()
    {
        this.animalSystem.SetOccupyLeftPosition(this.index + 1, is_occupying: true);
    }
    /// <summary>
    /// Request AnimalSystem to check whether my <u>left home</u> is occupied. If true, I have to step aside.
    /// </summary>
    public bool isSelfHomeLeftOccupied
    {
        get => this.animalSystem.GetLeftPositionOccupied(this.index);
    }
    /// <summary>
    /// Request AnimalSystem to check whether my <u>right home</u> is occupied. If true, I have to step aside.
    /// </summary>
    public bool isSelfHomeRightOccupied
    {
        get => this.animalSystem.GetRightPositionOccupied(this.index);
    }
    /// <summary>
    /// Request AnimalSystem to let me release the <u>LEFT</u> home.
    /// </summary>
    public void LeaveLeftHome()
    {
        this.animalSystem.SetOccupyRightPosition(this.index - 1, is_occupying: false);
    }
    /// <summary>
    /// Request AnimalSystem to let me release the <u>RIGHT</u> home.
    /// </summary>
    public void LeaveRightHome()
    {
        this.animalSystem.SetOccupyLeftPosition(this.index + 1, is_occupying: false);
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
