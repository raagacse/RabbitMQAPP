
namespace OrderAPI.Publisher;

public interface IPublisher
{
    void SendMessage<T>(T message);
}
