using FluentAssertions;
using L2Dn.Events;
using L2Dn.Extensions;

namespace L2Dn.Common.Tests.Events;

public class EventContainerTests
{
    [Fact]
    public void Access()
    {
        EventContainer eventContainer = new EventContainer("Test");

        for (int iter = 0; iter < 1000; iter++)
        {
            const int batchSize = 20;
            for (int i = 0; i < batchSize; i++)
            {
                EventListener listener = new EventListener();
                eventContainer.Subscribe<EventArgument>(this, listener.OnEvent);
            }

            eventContainer.HasSubscribers<EventArgument>().Should().Be(true);

            EventArgument argument = new EventArgument();
            eventContainer.Notify(argument);

            argument.Count.Should().Be(batchSize);

            // Unsubscribe
            eventContainer.UnsubscribeAll<EventArgument>(this);
        }

        eventContainer.HasSubscribers<EventArgument>().Should().BeFalse();
    }

    [Fact]
    public void SimultaneousAccess()
    {
        const int threadCount = 4;
        EventContainer eventContainer = new("Test");
        StopFlagHolder stopFlagHolder = new();

        void ThreadProc()
        {
            while (!stopFlagHolder.Stop)
            {
                object owner = new();
                const int batchSize = 20;
                for (int i = 0; i < batchSize; i++)
                {
                    EventListener listener = new EventListener();
                    eventContainer.Subscribe<EventArgument>(owner, listener.OnEvent);
                }

                eventContainer.HasSubscribers<EventArgument>().Should().Be(true);

                EventArgument argument = new EventArgument();
                eventContainer.Notify(argument);
                argument.Count.Should().BeGreaterOrEqualTo(batchSize);

                // Unsubscribe
                eventContainer.UnsubscribeAll<EventArgument>(owner);
            }
        }

        Thread[] threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
            threads[i] = new Thread(ThreadProc);

        threads.ForEach(x => x.Start());

        Thread.Sleep(1000);
        stopFlagHolder.Stop = true;

        threads.ForEach(x => x.Join());

        eventContainer.HasSubscribers<EventArgument>().Should().BeFalse();
    }

    private sealed class EventListener
    {
        public void OnEvent(EventArgument arg)
        {
            Interlocked.Increment(ref arg.Count);
        }
    }

    private sealed class EventArgument: EventBase
    {
        public int Count;
    }

    private sealed class StopFlagHolder
    {
        public bool Stop { get; set; }
    }
}