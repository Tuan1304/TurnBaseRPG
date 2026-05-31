using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BattleUnit target;

    private int damage;

    public float speed = 10f;

    // =========================
    // SETUP
    // =========================

    public void Setup(
        BattleUnit targetUnit,
        int dmg)
    {
        target = targetUnit;

        damage = dmg;
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                target.transform.position,
                speed * Time.deltaTime);

        float distance =
            Vector3.Distance(
                transform.position,
                target.transform.position);

        if (distance < 0.1f)
        {
            HitTarget();
        }
    }

    // =========================
    // HIT
    // =========================

    private void HitTarget()
    {
        target.TakeDamage(damage);

        Destroy(gameObject);
    }
}