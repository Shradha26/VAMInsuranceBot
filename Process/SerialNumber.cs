using System;

namespace VAMInsuranceBot.Process
{
    public class SerialNumber
    {
        private static string upper_alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string numbers = "0123456789";

        private static string chars = string.Concat(numbers, upper_alpha);
        private static char[] charArray = chars.ToCharArray();

        public static string claimNumber = string.Empty;
        public static string renewalNumber = string.Empty;

        public static string GenerateClaimNumber()
        {
            claimNumber = string.Empty;

            for (int i = 0; i < 8; i++)
            {
                string character = string.Empty;

                if (i < 6)
                {
                    do
                    {
                        int index = new Random().Next(0, 9);
                        character = charArray[index].ToString();
                    } while (claimNumber.IndexOf(character) != -1);

                    claimNumber += character;
                }
                else
                {
                    do
                    {
                        int index = new Random().Next(10, 35);
                        character = charArray[index].ToString();
                    } while (claimNumber.IndexOf(character) != -1);

                    claimNumber += character;
                }
            }

            return claimNumber;
        }

        public static string GenerateRenewalNumber()
        {
            renewalNumber = string.Empty;

            for (int i = 0; i < 8; i++)
            {
                string character = string.Empty;

                if (i > 2 && i < 8)
                {
                    do
                    {
                        int index = new Random().Next(0, 9);
                        character = charArray[index].ToString();
                    } while (renewalNumber.IndexOf(character) != -1);

                    renewalNumber += character;
                }
                else
                {
                    do
                    {
                        int index = new Random().Next(10, 35);
                        character = charArray[index].ToString();
                    } while (renewalNumber.IndexOf(character) != -1);

                    renewalNumber += character;
                }
            }

            return renewalNumber;
        }
    }
}
