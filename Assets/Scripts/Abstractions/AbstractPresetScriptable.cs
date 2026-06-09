using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbstractPresetScriptable", menuName = "Scriptable Objects/AbstractPresetScriptable")]
public abstract class AbstractPresetScriptable<T> : ScriptableObject where T : IBlock
{
    protected float[] _prob;
    protected int[] _alias;
    public abstract IEnumerable<T> Blocks { get; }
    public abstract T NextBlock();
    public static unsafe void BuildAliasTable(float[] weights, float[] prob, int[] alias)
    {
        int n = weights.Length;
        float* scaled = stackalloc float[n];
        int* small = stackalloc int[n];
        int* large = stackalloc int[n];
        int smallCount = 0, largeCount = 0;
        float total = 0f;
        for (int i = 0; i < n; i++) total += weights[i];
        if (total == 0) throw new System.Exception("Total of weights used for alias table is zero!");
        for (int i = 0; i < n; i++)
        {
            scaled[i] = weights[i] * n / total;
            if (scaled[i] < 1f) small[smallCount++] = i;
            else large[largeCount++] = i;
        }
        while (smallCount > 0 && largeCount > 0)
        {
            int s = small[--smallCount];
            int l = large[--largeCount];
            prob[s] = scaled[s];
            alias[s] = l;
            scaled[l] = (scaled[l] + scaled[s]) - 1f;
            if (scaled[l] < 1f) small[smallCount++] = l;
            else large[largeCount++] = l;
        }

        while (largeCount > 0) { int l = large[--largeCount]; prob[l] = 1f; }
        while (smallCount > 0) { int s = small[--smallCount]; prob[s] = 1f; }
    }

    public static int AliasDraw(float[] prob, int[] alias, System.Random rng)
    {
        int column = rng.Next(prob.Length);
        return rng.NextDouble() < prob[column] ? column : alias[column];
    }
}
