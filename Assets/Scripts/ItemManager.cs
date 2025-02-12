using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Heal(Unit target, int value)
    {
        target.Heal(value);
    }

    public void SpeedUp(Unit target, int value, int duration)
    {
        EffectManager.instance.AddEffect(EffectManager.EffectType.AlterSpeed, duration, target, value, EffectManager.ActivationType.Start);
    }

    public void DamageUp(Unit target, int value, int duration)
    {
        EffectManager.instance.AddEffect(EffectManager.EffectType.AlterDamage, duration, target, value, EffectManager.ActivationType.Start);
    }

    public void DefenseUp(Unit target, int value, int duration)
    {
        EffectManager.instance.AddEffect(EffectManager.EffectType.AlterDefense, duration, target, value, EffectManager.ActivationType.Start);
    }
}
