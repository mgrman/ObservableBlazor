using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ObservableBlazor.Example.Client;

public class TreeItem
{
    private readonly List<TreeItem> _children = new List<TreeItem>();
    private readonly List<ISubject<string>> _metadata = new List<ISubject<string>>();

    private readonly BehaviorSubject<IReadOnlyList<TreeItem>> childrenSubject = new BehaviorSubject<IReadOnlyList<TreeItem>>(Array.Empty<TreeItem>());
    private readonly BehaviorSubject<IReadOnlyList<ISubject<string>>> metadataSubject = new BehaviorSubject<IReadOnlyList<ISubject<string>>>(Array.Empty<ISubject<string>>());

    public ISubject<string> Name { get; } = new BehaviorSubject<string>(string.Empty);

    public IObservable<IReadOnlyList<TreeItem>> Children => childrenSubject;

    public IObservable<IReadOnlyList<ISubject<string>>> Metadata => metadataSubject;

    public void AddChild(TreeItem item)
    {
        _children.Add(item);
        childrenSubject.OnNext(_children.ToList());
    }

    public void RemoveChild(TreeItem item)
    {
        _children.Remove(item);
        childrenSubject.OnNext(_children.ToList());
    }

    public void AddMetadataItem()
    {
        _metadata.Add(new BehaviorSubject<string>(string.Empty));
        metadataSubject.OnNext(_metadata.ToList());
    }

    public void RemoveMetadataItem(ISubject<string> item)
    {
        _metadata.Remove(item);
        metadataSubject.OnNext(_metadata.ToList());
    }
}