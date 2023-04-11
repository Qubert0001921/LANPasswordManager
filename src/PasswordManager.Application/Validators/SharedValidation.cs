using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Application;

public static class SharedValidation
{
    public static bool BeAValidName(string name)
    {
        name = name.Replace("_", "");
        name = name.Replace("-", "");

        return name.All(char.IsLetterOrDigit);
    }

    public static bool CountBeGreaterThanZero<TElement>(List<TElement> list) => list.Count > 0;
}
