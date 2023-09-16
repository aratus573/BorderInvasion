using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;
    public int difficulty=0;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(Instance==null){
            Instance=this;
        }else{
            Destroy(gameObject);
        }
        
    }

    public void ChangeDifficulty(int dif){
        difficulty=dif;
    }
    

    //these formulas decides enemy health and count growth every wave.
    //these should take the game difficulty into account and produce a steeper progression in harder difficulty. (not implemented yet)
    public float GetDifficultyHealthMultiplier(int wave){
        double healthMult = System.Math.Tanh(((float)wave - 1f) / 50f) * 2 + 1;
        return (float)healthMult;
    }
    public int GetDifficultyEnemyCount(int wave){
        double count = System.Math.Tanh(((float)wave)/30f) * 100 + 1;
        return (int)count;
    }

}
