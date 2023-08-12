using Models.BankStatement;

namespace BLL.StatementReaders;

public interface IStatementsReader
{
    Task<Statement> ReadAsync();
    
    Task<Statement> ReadAsync(string pathToFile);
}