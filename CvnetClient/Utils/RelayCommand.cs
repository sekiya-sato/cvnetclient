using System;
using System.Windows.Input;

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Predicate<T>? _canExecute;

    public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || (parameter is T t && _canExecute(t));
    }

    public void Execute(object? parameter)
    {
        if (parameter is T t)
            _execute(t);
    }

    public event EventHandler? CanExecuteChanged;

}
