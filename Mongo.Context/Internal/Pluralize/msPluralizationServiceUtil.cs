using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace System.Data.Entity.Design.PluralizationServices;
internal static class PluralizationServiceUtil
{
    internal static bool DoesWordContainSuffix(string word, IEnumerable<string> suffixes, CultureInfo culture) => suffixes.Any(s => word.EndsWith(s, true, culture));

    internal static bool TryInflectOnSuffixInWord(string word, IEnumerable<string> suffixes, Func<string, string> operationOnWord, CultureInfo culture, out string newWord)
    {
        newWord = null;
        if (PluralizationServiceUtil.DoesWordContainSuffix(word, suffixes, culture))
        {
            newWord = operationOnWord(word);
            return true;
        }
        else
        {
            return false;
        }
    }
}