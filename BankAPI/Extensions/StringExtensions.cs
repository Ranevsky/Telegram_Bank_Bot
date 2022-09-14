using System.Text;

namespace BankAPI.Extensions;

internal static class StringExtensions
{
    internal static string Capitalize(this string text)
    {
        StringBuilder sb = new(text);

        bool beginWord = true;
        for (int i = 0; i < sb.Length; i++)
        {
            if (beginWord)
            {
                sb[i] = char.ToUpper(sb[i]);
                beginWord = false;
                continue;
            }

            if (sb[i] == ' ')
            {
                beginWord = true;
            }
            else if (char.IsUpper(sb[i]))
            {
                sb[i] = char.ToLower(sb[i]);
            }
        }
        return sb.ToString();
    }

}
