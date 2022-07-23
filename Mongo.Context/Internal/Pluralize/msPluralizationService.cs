namespace System.Data.Entity.Design.PluralizationServices;

public abstract class PluralizationService
{
    public System.Globalization.CultureInfo Culture { get; protected set; }

    public abstract bool IsPlural(string word);
    public abstract bool IsSingular(string word);
    public abstract string Pluralize(string word);
    public abstract string Singularize(string word);

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