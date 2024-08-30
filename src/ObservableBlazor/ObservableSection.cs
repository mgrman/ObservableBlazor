using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;

namespace ObservableBlazor;

public class ObservableSection : ComponentBase, IDisposable
{
    private readonly Dictionary<object, IDisposable> bindDisposableTracker = new Dictionary<object, IDisposable>();
    private readonly Dictionary<object, object?> observableToValue = new Dictionary<object, object?>();
    private readonly List<object> toDelete = new List<object>();
    private readonly HashSet<object> touchedObservables = new HashSet<object>();

    private bool isDisposed;
    private bool isBuilding;

    [Parameter]
    public required RenderFragment<ObservableSection> ChildContent { get; set; }

    [Inject]
    protected ILogger<ObservableSection> Logger { get; private set; } = default!;

    public T? Bind<T>(IObservable<T> observable )
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException($"{nameof(Bind)} can only be called when building RenderTree of this component!");
        }
        if (IsDebugLogEnabled())
        {
            LogDebug($"Bind {observable}[#{observable.GetHashCode()}] Exists:{observableToValue.ContainsKey(observable)}");
        }

        touchedObservables.Add(observable);
        if (observableToValue.TryGetValue(observable, out var value))
        {
            return (T)value!; // can force the cast as only way to add the value was via strongly typed !subscription 
        }

        var suspendInBind = true;
        var subscription = observable.Subscribe(new LambdaObserver<T>(o =>
        {
            observableToValue[observable] = o;
            if (!suspendInBind)
            {
                InvokeAsync(StateHasChanged);
            }
        }));
        bindDisposableTracker[observable] = subscription;
        suspendInBind = false;
        if (observableToValue.TryGetValue(observable, out value))
        {
            return (T)value!;
        }

        return default;
    }

    public Action<ChangeEventArgs> Set(IObserver<string> observer)
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException($"{nameof(Set)} can only be called when building RenderTree of this component!");
        }
        Action<ChangeEventArgs> handler = e => observer.OnNext(e.Value?.ToString() ?? string.Empty);
        return handler;
    }

    public Action<ChangeEventArgs> Set<T>(IObserver<T> observer, Func<string, T> converter)
    {
        if (!isBuilding)
        {
            throw new InvalidOperationException($"{nameof(Set)} can only be called when building RenderTree of this component!");
        }
        Action<ChangeEventArgs> handler = e => observer.OnNext(converter(e.Value?.ToString() ?? string.Empty));
        return handler;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        touchedObservables.Clear();

        isBuilding = true;

        if (IsDebugLogEnabled())
        {
            LogDebug("Starting binding");
        }

        builder.AddContent(0, ChildContent.Invoke(this));

        RemoveUnusedObservables();
        isBuilding = false;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                foreach (var subscription in bindDisposableTracker.Values)
                {
                    subscription.Dispose();
                }
            }

            isDisposed = true;
        }
    }

    private void RemoveUnusedObservables()
    {
        foreach (var observable in observableToValue.Keys)
        {
            if (!touchedObservables.Contains(observable))
            {
                toDelete.Add(observable);
            }
        }

        foreach (object o in toDelete)
        {
            if (bindDisposableTracker.Remove(o, out var subscription))
            {
                subscription.Dispose();
            }

            observableToValue.Remove(o);
        }

        if (IsDebugLogEnabled())
        {
            LogDebug($"RemoveUnusedObservables {toDelete.Count} Left:{observableToValue.Count}");
        }
        toDelete.Clear();
    }

    private void LogDebug(string message)
    {
        Logger.LogDebug($"[#{GetHashCode()}]{message}");
    }

    private bool IsDebugLogEnabled()
    {
        return Logger.IsEnabled(LogLevel.Debug);
    }
}