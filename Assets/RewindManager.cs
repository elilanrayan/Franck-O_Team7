using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour
{
    [SerializeField] List<GameObject> rewindableGameObjects;
    [SerializeField] static int maxRewindableTime = 300;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
