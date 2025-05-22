using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSlayer.Core
{
    public class GameSettings
    {
        public static GameSettings Instance { get; } = new GameSettings();

        // Игровые константы
        public int PlayerSpeed => 10;
        public int NormalZombieSpeed => 3;
        public int BigZombieSpeed => 2;
        public int SmallZombieSpeed => 5;
        public int BigZombieHealth => 3;
        public int SmallZombieHealth => 1;
        public int ZombieSpawnCount => 3;
        public int AmmoDropAmount => 5;
        public int HealAmount => 20;

        private GameSettings() { }
    }
}

