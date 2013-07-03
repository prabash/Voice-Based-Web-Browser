namespace UWIC.FinalProject.Common
{
    public enum FirstLevelCategory
    {
        Default,
        FunctionalCommand,
        MouseCommand,
        KeyboardCommand
    }

    public enum SecondLevelCategory
    {
        Default,
        BrowserCommand,
        InterfaceCommand,
        WebPageCommand,
    }

    public enum CommandType
    {
        back,
        startdictationmode,
        forth,
        go,
        refresh,
        stop,
        no,
        chng_backcolour,
        chng_forecolour,
        closetab,
        gototab,
        opennewtab,
        yes,
        scrolldown,
        scrollleft,
        scrollright,
        scrollup,
        alter,
        backspace,
        capslock,
        control,
        downarrow,
        enter,
        f5,
        leftarrow,
        rightarrow,
        space,
        tab,
        uparrow,
        move,
        click,
        doubleclick,
        rightclick
    }

    public enum FunctionalCommandType
    {
        Go,
        Forward,
        Backward,
        Refresh,
        Stop
    }

    public enum Mode
    {
        CommandMode,
        DictationMode,
        SpellMode
    }
}
