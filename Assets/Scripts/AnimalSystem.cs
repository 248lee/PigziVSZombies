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
    /// Return the position of the home number "index"
    /// </summary>
    /// <param name="index">NOTICE: The number of home you want to access its position.</param>
    public Vector3 GetHomePosition(int index)
    {
        return this.homesOnScene[index].transform.position;
    }
    /// <summary>
    /// Return whether the home number "index" is able to protect the animals.
    /// </summary>
    /// <param name="index">NOTICE: The number of home you want to check whether it's available.</param>
    public bool IsHomeAvailable(int index)
    {
        return this.homesOnScene[index].isTreeAlive;
    }
    /// <summary>
    /// Return whether the home number "index" still has space to accommodate new animal.
    /// </summary>
    /// <param name="index">NOTICE: The number of home you want to check whether it's escapable.</param>
    public bool IsHomeEscapable(int index)
    {
        return this.homesOnScene[index].isTreeAlive && !this.homesOnScene[index].leftOccupied && !this.homesOnScene[index].rightOccupied;
    }
    /// <summary>
    /// Return the <u>LEFT position</u> of the home number "index"
    /// </summary>
    /// <param name="index">NOTICE: The number of home you want to access its left position.</param>
    public Vector3 GetHomePositionLeft(int index)
    {
        return this.homesOnScene[index].leftPosition;
    }
    /// <summary>
    /// Return the <u>RIGHT position</u> of the home number "index"
    /// </summary>
    /// <param name="index">NOTICE: The number of home you want to access its right position.</param>
    public Vector3 GetHomePositionRight(int index)
    {
        return this.homesOnScene[index].rightPosition;
    }
    /// <summary>
    /// Occupy the <u>LEFT</u> of the home number "index."
    /// </summary>
    /// <param name="index">NOTICE: The number of home you want to occupy its left.</param>
    public void SetOccupyHomeLeft(int index, bool is_occupying)
    {
        this.homesOnScene[index].leftOccupied = is_occupying;
    }
    /// <summary>
    /// Occupy the <u>RIGHT</u> of the home number "index."
    /// </summary>
    /// <param name="index">NOTICE: The number of home you want to occupy its right.</param>
    public void SetOccupyHomeRight(int index, bool is_occupying)
    {
        this.homesOnScene[index].rightOccupied = is_occupying;
    }
}
