using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace numberguessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Regex Regex = new Regex("[^0-9]+");
        private int _lives = 10;
        private bool _isInputForced;
        private bool _canUserType = true;
        private char _keyDigit = '\0';

        public int Lives
        {
            get => _lives;
            set
            {
                _lives = value;
                OnLivesChange();
            }
        }

        private int _pickedNumber;

        public MainWindow()
        {
            InitializeComponent();

            ConsoleAllocator.ShowConsoleWindow();

            PickNumber();
            TxbInput.Focus();
        }

        private void PickNumber()
        {
            Random random = new Random();
            _pickedNumber = random.Next(0, 100);
        }

        private void OnLivesChange()
        {
            LblLives.Content = $"Remaining lives: {Lives}";
        }

        private void GuessChecker(int number)
        {
            _isInputForced = true;
            if (number == _pickedNumber)
            {
                TxbInput.Background = new SolidColorBrush(Colors.PaleGreen);
                TxbInput.Text = "Congrats!";
                _canUserType = false;
                TxbInput.IsReadOnly = true;
                LblHint.Content = $"{number}";
            }
            else
            {
                TxbInput.Background = new SolidColorBrush(Colors.PaleVioletRed);
                LblHint.Content = number > _pickedNumber ? $"{number} > ?" : $"{number} < ?";
                if (Lives > 0)
                {
                    TxbInput.Text = "Wrong.";
                    Lives -= 1;
                }
                else
                {
                    TxbInput.Text = "You lost!";
                    _canUserType = false;
                    TxbInput.IsReadOnly = true;
                }
            }
        }

        private bool TextValidator(string str)
        {
            return !Regex.IsMatch(str);
        }

        private bool KeyValidator(Key key)
        {
            bool isValid = TextValidator(_keyDigit.ToString());

            if (isValid)
            {
                return true;
            }

            _keyDigit = '\0';
            return key == Key.Enter || key == Key.Back || key == Key.Escape;
        }

        private void TxbInput_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TxbInput.Foreground = new SolidColorBrush(Colors.Black);
            TxbInput.Text = "";
        }

        private void TxbInput_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            _keyDigit = e.Key.ToString().ToCharArray()[e.Key.ToString().Length - 1];
            KeyValidator(e.Key);

            if (e.Key == Key.Escape)
            {
                Close();
            }

            if (!_canUserType)
            {
                e.Handled = true;
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    if (TextValidator(TxbInput.Text) && TxbInput.Text != "")
                    {
                        GuessChecker(Int32.Parse(TxbInput.Text));
                    }
                    else
                    {
                        _isInputForced = false;
                        TxbInput.Background = new SolidColorBrush(Colors.White);
                        TxbInput.Text = "";
                    }

                    break;
                default:
                    if (_isInputForced)
                    {
                        _isInputForced = false;
                        TxbInput.Background = new SolidColorBrush(Colors.White);
                        TxbInput.Text = "";
                    }

                    if (KeyValidator(e.Key))
                    {
                        if (Int32.Parse(TxbInput.Text + _keyDigit) > 100)
                        {
                            e.Handled = true;
                            TxbInput.Text = "100";
                        }
                        else if (Int32.Parse(TxbInput.Text + _keyDigit) < 0)
                        {
                            e.Handled = true;
                            TxbInput.Text = "0";
                        }
                    }
                    else
                    {
                        e.Handled = !KeyValidator(e.Key);
                    }

                    TxbInput.CaretIndex = TxbInput.Text.Length;
                    break;
            }
        }
    }
}