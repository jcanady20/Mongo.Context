using System;
using System.Globalization;
using System.Collections.Generic;

namespace Pluralization.Core;

public class EnglishPluralizationService : ICulturePluralizationService
{
    private readonly StringMap _userDictionary;
    public EnglishPluralizationService()
    {
        _userDictionary = new StringMap();
    }

    public bool IsSingular(string word)
    {
        return !IsPlural(word);
    }
    public bool IsPlural(string word)
    {
        if (_userDictionary.ExistsByKey(word)) return false;
        if (_userDictionary.ExistByValue(word)) return true;
        return true;
    }

    public string Pluralize(string word)
    {
        return word;
    }
    public ICulturePluralizationService AddWord(string singular, string plural)
    {
        if (singular is null) throw new ArgumentNullException(nameof(singular));
        if (plural is null)  throw new ArgumentNullException(nameof(plural));
        if (_userDictionary.ExistsByKey(singular)) return this;
        _userDictionary.Add(singular, plural);
        return this;
    }
}