namespace Models.Common;

public record Currency(string Name, decimal RateToBaseCurrency, string Prefix = "");