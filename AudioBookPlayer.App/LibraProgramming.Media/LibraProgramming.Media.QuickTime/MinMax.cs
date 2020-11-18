namespace LibraProgramming.QuickTime.Container
{
    internal static class Min
    {
        public static int For(int val1, int val2) => val1 < val2 ? val1 : val2;
    }

    internal static class MinMax
    {
        public static int For(int val1, int val2) => val1 < val2 ? val2 : val1;
    }
}