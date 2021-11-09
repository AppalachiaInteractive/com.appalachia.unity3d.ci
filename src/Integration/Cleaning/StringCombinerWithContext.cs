namespace Appalachia.CI.Integration.Cleaning
{
    public class StringCombinerWithContext<T1> : StringCombinerBase<StringCombinerWithContext<T1>>
    {
        public StringCombinerWithContext(T1 context1, ExecuteClean action, int capacity = 100) : base(
            action,
            capacity
        )
        {
            this.context1 = context1;
        }

        public T1 context1;
    }

    public class StringCombinerWithContext<T1, T2> : StringCombinerBase<StringCombinerWithContext<T1, T2>>
    {
        public StringCombinerWithContext(
            T1 context1,
            T2 context2,
            ExecuteClean action,
            int capacity = 100) : base(action, capacity)
        {
            this.context1 = context1;
            this.context2 = context2;
        }

        public T1 context1;
        public T2 context2;
    }

    public class
        StringCombinerWithContext<T1, T2, T3> : StringCombinerBase<StringCombinerWithContext<T1, T2, T3>>
    {
        public StringCombinerWithContext(
            T1 context1,
            T2 context2,
            T3 context3,
            ExecuteClean action,
            int capacity = 100) : base(action, capacity)
        {
            this.context1 = context1;
            this.context2 = context2;
            this.context3 = context3;
        }

        public T1 context1;
        public T2 context2;
        public T3 context3;
    }
}
