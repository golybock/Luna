namespace Luna.Tools.SharedModels.Models;

public class TransactionTools
{
	public static async Task ExecuteInTransaction(Func<Task> action1, Func<Task> action2, Func<Task>? action1Compensation = null)
	{
		// если ошибка здесь, то все действия не будут выполнены
		await action1();

		// если 2 операция вызвала исключение, компенсируем 1 операцию
		try
		{
			await action2();
		}
		catch (Exception)
		{
			if (action1Compensation != null)
			{
				await action1Compensation();
			}
			throw;
		}
	}
}