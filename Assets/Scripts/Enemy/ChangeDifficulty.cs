using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDifficulty : MonoBehaviour
{
    // Update is called once per frame
    public void ChangeDif(int dif)
    {
        DifficultyManager.Instance.ChangeDifficulty(dif);
    }
}
