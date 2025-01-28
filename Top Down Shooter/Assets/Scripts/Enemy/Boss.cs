using UnityEngine;

/// <summary>
/// Base script for handling boss attacks
/// </summary>
public class Boss : MonoBehaviour
{
    Player player;

    [Header("AOE Attack")]
    [SerializeField] GameObject hurtBox;
    [SerializeField] float maxDistanceBeforeAttack;

    Animator animator;


    /// <summary>
    /// Storing the reference of animator
    /// hurtBox is Area of Effect sphere for Boss
    /// </summary>
    private void Start()
    {
        player = WorldObjectPoolManager.instance.player;
        animator = GetComponent<Animator>();

        hurtBox.gameObject.SetActive(false);
    }

    /// <summary>
    /// If player gets close boss does Area of Effect attack
    /// </summary>
    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(maxDistanceBeforeAttack >= distance)
        {
            animator.SetTrigger("attack");
        }
    }


    /// <summary>
    /// Disable Area of Effect sphere / hurtBox
    /// </summary>
    public void DisableHurtBox()
    {
        hurtBox.gameObject.SetActive(false);
    }

    /// <summary>
    /// Enable Area of Effect Sphere / hurtBox
    /// </summary>
    public void EnableHurtBox()
    {
        hurtBox.gameObject.SetActive(true);
    }

}
