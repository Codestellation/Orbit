using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Orbit;

namespace Codestellation.Orbit.Example
{
    class ManyWritersManyReadersOneReader
    {
        private static int _sequenceGenerator;

        private readonly int _maxSequenceValue;
        private readonly bool _delay;

        private readonly RingBuffer<int> _buffer;
        private readonly IWaitStrategy _waitStrategy;
        private readonly ConcurrentRingBufferWriter _committer;
        private readonly RingBufferReader _journaler;
        private readonly RingBufferReader _replicator;
        private readonly RingBufferReader _processor;

        public ManyWritersManyReadersOneReader(uint bufferPower, int maxSequenceValue, bool delay)
        {
            _maxSequenceValue = maxSequenceValue;
            _delay = delay;

            _buffer = new RingBuffer<int>(bufferPower);

            _waitStrategy = new BlockingWaitStrategy();

            // init
            _committer = new ConcurrentRingBufferWriter(_buffer.Size, _waitStrategy);
            _journaler = new RingBufferReader(_waitStrategy);
            _replicator = new RingBufferReader(_waitStrategy);
            _processor = new RingBufferReader(_waitStrategy);

            // setup
            _committer.DependsOn(_processor);
            _journaler.DependsOn(_committer);
            _replicator.DependsOn(_committer);
            _processor.DependsOn(_journaler, _replicator);
        }

        public void Run()
        {
            var producingTask1 = new Task(() =>
                {
                    int value;
                    while ((value = Interlocked.Increment(ref _sequenceGenerator)) <= _maxSequenceValue)
                    {
                        Write("c1", value);
                        Delay();
                    }
                });
            var producingTask2 = new Task(() =>
                {
                    int value;
                    while ((value = Interlocked.Increment(ref _sequenceGenerator)) <= _maxSequenceValue)
                    {
                        Write("c2", value);
                        Delay();
                    }
                });
            var journalingTask = Utils.CreateTask(() =>
                {
                    for (int i = 1; i <= _maxSequenceValue; i += JournalNext())
                    {
                        Delay();
                    }
                });
            var replicatingTask = Utils.CreateTask(() =>
                {
                    for (int i = 1; i <= _maxSequenceValue; i += ReplicateNext())
                    {
                        Delay();
                    }
                });
            var processingTask = Utils.CreateTask(() =>
                {
                    for (int i = 1; i <= _maxSequenceValue; i += ProcessNext())
                    {
                        Delay();
                    }
                });

            producingTask1.Start();
            producingTask2.Start();
            journalingTask.Start();
            replicatingTask.Start();
            processingTask.Start();

            Task.WaitAll(producingTask1, producingTask2, journalingTask, replicatingTask, processingTask);
        }

        private void Write(string name, int value)
        {
            long sequence = _committer.Claim();
            _buffer[sequence] = value;
            Console.WriteLine("+{0}: {1}", name, value);
            _committer.Commit(sequence, 1);
        }

        private int JournalNext()
        {
            return Read(_journaler, x => Console.WriteLine("-j: {0}", string.Join(",", x)));
        }

        private int ReplicateNext()
        {
            return Read(_replicator, x => Console.WriteLine("-r: {0}", string.Join(",", x)));
        }

        private int ProcessNext()
        {
            return Read(_processor, x => Console.WriteLine("-p: {0}", string.Join(",", x)));
        }

        private int Read(IRingBufferReader reader, Action<IEnumerable<long>> callback)
        {
            long lastAvailable = reader.WaitAvailable();

            var items = new List<long>();
            for (long seq = reader.Position; seq <= lastAvailable; seq++)
            {
                int value = _buffer[seq];
                items.Add(value);
            }

            if (items.Count > 0)
            {
                callback(items);
                reader.Move(lastAvailable + 1);
            }
            return items.Count;
        }

        private void Delay()
        {
            if (_delay)
                Utils.RandomSleep();
        }
    }
}