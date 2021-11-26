using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // TODO : Change the name of the file to better suit its purpose.
    // Make it a parent class that can be derive by many class
    
    void Start()
    {
        if (FindObjectsOfType<DontDestroyOnLoad>().Length == 2)
            Destroy(this.gameObject);
        else
            DontDestroyOnLoad(this.gameObject);
    }

}
