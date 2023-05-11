using System;

namespace CoffeyUtils
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : Attribute
    {
        public string Label { get; set; } = "";
        public RuntimeMode Mode { get; set; } = RuntimeMode.Always;
        public int Spacing { get; set; } = 0;
        public ColorField Color { get; set; } = ColorField.None;
    }

    public enum RuntimeMode
    {
        Always,
        OnlyPlaying,
        OnlyEditor,
        None
    }
        
}