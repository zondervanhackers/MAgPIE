using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.SharedLibrary.Factory
{
    /// <summary>
    /// Provides methods for emitting dynamic constructor delegates.
    /// </summary>
    public static class EmitDelegate
    {
        /// <summary>
        /// Creates a delegate that when called will create an instance of the return type of <typeparamref name="TDelegate"/> passing in the delegate arguments as the arguments to the constructor.
        /// </summary>
        /// <typeparam name="TDelegate">The desired type of delegate that will be returned.</typeparam>
        /// <returns>A delegate that will call the constructor of the return type.</returns>
        /// <remarks>
        ///     <para>Use this function to create delegates where the type of the parameters is not known at compile time (potentially specified as a generic).</para>
        ///     <para>Because this function actually emits a delegate, it is much faster than its cousin <see cref="ConstructorInfo.Invoke"/>.</para>
        /// </remarks>
        public static TDelegate CreateConstructor<TDelegate>()
            where TDelegate : class
        {
            Contract.Requires(typeof(TDelegate).IsSubclassOf(typeof(Delegate)));

            Type delegateType = typeof(TDelegate);
            MethodInfo methodInfo = delegateType.GetMethod("Invoke");
            Type instanceType = methodInfo.ReturnType;
            Type[] parameters = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

            ConstructorInfo constructorInfo = instanceType.GetConstructor(parameters);

            if (constructorInfo == null)
            {
                throw new InvalidOperationException($"No constructor on type {instanceType.Name} could be found with arguments of type ({String.Join(", ", parameters.Select(p => p.Name))})");
            }

            // Generate a unique name for the emitted delegate
            String methodName = $"{constructorInfo.DeclaringType.Name}__{Guid.NewGuid().ToString().Replace("-", "")}";
            DynamicMethod method = new DynamicMethod(methodName, constructorInfo.DeclaringType, parameters, true);

            ILGenerator generator = method.GetILGenerator();

            // Emit a load opcode for each parameter
            for (int i = 0; i < parameters.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        generator.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        generator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        generator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        generator.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        generator.Emit(OpCodes.Ldarg_S, i);
                        break;
                }
            }

            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);

            return method.CreateDelegate(delegateType) as TDelegate;
        }
    }
}
