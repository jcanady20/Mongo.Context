using System;
using System.Collections.Generic;

namespace Pluralization.Core;

public class Map<TFirst, TSecond>
{
    private readonly IDictionary<TFirst, TSecond> _firsts = new Dictionary<TFirst, TSecond>();
    private readonly IDictionary<TSecond, TFirst> _seconds = new Dictionary<TSecond, TFirst>();

    internal virtual bool KeyExists(TFirst key) => _firsts.ContainsKey(key);
    internal virtual bool ValueExists(TSecond value) => _seconds.ContainsKey(value);

    internal virtual TSecond GetByKey(TFirst key)
    {
        if (_firsts.ContainsKey(key)) return _firsts[key];
        return default(TSecond);
    }

    internal virtual TFirst GetByValue(TSecond value)
    {
        if (_seconds.ContainsKey(value)) return _seconds[value];
        return default(TFirst);
    }

    internal virtual bool ExistsByKey(TFirst key) => _firsts.ContainsKey(key);
    internal virtual bool ExistByValue(TSecond value) => _seconds.ContainsKey(value);

    public void Add(TFirst first, TSecond second)
    {
        if (_firsts.ContainsKey(first) || _firsts.ContainsKey(second)) throw new Exception("Duplicate key detected");
        if (_seconds.Contains(first) || _seconds.ContainsKey(second)) throw new Exception("Duplicate key detected");
        _firsts.Add(first, second);
        _seconds.Add(second, first);
    }
}