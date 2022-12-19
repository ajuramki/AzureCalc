using System;
using System.Collections.Generic;

namespace AzureCalc.Models;

public partial class CalcOutput
{
    public string FirstValue { get; set; } = null!;

    public string SecondValue { get; set; } = null!;

    public string Operator { get; set; } = null!;

    public string? TotalVal { get; set; }

    public int Id { get; set; }
}
