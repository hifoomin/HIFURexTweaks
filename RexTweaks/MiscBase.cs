namespace HIFURexTweaks
{
    public abstract class MiscBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;

        public T ConfigOption<T>(T value, string name, string description)
        {
            return Main.HRTConfig.Bind<T>(Name, name, value, description).Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        //List<string> Names = new List<string>();

        public virtual void Init()
        {
            Hooks();
            Main.HRTLogger.LogInfo("Added " + Name);
        }
    }
}