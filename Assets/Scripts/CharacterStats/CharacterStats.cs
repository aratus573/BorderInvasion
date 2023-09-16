using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    //public CharacterData templateData;
    protected CharacterData characterData;
    /*
    protected virtual void Awake()
    {
        // this is to make stat upgrades only work on one character, and destroyed after game end
        // templateData = basic Stats
        // characterData = basic Stats + Upgrades
        // don't assign templateData if not using this system
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }
    */
    public virtual void TakeDamage(float damage)
    {
        characterData.CurrentHealth = Mathf.Max(0, characterData.CurrentHealth - damage);
        //update healthbar here in override method
    }

    //example: use CharacterStats.CurrentHealth to access health
    // so there's no need to use CharacterStats.characterData.CurrentHealth
    #region Read from characterData
    public float MaxHealth
    {
        get { if (characterData != null) return characterData.MaxHealth; else return 0; }
        set { characterData.MaxHealth = value; }
    }
    public float CurrentHealth
    {
        get { if (characterData != null) return characterData.CurrentHealth; else return 0; }
        set { characterData.CurrentHealth = value; }
    }
    public float MoveSpeed
    {
        get { if (characterData != null) return characterData.MoveSpeed; else return 0; }
        set { characterData.MoveSpeed = value; }
    }
    #endregion

}
