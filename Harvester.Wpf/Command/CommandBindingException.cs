using System;
using System.Runtime.Serialization;

namespace ZondervanLibrary.Harvester.Wpf.Command
{
    public class CommandBindingException : Exception, ISerializable
    {
        public CommandBindingException()
        { }

        public CommandBindingException(String message)
            : base(message)
        { }

        public CommandBindingException(String message, Type commandTarget)
            : base(message)
        {
            CommandTarget = commandTarget;

            Data["CommandTarget"] = commandTarget;
        }

        public CommandBindingException(String message, Type commandTarget, String commandMethod)
            : base(message)
        {
            CommandTarget = commandTarget;
            CommandMethod = commandMethod;

            Data["CommandTarget"] = commandTarget;
        }

        public CommandBindingException(String message, Type commandTarget, String commandMethod, Type[] commandArguments)
            : base(message)
        {
            CommandTarget = commandTarget;
            CommandMethod = commandMethod;
            CommandArguments = commandArguments;

            Data["CommandTarget"] = commandTarget;
        }

        public CommandBindingException(String message, Exception inner)
            : base(message, inner)
        { }

        public CommandBindingException(String message, Exception inner, Type commandTarget)
            : base(message, inner)
        {
            CommandTarget = commandTarget;
            Data["CommandTarget"] = commandTarget;
        }

        public CommandBindingException(String message, Exception inner, Type commandTarget, String commandMethod)
            : base(message, inner)
        {
            CommandTarget = commandTarget;
            CommandMethod = commandMethod;
            Data["CommandTarget"] = commandTarget;
        }

        public CommandBindingException(String message, Exception inner, Type commandTarget, String commandMethod, Type[] commandArguments)
            : base(message, inner)
        {
            CommandTarget = commandTarget;
            CommandMethod = commandMethod;
            CommandArguments = commandArguments;
            Data["CommandTarget"] = commandTarget;
        }

        protected CommandBindingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public Type CommandTarget { get; set; }
        public String CommandMethod { get; set; }
        public Type[] CommandArguments { get; set; }
    }
}
