
namespace Hado.Utils.ObjectPool
{
    public static class IdGenerator
    {
        static int count = 0;

        public static int Generate()
        {
            count += 1;
            return count;
        }
    }
}