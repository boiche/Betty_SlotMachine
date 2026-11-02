using Betty_SlotMachine.Configuration;
using Betty_SlotMachine.Utils;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Betty_SlotMachine.Games
{
    public class BettyGame : SlotGame
    {
        private const string LOSE = "No luck this time. Your balance is {0:F2}$";
        private const string WIN = "You won {0:F2}$. Your balance is {1:F2}$";
        private const string GAME_ERORR = "Game ERROR. Please contact your vendor";
        private const string MISCONFIGURATION_ERROR = "{0}: Exception on confiuration. {1}";
        private long _scale;
        private readonly Random _random = new();

        public override void Configure()
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var config = File.ReadAllText(path + @"\Configuration\probabilities.json");
                _configuration = JsonConvert.DeserializeObject<SlotGameConfiguration>(config);
                int decimalPlaces = _configuration?.Outcomes.Max(o => DecimalPlacesCalculator.DecimalPlaces(o.Probability)) ?? 0;
                _scale = 100L * (long)Math.Pow(10, decimalPlaces);

                CalculateBounaries();
                _configured = true;
            }
            catch (Exception e)
            {
                _configured = false;
                Debug.WriteLine(string.Format(MISCONFIGURATION_ERROR, nameof(BettyGame), e.Message));
                Console.WriteLine(GAME_ERORR);
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

        public override void Bet(decimal bet)
        {
            if (!_configured)
            {
                Console.WriteLine(GAME_ERORR);
                return;
            }

            try
            {
                long randomLong = _random.NextInt64(0, _scale);
                var outcome = _outcomeBounds.First(x => randomLong < x.Upper).Outcome;
                decimal randomValue = _random.NextInt64(0, 1_000_000) / 1_000_000m;
                decimal multiplier = outcome.MinMultiplier + (outcome.MaxMultiplier - outcome.MinMultiplier) * randomValue;
                decimal winnings = Math.Round(bet * multiplier, 2, MidpointRounding.ToZero);

                Player.Balance += winnings - bet;

                if (winnings > 0)
                {
                    Console.WriteLine(string.Format(WIN, winnings, Player.Balance));
                }
                else
                {
                    Console.WriteLine(string.Format(LOSE, Player.Balance));
                }
            }
            catch (Exception)
            {
                Console.WriteLine(GAME_ERORR);
            }
        }

        public override void Boot()
        {
            Console.WriteLine("Welcome to the slot game! Available actions: deposit X, bet X, exit, withdraw X");
        }

        public override void Exit()
        {
            Console.WriteLine("Thank you for playing our Betty game.");
        }
    }
}
