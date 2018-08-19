using System;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.SharedLibrary.Factory
{
	public static class LambdaFactory
	{
		public static LambdaFactory<TInstance> Wrap<TInstance>(Func<TInstance> constructor)
		{
			return new LambdaFactory<TInstance>(constructor);
		}

		public static LambdaFactory<TInstance, TMethodArg1> Wrap<TInstance, TMethodArg1>(Func<TMethodArg1, TInstance> constructor)
		{
			return new LambdaFactory<TInstance, TMethodArg1>(constructor);
		}

		public static LambdaFactory<TInstance, TMethodArg1, TMethodArg2> Wrap<TInstance, TMethodArg1, TMethodArg2>(Func<TMethodArg1, TMethodArg2, TInstance> constructor)
		{
			return new LambdaFactory<TInstance, TMethodArg1, TMethodArg2>(constructor);
		}

		public static LambdaFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3> Wrap<TInstance, TMethodArg1, TMethodArg2, TMethodArg3>(Func<TMethodArg1, TMethodArg2, TMethodArg3, TInstance> constructor)
		{
			return new LambdaFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3>(constructor);
		}

		public static LambdaFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4> Wrap<TInstance, TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4>(Func<TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4, TInstance> constructor)
		{
			return new LambdaFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4>(constructor);
		}
	}

	public class LambdaFactory<TInstance> : IFactory<TInstance>
	{
		private readonly Func<TInstance> _constructor;

		public LambdaFactory(Func<TInstance> constructor)
		{
			Contract.Requires(constructor != null);

			_constructor = constructor;
		}

		public TInstance CreateInstance()
		{
			return _constructor();
		}
	} 

	public class LambdaFactory<TInstance, TMethodArg1> : IFactory<TInstance, TMethodArg1>
	{
		private readonly Func<TMethodArg1, TInstance> _constructor;

		public LambdaFactory(Func<TMethodArg1, TInstance> constructor)
		{
			Contract.Requires(constructor != null);

			_constructor = constructor;
		}

		public TInstance CreateInstance(TMethodArg1 argument1)
		{
			return _constructor(argument1);
		}
	} 

	public class LambdaFactory<TInstance, TMethodArg1, TMethodArg2> : IFactory<TInstance, TMethodArg1, TMethodArg2>
	{
		private readonly Func<TMethodArg1, TMethodArg2, TInstance> _constructor;

		public LambdaFactory(Func<TMethodArg1, TMethodArg2, TInstance> constructor)
		{
			Contract.Requires(constructor != null);

			_constructor = constructor;
		}

		public TInstance CreateInstance(TMethodArg1 argument1, TMethodArg2 argument2)
		{
			return _constructor(argument1, argument2);
		}
	} 

	public class LambdaFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3> : IFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3>
	{
		private readonly Func<TMethodArg1, TMethodArg2, TMethodArg3, TInstance> _constructor;

		public LambdaFactory(Func<TMethodArg1, TMethodArg2, TMethodArg3, TInstance> constructor)
		{
			Contract.Requires(constructor != null);

			_constructor = constructor;
		}

		public TInstance CreateInstance(TMethodArg1 argument1, TMethodArg2 argument2, TMethodArg3 argument3)
		{
			return _constructor(argument1, argument2, argument3);
		}
	} 

	public class LambdaFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4> : IFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4>
	{
		private readonly Func<TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4, TInstance> _constructor;

		public LambdaFactory(Func<TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4, TInstance> constructor)
		{
			Contract.Requires(constructor != null);

			_constructor = constructor;
		}

		public TInstance CreateInstance(TMethodArg1 argument1, TMethodArg2 argument2, TMethodArg3 argument3, TMethodArg4 argument4)
		{
			return _constructor(argument1, argument2, argument3, argument4);
		}
	} 
}