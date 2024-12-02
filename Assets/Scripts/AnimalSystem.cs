using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSystem : MonoBehaviour
{
    [SerializeField]
    private List<AnimalHome> homesOnScene;
    [SerializeField]
    private List<AnimalController> animalControllersOnScene;
    // Start is called before the first frame update
    void Start()
    {
        if (homesOnScene.Count != animalControllersOnScene.Count)
        {
            Debug.LogError("There should be the same counts of animals and homes!!");
            return;
        }
        for (int i = 0; i < homesOnScene.Count; i++)  // Assign the id as the index
        {
            homesOnScene[i].index = i;
            animalControllersOnScene[i].index = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int countsOfAnimalControllersOnScene
    {
        get => this.animalControllersOnScene.Count;
    }
    /// <summary>
    /// Return the position of the home number "home_index"
    /// </summary>
    /// <param name="home_index">The number of home you want to access its position.</param>
    public Vector3 GetMiddlePosition(int home_index)
    {
        return this.homesOnScene[home_index].transform.position;
    }
    /// <summary>
    /// Return whether the home number "home_index" is able to protect the animals.
    /// </summary>
    /// <param name="home_index">The number of home you want to check whether it's available.</param>
    public bool IsHomeAvailable(int home_index)
    {
        return this.homesOnScene[home_index].isTreeAlive;
    }
    /// <summary>
    /// Return whether the home number "home_index" still has space to accommodate new animal.
    /// </summary>
    /// <param name="home_index">The number of home you want to check whether it's escapable.</param>
    public bool IsHomeEscapable(int home_index)
    {
        return this.homesOnScene[home_index].isTreeAlive && !this.homesOnScene[home_index].leftOccupied && !this.homesOnScene[home_index].rightOccupied;
    }
    /// <summary>
    /// Return the <u>LEFT position</u> of the home number "home_index"
    /// </summary>
    /// <param name="home_index">The number of home you want to access its left position.</param>
    public Vector3 GetLeftPosition(int home_index)
    {
        return this.homesOnScene[home_index].leftPosition;
    }
    /// <summary>
    /// Return the <u>RIGHT position</u> of the home number "home_index"
    /// </summary>
    /// <param name="home_index">The number of home you want to access its right position.</param>
    public Vector3 GetRightPosition(int home_index)
    {
        return this.homesOnScene[home_index].rightPosition;
    }
    /// <summary>
    /// Occupy the left <u>Position</u> of the home number "home_index."
    /// </summary>
    /// <param name="home_index">The number of home you want to occupy its left.</param>
    /// <param name="is_occupying">Set the value you wish.</param>
    public void SetOccupyLeftPosition(int home_index, bool is_occupying)
    {
        this.homesOnScene[home_index].leftOccupied = is_occupying;
    }
    /// <summary>
    /// Occupy the right <u>Position</u> of the home number "home_index."
    /// </summary>
    /// <param name="home_index">The number of home you want to occupy its right.</param>
    /// <param name="is_occupying">Set the value you wish.</param>
    public void SetOccupyRightPosition(int home_index, bool is_occupying)
    {
        this.homesOnScene[home_index].rightOccupied = is_occupying;
    }
    /// <summary>
    /// Check whether the <u>LEFT</u> of the home number "home_index is occupied."
    /// </summary>
    /// <param name="home_index">The number of home you want to check.</param>
    public bool GetLeftPositionOccupied(int home_index)
    {
        return this.homesOnScene[home_index].leftOccupied;
    }
    /// <summary>
    /// Check whether the <u>RIGHT</u> of the home number "home_index is occupied."
    /// </summary>
    /// <param name="home_index">The number of home you want to check.</param>
    public bool GetRightPositionOccupied(int home_index)
    {
        return this.homesOnScene[home_index].rightOccupied;
    }
}
