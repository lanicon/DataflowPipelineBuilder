﻿using System.Threading.Tasks.Dataflow;

namespace DataflowPipelineBuilder
{
    public class StartBuilder<TOrigin, TSource> : IBuilder<TOrigin, TSource>
    {
        readonly IPropagatorBlock<TOrigin, TSource> _start;
        readonly BuilderOptions _options;

        internal StartBuilder
        (
            IPropagatorBlock<TOrigin, TSource> start, 
            BuilderOptions options
        )
        {
            _start = start;
            _options = options;
        }

        public IBuilder<TOrigin, TTarget> Then<TTarget>(IPropagatorBlock<TSource, TTarget> block)
        {
            var wrappedBlock = block.WrapInLogger(_options);

            _start.LinkTo(wrappedBlock, new DataflowLinkOptions { PropagateCompletion = true });

            return new MiddleBuilder<TOrigin, TTarget>(_start, wrappedBlock, _options);
        }

        public IForkBuilder<TOrigin, TSource> Fork() => 
            new ForkBuilder<TOrigin, TSource>(_start, _start, _options);

        public IPropagatorBlock<TOrigin, TSource> End() => _start;
    }
}
