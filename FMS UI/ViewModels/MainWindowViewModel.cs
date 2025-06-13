using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FMS_UI.ViewModels;

// ViewModel для MainWindow
public class MainWindowViewModel
{
    
    // Делегат с параметром string
    public event Action<string> OnComboSelected;
    public event Action<string> OnCombo2Selected;
    
    // Массив строк, который будет источником данных для ComboBox
    public string[] MyStringArray { get; } = { "sample1", "sample2", "sample3", "sample4" };
    private string _selectedString;
    private string _selectedString2;

    public string SelectedString2
    {
        get => _selectedString2;
        set
        {
            _selectedString2 = value;
            OnCombo2Selected?.Invoke(value);
        }
    }
    
    public string SelectedString
    {
        get => _selectedString;
        set
        {
            
            _selectedString = value;
            OnComboSelected?.Invoke(value);
            
        }
    }
    
    // Конструктор ViewModel (можно использовать для инициализации)
    public MainWindowViewModel() 
    {
        string cachePath = "D:\\Юра\\школа\\projects\\10 класс\\FMS UI alive ver\\FMS UI\\cache.txt";
        string cache = File.ReadAllText(cachePath);
        
        string[] cacheLines = cache.Split(Environment.NewLine);
        _selectedString = MyStringArray[Convert.ToInt32(cacheLines[0])];
        _selectedString2 = MyStringArray[Convert.ToInt32(cacheLines[1])];

        // Устанавливаем начальный выбранный элемент и обновляем данные
        // SelectedString = MyStringArray.FirstOrDefault(); // Выбираем первый элемент по умолчанию
        
      
    }

}
