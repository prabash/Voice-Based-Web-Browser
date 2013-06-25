namespace UWIC.FinalProject.Common
{
    public enum FirstLevelProbabilityIndex
    {
        FunctionalCommand,
        MouseCommand,
        KeyboardCommand,
        Default
    }

    public class FirstLevelCategory : LevelCategory
    {
        public const int Default = 0;
        public const int Functionalcommand = 1;
        public const int Mousecommand = 2;
        public const int Keyboardcommand = 3;
    }

    public class LevelCategory {}

    public enum CommandType
    {
        MouseRightClick,
        MouseLeftClick,
        MouseClick,
        KeyUpArrow,
        KeySpace,
        KeyRightArrow,
        KeyLeftArrow,
        Keyf5,
        KeyEnter,
        KeyDownArrow,
        KeyControl,
        KeyAlter,
        KeyCapsLock,
        KeyTab,
        KeyBackSpace,
        ChangeForeColor,
        ChangeBackColor,
        OpenNewTab,
        PageScrollUp,
        PageScrollDown,
        PageScrollRight,
        PageScrollLeft,
        MouseMove,
        BrowserStop,
        BrowserRefresh,
        BrowserGo,
        BrowserForward,
        BrowserBackward
    }
}
