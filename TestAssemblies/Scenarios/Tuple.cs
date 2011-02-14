namespace Scenarios
{
    public class Tuple<T1, T2, T3>
    {
        private readonly T1 firstValue;
        private readonly T2 secondValue;
        private readonly T3 thirdValue;

        public Tuple(T1 firstValue, T2 secondValue, T3 thirdValue)
        {
            this.firstValue = firstValue;
            this.thirdValue = thirdValue;
            this.secondValue = secondValue;
        }

        public T3 ThirdValue
        {
            get { return thirdValue; }
        }

        public T2 SecondValue
        {
            get { return secondValue; }
        }

        public T1 FirstValue
        {
            get { return firstValue; }
        }
    }
}