using System;

namespace VAMInsuranceBot.Process
{
    public class Otp
    {
        private static string upper_alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string lower_alpha = "abcdefghijklmnopqrstuvwxyz";
        private static string numbers = "0123456789";
        private static string characters = "!@&*#";

        private static string chars = string.Concat(upper_alpha, lower_alpha, numbers, characters);
        private static char[] charArray = chars.ToCharArray();

        public static string otp = string.Empty;

        public static void GenerateOtp()
        {
            otp = string.Empty;

            for (int i = 0; i < 8; i++)
            {
                string character = string.Empty;

                do
                {
                    int index = new Random().Next(0, chars.Length);
                    character = charArray[index].ToString();
                } while (otp.IndexOf(character) != -1);

                otp += character;
            }
        }
    }
}