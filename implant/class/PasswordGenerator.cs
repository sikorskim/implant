using System;

namespace implant
{
    public static class PasswordGenerator
    {
        public static string generate(int passwordLength)
        {
            string password = null;
            Random random = new Random();

            for (int i=1; i<=passwordLength; i++)
            {
                int rand = random.Next(0, 93);
                char character = (char)('!' + rand);
                password += character;
            }
            return password;
        }
    }
}
