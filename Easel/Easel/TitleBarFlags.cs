using System;

namespace Easel;

[Flags]
public enum TitleBarFlags
{
    None = 1 << 0,
    ShowEasel = 1 << 1,
    ShowFps = 1 << 2,
    ShowGraphicsApi = 1 << 3
}