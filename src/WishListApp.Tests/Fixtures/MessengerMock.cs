using CommunityToolkit.Mvvm.Messaging;

namespace WishListApp.Tests.Fixtures;

public sealed class MessengerMock : IMessenger
{
    private readonly Lock _lock = new();
    private readonly List<object> _messages = [];

    public IReadOnlyList<object> Messages => _messages;

    public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
        => throw new NotImplementedException();

    public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
        where TRecipient : class
        where TMessage : class
        where TToken : IEquatable<TToken>
        => throw new NotImplementedException();

    public void UnregisterAll(object recipient) => throw new NotImplementedException();

    public void UnregisterAll<TToken>(object recipient, TToken token)
        where TToken : IEquatable<TToken>
        => throw new NotImplementedException();

    public void Unregister<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
        => throw new NotImplementedException();

    public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        lock (_lock)
        {
            _messages.Add(message);
        }

        return message;
    }

    public void Cleanup() => throw new NotImplementedException();

    public void Reset() => throw new NotImplementedException();
}
