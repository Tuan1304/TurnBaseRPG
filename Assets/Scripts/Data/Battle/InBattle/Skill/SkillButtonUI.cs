using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonUI : MonoBehaviour
{
    public Image icon;

    public TextMeshProUGUI cooldownText;

    private BattleUnit currentUnit;

    private bool isBasicAttack;

    // =========================
    // BASIC ATTACK
    // =========================

    public void SetupBasicAttack(BattleUnit unit)
    {
        currentUnit = unit;

        isBasicAttack = true;

        gameObject.SetActive(true);

        cooldownText.gameObject.SetActive(false);
    }

    // =========================
    // SKILL
    // =========================

    public void SetupSkill(BattleUnit unit)
    {
        currentUnit = unit;

        isBasicAttack = false;

        if (unit.skillData == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        icon.sprite =
            unit.skillData.icon;

        if (unit.currentCooldown > 0)
        {
            cooldownText.gameObject
                .SetActive(true);

            cooldownText.text =
                unit.currentCooldown.ToString();
        }
        else
        {
            cooldownText.gameObject
                .SetActive(false);
        }
    }

    // =========================
    // CLICK
    // =========================

    public void OnClickButton()
    {
        if (currentUnit == null)
        {
            Debug.Log("UNIT NULL");
            return;
        }

        if (!isBasicAttack)
        {
            if (currentUnit.currentCooldown > 0)
                return;
        }

        BattleManager.Instance
            .EnterSkillMode(
                currentUnit,
                isBasicAttack);
    }
}