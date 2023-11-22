using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrass : MonoBehaviour
{
    
    public GameObject grassPrefab;
    public int grassSize= 20;
    public GameObject character;
    public Material mat;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
            for (int z=-grassSize; z <= grassSize; z++)
                for (int x = -grassSize; x <= grassSize; x++)
            {
                Vector3 position = new Vector3(x / 4.0f + 
                                Random.Range (-0.25f, 0.25f), 0, z/4.0f + Random.Range (-0.25f, 0.25f));
              GameObject grass = Instantiate (grassPrefab, position, Quaternion.identity);
              grass.transform.localScale = new Vector3(1, Random.Range(0.1f, 0.2f), 1);

            }


    }

    // Update is called once per frame
    void Update()
    {
       mat.SetVector("_TramplePosition", character.transform.position);
    }
}
