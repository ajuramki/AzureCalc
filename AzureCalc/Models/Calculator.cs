using System;
using System.Collections.Generic;

namespace AzureCalc.Models;

public partial class Calculator
{
    public int Id { get; set; }

    public string FirstValue { get; set; } = null!;

    public string SecondValue { get; set; } = null!;

    public string Operator { get; set; } = null!;

    public string? Total { get; set; }
}
