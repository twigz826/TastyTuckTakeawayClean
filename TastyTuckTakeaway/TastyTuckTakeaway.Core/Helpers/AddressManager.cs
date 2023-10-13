using System.Text.RegularExpressions;

namespace TastyTuckTakeaway.Core.Helpers
{
    public interface IAddressManager
    {
        bool IsValidHouseNumber(string inputHouseNum);

        bool IsValidUKPostcode(string postcode);

        bool IsValidStreetName(string? streetName);
    }
    public class AddressManager : IAddressManager
    {
        public bool IsValidHouseNumber(string? inputHouseNum)
        {
            if (int.TryParse(inputHouseNum, out int houseNumber))
            {
                if (houseNumber > 0 && houseNumber < 9999)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsValidUKPostcode(string? postcode)
        {
            // UK postcode regular expression pattern
            string pattern = @"^(GIR 0AA|[A-PR-UWYZ](?:[0-9]{1,2}|([A-HK-Y][0-9]|[A-HK-Y][0-9](?:[0-9]|[ABEHMNPRV-Y]))|[0-9][A-HJKPS-UW])[0-9][ABD-HJLNP-UW-Z]{2})$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (postcode is not null)
            {
                return regex.IsMatch(postcode);
            }
            return false;
        }

        public bool IsValidStreetName(string? streetName)
        {
            return !String.IsNullOrEmpty(streetName) && streetName?.Length > 0;
        }
    }
}