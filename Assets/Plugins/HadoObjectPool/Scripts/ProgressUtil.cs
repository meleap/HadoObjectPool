using System;
using UniRx;

namespace Hado.Utils.Rx
{
    public static class ProgressUtil
    {
        public static readonly IProgress<float> Nop = new NopProgress();

        class NopProgress : IProgress<float>
        {
            public void Report(float value)
            {
            }
        }
    }
}