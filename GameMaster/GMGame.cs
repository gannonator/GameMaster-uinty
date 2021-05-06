using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameMaster
{
    [CreateAssetMenu(fileName = "newGMGame", menuName = "GameMaster/Game", order = 1)]
    public class GMGame : ScriptableObject
    {
        public List<LevelSceneData> Levels;
        public LevelSceneData StartingLevel;
        public string Name = "GameName" ;
        public string VertionText = "GM0.00";
    }

}