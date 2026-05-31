using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;

    private HeroEntity heroEntity;

    private BattleUnit battleUnit;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        animator =
            GetComponent<Animator>();

        heroEntity =
            GetComponentInParent<HeroEntity>();

        battleUnit =
            GetComponentInParent<BattleUnit>();

        if (animator == null)
        {
            Debug.LogError(
                "Missing Animator!");
        }
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (animator == null)
            return;

        bool moving = false;

        // =====================
        // VILLAGE
        // =====================

        if (heroEntity != null)
        {
            moving =
                heroEntity.IsMoving;
        }

        // =====================
        // BATTLE
        // =====================

        else if (battleUnit != null)
        {
            moving =
                battleUnit.IsMoving;
        }

        animator.SetBool(
            "isMoving",
            moving);
    }

    // =========================
    // PLAY ANIMATION
    // =========================

    public void PlayAttack()
    {
        if (animator == null)
            return;

        animator.SetTrigger("attack");
    }

    public void PlaySkill()
    {
        if (animator == null)
            return;

        animator.SetTrigger("skill");
    }

    public void PlayHurt()
    {
        if (animator == null)
            return;

        animator.SetTrigger("hurt");
    }

    public void PlayDie()
    {
        if (animator == null)
            return;

        animator.SetTrigger("die");
    }

    // =========================
    // ANIMATION EVENTS
    // =========================

    // đặt event trong animation attack
    public void OnAttackHit()
    {
        if (battleUnit != null)
        {
            battleUnit.DealAttackDamage();
        }
    }

    // đặt event trong animation skill
    public void OnSkillCast()
    {
        if (battleUnit != null)
        {
            battleUnit.DealSkillEffect();
        }
    }

    // đặt event cuối animation die
    public void OnDieFinished()
    {
        if (battleUnit != null)
        {
            battleUnit.DestroyUnit();
        }
    }
}