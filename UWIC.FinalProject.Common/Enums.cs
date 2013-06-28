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
        MouseCommand,
        WebPageCommand,
    }

    public enum CommandType
    {
        back,
        forth,
        go,
        refresh,
        stop,
        chng_backcolour,
        chng_forecolour,
        closetab,
        gototab,
        opennewtab,
        move,
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
        click,
        doubleclick,
        rightclick
    }
}
