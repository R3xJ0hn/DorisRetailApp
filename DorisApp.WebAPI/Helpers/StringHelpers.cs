﻿using DorisApp.Model.Library;
using System.Text;

namespace DorisApp.WebAPI.Helpers
{
    public static class StringHelpers
    {
        public static string GetFullName(UserModel user)
        {
            var firstName = char.ToUpper(user.FirstName[0]) + user.FirstName[1..];
            var lastName = char.ToUpper(user.LastName[0]) + user.LastName[1..];
            return $"{CapitalizeWords(firstName)} {CapitalizeWords(lastName)}";
        }

        public static string CapitalizeWords(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Length == 0)
                return value;

            StringBuilder result = new(value);
            result[0] = char.ToUpper(result[0]);

            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
            }

            return result.ToString();
        }

    }
}
