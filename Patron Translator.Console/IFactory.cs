namespace ZondervanLibrary.PatronTranslator.Console
{
    public interface IFactory<TInstance>
        where TInstance : class
    {
        TInstance CreateInstance();
    }

    public interface IFactory<TInstance, TParam1>
        where TInstance : class
        where TParam1 : class
    {
        TInstance CreateInstance(TParam1 param1);
    }

    public interface IFactory<TInstance, TParam1, TParam2>
        where TInstance : class
        where TParam1 : class
        where TParam2 : class
    {
        TInstance CreateInstance(TParam1 param1, TParam2 param2);
    }

    public interface IFactory<TInstance, TParam1, TParam2, TParam3>
        where TInstance : class
        where TParam1 : class
        where TParam2 : class
        where TParam3 : class
    {
        TInstance CreateInstance(TParam1 param1, TParam2 param2, TParam3 param3);
    }
}
