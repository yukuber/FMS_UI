using System;
using System.ComponentModel;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text.RegularExpressions;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using FMS_UI.ViewModels;
using Microsoft.VisualBasic;
using static Avalonia.Media.Colors;

namespace FMS_UI.Views;

public partial class MainWindow : Window
{
    private TextBlock? _selectedTextBox;
    private readonly Timer _clockTimer;

    

    public void HandleComboboxChanged(string filedb)
    {
        
        ComboBoxActDB.BorderBrush = Brushes.Aqua;
        string filePath = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\" + filedb + ".txt";
        string content = File.ReadAllText(filePath);
        string[] lines = content.Split(Environment.NewLine);
        TextBlockData6L.Text = lines[0]; //OP PROGRAM
        TextBlockData6R.Text = lines[1]; //CFG NO
        TextBlockDBTime.Text = lines[2]; //срок годности
        
    }

    private void HandleComboBox2Changed(string filedb2)
    {
        ComboBox2.BorderBrush = Brushes.Aqua;
        string file2Path = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\" + filedb2 + ".txt";
        string content2 = File.ReadAllText(file2Path);
        string[] lines2 = content2.Split();
        
    }

    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;
    
    public MainWindow()
    {
        InitializeComponent();
        TextBoxMessages.KeyDown += TextBoxMessages_KeyDown;
        
        TextBlockData6L.PointerPressed += TextBlockSelected_PointerPressed;
        TextBlockData6R.PointerPressed += TextBlockSelected_PointerPressed;
        TextBlockData12L.PointerPressed += TextBlockSelected_PointerPressed;
        TextBlockData12R.PointerPressed += TextBlockSelected_PointerPressed;
        TextBlockData14L.PointerPressed += TextBlockSelected_PointerPressed;
        TextBlockData14R.PointerPressed += TextBlockSelected_PointerPressed;
        TextBlockData16L.PointerPressed += TextBlockSelected_PointerPressed;
        
        TextBlockMode.PointerPressed += TextBlockSelected_PointerPressed; // кнопка модификаций
        
        TextBlockDecelFactor.PointerPressed += TextBlockSelected_PointerPressed;
        TextBlockFFactor.PointerPressed += TextBlockSelected_PointerPressed;
        
        TextBlockSync.PointerPressed += TextBlockSelected_PointerPressed; //отправляет не на ввод данных, а на проверку соответствия данных из DB. после выводит в messages "synchronised"

        TextBlockPosInit.PointerPressed += TextBlockSelected_PointerPressed;

        string filePath = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\AircraftConfig.txt";
        string content = File.ReadAllText(filePath);
        string[] lines = content.Split(Environment.NewLine);
        TextBlockData12L.Text = lines[0]; //AIRCRAFT
        TextBlockData12R.Text = lines[1]; //ENGINES
        TextBlockData14L.Text = lines[2]; //NAVGAR MODEL
        TextBlockData14R.Text = lines[3]; //POLICY
        TextBlockData16L.Text = lines[4]; //REG NBR
        string cachePath = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\cache.txt";
        string cache = File.ReadAllText(cachePath);
        string[] cacheLines = cache.Split(Environment.NewLine);
        TextBlockSync.Text = cacheLines[2];
        TextBlockDecelFactor.Text = cacheLines[3];
        TextBlockFFactor.Text = cacheLines[4];
        
        
        
        // Инициализация таймера (обновление каждую секунду)
        _clockTimer = new Timer(1000);
        _clockTimer.Elapsed += UpdateClock;
        _clockTimer.AutoReset = true;
        _clockTimer.Start();
        
        UpdateClock(null, null);
        
        this.Closed += (_, _) => _clockTimer.Stop();
        
        // Подписка на событие после загрузки
        Loaded += (s, e) =>
        {
            ViewModel.OnComboSelected += HandleComboboxChanged;
            ViewModel.OnCombo2Selected += HandleComboBox2Changed;
            HandleComboboxChanged(ViewModel.SelectedString);
            HandleComboBox2Changed(ViewModel.SelectedString2);
        };
        

    }
    private void TextBlockSelected_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock textBlock)
        {
            _selectedTextBox = textBlock;
            switch (_selectedTextBox.Name)
            {
               case "TextBlockMode":
                   TextBoxModification_PointerPressed(_selectedTextBox, e);
                   break;
               case "TextBlockSync":
                   TextBlockSync_PointerPressed(_selectedTextBox, e);
                   break;
               case "TextBlockPosInit":
                   TextBlockPosInit_PointerPressed(_selectedTextBox, e);
                   break;
               default:
                    if (TextBlockMode.Text == "MOD")
                    {
                        TextBoxMessages.IsEnabled = true;
                        TextBoxMessages.Focus();
                        TextBoxMessages.Clear();
                    }
                    break;
            }
        }
    }
    

    private void UpdateClock(object? sender, ElapsedEventArgs? e)
    {
        Dispatcher.UIThread.Post
        (
            () => { TextBlockTimestamp.Text = DateTime.Now.ToString("ddMMM'/'yy"); }
        );
    }


    private void TextBoxMessages_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (_selectedTextBox != null && TextBoxMessages.Text != "")
            {
                         // check "NAV DB ACTIVATE"
                         if (_selectedTextBox.Name == "TextBlockFFactor" || _selectedTextBox.Name == "TextBlockDecelFactor")
                         {
                             if (float.TryParse(TextBoxMessages.Text, out float result) == false)
                             {
                                 TextBlockMessages.Foreground = Brushes.Orange;
                                 TextBlockMessages.Text = "ERROR. CHECK DATA";
                                 TextBoxMessages.Text = "SCRATCHPAD LINE";
                                 // если не вводить данные и просто нажать enter вернет SCRATCHPAD LINE. под реализацию кнопки CLEAR
                                 TextBoxMessages.IsEnabled = false;
                                 TextBoxMessages.IsEnabled = true;
                             }
                             else if (float.Parse(TextBoxMessages.Text) >= -50 && float.Parse(TextBoxMessages.Text) <= 50)
                             {
                                 _selectedTextBox.Text = TextBoxMessages.Text + "%";
                                 TextBoxMessages.Text = "SCRATCHPAD LINE";
                                 TextBoxMessages.IsEnabled = false;
                                 TextBoxMessages.IsEnabled = true;
                             }
                             else
                             {
                                 TextBlockMessages.Foreground = Brushes.Orange;
                                 TextBlockMessages.Text = "ERROR. CHECK DATA";
                                 TextBoxMessages.Text = "SCRATCHPAD LINE";
                                 TextBoxMessages.IsEnabled = false;
                                 TextBoxMessages.IsEnabled = true;
                             }
                         }
                         // case "TextBlockData2R": 
//проверяем три разных файла с разными ошибками и два с совподающими данными. 
//таким образом реализуем провеку вывода сообщений в messages line 
//задача: читать данные не жестко по пути, а по выбору в выпадающем списке из папки.
//допустим есть 3 файла в папке, их имена отображаются в выпадающем списке. каждую из них можно выбрать. автоматически выбраны две последние в active и second. если из этого списка вручную выбрать 
                             // string filePath = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\sample.txt";
                             // string content = File.ReadAllText(filePath);
                             // string[] lines = content.Split();
                             // TextBlockData6L.Text = lines[0]; //OP PROGRAM
                             // TextBlockData12L.Text = lines[1]; //AIRCRAFT
                             // TextBlockData12R.Text = lines[2]; //ENGINES
                             // TextBlockData14L.Text = lines[3]; //NAVGAR MODEL
                             // TextBlockData14R.Text = lines[4]; //POLICY
                             // TextBlockData16L.Text = lines[5]; //REG NBR

//можнео вместо txt поставить json и поставить словарь, чтобы вытягивать юанные не по координате поля,
//а по ключу - названию поля
                             // break;
//проверка введенного текста на соответствие правилам ввода.
                             // case "TextBlockData4R":
                             //     // IRK2403B 21MAR-17APR/24
                             //     string pattern = @"^[A-Z0-9]{2,8}\s(0[1-9]|[12][0-9]|3[01])(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)-(0[1-9]|[12][0-9]|3[01])(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)\/([0-9]{2})$";
                             //     if (!Regex.IsMatch(TextBlockData4R.Text, pattern))
                             //     {
                             //         TextBlockData4R.Text = String.Empty;
                             //         TextBlockMessages.Text = "Error";
                             //     }     
                             // break;
                             
                             else
                             {
                                 _selectedTextBox.Text = TextBoxMessages.Text;
                                 TextBoxMessages.Text = "SCRATCHPAD LINE";
                                 // если не вводить данные и просто нажать enter вернет SCRATCHPAD LINE. под реализацию кнопки CLEAR
                                 TextBoxMessages.IsEnabled = false;
                                 TextBoxMessages.IsEnabled = true;
                             }
            }
        }
    }
    
    
    
    private void TextBlockPosInit_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        string DBPath = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\" + ComboBoxActDB.SelectedItem + ".txt";
        string DBupd = "";
        DBupd +=TextBlockData6L.Text + Environment.NewLine;
        DBupd += TextBlockData6R.Text + Environment.NewLine;
        DBupd+= TextBlockDBTime.Text;
        
        string ConfigPath = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\AircraftConfig.txt";
        string content = "";
        content += TextBlockData12L.Text + Environment.NewLine; //AIRCRAFT
        content += TextBlockData12R.Text + Environment.NewLine; //ENGINES
        content += TextBlockData14L.Text + Environment.NewLine; //NAVGAR MODEL
        content += TextBlockData14R.Text + Environment.NewLine; //POLICY
        content += TextBlockData16L.Text; //REG NBR
        
        string cachePath = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\cache.txt";
        string cache = "";
        cache += ComboBoxActDB.SelectedIndex + Environment.NewLine;
        cache += ComboBox2.SelectedIndex + Environment.NewLine;
        cache += TextBlockSync.Text + Environment.NewLine;
        cache += TextBlockDecelFactor.Text + Environment.NewLine;
        cache += TextBlockFFactor.Text;
        
        File.WriteAllText(DBPath, DBupd);
        File.WriteAllText(ConfigPath, content);
        File.WriteAllText(cachePath, cache);
        
        Environment.Exit(0);
    }

    private void TextBlockSync_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        
        if (TextBlockSync.Text == "\u25c0 SYNC")
        {
            TextBlockMessages.Foreground = Brushes.LimeGreen;
            TextBlockMessages.Text = "SYNCED";
            TextBlockSync.Text = "\u25c0 INDEP";
        }
        else
        {
            TextBlockMessages.Foreground = Brushes.LimeGreen;
            TextBlockMessages.Text = "INDEP";
            TextBlockSync.Text = "\u25c0 SYNC";
        }
        
    }

    public void TextBoxModification_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (_selectedTextBox.Text == "MOD")
        {
            _selectedTextBox.Text = "ACT";
            _selectedTextBox.Background = Brushes.Black;
        }
        else
        {
            _selectedTextBox.Text = "MOD";
            _selectedTextBox.Background = Brushes.Aqua;
        }
    }

    
}