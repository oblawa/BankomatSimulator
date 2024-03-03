using BankomatApp.Core;
using BankomatApp.Model;
using BankomatApp.Services;
using BankomatApp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace BankomatApp.ViewModel
{

    internal class FirstWindowVM
    {
        private string _cardNumberWithoutDashes;

        public string CardNumber
        {
            get { return _cardNumberWithoutDashes; }
            set
            {
                _cardNumberWithoutDashes = value.Replace("-", "");
            }
        }

        public string PinCode { get; set; }
        public RelayCommand CheckCardNumber_Command { get; set; }
        public RelayCommand CheckPinCode_Command { get; set; }
        AuthorizationService cardService;
        public FirstWindowVM()
        {
            CheckCardNumber_Command = new RelayCommand(o => CheckCardNumber());
            CheckPinCode_Command = new RelayCommand(o => CheckPinCode());

            cardService = new AuthorizationService();
            cardService.ThereIsNoCardEvent += OnThereIsNoCard;
            cardService.CardIsEnableEvent += OnCardIsEnable;
            cardService.CardIsBlockedEvent += OnCardIsBlocked;
            cardService.PinCodeCorrectEvent += OnPinCodeIsCorrect;
            cardService.PinCodeIncorrectEvent += OnPinCodeIsIncorrect;
        }
        // карты нет
        public void OnThereIsNoCard()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show("Карта не найдена");
            }));
        }
        // карта найдена
        public void OnCardIsEnable()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                CheckPinWindow checkPinWindow = new CheckPinWindow();
                checkPinWindow.DataContext = this;
                checkPinWindow.Show();
            }));
        }
        // карта заблокирована
        public void OnCardIsBlocked()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show("Карта заблокирована");
            }));
        }

        // пин-код введен правильно
        public void OnPinCodeIsCorrect(Card card)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow mainWindow = new MainWindow();
                MainWindowVM mainWindowVM = new MainWindowVM(card);
                mainWindow.DataContext = mainWindowVM;
                mainWindow.Show();
            }));
        }
        // пин-код введен неправильно
        public void OnPinCodeIsIncorrect()
        {
            MessageBox.Show("Пинкод неверный");
        }

        // запрос проверки номера карты
        public void CheckCardNumber()
        {
            
            if (CardNumber.Length == 16 && CardNumber.All(char.IsDigit))
            {
                cardService.CheckCardNumber(CardNumber);
            }
            else
            {
                MessageBox.Show("Номер должен состоять из 16 цифр.");
            }
        }
        // запрос проверки пин-кода
        public void CheckPinCode()
        {
            cardService.CheckPinCode(CardNumber, PinCode);
        }
    }
}
