using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public VolumeBar volumeBar;
    void Start()
    {
        volumeBar.Load();
    }
}
