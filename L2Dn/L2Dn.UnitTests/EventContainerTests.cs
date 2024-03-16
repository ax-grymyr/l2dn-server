using FluentAssertions;
using L2Dn.Events;
using L2Dn.Extensions;

namespace L2Dn;

public class EventContainerTests
{
    [Fact]
    public void Access()
    {
        EventProvider eventProvider = new EventProvider();

        List<EventListener> listeners = new List<EventListener>();
        for (int iter = 0; iter < 1000; iter++)
        {
            const int batchSize = 20;
            for (int i = 0; i < batchSize; i++)
            {
                EventListener listener = new EventListener();
                listeners.Add(listener);
                eventProvider.Event += listener.OnEvent;
            }

            eventProvider.ListenerCount.Should().Be(batchSize);

            EventArgument argument = new EventArgument();
            eventProvider.Notify(argument);
            
            argument.Count.Should().Be(batchSize);
                
            // Unsubscribe
            listeners.ForEach(x => eventProvider.Event -= x.OnEvent);
            listeners.Clear();
        }

        eventProvider.ListenerCount.Should().Be(0);
    }

    [Fact]
    public void SimultaneousAccess()
    {
        const int threadCount = 4;
        EventProvider eventProvider = new EventProvider();
        bool stop = false;
        
        void ThreadProc()
        {
            List<EventListener> listeners = new List<EventListener>();
            while (!Volatile.Read(ref stop))
            {
                const int batchSize = 20;
                for (int i = 0; i < batchSize; i++)
                {
                    EventListener listener = new EventListener();
                    listeners.Add(listener);
                    eventProvider.Event += listener.OnEvent;
                }

                eventProvider.ListenerCount.Should().BeGreaterOrEqualTo(batchSize);

                EventArgument argument = new EventArgument();
                eventProvider.Notify(argument);
                argument.Count.Should().BeGreaterOrEqualTo(batchSize);
                
                // Unsubscribe
                listeners.ForEach(x => eventProvider.Event -= x.OnEvent);
                listeners.Clear();
            }
        }

        Thread[] threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
            threads[i] = new Thread(ThreadProc);

        threads.ForEach(x => x.Start());
        
        Thread.Sleep(10000);
        stop = true;
        
        threads.ForEach(x => x.Join());

        eventProvider.ListenerCount.Should().Be(0);
    }

    private class EventProvider
    {
        private EventManager<EventArgument> _eventManager;

        public int ListenerCount => _eventManager.SubscriberCount;
        
        public event Action<EventArgument> Event
        {
            add => _eventManager.Subscribe(value);
            remove => _eventManager.Unsubscribe(value);
        }

        public void Notify(EventArgument argument) => _eventManager.Notify(argument);
    }

    private class EventListener
    {
        public void OnEvent(EventArgument arg)
        {
            arg.Count++;
        }
    }
    
    private class EventArgument: EventBase
    {
        public int Count;
    }
}