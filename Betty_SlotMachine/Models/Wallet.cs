namespace Betty_SlotMachine.Models
{
    public class Wallet
    {
        private decimal _balance = 0;
        public decimal Balance
        {
            get => _balance;
        }

        public void Add(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");

            _balance += Math.Round(amount, 2);
        }

        public void Remove(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");
            if (amount > _balance)
                throw new InvalidOperationException("Insufficient funds");

            _balance -= Math.Round(amount, 2);
        }
    }
}
