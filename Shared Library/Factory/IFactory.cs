﻿//----------------------------------------------------------------------------------------------------
// This code was generated by a tool (3/11/2015 4:38:31 PM).
//----------------------------------------------------------------------------------------------------

namespace ZondervanLibrary.SharedLibrary.Factory
{
	/// <summary>Provides a factory whose factory method has no arguments.</summary>
	/// <typeparam name="TInstance">The type of the object created by this factory.</typeparam>
	/// <remarks>
	///		<para>This interface is used for an implementation of the Abstract Factory Pattern.</para>
	///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
	/// </remarks>
	public interface IFactory<TInstance>
	{
		/// <summary> Creates a new instance of <typeparamref name="TInstance"/>.</summary>
		/// <returns>An object of type <typeparamref name="TInstance"/></returns>
		/// <remarks>
        ///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
        /// </remarks>
		TInstance CreateInstance();
	}

	/// <summary>Provides a factory whose factory method has one argument.</summary>
	/// <typeparam name="TInstance">The type of the object created by this factory.</typeparam>
	/// <typeparam name="TMethodArg1">The type of the first parameter of the <see cref="IFactory{TInstance,TMethodArg1}.CreateInstance(TMethodArg1)"/> method.</typeparam>
	/// <remarks>
	///		<para>This interface is used for an implementation of the Abstract Factory Pattern.</para>
	///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
	/// </remarks>
	public interface IFactory<TInstance, TMethodArg1>
	{
        /// <summary> Creates a new instance of <typeparamref name="TInstance"/>.</summary>
        /// <param name="methodArg1">The first parameter.</param>
        /// <returns>An object of type <typeparamref name="TInstance"/></returns>
        /// <remarks>
        ///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
        /// </remarks>
        TInstance CreateInstance(TMethodArg1 methodArg1);
	}

	/// <summary>Provides a factory whose factory method has two arguments.</summary>
	/// <typeparam name="TInstance">The type of the object created by this factory.</typeparam>
	/// <typeparam name="TMethodArg1">The type of the first parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2}.CreateInstance(TMethodArg1,TMethodArg2)"/> method.</typeparam>
	/// <typeparam name="TMethodArg2">The type of the second parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2}.CreateInstance(TMethodArg1,TMethodArg2)"/> method.</typeparam>
	/// <remarks>
	///		<para>This interface is used for an implementation of the Abstract Factory Pattern.</para>
	///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
	/// </remarks>
	public interface IFactory<TInstance, TMethodArg1, TMethodArg2>
	{
		/// <summary> Creates a new instance of <typeparamref name="TInstance"/>.</summary>
		/// <param name="methodArg1">The first parameter.</param>
		/// <param name="methodArg2">The second parameter.</param>
		/// <returns>An object of type <typeparamref name="TInstance"/></returns>
		/// <remarks>
        ///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
        /// </remarks>
		TInstance CreateInstance(TMethodArg1 methodArg1, TMethodArg2 methodArg2);
	}

	/// <summary>Provides a factory whose factory method has three arguments.</summary>
	/// <typeparam name="TInstance">The type of the object created by this factory.</typeparam>
	/// <typeparam name="TMethodArg1">The type of the first parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2,TMethodArg3}.CreateInstance(TMethodArg1,TMethodArg2,TMethodArg3)"/> method.</typeparam>
	/// <typeparam name="TMethodArg2">The type of the second parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2,TMethodArg3}.CreateInstance(TMethodArg1,TMethodArg2,TMethodArg3)"/> method.</typeparam>
	/// <typeparam name="TMethodArg3">The type of the third parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2,TMethodArg3}.CreateInstance(TMethodArg1,TMethodArg2,TMethodArg3)"/> method.</typeparam>
	/// <remarks>
	///		<para>This interface is used for an implementation of the Abstract Factory Pattern.</para>
	///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
	/// </remarks>
	public interface IFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3>
	{
		/// <summary> Creates a new instance of <typeparamref name="TInstance"/>.</summary>
		/// <param name="methodArg1">The first parameter.</param>
		/// <param name="methodArg2">The second parameter.</param>
		/// <param name="methodArg3">The third parameter.</param>
		/// <returns>An object of type <typeparamref name="TInstance"/></returns>
		/// <remarks>
        ///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
        /// </remarks>
		TInstance CreateInstance(TMethodArg1 methodArg1, TMethodArg2 methodArg2, TMethodArg3 methodArg3);
	}

	/// <summary>Provides a factory whose factory method has four arguments.</summary>
	/// <typeparam name="TInstance">The type of the object created by this factory.</typeparam>
	/// <typeparam name="TMethodArg1">The type of the first parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2,TMethodArg3,TMethodArg4}.CreateInstance(TMethodArg1,TMethodArg2,TMethodArg3,TMethodArg4)"/> method.</typeparam>
	/// <typeparam name="TMethodArg2">The type of the second parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2,TMethodArg3,TMethodArg4}.CreateInstance(TMethodArg1,TMethodArg2,TMethodArg3,TMethodArg4)"/> method.</typeparam>
	/// <typeparam name="TMethodArg3">The type of the third parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2,TMethodArg3,TMethodArg4}.CreateInstance(TMethodArg1,TMethodArg2,TMethodArg3,TMethodArg4)"/> method.</typeparam>
	/// <typeparam name="TMethodArg4">The type of the fourth parameter of the <see cref="IFactory{TInstance,TMethodArg1,TMethodArg2,TMethodArg3,TMethodArg4}.CreateInstance(TMethodArg1,TMethodArg2,TMethodArg3,TMethodArg4)"/> method.</typeparam>
	/// <remarks>
	///		<para>This interface is used for an implementation of the Abstract Factory Pattern.</para>
	///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
	/// </remarks>
	public interface IFactory<TInstance, TMethodArg1, TMethodArg2, TMethodArg3, TMethodArg4>
	{
		/// <summary> Creates a new instance of <typeparamref name="TInstance"/>.</summary>
		/// <param name="methodArg1">The first parameter.</param>
		/// <param name="methodArg2">The second parameter.</param>
		/// <param name="methodArg3">The third parameter.</param>
		/// <param name="methodArg4">The fourth parameter.</param>
		/// <returns>An object of type <typeparamref name="TInstance"/></returns>
		/// <remarks>
        ///     <para>For more information, see the <conceptualLink target="e75bc7a3-cbcb-4934-8c95-01bf84baccf1"/> topic.</para>
        /// </remarks>
		TInstance CreateInstance(TMethodArg1 methodArg1, TMethodArg2 methodArg2, TMethodArg3 methodArg3, TMethodArg4 methodArg4);
	}

}