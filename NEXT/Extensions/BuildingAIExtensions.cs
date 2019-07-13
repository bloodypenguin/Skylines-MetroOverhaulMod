using UnityEngine;

namespace MetroOverhaul.NEXT.Extensions {
    public static class BuildingAIExtensions
    {
        public static T CloneBuildingAI<T>(this T originalBuildingAI, string newName)
            where T : BuildingAI
        {
            var gameObject = Object.Instantiate(originalBuildingAI.gameObject);
            gameObject.transform.parent = originalBuildingAI.gameObject.transform; // N.B. This line is evil and removing it is killoing the game's performances
            gameObject.name = newName;

            var info = gameObject.GetComponent<T>();

            return info;
        }
    }
}
