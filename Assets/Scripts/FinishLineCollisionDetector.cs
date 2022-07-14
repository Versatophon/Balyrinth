using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineCollisionDetector : MonoBehaviour
{

    public LabyrinthGenerator mLabyrinthGenerator = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider pCollider)
    {
        FinishLine lFinishLine = pCollider.gameObject.GetComponent<FinishLine>();
        if( lFinishLine != null)
        {
            Debug.Log("Congratulations - You reached the finish line !");

            //Reload new labyrinth
            mLabyrinthGenerator?.InitiateGeneration();
        }
    }
}
