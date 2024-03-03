using BankomatApp.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace BankomatApp.Services
{
    internal class AuthorizationService
    {
        public AuthorizationService() 
        {
            
        }
        private string connectionString = "Data Source=.\\database.db";
        // проверка наличия карты
        public event Action CardIsEnableEvent;
        public event Action CardIsBlockedEvent;
        public event Action ThereIsNoCardEvent;
        public void CheckCardNumber(string cardNumber)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Blocked FROM Cards WHERE CardNumber = @CardNumber";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool isBlocked = Convert.ToBoolean(reader["Blocked"]);

                            if (!isBlocked)
                            {
                                // Карта не заблокирована
                                CardIsEnableEvent?.Invoke();
                            }
                            else
                            {
                                // Карта заблокирована
                                CardIsBlockedEvent?.Invoke();
                            }
                        }
                        else
                        {
                            // Карта не найдена
                            ThereIsNoCardEvent?.Invoke();
                        }
                    }
                }
            }
        }

        // проверка пин-кода
        public event Action<Card> PinCodeCorrectEvent;
        public event Action PinCodeIncorrectEvent;
        public void CheckPinCode(string cardNumber, string pinCode)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT  PinCode, FailedAttempts, Balance, Blocked FROM Cards WHERE CardNumber = @CardNumber";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CardNumber", cardNumber);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPinCode = (string)reader["PinCode"];
                            int failedAttempts = Convert.ToInt32(reader["FailedAttempts"]);

                            if (storedPinCode == pinCode)
                            {
                                // правильный пин-код
                                Card card = CreateCardInstance(cardNumber, reader);
                                PinCodeCorrectEvent?.Invoke(card);
                            }
                            else
                            {
                                // неправильный пин-код
                                failedAttempts++;
                                UpdateFailedAttempts(connection, cardNumber, failedAttempts);

                                if (failedAttempts > 3)
                                {
                                    BlockCard(connection, cardNumber);
                                    CardIsBlockedEvent?.Invoke();
                                }
                                else
                                {
                                    PinCodeIncorrectEvent?.Invoke();
                                }
                            }
                        }
                        else
                        {
                            ThereIsNoCardEvent?.Invoke();
                        }
                    }
                }
            }
        }
        // создание экземпляра карты
        private Card CreateCardInstance(string cardNumber, SQLiteDataReader reader)
        {
            string pinCode = (string)reader["PinCode"];
            int failedAttempts = Convert.ToInt32(reader["FailedAttempts"]);
            double balance = Convert.ToDouble(reader["Balance"]);
            bool isBlocked = Convert.ToBoolean(reader["Blocked"]);
            Card card = new Card(cardNumber, balance);
            return card;
        }


        // обновление неудачных попыток
        public void UpdateFailedAttempts(SQLiteConnection connection, string cardNumber, int newAttempts)
        {
            string sql = "UPDATE Cards SET FailedAttempts = @NewAttempts WHERE CardNumber = @CardNumber";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@NewAttempts", newAttempts);
                command.Parameters.AddWithValue("@CardNumber", cardNumber);
                command.ExecuteNonQuery();
            }
        }

        // блокировка карты
        public void BlockCard(SQLiteConnection connection, string cardNumber)
        {
            string sql = "UPDATE Cards SET Blocked = 1 WHERE CardNumber = @CardNumber";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@CardNumber", cardNumber);
                command.ExecuteNonQuery();
            }
        }
        // снятие денег с карты
    }
}
