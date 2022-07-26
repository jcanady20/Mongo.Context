namespace Pluralization.Core;
public interface ICulturePluralizationService
{
    bool IsPlural(string word);
    bool IsSingular(string word);
    string Pluralize(string word);
    ICulturePluralizationService AddWord(string singular, string plural);
}