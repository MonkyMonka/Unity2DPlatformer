using UnityEngine;

public enum EEnemyType : byte
{
    Unknown,
    PiranhaPlant

        //TODO: Add additional EnemyType enumerators here
}

public class Enemy : MonoBehaviour
{
    protected EEnemyType enemyType = EEnemyType.Unknown;

    public EEnemyType EnemyType
    {
        get { return enemyType; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
