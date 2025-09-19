using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game sInstance;

    public GameObject marioGameObject;

    public static Game Instance
    {
        get { return sInstance; }
    }

    public GameObject MarioGameObject
    {
        get { return marioGameObject; }
    }

    public Mario GetMario
    {
        get { return marioGameObject.GetComponent<Mario>(); }
    }

    public MarioState GetMarioState
    {
        get { return marioGameObject.GetComponent<MarioState>(); }
    }

    public MarioMovement GetMarioMovement
    {
        get { return marioGameObject.GetComponent<MarioMovement>(); }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Setup the static instance of the Game class
        if (sInstance != null && sInstance != this)
        {
            Destroy(this);
        }
        else
        {
            sInstance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
