namespace Betty_SlotMachine.Configuration
{
    public class GameOutcome
    {
        public string Name { get; set; }
        public decimal Probability { get; set; }
        public decimal MinMultiplier { get; set; }
        public decimal MaxMultiplier { get; set; }
    }
}