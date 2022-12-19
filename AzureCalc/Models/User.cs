using System;
using System.Collections.Generic;

namespace AzureCalc.Models;

public partial class User
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmailAddress { get; set; }

    public int UserId { get; set; }
}
