using System;
using Quickenshtein;

namespace Hax;

internal static partial class Helper {
    static int LongestCommonSubstring(ReadOnlySpan<char> query, ReadOnlySpan<char> original) {
        int originalLength = original.Length;
        int queryLength = query.Length;

        int[,] table = new int[2, originalLength + 1];
        int result = 0;

        for (int i = 1; i <= queryLength; i++) {
            for (int j = 1; j <= originalLength; j++) {
                if (query[i - 1] == original[j - 1]) {
                    table[i % 2, j] = table[(i - 1) % 2, j - 1] + 1;

                    if (table[i % 2, j] > result) {
                        result = table[i % 2, j];
                    }
                }

                else {
                    table[i % 2, j] = 0;
                }
            }
        }

        return result;
    }

    static int GetSimilarityWeight(string query, string original) {
        int distancePenalty = Levenshtein.GetDistance(query, original);
        int commonalityReward = Helper.LongestCommonSubstring(query, original) * -2;

        return distancePenalty + commonalityReward;
    }

    internal static string FuzzyMatch(string? query, ReadOnlySpan<string> strings) {
        if (strings.Length is 0) return "";
        if (string.IsNullOrWhiteSpace(query)) return strings[0];

        string closestMatch = strings[0];
        int lowestWeight = Helper.GetSimilarityWeight(query, strings[0]);

        for (int i = 1; i < strings.Length; i++) {
            int totalWeight = Helper.GetSimilarityWeight(query, strings[i]);

            if (totalWeight < lowestWeight) {
                lowestWeight = totalWeight;
                closestMatch = strings[i];
            }
        }

        return closestMatch;
    }
}
