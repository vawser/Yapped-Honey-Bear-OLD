namespace Yapped
{
    internal class GameMode
    {
        public enum GameType
        {
            DemonsSouls,
            DarkSouls1,
            DarkSouls1Remastered,
            DarkSouls2,
            Bloodborne,
            DarkSouls3,
            Sekiro
        }

        public GameType Game { get; }
        public string Name { get; }
        public string Directory { get; }

        public GameMode(GameType game, string name, string directory)
        {
            Game = game;
            Name = name;
            Directory = directory;
        }

        public static readonly GameMode[] Modes =
        {
            new GameMode(GameType.DarkSouls1, "DS1", "DS1"),
            new GameMode(GameType.DarkSouls1Remastered, "DS1R", "DS1R"),
            new GameMode(GameType.DarkSouls2, "DS2", "DS2"),
            new GameMode(GameType.DarkSouls3, "DS3", "DS3"),
            //new GameMode(GameType.DemonsSouls, "Demon's Souls", "DES"),
            //new GameMode(GameType.Bloodborne, "Bloodborne", "BB"),
            new GameMode(GameType.Sekiro, "Sekiro", "SDT"),
        };
    }
}
