namespace Betty_SlotMachine.Utils
{
    public static class DecimalPlacesCalculator
    {
        public static int DecimalPlaces(decimal value) => (decimal.GetBits(value)[3] >> 16) & 0xFF;
    }
}
