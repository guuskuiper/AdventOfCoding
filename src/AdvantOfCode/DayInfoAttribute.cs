using System.Diagnostics;

namespace AdventOfCode;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DayInfoAttribute : Attribute
{
    public int Day { get; }
    public int Year { get; }

    public DayInfoAttribute(int year, int day)
    {
        if (day is > 25 or < 0) throw new ArgumentOutOfRangeException(nameof(day),$"Invalid day {day}, must be between 1 and 25.");
        
        Day = day;
        Year = year;
    }    
}