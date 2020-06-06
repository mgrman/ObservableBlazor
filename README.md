# Observable Section
Provides way of tracking observable subscriptions in Blazor.

# Demo
[Demo available as GitHub page](https://mgrman.github.io/ObservableBlazor/)

# Overview
The main logic is handled in ObservableSection Component.
It subscribes to observables and provides last value during binding.
Also it tracks the observable usages while it is being rendered. 
So it can, dispose all subscriptions that were not touched during rendering. Since they are not active any more.

Main method to use is `ObservableSection.Bind()` or Bind extension method for `IObservable`. 
This subscribes to the observable (if it is used first time) and returns the last value the observable had.

# Example usage

```
<ObservableSection>
    <p>Current count: @currentCount.Bind(context)</p>
    <button class="btn btn-primary" @onclick="()=>currentCount.OnNext(currentCount.Bind(context))">Click me</button>
</ObservableSection>
```

[The example project](https://github.com/mgrman/ObservableBlazor/tree/master/src/ObservableBlazor.Example) contains the sample Blazor App with reimplemented the Counter and FetchData pages using observables.
The Index page is extended with demo of observables of observables (user manipulation of simple tree structure), [see TreeItemCard for more](https://github.com/mgrman/ObservableBlazor/blob/master/src/ObservableBlazor.Example/Shared/TreeItemCard.razor).