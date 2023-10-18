namespace TastyTuckTakeaway.Core.Interfaces
{
    public interface IAddressManager
    {
        bool IsValidHouseNumber(string inputHouseNum);

        bool IsValidUKPostcode(string postcode);

        bool IsValidStreetName(string? streetName);
    }
}