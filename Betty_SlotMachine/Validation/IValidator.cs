namespace Betty_SlotMachine.Validation
{
    /// <summary>
    /// Defines validating service
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates provided context
        /// </summary>
        /// <returns>Whether the state is valid</returns>
        bool Validate();
    }
}
