using BankomatApp.Core;
using BankomatApp.Model;
using BankomatApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BankomatApp.ViewModel
{
    internal class MainWindowVM
    {
        Card card {  get; set; }
        public double amountToWithdraw { get; set; }
        private static string BankomatNumber = "A1B231";
        public RelayCommand Withdraw_Command { get; set; }
        public RelayCommand OpenWithdrawWindow_Command { get; set; }
        public RelayCommand ShowBalance_Command { get; set; }
        public MainWindowVM(){}
        public MainWindowVM(Card card)
        {
            this.card = card;
            card.bankomatNumber = BankomatNumber;
            Withdraw_Command = new RelayCommand(o => Withdraw(amountToWithdraw));
            OpenWithdrawWindow_Command = new RelayCommand(o => OpenWithdrawWindow());
            ShowBalance_Command = new RelayCommand(o => ShowBalance());
            card.WithdrawSuccessEvent += OnWithdrawSuccess;
            card.WithdrawFailureEvent += OnWithdrawFailure;
        }
        // показать баланс
        private void ShowBalance()
        {
            MessageBox.Show(card.balance.ToString());
        }
        // показать данные транзакции
        private void OnWithdrawSuccess(Transaction transaction)
        {
            MessageBox.Show($"Дата/Время: {transaction.DateTime.ToString()}\tНомер банкомата: {transaction.BankomatNumber}\tНомер карты: {transaction.CardNumber}\tСумма: {transaction.Amount}\tОстаток: {transaction.Remains}", "Транзакция проведена успешно");
        }
        // ошибка транзакции
        private void OnWithdrawFailure()
        {
            MessageBox.Show($"Недопустимая сумма");
        }
        // открыть окно для ввода суммы
        private void OpenWithdrawWindow()
        {
            WithdrawWindow withdrawWindow = new WithdrawWindow();
            withdrawWindow.DataContext = this;
            withdrawWindow.Show();
        }
        // запрос на снятие денег
        private void Withdraw(double amount)
        {
            card.Withdraw(amount);
        }
    }
}
