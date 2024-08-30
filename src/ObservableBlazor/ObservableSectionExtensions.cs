using System;
using Microsoft.AspNetCore.Components;

namespace ObservableBlazor;

public static class ObservableBlazorExtensions
{
    public static T? Bind<T>(this IObservable<T> observable, ObservableSection section) => section.Bind(observable);

    public static Action<ChangeEventArgs> Set(this IObserver<string> observer, ObservableSection section)
    {
        return section.Set(observer);
    }

    public static Action<ChangeEventArgs> Set<T>(this IObserver<T> observer, ObservableSection section, Func<string, T> converter)
    {
        return section.Set(observer, converter);
    }
}