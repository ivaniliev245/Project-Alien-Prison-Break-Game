using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Cinemachine;


public class freeze : MonoBehaviour
{
    public MonoBehaviour componentToFreeze; // Drag your component (e.g., a script) here in the Inspector

    public void Freeze()
    {
        componentToFreeze.enabled=false;
    }

        public void Unfreeze()
    {
        componentToFreeze.enabled=true;
    }
}
