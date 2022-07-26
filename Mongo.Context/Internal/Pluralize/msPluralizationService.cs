using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Design.PluralizationServices;

public abstract class PluralizationService
{
    public System.Globalization.CultureInfo Culture { get; protected set; }

    public abstract bool IsPlural(string word);
    public abstract bool IsSingular(string word);
    public abstract string Pluralize(string word);
    public abstract string Singularize(string word);
    public abstract void AddWord(string singular, string plural);

    protected static bool DoesWordContainSuffix(string word, IEnumerable<string> suffixes, CultureInfo culture) => suffixes.Any(s => word.EndsWith(s, true, culture));
    protected static bool TryInflectOnSuffixInWord(string word, IEnumerable<string> suffixes, Func<string, string> operationOnWord, CultureInfo culture, out string newWord)
    {
        newWord = null;
        if (DoesWordContainSuffix(word, suffixes, culture))
        {
            newWord = operationOnWord(word);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Factory method for PluralizationService. Only support english pluralization.
    /// Please set the PluralizationService on the System.Data.Entity.Design.EntityModelSchemaGenerator
    /// to extend the service to other locales.
    /// </summary>
    /// <param name="culture">CultureInfo</param>
    /// <returns>PluralizationService</returns>
    public static PluralizationService CreateService(System.Globalization.CultureInfo culture)
    {
        if (culture.TwoLetterISOLanguageName == "en") return new EnglishPluralizationService();
        throw new NotImplementedException($"The current culture is not supported {culture.DisplayName}");
    }
}