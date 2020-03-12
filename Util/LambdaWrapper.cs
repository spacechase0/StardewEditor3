using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.Util
{
    // These are used for using lambdas in signals
    class LambdaWrapper : Godot.Object
    {
        private readonly Action action;

        public LambdaWrapper(Action theAction)
        {
            action = theAction;
        }

        public void Invoke()
        {
            action.Invoke();
        }

        public void SelfConnect(Godot.Object obj, string signal)
        {
            obj.Connect(signal, this, nameof(Invoke));
        }
    }

    class LambdaWrapper<T> : Godot.Object
    {
        private readonly Action<T> action;

        public LambdaWrapper(Action<T> theAction)
        {
            action = theAction;
        }

        public void Invoke(T t)
        {
            action.Invoke(t);
        }

        public void SelfConnect(Godot.Object obj, string signal)
        {
            obj.Connect(signal, this, nameof(Invoke));
        }
    }

    class LambdaWrapper<T1, T2> : Godot.Object
    {
        private readonly Action<T1, T2> action;

        public LambdaWrapper(Action<T1, T2> theAction)
        {
            action = theAction;
        }

        public void Invoke(T1 t1, T2 t2)
        {
            action.Invoke(t1, t2);
        }

        public void SelfConnect(Godot.Object obj, string signal)
        {
            obj.Connect(signal, this, nameof(Invoke));
        }
    }
    class LambdaWrapper<T1, T2, T3> : Godot.Object
    {
        private readonly Action<T1, T2, T3> action;

        public LambdaWrapper(Action<T1, T2, T3> theAction)
        {
            action = theAction;
        }

        public void Invoke(T1 t1, T2 t2, T3 t3)
        {
            action.Invoke(t1, t2, t3);
        }

        public void SelfConnect(Godot.Object obj, string signal)
        {
            obj.Connect(signal, this, nameof(Invoke));
        }
    }
}
