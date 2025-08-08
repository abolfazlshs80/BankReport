using System;
using System.Windows.Input;
using Xamarin.Forms;

public class SwitchCommandBehavior : Behavior<Switch>
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SwitchCommandBehavior));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(SwitchCommandBehavior), null);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnAttachedTo(Switch bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Toggled += OnSwitchToggled;
    }

    protected override void OnDetachingFrom(Switch bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.Toggled -= OnSwitchToggled;
    }

    private void OnSwitchToggled(object sender, ToggledEventArgs e)
    {
        var parameter = CommandParameter ?? e.Value; // اگر پارامتر تعیین نشده باشد، مقدار Toggled را ارسال می‌کنیم
        if (Command?.CanExecute(parameter) == true)
        {
            Command.Execute(parameter);
        }
    }
}