namespace SharpMock.Core
{
    public delegate TReturn Function<TReturn>();
    public delegate TReturn Function<T, TReturn>(T a);
    public delegate TReturn Function<T1, T2, TReturn>(T1 a1, T2 a2);
    public delegate TReturn Function<T1, T2, T3, TReturn>(T1 a, T2 a2, T3 a3);
    public delegate TReturn Function<T1, T2, T3, T4, TReturn>(T1 a1, T2 a2, T3 a3, T4 a4);
    public delegate TReturn Function<T1, T2, T3, T4, T5, TReturn>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5);
    public delegate TReturn Function<T1, T2, T3, T4, T5, T6, TReturn>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6);
    public delegate TReturn Function<T1, T2, T3, T4, T5, T6, T7, TReturn>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7);
    public delegate TReturn Function<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8);
    public delegate TReturn Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9);
    public delegate TReturn Function<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10);

    public delegate void VoidAction();
    public delegate void VoidAction<T>(T a);
    public delegate void VoidAction<T1, T2>(T1 a1, T2 a2);
    public delegate void VoidAction<T1, T2, T3>(T1 a1, T2 a2, T3 a3);
    public delegate void VoidAction<T1, T2, T3, T4>(T1 a1, T2 a2, T3 a3, T4 a4);
    public delegate void VoidAction<T1, T2, T3, T4, T5>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5);
    public delegate void VoidAction<T1, T2, T3, T4, T5, T6>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6);
    public delegate void VoidAction<T1, T2, T3, T4, T5, T6, T7>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7);
    public delegate void VoidAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8);
    public delegate void VoidAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9);
    public delegate void VoidAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10);
}
