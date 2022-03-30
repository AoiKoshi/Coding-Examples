using System;

public class DebugCommandBase
{
    public string commandID;
    public string commandDescription;
    public string commandFormat;

    public DebugCommandBase(string ID, string description, string format)
    {
        commandID = ID;
        commandDescription = description;
        commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string ID, string description, string format, Action command) : base(ID, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;

    public DebugCommand(string ID, string description, string format, Action<T1> command) : base(ID, description, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}


