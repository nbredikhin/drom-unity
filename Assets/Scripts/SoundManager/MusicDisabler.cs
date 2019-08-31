using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDisabler : MonoBehaviour
{
    void Start()
    {
        MusicPlayer.isMusicDisabled = true;
    }
}
