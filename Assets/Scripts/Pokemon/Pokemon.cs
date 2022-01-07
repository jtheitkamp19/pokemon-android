using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Pokemon {
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public PokemonBase Base {
        get { return _base; }
        set { _base = value; }
    }

    public int Level {
        get { return level; }
        set { level = value; }
    }

    public int HP {
        get; set;
    }

    public List<Move> Moves {
        get; set;
    }

    public Move CurrentMove {
        get; set;
    }

    public Dictionary<Stat, int> Stats {
        get; private set;
    }

    public Dictionary<Stat, int> StatBoosts {
        get; private set;
    }

    public Condition Status {
        get; private set;
    }

    public int StatusTime {
        get; set;
    }

    public Condition VolatileStatus {
        get; set;
    }

    public int VolatileStatusTime {
        get; set;
    }

    public Queue<string> StatusChanges {
        get; private set;
    } = new Queue<string>();

    public bool HPChanged {
        get; set;
    }

    public event System.Action OnStatusChanged;

    public void Init() {
        // Generate Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves) {
            if (move.Level <= Level) {
                Moves.Add(new Move(move.Base));

                if (Moves.Count >= 4) {
                    break;
                }
            }
        }

        CalculateStats();
        HP = MaxHp;

        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    void ResetStatBoost() {
        StatBoosts = new Dictionary<Stat, int>() {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAtk, 0 },
            { Stat.SpDef, 0 },
            { Stat.Speed, 0 },
            { Stat.Accuracy, 0 },
            { Stat.Evasion, 0 }
        };
    }

    void CalculateStats() {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, CalculateStat(Base.Attack));
        Stats.Add(Stat.Defense, CalculateStat(Base.Defense));
        Stats.Add(Stat.SpAtk, CalculateStat(Base.SpAtk));
        Stats.Add(Stat.SpDef, CalculateStat(Base.SpDef));
        Stats.Add(Stat.Speed, CalculateStat(Base.Speed));

        MaxHp = Mathf.FloorToInt((Base.Hp * Level) / 100f) + 10 + level;
    }

    private int CalculateStat(int stat) {
        return Mathf.FloorToInt((stat * Level) / 100f) + 5;
    }

    int GetStat(Stat stat) {
        int statValue = Stats[stat];

        // Apply Stat Boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0) {
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        } else {
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);
        }

        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> boosts) {
        foreach (var statBoost in boosts) {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0) {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            } else {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
            }

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public int MaxHp {
        get; private set;
    }

    public int Attack {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAtk {
        get { return GetStat(Stat.SpAtk); }
    }

    public int SpDef {
        get { return GetStat(Stat.SpDef); }
    }
    
    public int Speed {
        get { return GetStat(Stat.Speed); }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker) {
        DamageDetails details = new DamageDetails();
        details.Fainted = false;
        float critical = 1f;

        if (Random.value * 100f <= 6.25) {
            critical = 2f;
        }

        details.Critical = critical;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);
        details.TypeEffectiveness = type;

        float atkPower = (move.Base.Category == MoveCategory.Special) ? attacker.SpAtk : attacker.Attack;
        float defPower = (move.Base.Category == MoveCategory.Special) ? SpDef : Defense;

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * (atkPower / defPower) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return details;
    }

    public void SetStatus(ConditionID conditionId) {
        if (Status != null) {
            return;
        }

        Status = ConditionDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus() {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId) {
        if (VolatileStatus != null) {
            return;
        }

        VolatileStatus = ConditionDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus() {
        VolatileStatus = null;
    }

    public void UpdateHP(int damage) {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HPChanged = true;
    }

    public Move GetRandomMove() {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public void OnBattleOver() {
        ResetStatBoost();
        VolatileStatus = null;
    }

    public void OnAfterTurn() {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public bool OnBeforeMove() {
        bool canPerformMove = true;

        if (Status?.OnBeforeMove != null) {
            canPerformMove = Status.OnBeforeMove(this);
        }

        if (canPerformMove && VolatileStatus?.OnBeforeMove != null) {
            canPerformMove = VolatileStatus.OnBeforeMove(this);
        }

        return canPerformMove;
    }
}

public class DamageDetails {
    public bool Fainted {
        get; set;
    }

    public float Critical {
        get; set;
    }

    public float TypeEffectiveness {
        get; set;
    }
}
