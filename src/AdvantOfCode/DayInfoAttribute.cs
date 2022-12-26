using System.Diagnostics;

namespace AdventOfCode;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DayInfoAttribute : Attribute
{
    public int Day { get; }
    public int Year { get; }

    public DayInfoAttribute(int year, int day)
    {
        if (day is > 25 or < 0) throw new Exception($"Invalid day {day}");
        
        Day = day;
        Year = year;
    }    
}