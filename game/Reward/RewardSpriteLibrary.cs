using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardSpriteLibrary : MonoBehaviour
{
    public static RewardSpriteLibrary Instance;

    [Serializable]
    public class ItemSpriteEntry
    {
        public ItemType item;
        public Sprite sprite;
    }

    [Serializable]
    public class RelicSpriteEntry
    {
        public RelicType relic;
        public Sprite sprite;
    }

    public List<ItemSpriteEntry> itemSprites = new List<ItemSpriteEntry>();
    public List<RelicSpriteEntry> relicSprites = new List<RelicSpriteEntry>();

    private void Awake()
    {
        Instance = this;
    }

    public Sprite GetItemSprite(ItemType item)
    {
        for (int i = 0; i < itemSprites.Count; i++)
            if (itemSprites[i].item == item)
                return itemSprites[i].sprite;
        return null;
    }

    public Sprite GetRelicSprite(RelicType relic)
    {
        for (int i = 0; i < relicSprites.Count; i++)
            if (relicSprites[i].relic == relic)
                return relicSprites[i].sprite;
        return null;
    }
}
