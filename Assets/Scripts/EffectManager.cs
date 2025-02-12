using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    public enum EffectType
    {
        AlterSpeed,
        AlterDamage,
        AlterDefense,
        DamageOverTime
    }

    public enum ActivationType
    {
        Start,
        EachTurn,
        End
    }

    public class Effect
    {
        public int Duration { get; private set; }
        public int Value { get; }
        public EffectType Type { get; }
        public Unit Target { get; }
        public ActivationType Activation { get; }

        public Effect(EffectType type, int duration, Unit target, ActivationType activation, int value = 0)
        {
            Type = type;
            Duration = duration;
            Target = target;
            Activation = activation;
            Value = value;
        }

        public void DecrementDuration() => Duration--;
    }

    List<Effect> activeEffects;
    List<Effect> effectsToRemove;

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
        activeEffects = new List<Effect>();
        effectsToRemove = new List<Effect>();
    }

    public void AddEffect(EffectType type, int duration, Unit target, int value, ActivationType activation)
    {
        Effect effect = new Effect(type, duration, target, activation, value);
        if (effect.Activation == ActivationType.Start)
        {
            ApplyEffect(effect);
        }
        activeEffects.Add(effect);
    }

    public void CheckActiveEffects()
    {
        print("Current Active Effects: " + activeEffects.Count);

        foreach(Effect effect in activeEffects)
        {
            effect.DecrementDuration();
            if(effect.Duration == 0)
            {
                effectsToRemove.Add(effect);
            }
        }
        print("Active Effects To Cull: " + effectsToRemove.Count);


        foreach(Effect effect in effectsToRemove)
        {
            if (effect.Activation == ActivationType.End)
            {
                ApplyEffect(effect);
            }
            RemoveEffect(effect);
            activeEffects.Remove(effect);
        }

        print("Current Active Effects: " + activeEffects.Count);

        effectsToRemove.Clear();
    }

    void ActivateTurnEffects()
    {
        foreach(Effect effect in activeEffects)
        {
            if (effect.Activation == ActivationType.EachTurn)
            {
                ApplyEffect(effect);
            }
        }
    }

    private void ApplyEffect(Effect effect)
    {
        switch (effect.Type)
        {
            case EffectType.AlterDamage:
                effect.Target.damage += effect.Value;
                break;
            case EffectType.AlterDefense:
                effect.Target.defense += effect.Value;
                break;
            case EffectType.AlterSpeed:
                effect.Target.speed += effect.Value;
                break;
            case EffectType.DamageOverTime:
                effect.Target.TakeDamage(effect.Value);
                break;
        }
    }

    private void RemoveEffect(Effect effect)
    {
        switch (effect.Type)
        {
            case EffectType.AlterDamage:
                effect.Target.damage -= effect.Value;
                break;
            case EffectType.AlterDefense:
                effect.Target.defense -= effect.Value;
                break;
            case EffectType.AlterSpeed:
                effect.Target.speed -= effect.Value;
                break;
        }
    }
}
