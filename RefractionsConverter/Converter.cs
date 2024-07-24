using System;
using System.Text.RegularExpressions;

namespace RefractionsConverter
{
  class Converter
  {
    public static void Main()
    {
      string input = "+1.50 -1.25 x080";
      Converter converter = new Converter();
      converter.ConvertInput(input);
    }

    public void ConvertInput(string input)
    {
      List<string> splitInputs = RunRegex(input);
      if (splitInputs[2] == null || splitInputs[2] == "") // the regex will always match on group 2 and 3 as long as cylinder and axis exist
      {
        Console.WriteLine("The input does not match either the plus or minus cylinder notation.");
        return;
      }
      if (splitInputs[0] == "")
      {
        Console.WriteLine("Sphere doesn't exist or is incorrect format. Assuming sphere is 0.");
      }
      decimal sphere = splitInputs[0] == "" ? 0 : Convert.ToDecimal(splitInputs[0]); // if sphere doesnt exist, set it to 0
      decimal cylinder = Convert.ToDecimal(splitInputs[1]);
      int axis = Convert.ToInt16(splitInputs[2]);

      int adjustedAxis = axis < 0 ? axis + 180 : axis > 180 ? axis - 180 : axis; // if negative add 180, if positive subtract 180

      if (!IsValidSphere(sphere) || !IsValidCylinder(cylinder) || !IsValidAxis(adjustedAxis)) // if sphere or cylinder are greater or less than their ranges or if the adjusted axis is still outside the range, break
      {
        return;
      }

      string result = ConvertNotation(sphere, cylinder, adjustedAxis);

      Console.WriteLine(result);
    }

    public bool IsValidAxis(int axis)
    {
      if (axis < 0 || axis > 180)
      {
        Console.WriteLine("The input for axis still exceeds the range of -180 to 180 after adjustment.");
        return false;
      }
      return true;
    }

    public bool IsValidSphere(decimal sphere)
    {
      if (Math.Abs(sphere) > 25)
      {
        Console.WriteLine("The input for sphere exceeds the range of -25.00 to 25.00.");
        return false;
      }
      return true;
    }

    public bool IsValidCylinder(decimal cylinder)
    {
      if (Math.Abs(cylinder) > 15)
      {
        Console.WriteLine("The input for cylinder exceeds the range of -15.00 to 15.00.");
        return false;
      }
      return true;
    }

    public List<string> RunRegex(string input)
    {
      string inputWithoutSpaces = Regex.Replace(input, @"\s+", ""); // remove white spaces between numbers to account for variations of inputs
      string regexPattern = @"([\+\-]\d{0,2}\.\d{0,2})?([\+\-]\d{0,2}\.\d{0,2})x(\-?\d{3})";

      Regex regex = new Regex(regexPattern);
      Match matcher = regex.Match(inputWithoutSpaces);

      string sphere = matcher.Groups[1].ToString();
      string cylinder = matcher.Groups[2].ToString();
      string axis = matcher.Groups[3].ToString();

      List<string> result = new List<string>
        {
            sphere,
            cylinder,
            axis
        };

      return result;
    }

    public string PositiveOrNegative(decimal requestNum)
    {
      return requestNum >= 0 ? "+" : "-"; // determine the sign for the print out
    }

    public string ConvertNotation(decimal sphere, decimal cylinder, int axis)
    {
      decimal convertedSphere = sphere + cylinder;
      decimal convertedCylinder = cylinder * -1; // negate the cylinder
      int convertedAxis = (axis + 90) % 180;

      if (convertedAxis == 0)
      {
        convertedAxis += 180; // Axis here is less than 001. "Range between 001 and 180. Axis less than 001 or greater than 180 is equivalent to adding or subtracting 180 degrees."
      }

      return $"{PositiveOrNegative(convertedSphere)}{Math.Abs(convertedSphere):F2} {PositiveOrNegative(convertedCylinder)}{Math.Abs(convertedCylinder):F2} x{convertedAxis:D3}";
    }
  }
}