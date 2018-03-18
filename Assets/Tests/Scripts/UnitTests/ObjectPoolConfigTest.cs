#pragma warning disable RECS0026

using System;
using RuntimeUnitTestToolkit;

namespace Hado.Utils.ObjectPool.UnitTests
{
    public class ObjectPoolConfigTest
    {
        public void ThrowArgumentExceptionWhenCreateCountPerFrameIsZero()
        {
            try
            {
                new ObjectPoolConfig(
                    numberOfInstances: 0,
                    createCountPerFrame: 0,
                    needPreactivation: false
                );
            }
            catch (Exception e)
            {
                var argumentException = e as ArgumentException;
                argumentException.Message.Is("createCountPerFrame must be > 0: 0");
            }
        }

        public void ThrowArgumentExceptionWhenNumberOfInstancesIsLowerThanZero()
        {
            try
            {
                new ObjectPoolConfig(
                    numberOfInstances: -1,
                    createCountPerFrame: 1,
                    needPreactivation: false
                );
            }
            catch (Exception e)
            {
                var argumentException = e as ArgumentException;
                argumentException.Message.Is("numberOfInstances must be >= 0: -1");
            }
        }

        public void ValidConfigsDoNotThrowException()
        {
            new ObjectPoolConfig(
                numberOfInstances: 0,
                createCountPerFrame: 1,
                needPreactivation: false
            );

            new ObjectPoolConfig(
                numberOfInstances: 0,
                createCountPerFrame: 1,
                needPreactivation: true
            );

            new ObjectPoolConfig(
                numberOfInstances: 1,
                createCountPerFrame: 1,
                needPreactivation: false
            );

            new ObjectPoolConfig(
                numberOfInstances: 10,
                createCountPerFrame: 10,
                needPreactivation: false
            );
        }
    }
}
#pragma warning restore RECS0026
