using Betty_SlotMachine.Configuration;
using Betty_SlotMachine.Utils;
using Newtonsoft.Json;

namespace Betty_SlotMachine.Games
{
    /// <param name="output">Where game messages are written</param>
    /// <param name="random">Source of randomness for outcomes and multipliers</param>
    public class BettyGame(TextWriter output, Random random) : SlotGame
    {
        public static readonly string DefaultConfigPath =
            Path.Combine(AppContext.BaseDirectory, "Configuration", "slotGameSettings.json");

        private long _scale;

        public BettyGame(TextWriter output) : this(output, new Random()) { }

        public override bool Configure()
        {
            try
            {
                var config = File.ReadAllText(DefaultConfigPath);
                _configuration = JsonConvert.DeserializeObject<SlotGameConfiguration>(config);

                int decimalPlaces = _configuration?.Outcomes.Max(o => DecimalPlacesCalculator.DecimalPlaces(o.Probability)) ?? 0;
                _scale = 100L * (long)Math.Pow(10, decimalPlaces);

                CalculateBounaries();
                _configured = true;
                return true;
            }
            catch (Exception)
            {
                _configured = false;
                return false;
            }
        }

        private void CalculateBounaries()
        {
            _outcomeBounds = [];

            long running = 0;
            foreach (var outcome in _configuration.Outcomes)
            {
                running += (long)(outcome.Probability * (_scale / 100m));
                _outcomeBounds.Add((running, outcome));
            }

            if (running != _scale)
                throw new InvalidOperationException("Chances must sum to 100");
        }

        public override decimal Bet(decimal bet)
        {
            if (!_configured)
            {
                throw new InvalidOperationException("Game is not configured.");
            }

            try
            {
                long randomLong = random.NextInt64(0, _scale);
                var outcome = _outcomeBounds.First(x => randomLong < x.Upper).Outcome;
                decimal randomValue = random.NextInt64(0, 1_000_001) / 1_000_000m;
                decimal multiplier = outcome.MinMultiplier + (outcome.MaxMultiplier - outcome.MinMultiplier) * randomValue;
                decimal winnings = Math.Round(bet * multiplier, 2, MidpointRounding.ToZero);

                return winnings;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("An error occurred while processing the bet.", e);
            }
        }

        public override void Boot()
        {
            output.WriteLine("Welcome to the slot game! Available actions: deposit X, bet X, exit, withdraw X");
        }

        public override void Exit()
        {
            output.WriteLine("Thank you for playing our Betty game.");
        }
    }
}
