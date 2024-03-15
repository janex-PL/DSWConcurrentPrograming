using FluentResults;

namespace DSW.ConcurrentPrograming.Lab01;

public class OperationConsumer(Buffer? buffer = null)
{
    public readonly string Id = $"Consumer_{Guid.NewGuid().ToString()[..5]}";

    public void StartConsuming()
    {
        if (buffer is null)
            throw new InvalidOperationException("Buffer is not set");
        
        while (true)
        {
            var operation = buffer.Take();
            if (operation is null)
                continue;
            
            LogOperationMessage(operation.Id, $"Dequeued operation {operation.Id}");
            var result = PerformOperation(operation);
            if (result.IsFailed)
                Console.WriteLine($"[{Id}] Operation {operation.Id} failed - {result.GetLogMessage()}");
        }
    }
    
    public Result PerformOperation(Operation operation)
    {
        return operation.OperationType switch
        {
            OperationType.Withdraw => PerformWithdraw(operation),
            OperationType.Deposit => PerformDeposit(operation),
            OperationType.Transfer => PerformTransfer(operation),
            _ => throw new ArgumentOutOfRangeException(nameof(operation.OperationType))
        };
    }

    private Result PerformTransfer(Operation operation)
    {
        if (operation.From is null)
            return Result.Fail("Operation did not provide an account to transfer from");
        if (operation.To is null)
            return Result.Fail("Operation did not provide an account to transfer to");
        if (operation.From == operation.To)
            return Result.Fail("Operation cannot transfer to the same account");
        
        if (operation.SafeMode)
        {
            var (first,second) = string.Compare(operation.From.Id, operation.To.Id, StringComparison.Ordinal) < 0
                ? (operation.From, operation.To)
                : (operation.To, operation.From);   
            
            return PerformTransfer(operation,first, second);
        }
        else
        {
            return PerformTransfer(operation, operation.From, operation.To);
        }
    }

    private Result PerformTransfer(Operation operation, BankAccount first, BankAccount second)
    {
        LogOperationMessage(operation.Id,
            $"Performing transfer of {operation.Amount} from account {operation.From!.Id} to account {operation.To!.Id}");

        lock (first)
        {
            var result = operation.From.WithdrawSafe(operation.Amount);
            if (result.IsFailed)
            {
                LogOperationMessage(operation.Id, $"Could not complete transfer - {result.GetLogMessage()}");
                return result;
            }

            lock (second)
            {
                result = operation.To.DepositSafe(operation.Amount);
                if (result.IsFailed)
                {
                    LogOperationMessage(operation.Id, $"Could not complete transfer - {result.GetLogMessage()}");

                    var rollbackResult = operation.From.DepositSafe(operation.Amount);
                    LogOperationMessage(operation.Id,
                        rollbackResult.IsFailed
                            ? $"Could not rollback transfer - {rollbackResult.GetLogMessage()}"
                            : "Rollback completed");

                    return result.WithErrors(rollbackResult.Errors);
                }
            }

        }

        LogOperationMessage(operation.Id,
            $"Transfer completed, current balance of {first.Id} is {first.Balance}, current balance of {second.Id} is {second.Balance}");

        return Result.Ok();
    }

    private Result PerformDeposit(Operation operation)
    {
        if (operation.To is null)
            return Result.Fail("Operation did not provide an account to deposit to");
        
        LogOperationMessage(operation.Id,$"Performing deposit of {operation.Amount} to account {operation.To.Id}");
        
        var result = operation.SafeMode
            ? operation.To.DepositSafe(operation.Amount)
            : operation.To.DepositUnsafe(operation.Amount);
        
        LogOperationMessage(operation.Id, 
            result.IsSuccess
                ? $"Deposit completed, current balance is {operation.To.Balance}"
                : $"Could not complete deposit - {result.GetLogMessage()}");
        
        return result;
    }

    private Result PerformWithdraw(Operation operation)
    {
        if (operation.From is null)
            return Result.Fail("Operation did not provide an account to withdraw from");

        LogOperationMessage(operation.Id,
            $"Performing withdraw of {operation.Amount} from account {operation.From.Id}");

        var result = operation.SafeMode
            ? operation.From.WithdrawSafe(operation.Amount)
            : operation.From.WithdrawUnsafe(operation.Amount);
        
        LogOperationMessage(operation.Id,
            result.IsSuccess
                ? $"Withdrawal completed, current balance is {operation.From.Balance}"
                : $"Could not complete withdrawal - {result.GetLogMessage()}");

        return result;
    }

    private void LogOperationMessage(string operationId, string message) => Console.WriteLine($"[{Id}] [{operationId}] {message}");
}