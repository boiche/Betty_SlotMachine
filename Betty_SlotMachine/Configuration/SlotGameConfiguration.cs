namespace Betty_SlotMachine.Configuration
{
    public class SlotGameConfiguration
    {
        public List<GameOutcome> Outcomes { get; set; }
        public decimal MaxBet { get; set; }
        public decimal MinBet { get; set; }
        public decimal MaxDeposit { get; set; }
    }
}