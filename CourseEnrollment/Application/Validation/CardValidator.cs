namespace CourseEnrollment.Application.Validation
{
    public static class CardValidator
    {
        public static bool IsValidLuhn(string cardNumber)
        {
            var digits = cardNumber.Where(char.IsDigit).ToArray();
            if (digits.Length < 13 || digits.Length > 19) return false;

            int sum = 0;
            bool doubleIt = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int digit = digits[i] - '0';
                if (doubleIt)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }
                sum += digit;
                doubleIt = !doubleIt;
            }
            return sum % 10 == 0;
        }

        public static bool IsValidExpiry(int month, int year)
        {
            if (month < 1 || month > 12) return false;
            // Card is valid through the end of the expiry month
            var expiryEnd = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
            return expiryEnd > DateTime.UtcNow;
        }

        public static bool IsValidCvv(string cvv) =>
            cvv.Length is >= 3 and <= 4 && cvv.All(char.IsDigit);
    }
}
