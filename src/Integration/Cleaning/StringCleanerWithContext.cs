namespace Appalachia.CI.Integration.Cleaning
{
    
    public class StringCleanerWithContext<T1> : StringCleanerBase<StringCleanerWithContext<T1>>
    {
        public StringCleanerWithContext(T1 context1, ExecuteClean action, int capacity = 100) : base(
            action,
            capacity
        )
        {
            this.context1 = context1;
        }

        public T1 context1;
    }

    public class StringCleanerWithContext<T1, T2> : StringCleanerBase<StringCleanerWithContext<T1, T2>>
    {
        public StringCleanerWithContext(
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
        StringCleanerWithContext<T1, T2, T3> : StringCleanerBase<StringCleanerWithContext<T1, T2, T3>>
    {
        public StringCleanerWithContext(
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
