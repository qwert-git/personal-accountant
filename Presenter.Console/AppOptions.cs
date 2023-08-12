using CommandLine;

record AppOptions
{
    [Option('s', "statements", Required = true, HelpText = "Path to statements file")]
    public string StatementsPath { get; set; }

    [Option('t', "show-transactions", Required = false, HelpText = "Show transactions in report", Default = false)]
    public bool ShowTransactions { get; set; }

    [Option('e', "expand-category", Required = false, HelpText = "Show transactions in report for specific category")]
    public string ExpandCategory { get; set; } = string.Empty;
}