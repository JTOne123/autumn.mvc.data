using System;
using Foundation.ObjectHydrator.Interfaces;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models.Generators
{
    public abstract class NullableGenerator<T> : IGenerator<T?>
        where T : struct
    {
        
        private readonly Random _random;
        private readonly int _limit;
        private readonly bool _reverse;
        
        protected Random Random()
        {
            return _random;
        }

        protected NullableGenerator(int limit = 2,bool reverse = false)
        {
            _random = new Random();
            _limit = limit;
            _reverse = reverse;
        }

        public T? Generate()
        {
            var toGenerate = _random.Next(0, _limit) % _limit != 0;
            if (toGenerate)
            {
                if (!_reverse)
                {
                    return OnGenerate();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (_reverse)
                {
                    return OnGenerate();
                }
                else
                {
                    return null;
                }
            }
        }

        protected abstract T OnGenerate();

    }
}