namespace SharpMock.Core.DelegateTypes
{
    public delegate TReturn Function<TReturn>();

    public delegate TReturn Function<TArg, TReturn>(TArg arg);

    public delegate TReturn Function<TArg1, TArg2, TReturn>(TArg1 arg1, TArg2 arg2);

    public delegate TReturn Function<TArg1, TArg2, TArg3, TReturn>(TArg1 arg, TArg2 arg2, TArg3 arg3);

    public delegate TReturn Function<TArg1, TArg2, TArg3, TArg4, TReturn>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    public delegate TReturn Function<TArg1, TArg2, TArg3, TArg4, TArg5, TReturn>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);

    public delegate TReturn Function<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);

    public delegate void VoidAction();

    public delegate void VoidAction<TArg>(TArg arg);

    public delegate void VoidAction<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);

    public delegate void VoidAction<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3);

    public delegate void VoidAction<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    public delegate void VoidAction<TArg1, TArg2, TArg3, TArg4, TArg5>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);

    public delegate void VoidAction<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);
}
