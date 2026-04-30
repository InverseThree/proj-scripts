using System.Collections.Generic;
using UnityEngine;

public class NPCAppearanceAssigner : MonoBehaviour
{
    public Material[] headMaterials; // 5
    public Material[] bodyMaterials; // 5

    public void AssignUniqueAppearances(List<NPCAppearance> npcs, System.Random rng)
    {
        List<(int head, int body)> combos = new List<(int, int)>();

        for (int h = 0; h < headMaterials.Length; h++)
        {
            for (int b = 0; b < bodyMaterials.Length; b++)
            {
                combos.Add((h, b));
            }
        }

        // shuffle
        for (int i = 0; i < combos.Count; i++)
        {
            int j = rng.Next(i, combos.Count);
            (combos[i], combos[j]) = (combos[j], combos[i]);
        }

        for (int i = 0; i < npcs.Count; i++)
        {
            var combo = combos[i];
            npcs[i].Apply(headMaterials[combo.head], bodyMaterials[combo.body]);
        }
    }
}
