using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace System.Data.Entity.Design.PluralizationServices;

/// <summary>
/// This class provide service for both the singularization and pluralization, it takes the word pairs
/// in the ctor following the rules that the first one is singular and the second one is plural.
/// </summary>
internal class BidirectionalDictionary<TFirst, TSecond>
{
    internal Dictionary<TFirst, TSecond> FirstToSecondDictionary { get; set; }
    internal Dictionary<TSecond, TFirst> SecondToFirstDictionary { get; set; }

    internal BidirectionalDictionary()
    {
        this.FirstToSecondDictionary = new Dictionary<TFirst, TSecond>();
        this.SecondToFirstDictionary = new Dictionary<TSecond, TFirst>();
    }

    internal BidirectionalDictionary(Dictionary<TFirst,TSecond> firstToSecondDictionary) : this()
    {
        foreach (var key in firstToSecondDictionary.Keys)
        {
            this.AddValue(key, firstToSecondDictionary[key]);
        }
    }

    internal virtual bool ExistsInFirst(TFirst value)
    {
        if (this.FirstToSecondDictionary.ContainsKey(value))  return true;
        return false;
    }

    internal virtual bool ExistsInSecond(TSecond value)
    {
        if (this.SecondToFirstDictionary.ContainsKey(value)) return true;
        return false;
    }

    internal virtual TSecond GetSecondValue(TFirst value)
    {
        if (this.ExistsInFirst(value))  return this.FirstToSecondDictionary[value];
        return default(TSecond);
    }

    internal virtual TFirst GetFirstValue(TSecond value)
    {
        if (this.ExistsInSecond(value)) return this.SecondToFirstDictionary[value];
        return default(TFirst);
    }

    internal void AddValue(TFirst firstValue, TSecond secondValue)
    {
        this.FirstToSecondDictionary.Add(firstValue, secondValue);
        if (!this.SecondToFirstDictionary.ContainsKey(secondValue))
        {
            this.SecondToFirstDictionary.Add(secondValue, firstValue);
        }
    }
}

internal class StringBidirectionalDictionary : BidirectionalDictionary<string, string>
{

    internal StringBidirectionalDictionary() : base()
    { }
    internal StringBidirectionalDictionary(Dictionary<string, string> firstToSecondDictionary) : base(firstToSecondDictionary)
    { }
    internal override bool ExistsInFirst(string value) => base.ExistsInFirst(value.ToLowerInvariant());
    internal override bool ExistsInSecond(string value) => base.ExistsInSecond(value.ToLowerInvariant());
    internal override string GetFirstValue(string value) => base.GetFirstValue(value.ToLowerInvariant());
    internal override string GetSecondValue(string value) =>  base.GetSecondValue(value.ToLowerInvariant());
}

