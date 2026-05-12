using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatementParser
{
    public StatementList statement;

    [Min(0)]
    public int variant = 0;

    public int a;
    public int b;
    public int c;
    public int d;
    public int e;

    public int GetVariantCount(int npcCount)
    {
        switch (statement)
        {
            case StatementList.IsKnight:
                return 4;

            case StatementList.IsKnave:
                return 2;

            case StatementList.OnlyKnightSayKnight:
            case StatementList.OnlyKnightSayKnave:
            case StatementList.OnlyKnaveSayKnight:
            case StatementList.OnlyKnaveSayKnave:
            case StatementList.ExactlyOneIsKnight:
            case StatementList.ExactlyOneIsKnave:
                return 1;

            case StatementList.EitherKnightOrKnight:
            case StatementList.EitherKnaveOrKnave:
            case StatementList.BothAreKnights:
            case StatementList.BothAreKnaves:
                if (npcCount == 2)
                    return 1;
                else
                    return 3;

            case StatementList.EitherKnightOrKnave:
            case StatementList.RoleSame:
                if (npcCount == 2)
                    return 2;
                else
                    return 4;

            default:
                return 1;
        }
    }

    public bool IsCompatibleWithPeasantFloor()
    {
        switch (statement)
        {
            case StatementList.OnlyKnightSayKnight:
            case StatementList.OnlyKnightSayKnave:
            case StatementList.OnlyKnaveSayKnight:
            case StatementList.OnlyKnaveSayKnave:
                return false;

            default:
                return true;
        }
    }

    public StatementDifficulty GetSuggestedDifficulty()
    {
        switch (statement)
        {
            case StatementList.IsKnight:
                return (variant == 1 || variant == 3) ? StatementDifficulty.Easy : StatementDifficulty.Medium;

            case StatementList.IsKnave:
                return StatementDifficulty.Easy;

            case StatementList.BothAreKnights:
            case StatementList.BothAreKnaves:
            case StatementList.RoleSame:
                return StatementDifficulty.Medium;

            case StatementList.EitherKnightOrKnight:
            case StatementList.EitherKnightOrKnave:
            case StatementList.EitherKnaveOrKnave:
            case StatementList.ExactlyOneIsKnight:
            case StatementList.ExactlyOneIsKnave:
            case StatementList.OnlyKnightSayKnight:
            case StatementList.OnlyKnightSayKnave:
            case StatementList.OnlyKnaveSayKnight:
            case StatementList.OnlyKnaveSayKnave:
                return StatementDifficulty.Hard;

            default:
                return StatementDifficulty.Medium;
        }
    }

    public bool Evaluate(Role[] roles, int selfIndex)
    {
        bool IsKnight(int i) => roles[i] == Role.Knight;
        bool IsKnave(int i) => roles[i] == Role.Knave;
        bool Xor(bool left, bool right) => left ^ right;
        bool SameRole(int x, int y) => roles[x] == roles[y];

        switch (statement)
        {
            case StatementList.IsKnight:
                switch (variant)
                {
                    case 0:
                        // "I am a knight or A is a knight."
                        return IsKnight(selfIndex) || IsKnight(a);

                    case 1:
                        // "A is a knight."
                        return IsKnight(a);

                    case 2:
                        // "I am a knight or A is a knave."
                        return IsKnight(selfIndex) || IsKnave(a);

                    case 3:
                        // "It is not the case that A is a knight."
                        return !IsKnight(a);
                }
                break;

            case StatementList.IsKnave:
                switch (variant)
                {
                    case 0:
                        // "A is a knave."
                        return IsKnave(a);

                    case 1:
                        // "It is not the case that A is a knave."
                        return !IsKnave(a);
                }
                break;

            case StatementList.OnlyKnightSayKnight:
                // "Only a knight would say that A is a knight."
                return IsKnight(a);

            case StatementList.OnlyKnightSayKnave:
                // "Only a knight would say that A is a knave."
                return IsKnave(a);

            case StatementList.OnlyKnaveSayKnight:
                // "Only a knave would say that A is a knight."
                return IsKnave(a);

            case StatementList.OnlyKnaveSayKnave:
                // "Only a knave would say that A is a knave."
                return IsKnight(a);

            case StatementList.EitherKnightOrKnight:
                switch (variant)
                {
                    case 0:
                        // "Either I am a knight or B is a knight, but not both."
                        return Xor(IsKnight(selfIndex), IsKnight(b));

                    case 1:
                        // "Either A is a knight or B is a knight, but not both."
                        return Xor(IsKnight(a), IsKnight(b));

                    case 2:
                        // "Neither A nor B is a knight."
                        return !IsKnight(a) && !IsKnight(b);
                }
                break;

            case StatementList.EitherKnightOrKnave:
                switch (variant)
                {
                    case 0:
                        // "Either I am a knight or B is a knave, but not both."
                        return Xor(IsKnight(selfIndex), IsKnave(b));

                    case 1:
                        // "Either A is a knave or I am a knight, but not both."
                        return Xor(IsKnave(a), IsKnight(selfIndex));

                    case 2:
                        // "Either A is a knight or B is a knave, but not both."
                        return Xor(IsKnight(a), IsKnave(b));

                    case 3:
                        // "Either A is a knave or B is a knight, but not both."
                        return Xor(IsKnave(a), IsKnight(b));
                }
                break;

            case StatementList.EitherKnaveOrKnave:
                switch (variant)
                {
                    case 0:
                        // "Neither I nor B is a knave."
                        return !IsKnave(selfIndex) && !IsKnave(b);

                    case 1:
                        // "Neither A nor B is a knave."
                        return !IsKnave(a) && !IsKnave(b);
                    case 2:
                        // "Either A is a knave or B is a knave, but not both."
                        return Xor(IsKnave(a), IsKnave(b));
                }
                break;

            case StatementList.BothAreKnights:
                switch (variant)
                {
                    case 0:
                        // "I and B are both knights."
                        return IsKnight(selfIndex) && IsKnight(b);

                    case 1:
                        // "A and B are both knights."
                        return IsKnight(a) && IsKnight(b);

                    case 2:
                        // "It is not the case that both A and B are knights."
                        return !(IsKnight(a) && IsKnight(b));
                }
                break;

            case StatementList.BothAreKnaves:
                switch (variant)
                {
                    case 0:
                        // "It is not the case that both I and B are knaves."
                        return !(IsKnave(selfIndex) && IsKnave(b));

                    case 1:
                        // "It is not the case that both A and B are knaves."
                        return !(IsKnave(a) && IsKnave(b));

                    case 2:
                        // "A and B are both knaves."
                        return IsKnave(a) && IsKnave(b);
                }
                break;

            case StatementList.RoleSame:
                switch (variant)
                {
                    case 0:
                        // "I and B have the same role."
                        return SameRole(selfIndex, b);

                    case 1:
                        // "I and B do not have the same role."
                        return !SameRole(selfIndex, b);

                    case 2:
                        // "A and B have the same role."
                        return SameRole(a, b);


                    case 3:
                        // "A and B do not have the same role."
                        return !SameRole(a, b);
                }
                break;

            case StatementList.ExactlyOneIsKnight:
            {
                int count = 0;
                for (int i = 0; i < roles.Length; i++)
                {
                    if (roles[i] == Role.Knight)
                        count++;
                }
                return count == 1;
            }

            case StatementList.ExactlyOneIsKnave:
            {
                int count = 0;
                for (int i = 0; i < roles.Length; i++)
                {
                    if (roles[i] == Role.Knave)
                        count++;
                }
                return count == 1;
            }
        }

        Debug.LogWarning($"Invalid statement/variant combination: {statement} / {variant}");
        return false;
    }

    public string ToText(string[] names, int selfIndex)
    {
        string A = GetName(names, a);
        string B = GetName(names, b);
        string C = GetName(names, c);
        string D = GetName(names, d);
        string E = GetName(names, e);

        switch (statement)
        {
            case StatementList.IsKnight:
                switch (variant)
                {
                    case 0:
                        return $"I am a knight or {A} is a knight.";

                    case 1:
                        return $"{A} is a knight.";

                    case 2:
                        return $"I am a knight or {A} is a knave.";

                    case 3:
                        return $"It is not the case that {A} is a knight.";
                }
                break;

            case StatementList.IsKnave:
                switch (variant)
                {
                    case 0:
                        return $"{A} is a knave.";

                    case 1:
                        return $"It is not the case that {A} is a knave.";
                }
                break;

            case StatementList.OnlyKnightSayKnight:
                return $"Only a knight would say that {A} is a knight.";

            case StatementList.OnlyKnightSayKnave:
                return $"Only a knight would say that {A} is a knave.";

            case StatementList.OnlyKnaveSayKnight:
                return $"Only a knave would say that {A} is a knight.";

            case StatementList.OnlyKnaveSayKnave:
                return $"Only a knave would say that {A} is a knave.";

            case StatementList.EitherKnightOrKnight:
                switch (variant)
                {
                    case 0:
                        return $"Either I am a knight or {B} is a knight, but not both.";

                    case 1:
                        return $"Either {A} is a knight or {B} is a knight, but not both.";

                    case 2:
                        return $"Neither {A} nor {B} is a knight.";
                }
                break;

            case StatementList.EitherKnightOrKnave:
                switch (variant)
                {
                    case 0:
                        return $"Either I am a knight or {B} is a knave, but not both.";

                    case 1:
                        return $"Either {A} is a knave or I am a knight, but not both.";

                    case 2:
                        return $"Either {A} is a knight or {B} is a knave, but not both.";

                    case 3:
                        return $"Either {A} is a knave or {B} is a knight, but not both.";
                }
                break;

            case StatementList.EitherKnaveOrKnave:
                switch (variant)
                {
                    case 0:
                        return $"Neither I nor {B} is a knave.";
                    case 1:
                        return $"Neither {A} nor {B} is a knave.";
                    case 2:
                        return $"Either {A} is a knave or {B} is a knave, but not both.";
                }
                break;

            case StatementList.BothAreKnights:
                switch (variant)
                {
                    case 0:
                        return $"I and {B} are both knights.";

                    case 1:
                        return $"{A} and {B} are both knights.";

                    case 2:
                        return $"It is not the case that both {A} and {B} are knights.";
                }
                break;

            case StatementList.BothAreKnaves:
                switch (variant)
                {
                    case 0:
                        return $"It is not the case that both I and {B} are knaves.";

                    case 1:
                        return $"It is not the case that both {A} and {B} are knaves.";

                    case 2:
                        return $"{A} and {B} are both knaves.";
                }
                break;

            case StatementList.RoleSame:
                switch (variant)
                {
                    case 0:
                        return $"I and {B} have the same role.";

                    case 1:
                        return $"I and {B} do not have the same role.";

                    case 2:
                        return $"{A} and {B} have the same role.";

                    case 3:
                        return $"{A} and {B} do not have the same role.";
                }
                break;

            case StatementList.ExactlyOneIsKnight:
                return "Exactly one of us is a knight.";

            case StatementList.ExactlyOneIsKnave:
                return "Exactly one of us is a knave.";
        }

        return "...";
    }

    private string GetName(string[] names, int index)
    {
        if (names == null || index < 0 || index >= names.Length)
            return $"NPC[{index}]";

        return names[index];
    }

    public List<int> GetReferencedIndices(int selfIndex)
    {
        List<int> result = new List<int>();

        void Add(int index)
        {
            if (index < 0)
                return;
            if (index == selfIndex)
                return;
            if (!result.Contains(index))
                result.Add(index);
        }

        switch (statement)
        {
            case StatementList.IsKnight:
            case StatementList.IsKnave:
            case StatementList.OnlyKnightSayKnight:
            case StatementList.OnlyKnightSayKnave:
            case StatementList.OnlyKnaveSayKnight:
            case StatementList.OnlyKnaveSayKnave:
                Add(a);
                break;

            case StatementList.EitherKnightOrKnight:
                if (variant == 0)
                    Add(b);
                else
                {
                    Add(a);
                    Add(b);
                }
                break;

            case StatementList.EitherKnightOrKnave:
                if (variant == 0)
                    Add(b);
                else if (variant == 1)
                    Add(a);
                else
                {
                    Add(a);
                    Add(b);
                }
                break;

            case StatementList.EitherKnaveOrKnave:
                if (variant == 0)
                    Add(b);
                else
                {
                    Add(a);
                    Add(b);
                }
                break;

            case StatementList.BothAreKnights:
                if (variant == 0)
                    Add(b);
                else
                {
                    Add(a);
                    Add(b);
                }
                break;

            case StatementList.BothAreKnaves:
                if (variant == 0)
                    Add(b);
                else
                {
                    Add(a);
                    Add(b);
                }
                break;

            case StatementList.RoleSame:
                if (variant == 0 || variant == 1)
                    Add(b);
                else
                {
                    Add(a);
                    Add(b);
                }
                break;
        }

        return result;
    }
}
