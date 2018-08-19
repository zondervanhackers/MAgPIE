namespace ZondervanLibrary.SharedLibrary.Pipeline
{
    //public class UniqueEnumerable<T> : IEnumerable<T>
    //    where T : IEquatable<T>
    //{
    //    private readonly IEnumerable<T> _input;

    //    public UniqueEnumerable(IEnumerable<T> input)
    //    {
    //        Contract.Requires(input != null);

    //        _input = input;
    //    }


    //}



    //public class UniqueEnumerable<T> : IEnumerable<T>
    //{
    //    private readonly IEnumerable<T> _input;

    //    public UniqueEnumerable(IEnumerable<T> input)
    //    {
    //        _input = input;
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return new UniqueEnumerator<T, T>(_input.GetEnumerator(), element => element);
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }
    //}

    //public class UniqueEnumerable<T, TUnique> : IEnumerable<T>
    //{
    //    private readonly IEnumerable<T> _input;
    //    private readonly Func<T, TUnique> _uniqueField;

    //    public UniqueEnumerable(IEnumerable<T> input, Func<T, TUnique> uniqueField)
    //    {
    //        _input = input;
    //        _uniqueField = uniqueField;
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return new UniqueEnumerator<T, TUnique>(_input.GetEnumerator(), _uniqueField);
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }
    //}
}
