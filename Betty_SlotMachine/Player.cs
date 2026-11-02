namespace Betty_SlotMachine
{
    public class Player
    {
        private static decimal _balance = 0;
        public static decimal Balance
        {
            get => _balance;
            set => _balance = Math.Round(value, 2);
        }
    }
}
