using System;

[Serializable]
public class RunModifierState
{
    public RelicType heldRelic = RelicType.None;

    public int currentMaxHealth = 3;

    public bool mirrorPending = false;

    public bool coinRevived = false;

    public int lampTotalUsed = 0;
    public int lampIdentityTotalUsed = 0;
    public int lampPairTotalUsed = 0;
    public int lampCountTotalUsed = 0;

    public bool hasRelic => heldRelic != RelicType.None;
    public bool itemSlotDisabled => heldRelic == RelicType.Scythe;
    public bool coinActive => heldRelic == RelicType.Coin;
    public bool brushActive => heldRelic == RelicType.Brush;
    public bool talismanActive => heldRelic == RelicType.Talisman;
    public bool shardActive => heldRelic == RelicType.Shard;
    public bool scytheActive => heldRelic == RelicType.Scythe;
    public bool lampActive => heldRelic == RelicType.Lamp;

    public void ResetForNewRun(int baseMaxHealth)
    {
        heldRelic = RelicType.None;
        currentMaxHealth = baseMaxHealth;
        mirrorPending = false;
        coinRevived = false;
        lampTotalUsed = 0;
        lampIdentityTotalUsed = 0;
        lampPairTotalUsed = 0;
        lampCountTotalUsed = 0;
    }
}
