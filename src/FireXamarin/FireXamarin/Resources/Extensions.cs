using System;
namespace FireXamarin.Resources
{
    public static class Extensions
    {
        public static string ToUnicode(this string code)
        {
            var errorCode = "\uf00d";

            try
            {
                if (code.Length != 4)
                    return errorCode;

                if (int.TryParse(code, System.Globalization.NumberStyles.HexNumber, null, out var codeInt))
                    return char.ConvertFromUtf32(codeInt);

                else
                    return errorCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return errorCode;
            }
        }
    }
}
