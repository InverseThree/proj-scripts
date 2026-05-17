public static class RewardTextLibrary
{
    public static string GetItemName(ItemType item)
    {
        switch (item)
        {
            case ItemType.Tonic: return "Tonic of God";
            case ItemType.Shield: return "Shield of Ephemeral";
            case ItemType.Lens: return "Lens of Devil";
            case ItemType.Scroll: return "Scroll of Insight";
            case ItemType.Mirror: return "Mirror of Truth";
            case ItemType.Hourglass: return "Hourglass of Infinity";
            default: return "None";
        }
    }

    public static string GetItemDescription(ItemType item)
    {
        switch (item)
        {
            case ItemType.Tonic:
                return "Restore 1 health.";
            case ItemType.Shield:
                return "The next time you are about to lose health on this floor, prevent it.";
            case ItemType.Lens:
                return "Lose 1 health to reveal a random inhabitant's identity on this floor.";
            case ItemType.Scroll:
                return "Reveal the number of either knights or knaves on this floor,  but you don't know which.";
            case ItemType.Mirror:
                return "Reveal which 2 random inhabitants have the same identity on the next floor.";
            case ItemType.Hourglass:
                return "Reset the state of this floor.  (inhabitants might have different identities and statements!)";
            default:
                return "";
        }
    }

    public static string GetRelicName(RelicType relic)
    {
        switch (relic)
        {
            case RelicType.Coin: return "Coin of Polarity";
            case RelicType.Brush: return "Brush of Chaos";
            case RelicType.Talisman: return "Talisman of Sin";
            case RelicType.Shard: return "Shard of Falsehood";
            case RelicType.Scythe: return "Scythe of Origination";
            case RelicType.Lamp: return "Lamp of Oracle";
            default: return "None";
        }
    }

    public static string GetRelicDescription(RelicType relic)
    {
        switch (relic)
        {
            case RelicType.Coin:
                return "Upon pick up,  add 1 additional inhabitant for future floors. When your health reaches 0 for the first time,  revive into full health instead.";
            case RelicType.Brush:
                return "Upon pick up,  add 5 additional floors after floor 10.  Future floors have random number of inhabitants instead of set number.";
            case RelicType.Talisman:
                return "Upon pick up,  set your max health to 1.  The first time you are about to lose health on a floor,  prevent it.";
            case RelicType.Shard:
                return "Upon pick up,  you now lose 1 health for each incorrect guess in your submission instead.  At the start of each floor,  choose 1 out of 3 random items to get.";
            case RelicType.Scythe:
                return "Upon pick up,  lose 1 item slot. Once per floor,  you may kill any number of inhabitants to create new ones in their places.  (with potentially different identities and statements!)";
            case RelicType.Lamp:
                return "Upon pick up,  hide the names of other inhabitants mentioned in a random inhabitant's statement for future floors.  You now have 3 chances in total to make 1 of the following effects come true:\n• Reveal the identity of an inhabitant of your choice on this floor  (can only be wished once).\n• Reveal whether the identity of 2 inhabitants of your choice is the same or not on this floor  (can only be wished twice).\n• Reveal the number of knights and knaves on this floor  (can be wished thrice).";
            default:
                return "";
        }
    }
}
