using BankomatApp.Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace BankomatApp.Model
{
    internal class Card
    {
        public string cardNumber { get; set; }
        public double balance;
        public string bankomatNumber;
        public event Action<Transaction> WithdrawSuccessEvent;
        public event Action WithdrawFailureEvent;

        public Transaction currentTransaction;
        public Card(string cardNumber, double balance)
        {
            this.cardNumber = cardNumber;
            this.balance = balance;
        }
        private string connectionString = "Data Source=.\\database.db";
        // снять сумму
        public void Withdraw(double amount)
        {
            bool withdrawSuccessful = WithdrawTransaction(amount);
            if (withdrawSuccessful)
            {
                WithdrawSuccessEvent?.Invoke(currentTransaction);
            }
            else
            {
                WithdrawFailureEvent?.Invoke();
            }
        }
        // транзакция по снятию денег
        public bool WithdrawTransaction(double amount)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (balance < amount)
                        {
                            return false;
                        }
                        balance -= amount;
                        UpdateBalance(connection, balance);
                        currentTransaction = new Transaction(1, cardNumber,  bankomatNumber, amount, balance);
                        RecordTransaction(connection, currentTransaction);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }
        }
        //метод обновления баланса
        private void UpdateBalance(SQLiteConnection connection, double newBalance)
        {
            string sql = "UPDATE Cards SET Balance = @NewBalance WHERE CardNumber = @CardNumber";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@NewBalance", newBalance);
                command.Parameters.AddWithValue("@CardNumber", cardNumber);
                command.ExecuteNonQuery();
            }
        }
        // запись в таблицу транзакций
        private void RecordTransaction(SQLiteConnection connection, Transaction transaction)
        {
            string sql = "INSERT INTO Transactions (Type, CardNumber, DateTime, BankomatNumber, Amount, Remains) " +
                         "VALUES (@Type, @CardNumber, @DateTime, @BankomatNumber, @Amount, @Remains)";

            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Type", transaction.Type);
                command.Parameters.AddWithValue("@CardNumber", transaction.CardNumber);
                command.Parameters.AddWithValue("@DateTime", transaction.DateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@BankomatNumber", transaction.BankomatNumber);
                command.Parameters.AddWithValue("@Amount", transaction.Amount);
                command.Parameters.AddWithValue("@Remains", transaction.Remains);
                command.ExecuteNonQuery();
            }
        }


    }
}
