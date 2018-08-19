namespace ZondervanLibrary.PatronTranslator.Console
{
    public interface IConverter<TDomain, TRange>
    {
        TRange Convert(TDomain obj);

        TDomain ConvertBack(TRange obj);
    }
}
