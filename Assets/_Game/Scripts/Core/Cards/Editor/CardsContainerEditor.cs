#if UNITY_EDITOR
using NothingAwaits.Core.Cards.Data;
using UnityEditor;
using UnityEngine;

namespace NothingAwaits.Core.Cards
{
    [CustomEditor(typeof(CardsContainer))]
    public class CardsContainerEditor : Editor
    {
        private bool _isInitialized = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var container = (CardsContainer)target;

            if (!_isInitialized)
            {
                CollectAllCardConfigs(container);
                _isInitialized = true;
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Refresh Card Collection"))
            {
                CollectAllCardConfigs(container);
            }
        }

        private void CollectAllCardConfigs(CardsContainer container)
        {
            var guids = AssetDatabase.FindAssets("t:CardConfig");
            var cardConfigs = new System.Collections.Generic.List<CardConfig>();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var cardConfig = AssetDatabase.LoadAssetAtPath<CardConfig>(path);
                if (cardConfig != null)
                {
                    cardConfigs.Add(cardConfig);
                }
            }

            container.GetType().GetField("_allCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(container, cardConfigs.ToArray());

            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();

            Debug.Log($"Collected {cardConfigs.Count} CardConfigs in container {container.name}");
        }
    }
}
#endif