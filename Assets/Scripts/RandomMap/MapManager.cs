using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public MapConfig config;
        public MapView view;

        public Map CurrentMap { get; private set; }

        private void Start()
        {
            
                GenerateNewMap();
            
        }

        public void GenerateNewMap()
        {
            var map = MapGenerator.GetMap(config);
            CurrentMap = map;
            view.ShowMap(map);
        }

        private void OnApplicationQuit()
        {
        }
    }
}
