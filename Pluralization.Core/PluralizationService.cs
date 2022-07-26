using System;
using System.Globalization;
using System.Collections.Generic;

namespace Mongo.Context.Internal.Pluralize;

internal class PluralizationService
{
    private readonly Dictionary<string, ICulturePluralizationService> _pluralizationMap;
    public PluralizationHandler()
    {
        _pluralizationMap = new Dictionary<string, PluralizationHandle>();
    }


    public PluralizationHandler AddHandler(CultureInfo cultureInfo, ICulturePluralizationService pluralization)
    {
        if (!_pluralizationMap.ContainsKey(cultureInfo))
        {
            _pluralizationMap.Add(cultureInfo.TwoLetterISOLanguageName, pluralization);
        }
        return this;
    }

    public ICulturePluralizationService CreatePluralizationService(CultureInfo cultureInfo)
    {
        if (!_pluralizationMap.ContainsKey(cultureInf.TwoLetterISOLanguageName)) throw new Exception("Unregistered pluralization service requested");
        return _pluralizationMap[cultureInfo.TwoLetterISOLanguageName];
    }
}
