using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    Animator animator;
    EnemyMovement enemyMovement;
    [SerializeField] GameObject droneExplosion;
    [SerializeField] bool hasAnimation;

    MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponent<Animator>();
        enemyMovement=GetComponent<EnemyMovement>();
        meshRenderer=GetComponent<MeshRenderer>();
    }
    private void Update() {
        PlayMovingAnimation();
    }
    private void PlayMovingAnimation(){
        if(!hasAnimation){return;}
        if(enemyMovement.IsMoving){
            animator.SetBool("IsStanding",false);
        }
        else{
            animator.SetBool("IsStanding",true);
        }
    }
    public void PlayHurtAnimation(){
        if(!hasAnimation){return;}
        animator.SetTrigger("Hurt");
    }
    public void PlayDeathAnimation(){
        if(!hasAnimation){
            //flying enemy
        GameObject explosion = GameObject.Instantiate(droneExplosion,transform.position,Quaternion.identity);
        meshRenderer.enabled=false;
        Destroy(explosion,3f);
        return;}
        if(!hasAnimation){return;}
        animator.SetBool("IsDead",true);
    }
    public void PlayCheerAnimation(){
        if(!hasAnimation){return;}
        animator.SetBool("Cheer", true);
    }
    public void PlayMeleeAnimation(){
        if(!hasAnimation){return;}
        animator.SetTrigger("Attack");
    }
}
